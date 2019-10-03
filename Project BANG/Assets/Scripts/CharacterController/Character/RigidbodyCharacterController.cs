using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;





namespace CharacterController
{
    using DebugUI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
    public abstract class RigidbodyCharacterController : MonoBehaviour
    {
        protected const float tinyOffset = .0001f;
        protected const float tinyTolerance = .001f;
        protected const float tolerance = 0.05f;
        public enum MovementTypes { Adventure, Combat };


        

        #region Inspector properties

        //  Locomotion variables
        [SerializeField, HideInInspector] protected bool m_useRootMotionPosition = true;
        [FormerlySerializedAs("m_useRootMotionRotation")]
        [SerializeField, HideInInspector] protected bool m_useRootMotionRotation;
        [SerializeField, HideInInspector] protected float m_rootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector] protected float m_rootMotionRotationMultiplier = 1;
        [FormerlySerializedAs("m_motorAcceleration")]
        [SerializeField, HideInInspector] protected float m_motorAcceleration = 2.5f;
        [FormerlySerializedAs("m_motorDamping")]
        [Range(0, 1)]
        [SerializeField, HideInInspector] protected float m_motorDamping = 0.3f;
        [FormerlySerializedAs("m_acceleration")]
        [SerializeField, HideInInspector] protected Vector3 m_acceleration = new Vector3(0.18f, 0, 0.18f);
        [FormerlySerializedAs("m_desiredSpeed")]
        [Range(0, 10)]
        [SerializeField, HideInInspector] protected float m_desiredSpeed = 4f;
        [FormerlySerializedAs("m_rotationSpeed")]
        [SerializeField, HideInInspector] protected float m_rotationSpeed = 4f;



        //  -- Physics variables
        [FormerlySerializedAs("m_mass")]
        [SerializeField, HideInInspector] protected float m_mass = 100;
        [FormerlySerializedAs("m_skinWidth")]
        [SerializeField, HideInInspector] protected float m_skinWidth = 0.08f;
        [FormerlySerializedAs("m_slopeLimit"), Range(0, 85)]
        [SerializeField, HideInInspector] protected float m_slopeLimit = 45f;
        [FormerlySerializedAs("m_maxStepHeight")]
        [SerializeField, HideInInspector] protected float m_maxStepHeight = 0.4f;
        [FormerlySerializedAs("m_gravityModifier")]
        [SerializeField, HideInInspector] protected float m_gravityModifier = 1f;
        [FormerlySerializedAs("m_groundStickiness")]
        [SerializeField, HideInInspector] protected float m_groundStickiness = 2f;

        //  -- Collision detection
        [FormerlySerializedAs("m_collisionsLayerMask")]
        [SerializeField, HideInInspector] protected LayerMask m_collisionsLayerMask;
        [FormerlySerializedAs("m_maxCollisionCount")]
        [SerializeField, HideInInspector] protected int m_maxCollisionCount = 32;



        protected float m_backwardsMultiplier = 0.7f;
        [Range(0, 4), Tooltip("The local time scale")]
        protected float m_timeScale = 1;

        protected float m_maxSpeed = 6f;

        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();


        // --- Kinematic




        #endregion





        protected CapsuleCollider m_collider;

        protected MovementTypes m_MovementType = MovementTypes.Adventure;

        protected bool m_moving, m_grounded = true;
        protected float m_targetAngle, m_viewAngle;
        protected Vector3 m_inputVector, m_inputDirection;
        protected Vector3 m_moveDirection, m_angularVelocity;
        protected Vector3 m_movePosition;
        protected Quaternion m_targetRotation = Quaternion.identity, m_moveRotation = Quaternion.identity, m_lookRotation = Quaternion.identity;
        protected Vector3 m_gravity;
        protected Vector3 m_lookDirection, m_smoothLookDirection;

        //  root motion variables.
        protected Vector3 m_rootMotionVelocity;
        protected Quaternion m_rootMotionRotation;
        protected Vector3 m_deltaPosition;
        protected Quaternion m_deltaRotation;

        protected float m_airborneThreshold = 0.3f;
        protected float m_groundAngle;

        protected float m_spherecastRadius = 0.1f;
        protected RaycastHit m_groundHit;
        protected Collider[] m_probedColliders;



        protected PhysicMaterial m_physicsMaterial;

        protected float m_previousAngle;
        protected Vector3 m_previousPosition;
        protected Vector3 m_velocity;
        protected Quaternion m_previousRotation;


        protected float m_moveAmount;
        protected float m_speed;




        protected Vector3 m_verticalVelocity;
        


        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_layerManager;
        protected Rigidbody m_rigidbody;
        protected GameObject m_gameObject;
        protected Transform m_transform;
        protected float m_deltaTime;


