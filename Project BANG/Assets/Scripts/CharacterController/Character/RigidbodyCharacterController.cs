namespace CharacterController
{
    using UnityEngine;
    using UnityEngineInternal;
    using System;
    using System.Collections.Generic;


    using DebugUI;


    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
    public abstract class RigidbodyCharacterController : MonoBehaviour
    {
        protected const float tinyOffset = .0001f;

        public enum MovementTypes { Adventure, Combat };

        


        #region Inspector properties



        //  Locomotion variables
        [SerializeField, Group("Motor")] protected bool m_useRootMotion = true;
        protected bool m_UseRootMotionRotation;
        [SerializeField, Group("Motor")] protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, Group("Motor")] protected float m_RootMotionRotationMultiplier = 1;
        [SerializeField, Group("Motor")] protected Vector3 m_groundAcceleration = new Vector3(0.18f, 0, 0.18f);
        [SerializeField, Group("Motor")] protected Vector3 m_AirborneAcceleration = new Vector3(0.15f, 0, 0.15f);
        [SerializeField, Group("Motor")] protected float m_motorDamping = 0.3f;
        [SerializeField, Group("Motor")] protected float m_AirborneDamping = 0.3f;
        [SerializeField, Group("Motor")] protected float m_rotationSpeed = 4f;
        [SerializeField, Group("Motor")] protected float m_SlopeForceUp = 1f;
        [SerializeField, Group("Motor")] protected float m_SlopeForceDown = 1.25f;

        
        //  -- Physics variables
        [Group("Physics")]
        [SerializeField] protected float m_Mass = 100;
        [Group("Physics")]
        [SerializeField] protected float m_skinWidth = 0.08f;
        [Group("Physics"), Range(0, 90)]
        [SerializeField] protected float m_SlopeLimit = 45f;
        [Group("Physics")]
        [SerializeField] protected float m_MaxStepHeight = 0.4f;
        [Group("Physics")]
        [SerializeField] protected float m_gravityModifier = 0.4f;
        [Group("Physics")]
        [SerializeField] protected float m_groundStickiness = 6f;

        //  -- Collision detection
        [Group("Collisions")]
        [SerializeField] protected LayerMask m_collisionsLayerMask;
        [Group("Collisions")]
        [SerializeField] protected int m_MaxCollisionCount = 100;




        //  -- Animation
        [Group("Animation")]
        [SerializeField] protected float m_IdleRotationMultiplier = 2f;


        #endregion

        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();


        protected CapsuleCollider charCollider;
        protected float timeScale = 1;


        protected MovementTypes m_MovementType = MovementTypes.Adventure;

        protected bool m_moving, m_grounded = true;
        protected float m_previousMoveAngle, m_moveAngle;
        protected Vector3 m_inputVector, m_relativeInputVector;
        protected Vector3 m_moveDirection, m_velocity , m_angularVelocity;
        protected Quaternion m_moveRotation = Quaternion.identity;
        protected Vector3 m_gravity;

        //  root motion variables.
        Vector3 m_rootMotionVelocity;
        Quaternion m_rootMotionRotation;
        //  kinematic variables.
        Vector3 m_acceleration;
        Vector3 m_initialVelocity;
        Vector3 m_finalVelocity;
        Vector3 m_displacement;

        protected float m_groundAngle;
        protected Vector3 m_GroundSlopeDir;


        protected float m_spherecastRadius = 0.1f;
        protected RaycastHit m_groundHit;
        [SerializeField, Group("Collisions")]
        protected Collider[] probedColiders;
        [SerializeField, Group("Collisions")]
        protected RaycastHit[] probedHits;
        protected List<Collider> colliderBuffer = new List<Collider>();
        protected int totalColliderHits, totalRaycastHits;

        protected PhysicMaterial characterMaterial;


        protected Vector3 m_previousPosition, m_targetPosition;
        protected Vector3 m_previousVelocity;

        protected int motionTrajectoryResolution = 5;
        protected Vector3[] motionTrajectory;
        protected float deltaAngle;
        protected Vector3 targetAngularVelocity;
        protected float m_Speed;


        protected Vector3 moveDirectionSmooth, m_ExternalForceSmooth;
        protected Vector3 velocitySmooth;
        protected Vector3 externalForceSmooth;
        protected float rotationAngleSmooth, angularDragSmooth;

        float castDistance = 10;
        float airborneThreshold = 0.3f;


        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_LayerManager;
        protected Rigidbody m_rigidbody;
        protected GameObject m_GameObject;
        protected Transform m_transform;
        protected float m_deltaTime, deltaTime, fixedDeltaTime;



        #region Parameters for Editor

        //  For Editor.
        //  Debug parameters.

        
        //protected bool m_Debug;
        public CharacterControllerDebugger Debugger { get { return debugger; } }
        protected bool DebugGroundCheck { get { return Debugger.states.showGroundCheck; } }
        protected bool DebugCollisions { get { return Debugger.states.showCollisions; } }
        protected bool DebugMotion { get { return Debugger.states.showMotion; } }

        [SerializeField, HideInInspector]
        private bool displayMovement, displayPhysics, displayAnimations, displayActions;

        #endregion





        #region Properties

        public MovementTypes Movement { get { return m_MovementType; } }

        public bool Moving { get { return m_moving; } set { m_moving = value; } }

        public bool Grounded { get { return m_grounded; } set { m_grounded = value; } }



        //public Vector3 InputVector {
        //    get { return m_inputVector; }
        //    set {
        //        if (value.sqrMagnitude > 1) value.Normalize();
        //        m_inputVector = value;
        //    }
        //}
        //public Vector3 InputVector { get { return m_inputVector;} set{ m_inputVector = value; } }
        public Vector3 InputVector { get; set; }

        public Vector3 RelativeInputVector{
            get{
                m_relativeInputVector = Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(InputVector);
                return m_relativeInputVector;
            }
        }

        public Vector3 MoveDirection { get { return m_moveDirection; } set { m_moveDirection = value; } }

        public Quaternion LookRotation { get; set; } = default;

        public float Speed { get { return Mathf.Abs(m_Speed); } set { m_Speed = Mathf.Abs(value); } }

        public float RotationSpeed { get { return m_rotationSpeed; } set { m_rotationSpeed = value; } }

        public bool UseRootMotion { get { return m_useRootMotion; } set { m_useRootMotion = value; } }

        public Vector3 Gravity {
            get {
                m_gravity.y = m_gravity.y * m_gravityModifier;
                return m_gravity;
            }
            protected set { m_gravity = value; }
        }

        public Quaternion AngularRotation { get { return m_moveRotation; } set { m_moveRotation = value; } }

        public Vector3 Velocity { get { return m_velocity; } set { m_velocity = value; } }

        public CapsuleCollider Collider { get{ return charCollider; } protected set { charCollider = value; }}

        public RaycastHit GroundHit { get { return m_groundHit; } }



        public Vector3 RootMotionVelocity { get { return m_rootMotionVelocity; } set { m_rootMotionVelocity = value; } }

        public Quaternion RootMotionRotation { get { return m_rootMotionRotation; } set { m_rootMotionRotation = value; } }


        public float LookAngle{
            get{
                var lookDirection = m_transform.InverseTransformDirection(LookRotation * m_transform.forward);
                var axisSign = Vector3.Cross(lookDirection, m_transform.forward);
                return Vector3.Angle(m_transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            }
        }

        public Vector3 ColliderCenter{ get { return Collider.center; } }
        public float ColliderHeight { get { return Collider.height * m_transform.lossyScale.x; } }
        public float ColliderRadius { get { return Collider.radius * m_transform.lossyScale.x; } }
        public Vector3 RaycastOrigin { get { return m_transform.position + Vector3.up * m_skinWidth; } }

        #endregion





        private void Start()
        {
            m_animatorMonitor = GetComponent<AnimatorMonitor>();
            m_animator = GetComponent<Animator>();

            m_rigidbody = GetComponent<Rigidbody>();
            m_LayerManager = GetComponent<LayerManager>();

            charCollider = GetComponent<CapsuleCollider>();
            if (charCollider == null) {
                charCollider = gameObject.AddComponent<CapsuleCollider>();
                charCollider.radius = 0.3f;
                charCollider.height = MathUtil.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2);
                charCollider.center = new Vector3(0, charCollider.height / 2, 0);
            }



            m_collisionsLayerMask = m_LayerManager.SolidLayers;


            probedColiders = new Collider[m_MaxCollisionCount];
            probedHits = new RaycastHit[m_MaxCollisionCount];

            m_GameObject = gameObject;
            m_transform = transform;

            m_deltaTime = Time.deltaTime;

            Gravity = Physics.gravity;

            deltaTime = Time.deltaTime;
            fixedDeltaTime = Time.fixedDeltaTime;

            LookRotation = Quaternion.LookRotation(m_transform.forward);

            m_rigidbody.mass = m_Mass;
            m_rigidbody.useGravity = false;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            m_rigidbody.isKinematic = false;
            if (!m_rigidbody.isKinematic) m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            else {
                m_rigidbody.constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            }




            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.applyRootMotion = m_useRootMotion;

            characterMaterial = new PhysicMaterial() { name = "Character Physics Material" };

            //  Initialize debugger;
            Debugger.Initialize(this);
        }





        protected virtual void OnAnimatorMove()
        {
            Vector3 f = m_animator.deltaRotation * Vector3.forward;
            deltaAngle += Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;

            if (m_useRootMotion)
            {
                m_rootMotionVelocity = (m_animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_deltaTime;
                //if (m_animator.hasRootMotion) m_rigidbody.MovePosition(m_animator.rootPosition);
            }

            if (m_useRootMotion)
            {
                m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = (angle * m_RootMotionSpeedMultiplier * Mathf.Deg2Rad) / m_deltaTime;
                m_rootMotionRotation = Quaternion.AngleAxis(angle, axis);

                //if (m_animator.hasRootMotion) m_rigidbody.MoveRotation(m_animator.rootRotation);
            }

        }




        protected virtual void Move()
        {
            //if (InputVector.sqrMagnitude > 1) InputVector.Normalize();
            //m_inputVector = InputVector;
            ////m_inputVector = m_transform.InverseTransformDirection(m_inputVector);
            ////m_moveDirection = m_inputVector;



            ////  Set the input vector, move direction and rotation angle based on the movement type.
            //switch (m_MovementType) {
            //    case (MovementTypes.Adventure):
            //        //  Get the correct target rotation based on input.
            //        m_moveAngle = GetAngleFromForward(m_inputVector);
            //        //m_inputVector = m_transform.InverseTransformDirection(m_inputVector);
            //        //m_inputVector = m_transform.rotation * m_inputVector;
            //        m_inputVector = m_transform.InverseTransformDirection(m_inputVector);
            //        //m_moveDirection = m_inputVector;
            //        m_inputVector.z = Mathf.Clamp01(m_inputVector.z + (Mathf.Abs(m_moveAngle) * Mathf.Deg2Rad));
            //        //m_inputVector.x = InputVector.x;
            //        m_moveDirection = m_transform.forward * Mathf.Abs(m_inputVector.z);

            //        break;
            if (InputVector.sqrMagnitude > 1) InputVector.Normalize();
            m_inputVector = InputVector;
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType) {
                case (MovementTypes.Adventure):
                    //  Get the correct target rotation based on input.
                    m_moveAngle = GetAngleFromForward(m_inputVector);
                    m_inputVector = m_transform.InverseTransformDirection(m_inputVector);
                    m_inputVector.z = Mathf.Clamp01(m_inputVector.z + (Mathf.Abs(m_moveAngle) * Mathf.Deg2Rad));
                    m_moveDirection = m_transform.forward * Mathf.Abs(m_inputVector.z);
                    break;

                case (MovementTypes.Combat):
                    //  Get the correct target rotation based on look rotation.
                    m_moveAngle = GetAngleFromForward(LookRotation * m_transform.forward);
                    m_inputVector = m_transform.TransformDirection(m_inputVector);
                    m_moveDirection = Vector3.SmoothDamp(m_moveDirection, m_inputVector, ref moveDirectionSmooth, 0.1f);

                    break;
            }
            //  Is there enough movement to be considered moving.
            m_moving = m_moveDirection.sqrMagnitude > 0;

            //if (m_moving) {
            //    m_acceleration = Vector3Util.Multiply(m_groundAcceleration, m_moveDirection);
            //    m_initialVelocity += m_acceleration;
            //} else {
            //    m_acceleration = Vector3Util.Multiply(m_groundAcceleration, m_moveDirection);
            //    m_initialVelocity = Vector3.SmoothDamp(m_initialVelocity - m_acceleration, Vector3.zero, ref velocitySmooth, m_motorDamping);
            //}

            //  Set rotation.
            m_moveRotation = Quaternion.AngleAxis(m_moveAngle * m_rotationSpeed * m_deltaTime, m_transform.up);

            if (m_useRootMotion) {
                m_moveRotation *= m_rootMotionRotation;
                m_velocity = m_rootMotionVelocity;
            }

            m_velocity = m_moveRotation * m_velocity;
        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            if (m_grounded && !m_moving) return;

            Vector3 origin = m_transform.position + Vector3.up * (ColliderCenter.y - ColliderHeight / 2 + m_skinWidth);
            Vector3 spherecastOrigin = origin + Vector3.up * m_spherecastRadius;
            m_groundAngle = 0;

            float groundDistance = 10;
            m_groundHit = new RaycastHit();
            m_groundHit.point = m_transform.position - Vector3.up * airborneThreshold;
            m_groundHit.normal = m_transform.up;


            if (Physics.Raycast(origin, Vector3.down, out m_groundHit, airborneThreshold, m_collisionsLayerMask)) {
                groundDistance = Vector3.Project(m_transform.position - m_groundHit.point, transform.up).magnitude;
                m_groundAngle = Vector3.Angle(m_groundHit.normal, Vector3.up);

                if (Physics.SphereCast(spherecastOrigin, m_spherecastRadius, Vector3.down, out m_groundHit,
                    ColliderRadius + airborneThreshold, m_collisionsLayerMask))
                {
                    m_groundAngle = Vector3.Angle(m_groundHit.normal, m_transform.up);
                    if (groundDistance > m_groundHit.distance - m_skinWidth)
                        groundDistance = m_groundHit.distance - m_skinWidth;

                }  // End of SphereCast

            }  //  End of Raycast.



            if (groundDistance < 0.05f && m_groundAngle < 85) {
                m_grounded = true;
                //m_velocity += Vector3.ProjectOnPlane(m_groundHit.point - m_transform.position, m_transform.up * m_groundStickiness);
            } else {
                if (groundDistance > airborneThreshold)
                {    
                    m_groundAngle = 0;
                    m_groundHit.point = m_transform.position - Vector3.up * airborneThreshold;
                    m_groundHit.normal = m_transform.up;
                    m_grounded = false;
                }

            }


        }



        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {

        }


    
        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            if (m_grounded) {
                //m_velocity.y = 0;
                float velocityMag = m_velocity.magnitude;
                Vector3 groundNormal = m_groundHit.normal;

                //if (Vector3.Dot(m_moveDirection, groundNormal) >= 0){
                //    //  If greater than 0, than we are going down a slope.
                //} else {
                //    //  We are going up the slope if it is negative.
                //}

                Vector3 directionTangent = GetDirectionTangentToSurface(m_velocity, groundNormal) * velocityMag;

                DebugDraw.Arrow(m_transform.position.AddY(0.1f), directionTangent, Color.magenta);
                // Reorient target velocity.
                Vector3 inputRight = Vector3.Cross(m_velocity, m_transform.up);
                m_velocity = Vector3.Cross(groundNormal, inputRight).normalized * velocityMag;// m_moveDirection.magnitude;

                //m_velocity = Vector3.Lerp(m_velocity, m_velocity, 1f - Mathf.Exp(-15 * m_deltaTime));

            } else {
                Vector3 verticalVelocity = Vector3.Project(m_previousVelocity, m_gravity);
                verticalVelocity += m_gravity * m_gravityModifier * m_deltaTime;



                m_velocity += verticalVelocity;

                Debug.Log(verticalVelocity);
            }

        }


        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {


            m_moveRotation.ToAngleAxis(out float angle, out Vector3 axis);
            m_angularVelocity = axis.normalized * angle;

            DebugUI.Log(this, "Angle", angle, RichTextColor.Magenta);

            m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, m_angularVelocity, m_deltaTime * m_rotationSpeed);

        }



        /// <summary>
        /// Apply rotation.
        /// </summary>
        protected virtual void ApplyRotation()
        {
            
        }


        /// <summary>
        /// Apply position values.
        /// </summary>
        protected virtual void ApplyMovement()
        {
            //if (m_moving) {
            //    m_finalVelocity = Vector3.Le(m_finalVeloci, m_velocity, m_delTime * 5);
            //}
            //else {
            //    m_finalVelocity = Vector3.SmoothDamp(m_finalVelocity,_velocity, refelocitySmooth, m_motorDamping);
            //}
            //m_rigidbody.velocity = m_finalVelocity;
            m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, m_velocity, m_deltaTime * 10);
        }


        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            m_animator.SetBool(HashID.Moving, Moving);




            m_Speed = InputVector.normalized.sqrMagnitude * (Movement == MovementTypes.Adventure ? 1 : 0);
            m_animator.SetFloat(HashID.Speed, Speed);


            //m_animator.SetFloat(HashID.Rotation, m_moveAngle);



            m_animatorMonitor.SetForwardInputValue(m_inputVector.z);
            m_animatorMonitor.SetHorizontalInputValue(m_inputVector.x);

        }





        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected virtual void SetPhysicsMaterial()
        {
            //change the physics material to very slip when not grounded or maxFriction when is

            //  Airborne.
            if (!Grounded && Mathf.Abs(m_rigidbody.velocity.y) > 0) {
                charCollider.material.staticFriction = 0f;
                charCollider.material.dynamicFriction = 0f;
                charCollider.material.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            //  Grounded and is moving.
            else if (Grounded && Moving) {
                charCollider.material.staticFriction = 0.25f;
                charCollider.material.dynamicFriction = 0f;
                charCollider.material.frictionCombine = PhysicMaterialCombine.Multiply;
            }
            //  Grounded but not moving.
            else if (Grounded && !Moving) {
                charCollider.material.staticFriction = 1f;
                charCollider.material.dynamicFriction = 1f;
                charCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            } else {
                charCollider.material.staticFriction = 1f;
                charCollider.material.dynamicFriction = 1f;
                charCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            }

            //charCollider.material = m_AirFrictionMaterial;
        }






        public RaycastHit GetSphereCastGroundHit()
        {
            float radius = 0.1f;
            Vector3 origin = m_transform.position + Vector3.up * (ColliderCenter.y - ColliderHeight / 2 + m_skinWidth);
            origin += Vector3.up * radius;

            m_groundHit = new RaycastHit();
            m_groundHit.point = m_transform.position - Vector3.up * airborneThreshold;
            m_groundHit.normal = m_transform.up;

            Physics.SphereCast(origin, radius, Vector3.down, out m_groundHit, airborneThreshold * 2, m_collisionsLayerMask);

            return m_groundHit;
        }



        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            float scale = direction.magnitude;
            Vector3 temp = Vector3.Cross(surfaceNormal, direction);
            Vector3 tangent = Vector3.Cross(temp, surfaceNormal);
            tangent = tangent.normalized * scale;
            return tangent;
        }



        public void CollisionsOverlap(Vector3 position)
        {
            float castRadius = charCollider.radius + m_skinWidth;

            Vector3 p1 = position + charCollider.center + Vector3.up * (charCollider.height * 0.5f - castRadius);
            Vector3 p2 = position + charCollider.center - Vector3.up * (charCollider.height * 0.5f - castRadius);

            //DebugDraw.Capsule(position + charCollider.center, Vector3.up, castRadius, charCollider.height, Color.white);

            var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, probedColiders, m_collisionsLayerMask);

            if(hits > 0)
            {
                for (int i = 0; i < hits; i++) {
                    Debug.DrawLine(p1, colliderBuffer[i].ClosestPoint(p1), Color.red);
                    //colliderBuffer[i].
                }
            }

        }









        #region Public Functions


        protected Vector3 GetDisplacement(Vector3 initialVelocity, Vector3 acceleration, float time)
        {
            Vector3 displacement = initialVelocity * time + (acceleration * (time * time)) / 2;
            return displacement;
        }

        protected Vector3 GetFinalVelocity(Vector3 initialVelocity, Vector3 acceleration, float time)
        {
            return initialVelocity + acceleration * time; 
        }




        // Scale the capsule collider to 'mlp' of the initial value
        protected void ScaleCapsule( float scale )
        {
            scale = Mathf.Abs(scale);
            if (charCollider.height < ColliderHeight * scale || charCollider.height > ColliderHeight * scale) {
                charCollider.height = Mathf.MoveTowards(charCollider.height, ColliderHeight * scale, Time.deltaTime * 4);
                charCollider.center = Vector3.MoveTowards(charCollider.center, ColliderCenter * scale, Time.deltaTime * 2);
            }
        }


        public virtual float GetColliderHeightAdjustment()
        {
            return charCollider.height;
        }


        // Gets angle around y axis from a world space direction
        public float GetAngleFromForward(Vector3 worldDirection)
        {
            Vector3 local = transform.InverseTransformDirection(worldDirection);
            return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
        }



        // Rotate a rigidbody around a point and axis by angle
        protected void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 d = transform.position - point;
            m_rigidbody.MovePosition(point + rotation * d);
            m_rigidbody.MoveRotation(rotation * transform.rotation);
        }



        public bool CheckCapsule(Vector3 segment1, Vector3 segment2, float radius, LayerMask mask)
        {
            //Vector3 transformUp = m_transform.up;
            //Vector3 start = GetCapsulePoint(m_transform.position + ColliderCenter, transformUp);
            //Vector3 end = GetCapsulePoint(m_transform.position + ColliderCenter, -transformUp);
            return Physics.CheckCapsule(segment1, segment2, radius, mask);
        }


        public Vector3 GetCapsulePoint(Vector3 origin, Vector3 direction)
        {
            var pointsDist = ColliderHeight - (ColliderRadius * 2f);
            return origin + (direction * (pointsDist * .5f));
        }



        protected void ClearNonAllocArrays()
        {
            if (totalColliderHits > 0)
                for (int i = 0; i < totalColliderHits; i++)
                    probedColiders[i] = null;

            //if (totalRaycastHits > 0)
            //    for (int i = 0; i < totalRaycastHits; i++)
            //        probedHits[i] = 
        }





        #endregion










        #region Debugging





        protected virtual void DebugAttributes()
        {
            if (debugger.states.showDebugUI == false) return;

            DebugUI.Log(this, "m_grounded", m_grounded, RichTextColor.White);



            DebugUI.Log(this, "m_moveAngle", m_moveAngle, RichTextColor.Green);
            DebugUI.Log(this, "m_moveRotation", m_moveRotation, RichTextColor.Green);

            //DebugUI.Log(this, "m_velocity", m_velocity, RichTextColor.Cyan);
            //DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.Cyan);

            //DebugUI.Log(this, "m_angularVelocity", m_angularVelocity, RichTextColor.Magenta);
            //DebugUI.Log(this, "r_angularVelocity", m_rigidbody.angularVelocity, RichTextColor.Magenta);
        }



        protected abstract void DrawGizmos();



        private void OnDrawGizmos()
        {
            if (Debugger.debugMode && Application.isPlaying)
            {
                Debugger.DrawGizmos();
                //  Draw Gizmos
                DrawGizmos();
            }

        }


        #endregion





    }

}