        [Group("Movement")]
        [Range(0, 10)]
        //[SerializeField]
        protected int m_sampleRate = 4;
        [Group("Movement")]
        [Range(0, 10)]
        //[SerializeField]
        protected float m_positionBias = 1.2f;
        [Group("Movement")]
        [Range(0, 10)]
        //[SerializeField]
        protected float m_directionBias = 1.2f;
        //[SerializeField]
        protected Trajectory m_motionPath;


        #region Parameters for Editor

        //  For Editor.
        //  Debug parameters.


        //protected bool m_Debug;
        [HideInInspector]
        public CharacterControllerDebugger Debugger { get { return debugger; } }
        protected bool DebugGroundCheck { get { return Debugger.states.showGroundCheck; } }
        protected bool DebugCollisions { get { return Debugger.states.showCollisions; } }
        protected bool DebugMotion { get { return Debugger.states.showMotion; } }

        [SerializeField, HideInInspector]
        private bool displayMovement, displayPhysics, displayCollisions, displayAnimations, displayActions;

        #endregion





        #region Properties

        public MovementTypes Movement { get { return m_MovementType; } }

        public bool Moving { get { return m_moving; } set { m_moving = value; } }

        public bool Grounded { get { return m_grounded; } set { m_grounded = value; } }

        public Vector3 InputVector { get { return m_inputVector; } set { m_inputVector = value; } }
        //public Vector3 InputVector { get; set; }

        public Vector3 InputDirection { get { return m_inputDirection; } }

        public Vector3 MoveDirection { get { return m_moveDirection; } set { m_moveDirection = value; } }

        public Vector3 LookDirection { get { return m_lookDirection; } }

        public Quaternion LookRotation { get { return m_lookRotation; } set { m_lookRotation = value; } }

        public float Speed { get { return Mathf.Abs(m_speed); } set { m_speed = Mathf.Abs(value); } }

        public float RotationSpeed { get { return m_rotationSpeed; } set { m_rotationSpeed = value; } }

        public bool UseRootMotion { get { return m_useRootMotionPosition; } set { m_useRootMotionPosition = value; } }

        public Vector3 Gravity
        {
            get {
                m_gravity.y = m_gravity.y * m_gravityModifier;
                return m_gravity;
            }
            protected set { m_gravity = value; }
        }

        public Quaternion MoveRotation { get { return m_targetRotation; } set { m_targetRotation = value; } }





        public CapsuleCollider Collider { get { return m_collider; } protected set { m_collider = value; } }

        public RaycastHit GroundHit { get { return m_groundHit; } }



        public Vector3 RootMotionVelocity { get { return m_rootMotionVelocity; } set { m_rootMotionVelocity = value; } }

        public Quaternion RootMotionRotation { get { return m_rootMotionRotation; } set { m_rootMotionRotation = value; } }





        public float LookAngle
        {
            get {
                var lookDirection = m_transform.InverseTransformDirection(LookRotation * m_transform.forward);
                var axisSign = Vector3.Cross(lookDirection, m_transform.forward);
                return Vector3.Angle(m_transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            }
        }




        protected Vector3 m_colliderCenter;
        protected float m_colliderHeight, m_colliderRadius;

        #endregion



        protected void InitializeVariables()
        {
            //  Cached variables.
            m_lookDirection = m_transform.forward;
            m_lookRotation = Quaternion.LookRotation(m_transform.forward);
            m_previousPosition = m_transform.position;
            m_velocity = Vector3.zero;


            //  Colider properties.
            m_colliderCenter = m_collider.center;
            m_colliderHeight = m_collider.height * m_transform.lossyScale.x;
            m_colliderRadius = m_collider.radius * m_transform.lossyScale.x;

            //  Rigidbody properties.

            m_rigidbody.mass = m_mass;
            m_rigidbody.useGravity = false;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            m_rigidbody.isKinematic = true;

            if (!m_rigidbody.isKinematic)   //  Not kinematic
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            else {
                //m_rigidbody.interpolation = RigidbodyInterpolation.None;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }



            //  Motion path
            m_motionPath = new Trajectory(1, m_sampleRate, new AffineTransform(m_transform));

            if (m_rigidbody.isKinematic) Debug.LogFormat("<b><color=yellow>[{0}]</color> is kinematic. </b>", this.gameObject.name);
        }



        protected virtual void AnimatorMove()
        {

            if (m_useRootMotionPosition) {
                m_deltaPosition = m_animator.deltaPosition * m_rootMotionSpeedMultiplier;
                m_rootMotionVelocity = m_deltaPosition / m_deltaTime;
                //if (m_animator.hasRootMotion) m_rigidbody.MovePosition(m_animator.rootPosition);
            }

            if (m_useRootMotionPosition) {
                //m_previousRotation *= m_animator.deltaRotation;

                m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = (angle * m_rootMotionRotationMultiplier * Mathf.Deg2Rad) / m_deltaTime;
                m_rootMotionRotation = Quaternion.AngleAxis(angle, axis);
            }


        }



        protected virtual void InternalMove()
        {

            m_velocity = m_useRootMotionPosition ? m_deltaPosition / m_deltaTime : (m_transform.position - m_previousPosition) / m_deltaTime;
            m_previousPosition = m_transform.position;

            m_velocity = m_rigidbody.velocity;
            DebugUI.Log(this, "m_velocity", m_velocity, RichTextColor.Red);

            m_lookDirection = m_lookRotation * m_transform.forward;
            m_previousAngle = m_viewAngle;
            m_viewAngle = m_transform.AngleFromForward(m_lookDirection);

            //  float h = Input.GetAxisRaw("Horizontal");
            //  float v = Input.GetAxisRaw("Vertical");
            //  movement.Set(h, 0f, v);
            //  movement = movement.normalized * speed * Time.deltaTime;
            //  movement = transform.worldToLocalMatrix.inverse * movement;
            //  playerRigidbody.MovePosition(transform.position + movement);
        }



        /// <summary>
        /// Apply rotation.
        /// </summary>
        protected virtual void ApplyRotation()
        {
            //m_targetRotation = Quaternion.Slerp(m_transform.rotation, m_targetRotation, 1 - Mathf.Exp(-m_rotationSpeed) * m_deltaTime);
            //m_targetRotation = Quaternion.Slerp(m_transform.rotation, m_targetRotation, m_rotationSpeed * m_deltaTime);
            m_rigidbody.MoveRotation(m_targetRotation * m_rigidbody.rotation);

        }


        /// <summary>
        /// Apply position values.
        /// </summary>
        protected virtual void ApplyMovement()
        {
            //m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, m_moveDirection, m_deltaTime * 8);
            //m_rigidbody.velocity = Vector3.zero;

            m_movePosition = m_rigidbody.position + m_moveDirection * m_deltaTime;
            DebugUI.Log(this, "m_moveDirection", m_moveDirection, RichTextColor.Red);
            DebugUI.Log(this, "m_movePosition", m_movePosition, RichTextColor.Red);
            m_rigidbody.MovePosition(m_movePosition);
        }


        


        protected virtual void Move()
        {
            if (m_inputVector.sqrMagnitude > 1)
                m_inputVector.Normalize();
            m_moveAmount = Mathf.Clamp01(Mathf.Abs(m_inputVector.x) + Mathf.Abs(m_inputVector.z));
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType)
            {
                case (MovementTypes.Adventure):
                    //  Get the correct input direction.

                    //m_targetAngle = Mathf.Atan2(m_inputVector.x, m_inputVector.z) * Mathf.Rad2Deg;
                    //Vector3 local = transform.InverseTransformDirection(m_lookDirection);
                    m_targetAngle =m_transform.AngleFromForward(m_lookDirection);
                    m_inputDirection =  m_transform.forward * m_moveAmount;
                    break;
                case (MovementTypes.Combat):
                    //  Get the correct input direction.
                    //var local = m_transform.InverseTransformDirection(m_inputVector);
                    //m_targetAngle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
                    m_inputDirection = m_transform.TransformDirection(m_inputVector);
                    break;
            }
            

            //  Calculate linear velocity
            Vector3 drag = m_velocity * MathUtil.SmoothStop3(m_motorDamping);
            if (m_inputDirection.sqrMagnitude > 0)
                drag *= (1 - Vector3.Dot(m_inputDirection.normalized, m_velocity.normalized));
            if (drag.sqrMagnitude > m_velocity.sqrMagnitude)
                m_velocity = Vector3.zero;
            else
                m_velocity -= drag;

            m_velocity += m_inputDirection * (m_motorAcceleration * m_deltaTime);
            if (m_velocity.sqrMagnitude > m_desiredSpeed * m_desiredSpeed)
                m_velocity = m_velocity.normalized * m_desiredSpeed;

            m_moveDirection = m_inputDirection + m_velocity;

            //  Is there enough movement to be considered moving.
            m_moving = m_inputDirection.sqrMagnitude > 0;




            //if(m_moveAmount > 0)
            //    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
            //else
            //    m_targetRotation = m_transform.rotation;
            m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);


            if (m_moving) {
                float moveLength = m_moveDirection.magnitude;
                float velocityLength = m_velocity.magnitude;
                DebugDrawer.DrawArrow(m_transform.position.WithY(0.5f), m_moveDirection, Color.blue, moveLength, 0.12f * moveLength);
                DebugDrawer.DrawArrow(m_transform.position.WithY(0.25f), m_velocity, Color.green, velocityLength, 0.12f * velocityLength);
            }
            //DebugDrawer.DrawArrow(m_transform.position.WithY(0.25f), m_inputDirection, Color.yellow);

            DebugUI.Log(this, "m_targetAngle", m_targetAngle, RichTextColor.Cyan);
            DebugUI.Log(this, "m_inputDirection", m_inputDirection, RichTextColor.Cyan);
        }









        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            //if (m_grounded && !m_moving) return;

            Vector3 origin = m_transform.position + Vector3.up * (m_colliderCenter.y - m_colliderHeight / 2 + m_skinWidth);
            Vector3 spherecastOrigin = origin + Vector3.up * m_spherecastRadius;
            float groundAngle = 0;

            float groundDistance = 10;
            m_groundHit = new RaycastHit();
            m_groundHit.point = m_transform.position - Vector3.up * m_airborneThreshold;
            m_groundHit.normal = m_transform.up;


            if (Physics.Raycast(origin, Vector3.down, out m_groundHit, m_airborneThreshold, m_collisionsLayerMask)) {
                groundDistance = Vector3.Project(m_transform.position - m_groundHit.point, transform.up).magnitude;
                groundAngle = Vector3.Angle(m_groundHit.normal, Vector3.up);

                if (Physics.SphereCast(spherecastOrigin, m_spherecastRadius, Vector3.down, out m_groundHit,
                    m_colliderRadius + m_airborneThreshold, m_collisionsLayerMask)) {
                    groundAngle = Vector3.Angle(m_groundHit.normal, m_transform.up);
                    if (groundDistance > m_groundHit.distance - m_skinWidth)
                        groundDistance = m_groundHit.distance - m_skinWidth;

                }  // End of SphereCast

            }  //  End of Raycast.



            if (groundDistance < 0.05f) {
                m_grounded = true;
                m_groundAngle = groundAngle;
            }
            else {
                if (groundDistance > m_airborneThreshold) {
                    m_groundAngle = 0;
                    m_grounded = false;
                    DebugDrawer.DrawSphere(m_groundHit.point, m_colliderRadius + m_airborneThreshold, Color.red);
                }

            }


        }






        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {

            ////  Set maxDepentration velocity.
            //m_rigidbody.maxDepenetrationVelocity = 0.01f;


            //float castRadius = m_collider.radius + m_skinWidth;
            //Vector3 castPosition = transform.position.WithYZ(castRadius, castRadius + m_skinWidth);
            //int hits = Physics.OverlapSphereNonAlloc(castPosition, castRadius, m_probedColliders, m_collisionsLayerMask);
            //if (hits > 0) {
            //    for (int i = 0; i < hits; i++) {
            //        if (m_probedColliders[i] == m_collider) continue;
            //        Collider collision = m_probedColliders[i];
            //        Vector3 position = castPosition;
            //        Vector3 contactPoint = collision.ClosestPoint(position);

            //        if (contactPoint != Vector3.zero)
            //        {

            //            //CalculatePenetration();


            //            Vector3 contactDirection = contactPoint - m_transform.position;

            //            float colliderAngle = Vector3.Dot(m_transform.forward, contactDirection.normalized);
            //            if (colliderAngle > 0) {
            //                Vector3 normal = m_transform.position - contactPoint;   //  Normal vector
            //                Vector3 v2 = Vector3.Cross(normal, Vector3.up);         //  Wall angle

            //                Vector3 slideDir = normal * Vector3.Dot(m_moveDirection, normal);
            //                Vector3 wallSlideDir = (m_moveDirection - slideDir).normalized;
            //                Vector3 projectedVelocity = Vector3.Project(m_moveDirection, wallSlideDir);
            //                Vector3 tangent = projectedVelocity;
            //                Vector3.OrthoNormalize(ref normal, ref tangent);
            //                //  ---------------
            //                // ----- Debugging
            //                //  ---------------
            //                //Debug.DrawRay(position, m_moveDirection, Color.blue);
            //                //Debug.DrawRay(position.WithY(.1f), tangent, Color.green);


            //            }
            //            //  ---------------
            //            // ----- Debugging
            //            //  ---------------
            //            if (DebugCollisions) {
            //                //  Draw a line to the contact point.
            //                Debug.DrawRay(position, position - contactPoint, Color.yellow);
            //                DebugDraw.Sphere(position, 0.05f, Color.yellow);
            //                DebugUI.Log(this, collision.name, Vector3.Dot(m_transform.forward, (position - m_transform.position).normalized), RichTextColor.LightBlue);
            //            }
            //        }
            //    }

            //    //  Draw the spherecast.
            //    //if(DebugCollisions) DebugDraw.DrawCapsule(p1, p2, castRadius, Color.gray);
            //    if (DebugCollisions) DebugDraw.Sphere(castPosition, castRadius, Color.gray);
            //}


        }




        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            //if (m_grounded) {



            //    float velocitySpeed = Vector3.Dot(m_moveDirection, m_transform.forward);
            //    Vector3 groundNormal = m_groundHit.normal;


            //    if (Vector3.Dot(m_moveDirection, groundNormal) >= 0) {
            //        //  If greater than 0, than we are going down a slope.
            //    }
            //    else {
            //        //  We are going up the slope if it is negative.
            //    }

            //    Vector3 directionTangent = GetDirectionTangentToSurface(m_moveDirection, groundNormal) * velocitySpeed;

            //    // Reorient target velocity.
            //    Vector3 inputRight = Vector3.Cross(m_moveDirection, m_transform.up);
            //    m_moveDirection = Vector3.Cross(groundNormal, inputRight).normalized * velocitySpeed;// m_moveDirection.magnitude;

            //    m_verticalVelocity.Zero();
            //}
            //else {

            //    Vector3 verticalVelocity = Vector3.Project(m_velocity, m_gravity);
            //    m_verticalVelocity = verticalVelocity + Gravity * m_deltaTime;
            //}

            

            //m_moveDirection += m_verticalVelocity;
        }



        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            //if(m_lookDirection.sqrMagnitude > 0) {
            //    m_targetAngle = Mathf.Lerp(m_targetAngle, m_viewAngle, 1 - Mathf.Exp(-m_rotationSpeed * m_deltaTime));

            //    //m_targetRotation = Quaternion.LookRotation(m_inputDirection, Vector3.up);

            //    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
            //}


            //m_targetAngle = m_transform.AngleFromForward(m_lookDirection);
            //m_targetAngle = Vector3.SignedAngle(m_smoothLookDirection, m_lookDirection, m_transform.up);

            //m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_deltaTime, m_transform.up);
            //float t = m_rotationSpeed * m_deltaTime;
            //m_targetRotation = Quaternion.LookRotation(m_smoothLookDirection, m_transform.up);


        }






        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            m_animator.SetBool(HashID.Moving, Moving);

            m_speed = 2;
            m_animator.SetFloat(HashID.Speed, m_speed);

            //if (Mathf.Approximately(m_targetAngle, 0)) m_targetAngle = 0;
            //m_animator.SetFloat(HashID.Rotation, m_targetAngle);


            //if (Mathf.Approximately(m_viewAngle, 0)) m_viewAngle = 0;
            //m_animator.SetFloat(HashID.LookAngle, m_viewAngle);



            //m_animator.SetFloat(HashID.Rotation, m_targetAngle);
            m_animatorMonitor.SetForwardInputValue(m_inputVector.z * 2);
            m_animatorMonitor.SetHorizontalInputValue(m_inputVector.x * 2);
        }




        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected virtual void SetPhysicsMaterial()
        {
            //change the physics material to very slip when not grounded or maxFriction when is

            ////  Airborne.
            //if (!Grounded && Mathf.Abs(m_rigidbody.velocity.y) > 0) {
            //    m_collider.material.staticFriction = 0f;
            //    m_collider.material.dynamicFriction = 0f;
            //    m_collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
            //}
            ////  Grounded and is moving.
            //else if (Grounded && Moving) {
            //    m_collider.material.staticFriction = 0.25f;
            //    m_collider.material.dynamicFriction = 0f;
            //    m_collider.material.frictionCombine = PhysicMaterialCombine.Multiply;
            //}
            ////  Grounded but not moving.
            //else if (Grounded && !Moving) {
            //    m_collider.material.staticFriction = 1f;
            //    m_collider.material.dynamicFriction = 1f;
            //    m_collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            //}
            //else {
            //    m_collider.material.staticFriction = 1f;
            //    m_collider.material.dynamicFriction = 1f;
            //    m_collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            //}

            //m_collider.material = m_AirFrictionMaterial;
        }













        protected virtual Vector3 GetLinearVelocity(float speed, bool local = false)
        {
            //Vector3 moveDirection = m_lookRotation * m_transform.forward;
            //Vector3 forward = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
            //Vector3 linearVelocity = Quaternion.FromToRotation(Vector3.forward, forward) * m_inputVector;
            //linearVelocity 
            //float moveAmount = Mathf.Abs(m_inputVector.x) + Mathf.Abs(m_inputVector.z);
            //if (moveAmount >= 0.2f || m_inputVector.sqrMagnitude >= 0.2f) {
            //    Vector3 moveDirection = m_lookRotation * m_transform.forward;
            //    Vector3 forward = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
            //    Vector3 linearVelocity = Quaternion.FromToRotation(Vector3.forward, forward) * m_inputVector;
            //    linearVelocity = linearVelocity.Multiply(m_desiredSpeed);

            //    return world ? linearVelocity : Quaternion.Inverse(m_transform.rotation) * linearVelocity;
            //}

            return Vector3.zero;
        }






        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            float scale = direction.magnitude;
            Vector3 temp = Vector3.Cross(surfaceNormal, direction);
            Vector3 tangent = Vector3.Cross(temp, surfaceNormal);
            tangent = tangent.normalized * scale;
            return tangent;
        }








        #region Public Functions


        private bool ComputePenetration(Vector3 currentPosition, Vector3 positionOffset,
                                        Collider collision, Vector3 colliderPosition, Quaternion colliderRotation,
                                        out Vector3 direction, out float distance)
        {
            if(collision == m_collider) {
                direction = Vector3.one;
                distance = 0.0f;
                return false;
            }

            bool result = Physics.ComputePenetration(m_collider, currentPosition + positionOffset, Quaternion.identity,
                                                     collision, colliderPosition, colliderRotation,
                                                     out direction, out distance);

            return result;
        }











        private void CalculatePenetration()
        {
            float castRadius = m_collider.radius + m_skinWidth;
            //Vector3 castPosition = m_transform.position.WithY(castRadius);
            Vector3 point0 = GetCapsulePoint(m_transform.position.WithY(m_skinWidth), Vector3.down);
            Vector3 point1 = GetCapsulePoint(m_transform.position.WithY(m_skinWidth), Vector3.up);
            int hits = Physics.OverlapCapsuleNonAlloc(point0, point1, castRadius, m_probedColliders, m_collisionsLayerMask);
            if (hits > 0) {
                for (int i = 0; i < hits; i++) {
                    if (m_probedColliders[i] == m_collider) continue;
                    Collider collision = m_probedColliders[i];
                    Vector3 position = m_transform.position.WithY(castRadius);
                    
                    Vector3 contactPoint = collision.ClosestPoint(position);

                    if (contactPoint != Vector3.zero) {
                        //Vector3 contactDirection = contactPoint - m_transform.position;

                        bool overlapped = Physics.ComputePenetration(m_collider, m_transform.position, m_transform.rotation,
                                                                        collision, collision.transform.position, collision.transform.rotation,
                                                                        out Vector3 direction, out float distance);
                        if (overlapped) {
                            Vector3 penetrationVector = direction * distance;
                            Vector3 moveDirectionProjected = Vector3.Project(m_moveDirection, -penetrationVector);

                            m_rigidbody.MovePosition(penetrationVector);
                            m_moveDirection = m_moveDirection.Subtract(moveDirectionProjected);      //  TODO: optimize me
                                                                                                     //m_moveDirection = Vector3.ClampMagnitude(m_moveDirection, 10);
                        }
                    }
                }

                if (DebugCollisions) DebugDraw.DrawCapsule(point0, point1, castRadius, Color.white);
            }
        }



        public RaycastHit SphereGroundCast(float radius, float maxDistance)
        {
            m_groundHit = new RaycastHit();
            m_groundHit.point = m_transform.position.WithY(-m_airborneThreshold);
            m_groundHit.normal = m_transform.up;

            float castHeight = m_skinWidth + radius;
            Physics.SphereCast(m_transform.position.WithY(castHeight), radius, Vector3.down, out m_groundHit, castHeight + maxDistance, m_collisionsLayerMask);

            return m_groundHit;
        }


        // Scale the capsule collider to 'mlp' of the initial value
        protected void ScaleCapsule(float scale)
        {
            scale = Mathf.Abs(scale);
            if (m_collider.height < m_colliderHeight * scale || m_collider.height > m_colliderHeight * scale) {
                m_collider.height = Mathf.MoveTowards(m_collider.height, m_colliderHeight * scale, Time.deltaTime * 4);
                m_collider.center = Vector3.MoveTowards(m_collider.center, m_colliderCenter * scale, Time.deltaTime * 2);
            }
        }


        public virtual float GetColliderHeightAdjustment()
        {
            return m_collider.height;
        }


        // Gets angle around y axis from a world space direction
        public float GetAngleFromForward(Vector3 worldDirection)
        {
            Vector3 local = transform.InverseTransformDirection(worldDirection);
            return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
        }



        // Rotate a rigidbody around a point and axis by angle
        public void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 d = transform.position - point;
            m_rigidbody.MovePosition(point + rotation * d);
            m_rigidbody.MoveRotation(rotation * transform.rotation);
        }




        public Vector3 GetCapsulePoint(Vector3 origin, Vector3 direction)
        {
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (direction * (pointsDist * .5f));
        }



        protected void ClearNonAllocArrays()
        {
            for (int i = 0; i < m_probedColliders.Length; i++) {
                if (m_probedColliders[i] == null) break;
                m_probedColliders[i] = default;
            }
        }






        // protected void DetectEdge()
        // {
        //     //Vector3 toCenter = Math3d.ProjectVectorOnPlane(up, (controller.transform.position - hit.point).normalized * TinyTolerance);
        //     Vector3 toCenter = Vector3.ProjectOnPlane((controller.transform.position - hit.point).normalized * TinyTolerance, m_transform.up);
        //     Vector3 awayFromCenter = Quaternion.AngleAxis(-80.0f, Vector3.Cross(toCenter, up)) * -toCenter;

        //     Vector3 nearPoint = hit.point + toCenter + (up * TinyTolerance);
        //     Vector3 farPoint = hit.point + (awayFromCenter * 3);

        //     RaycastHit nearHit;
        //     RaycastHit farHit;

        //     Physics.Raycast(nearPoint, down, out nearHit, m_colliderRadius * 2, m_collisionsLayerMask);
        //     Physics.Raycast(farPoint, down, out farHit, m_colliderRadius * 2, m_collisionsLayerMask);

        //     // If we are currently standing on a ledge then the face nearest the center of the
        //     // controller should be steep enough to be counted as a wall. Retrieve the ground
        //     // it is connected to at it's base, if there exists any
        //     if (Vector3.Angle(nearHit.normal, up) > superColType.StandAngle || nearHit.distance > Tolerance)
        //     {
        //         SuperCollisionType col = null;

        //         if (nearHit.collider != null)
        //         {
        //             col = nearHit.collider.gameObject.GetComponent<SuperCollisionType>();
        //         }

        //         if (col == null)
        //         {
        //             col = defaultCollisionType;
        //         }

        //         // We contacted the wall of the ledge, rather than the landing. Raycast down
        //         // the wall to retrieve the proper landing
        //         if (Vector3.Angle(nearHit.normal, up) > col.StandAngle)
        //         {
        //             // Retrieve a vector pointing down the slope
        //             Vector3 r = Vector3.Cross(nearHit.normal, down);
        //             Vector3 v = Vector3.Cross(r, nearHit.normal);

        //             RaycastHit stepHit;

        //             if (Physics.Raycast(nearPoint, v, out stepHit, Mathf.Infinity, walkable, triggerInteraction))
        //             {
        //                 stepGround = new GroundHit(stepHit.point, stepHit.normal, stepHit.distance);
        //             }
        //         }
        //         else
        //         {
        //             stepGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
        //         }
        //     }
        // }


        protected void PredictTrajectory(Vector3 linearVelocity, float positionBias, float directionBias, bool debug = false)
        {
            float sampleRate = m_motionPath.sampleRate;
            int halfTrajectoryLength = m_motionPath.Length / 2;

            float desiredOrientation = 0;
            Vector3 desiredLinearDisplacement = linearVelocity / sampleRate;
            if (desiredLinearDisplacement.sqrMagnitude >= 0.1f) {
                desiredOrientation = Mathf.Atan2(desiredLinearDisplacement.x, desiredLinearDisplacement.z);
            }

            Quaternion targetRotation = Quaternion.AngleAxis(desiredOrientation, Vector3.up);

            Vector3[] trajectoryPositions = new Vector3[halfTrajectoryLength];
            trajectoryPositions[0] = m_motionPath[halfTrajectoryLength].translation;

            //            Debug.Log(desiredLinearDisplacement);
            //if(debug) DebugUI.Log(this, "desiredLinearDisplacement", desiredLinearDisplacement, RichTextColor.Aqua);
            for (int i = 1; i < halfTrajectoryLength; i++) {
                float percentage = (float)i / (float)(halfTrajectoryLength - 1);

                float oneMinusPercentage = 1.0f - percentage;
                float blendedWeightDisplacement = 1f - Mathf.Pow(oneMinusPercentage, positionBias);
                float blendedWeightOrientation = 1f - Mathf.Pow(oneMinusPercentage, directionBias);

                Vector3 trajectoryDisplacement = m_motionPath[halfTrajectoryLength + i].translation - m_motionPath[halfTrajectoryLength + i - 1].translation;
                Vector3 adjustedTrajectoryDisplacement = Vector3.Lerp(trajectoryDisplacement, desiredLinearDisplacement, blendedWeightDisplacement);

                trajectoryPositions[i] = trajectoryPositions[i - 1] + adjustedTrajectoryDisplacement;

                //if (debug) DebugUI.Log(this, "trajectoryPositions " + (halfTrajectoryLength + i).ToString(), trajectoryPositions[i] + " |Adjust: " + adjustedTrajectoryDisplacement, RichTextColor.Aqua);
                //DebugUI.Log(this, "trajectoryPositions " + i, trajectoryPositions[i - 1] + " |Adjust: " + adjustedTrajectoryDisplacement, RichTextColor.LightBlue);

                //m_motionPath[halfTrajectoryLength + i].q = Quaternion.Slerp(m_motionPath[halfTrajectoryLength + i].q, targetRotation, blendedWeightOrientation);
                m_motionPath[i - 1].rotation = Quaternion.Slerp(m_motionPath[i - 1].rotation, targetRotation, blendedWeightOrientation);
            }


            for (int i = 1; i < halfTrajectoryLength; i++) {
                m_motionPath[halfTrajectoryLength + i].Set(trajectoryPositions[i], m_motionPath[halfTrajectoryLength + i].rotation);

                var worldSpacePos = m_transform.TransformPoint(m_motionPath[halfTrajectoryLength + i].translation);
                var positionDifference = m_transform.localPosition + m_motionPath[halfTrajectoryLength + i].translation - worldSpacePos;
                var pos = worldSpacePos + positionDifference;

                if (DebugMotion) DebugDraw.Sphere(pos, 0.1f, Color.green);
                //Debug.DrawLine(m_motionPath[halfTrajectoryLength + i].translation.AddY(0.1f), m_motionPath[halfTrajectoryLength + i - 1].translation.AddY(0.1f), Color.green);
            }

            //if (debug)
            //    for (int i = 0; i < m_motionPath.Length; i++)
            //        DebugUI.Log(this, "trajectory " + i, m_motionPath[i].translation + " | " + m_motionPath[i].rotation.eulerAngles, RichTextColor.LightBlue);
        }



        #endregion










        #region Debugging





        protected virtual void DebugAttributes()
        {
            //if (debugger.states.showDebugUI == false) return;

            DebugUI.Log(this, "m_grounded", m_grounded, RichTextColor.White);

            //DebugUI.Log(this, "pivotWeight", m_animator.pivotWeight, RichTextColor.Green);
            //DebugUI.Log(this, "InputDirection", InputDirection, RichTextColor.Green);

            DebugUI.Log(this, "m_viewAngle", m_viewAngle, RichTextColor.White);

            


            //DebugUI.Log(this, "m_velocity0", m_moveDirection, RichTextColor.Cyan);
            DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.White);
            DebugUI.Log(this, "fwd_speed", Vector3.Dot(m_rigidbody.velocity, m_transform.forward), RichTextColor.White);

            //DebugUI.Log(this, "r_angularVelocity", m_rigidbody.angularVelocity, RichTextColor.Magenta);

            //DebugUI.Log(this, "transformPos", m_transform.position, RichTextColor.Orange);
            //DebugUI.Log(this, "rbPois", m_rigidbody.position, RichTextColor.Orange);
            //DebugUI.Log(this, "rootPos", m_animator.rootPosition, RichTextColor.Orange);


        }



        protected abstract void DrawGizmos();



        private void OnDrawGizmos()
        {
            if (Debugger.debugMode && Application.isPlaying) {



                Debugger.DrawGizmos();
                //  Draw Gizmos
                DrawGizmos();
            }

        }


        #endregion





    }

}




//protected virtual void KinematicMove()
//{
//    m_rigidbody.isKinematic = true;
//    m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

//    if (m_inputVector.sqrMagnitude > 1)
//        m_inputVector.Normalize();
//    m_lookDirection = m_lookRotation * m_transform.forward;

//    m_targetAngle = m_transform.AngleFromForward(m_lookDirection);
//    m_inputDirection = Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(m_inputVector);



//    Vector3 drag = m_moveDirection * MathUtil.SmoothStop2(m_motorDamping);
//    //if (m_inputDirection.sqrMagnitude > 0)
//    //    drag *= (1f - Vector3.Dot(m_lookDirection.normalized, m_moveDirection.normalized));
//    if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
//        m_moveDirection = Vector3.zero;
//    else
//        m_moveDirection -= drag;

//    Vector3 direction = m_transform.TransformDirection(m_inputVector);
//    m_moveDirection.x += direction.x * m_motorAcceleration;
//    m_moveDirection.z += direction.z * m_motorAcceleration;
//    m_moveDirection.y = 0;

//    if (m_moveDirection.sqrMagnitude > m_desiredSpeed * m_desiredSpeed)
//        m_moveDirection = m_moveDirection.normalized * m_desiredSpeed;


//    //m_movePosition = Vector3.Lerp(m_transform.position, m_transform.position + m_moveDirection, m_deltaTime);

//    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
//    m_movePosition = m_transform.position + m_moveDirection;
//    m_rigidbody.MoveRotation(m_transform.rotation * m_targetRotation);
//    m_rigidbody.MovePosition(m_movePosition);

//    //m_animatorMonitor.SetForwardInputValue(m_inputDirection.z);
//    //m_animatorMonitor.SetHorizontalInputValue(m_inputDirection.x);

//    DebugUI.Log(this, "m_inputVector1", m_inputVector, RichTextColor.Yellow);
//    DebugUI.Log(this, "m_inputDirection1", m_inputDirection, RichTextColor.Yellow);
//    DebugUI.Log(this, "moveDirection1", m_moveDirection, RichTextColor.Yellow);
//    DebugUI.Log(this, "m_moveAngle1", m_targetAngle, RichTextColor.Yellow);
//    DebugUI.Log(this, "moveDot", 1 - Vector3.Dot(m_inputDirection.normalized, m_moveDirection.normalized), RichTextColor.Yellow);
//}