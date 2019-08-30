namespace CharacterController
{
    using UnityEngine;
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
        [SerializeField, Group("Motor")] protected Vector3 m_GroundAcceleration = new Vector3(0.18f, 0, 0.18f);
        [SerializeField, Group("Motor")] protected float m_Acceleration = 0.12f;
        [SerializeField, Group("Motor")] protected float m_AirborneAcceleration = 0.2f;
        [SerializeField, Group("Motor")] protected float m_MotorDamping = 0.3f;
        [SerializeField, Group("Motor")] protected float m_AirborneDamping = 0.3f;
        [SerializeField, Group("Motor")] protected float m_GroundSpeed = 1f;
        [SerializeField, Group("Motor")] protected float m_AirborneSpeed = 0.5f;
        [SerializeField, Group("Motor")] protected float m_rotationSpeed = 4f;
        [SerializeField, Group("Motor")] protected float m_SlopeForceUp = 1f;
        [SerializeField, Group("Motor")] protected float m_SlopeForceDown = 1.25f;

        

        //  -- Physics variables
        
        [Group("Physics")]
        [SerializeField] protected float m_DetectObjectHeight = 0.4f;
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
        protected float m_previousTargetAngle, m_targetAngle;
        protected Vector3 m_inputVector, m_relativeInputVector;
        protected Vector3 m_moveDirection, m_velocity , m_angularVelocity;
        protected Quaternion m_targetRotation = Quaternion.identity;



        protected float groundAngle;
        protected Vector3 m_GroundSlopeDir;


        protected RaycastHit groundHit;
        [SerializeField, Group("Collisions")]
        protected Collider[] probedColiders;
        [SerializeField, Group("Collisions")]
        protected RaycastHit[] probedHits;
        protected List<Collider> colliderBuffer = new List<Collider>();
        protected int totalColliderHits, totalRaycastHits;

        protected PhysicMaterial characterMaterial;

        protected Vector3 previousForward;
        protected Vector3 previousPosition, currentPosition, m_targetPosition;
        protected Vector3 previousVelocity, currentVelocity, m_targetVelocity;

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
        protected AnimatorMonitor m_AnimationMonitor;
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

        public bool RotateToLookDir { get; set; }

        //public Vector3 InputVector{
        //    get { return m_inputVector; }
        //    set{
        //        if (value.sqrMagnitude > 1) value.Normalize();
        //        m_inputVector = value;
        //    }
        //}
        public Vector3 InputVector { get; set; }


        public Vector3 RelativeInputVector{
            get{
                m_relativeInputVector = m_transform.InverseTransformDirection(InputVector);
                return m_relativeInputVector;
                //return Quaternion.Inverse(m_transform.rotation) * InputVector;
            }
        }

        public Vector3 MoveDirection { get { return m_moveDirection; } set { m_moveDirection = value; } }

        public Quaternion LookRotation { get; set; } = default;

        public float Speed { get { return Mathf.Abs(m_Speed); } set { m_Speed = Mathf.Abs(value); } }

        public float RotationSpeed { get { return m_rotationSpeed; } set { m_rotationSpeed = value; } }

        public bool UseRootMotion { get { return m_useRootMotion; } set { m_useRootMotion = value; } }

        public Vector3 Gravity { get; protected set; }

        public Vector3 Velocity { get { return m_velocity; } set { m_velocity = value; } }

        public CapsuleCollider Collider { get{ return charCollider; } protected set { charCollider = value; }}

        public RaycastHit GroundHit { get { return groundHit; } }



        public Vector3 RootMotionVelocity { get; set; }

        public Quaternion RootMotionRotation { get; set; }

        public Vector3 LookDirection{
            get{
                var lookDirection = m_transform.InverseTransformDirection(LookRotation * m_transform.forward);
                return lookDirection;
            }
        }

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



        protected virtual void Awake()
        {
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_animator = GetComponent<Animator>();

            m_rigidbody = GetComponent<Rigidbody>();
            m_LayerManager = GetComponent<LayerManager>();

            charCollider = GetComponent<CapsuleCollider>();
            if (charCollider == null)
            {
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

            //  Initialize debugger;
            Debugger.Initialize(this);

        }


        protected void Start()
        {
            m_rigidbody.mass = m_Mass;
            m_rigidbody.useGravity = false;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            //m_rigidbody.isKinematic = true;


            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.applyRootMotion = m_useRootMotion;

            characterMaterial = new PhysicMaterial() { name = "Character Physics Material"};
        }


        private void OnEnable()
        {
            motionTrajectory = new Vector3[motionTrajectoryResolution];

            LookRotation = Quaternion.LookRotation(m_transform.forward);

            previousForward = m_transform.forward;
            previousPosition = m_rigidbody.position;
            previousVelocity = m_rigidbody.velocity;
        }


        protected virtual void Update()
        {
            timeScale = Time.timeScale;
            if (Math.Abs(timeScale) < float.Epsilon) return;
            m_deltaTime = deltaTime;


            previousPosition = m_rigidbody.position;
            previousVelocity = m_rigidbody.velocity;

            m_previousTargetAngle = m_targetAngle;
        }


        protected virtual void FixedUpdate()
        {
            if (Math.Abs(timeScale) < float.Epsilon) return;
            m_deltaTime = fixedDeltaTime;



        }


        protected virtual void LateUpdate()
        {
            DebugAttributes();
        }


        protected virtual void OnAnimatorMove()
        {
            Vector3 f = m_animator.deltaRotation * Vector3.forward;
            deltaAngle += Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;

            if (m_useRootMotion)
            {
                RootMotionVelocity = (m_animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_deltaTime;
                //if (m_animator.hasRootMotion) m_rigidbody.MovePosition(m_animator.rootPosition);
            }

            if (m_useRootMotion)
            {
                m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = (angle * m_RootMotionSpeedMultiplier * Mathf.Deg2Rad) / m_deltaTime;
                RootMotionRotation = Quaternion.AngleAxis(angle, axis);

                //if (m_animator.hasRootMotion) m_rigidbody.MoveRotation(m_animator.rootRotation);
            }

        }




        protected virtual void DebugAttributes()
        {
            DebugUI.Log(this, "Moving", Moving, RichTextColor.Brown);
            DebugUI.Log(this, "Grounded", Grounded, RichTextColor.Brown);

            DebugUI.Log(this, "m_inputVector", m_inputVector, RichTextColor.Green);
            DebugUI.Log(this, "m_moveDirection", m_moveDirection, RichTextColor.Green);
            DebugUI.Log(this, "m_velocity", m_velocity, RichTextColor.Yellow);
            DebugUI.Log(this, "m_targetVelocity", m_targetVelocity, RichTextColor.Yellow);
            DebugUI.Log(this, "rb_Velocity", m_rigidbody.velocity, RichTextColor.Green);
            DebugUI.Log(this, "rb_AngularVel", m_rigidbody.angularVelocity, RichTextColor.DarkBlue);

        }





        /// <summary>
        /// Move charatcer based on input values.
        /// </summary>
        protected virtual void Move()
        {
            if (InputVector.sqrMagnitude > 1) InputVector.Normalize();
            m_inputVector = InputVector;
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType) {
                case (MovementTypes.Adventure):
                    //  Get the correct target rotation based on input.
                    m_targetAngle = GetAngleFromForward(m_inputVector);
                    //m_inputVector = Quaternion.Inverse(m_transform.rotation) * m_inputVector;
                    m_inputVector = m_transform.InverseTransformDirection(m_inputVector);
                    m_inputVector.z = Mathf.Clamp01(m_inputVector.z + Mathf.Abs(m_targetAngle) * Mathf.Deg2Rad);
                    //m_moveDirection = Vector3.Scale(m_inputVector, m_transform.forward);
                    m_moveDirection = m_transform.forward * Mathf.Abs(m_inputVector.z);
                    break;

                case (MovementTypes.Combat):
                    //  Get the correct target rotation based on look rotation.
                    m_targetAngle = GetAngleFromForward(LookRotation * m_transform.forward);
                    m_inputVector = m_transform.TransformDirection(m_inputVector);
                    m_moveDirection = Vector3.SmoothDamp(m_moveDirection, m_inputVector, ref moveDirectionSmooth, 0.1f);

                    break;
            }


            //  Is there enough movement to be considered moving.
            m_moving = m_moveDirection.sqrMagnitude > 0;


            if(m_useRootMotion)
            {
                m_targetRotation = RootMotionRotation;
                m_targetVelocity = RootMotionVelocity;

            }

            m_targetVelocity = m_targetRotation * m_targetVelocity;
            m_targetPosition = previousPosition + Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(m_targetVelocity);



            m_velocity = m_rigidbody.velocity;

            //if (DebugMotion) Debug.DrawRay(m_transform.position + Vector3.up * 1.5f, m_targetVelocity, Color.blue);
            //if (DebugMotion) Debug.DrawLine(m_transform.position + Vector3.up * 1.8f, m_targetPosition + Vector3.up *1.8f, Color.green);
            //if (DebugMotion && Moving) DebugDraw.Sphere(m_targetPosition + Vector3.up * 1.8f, 0.1f, Color.green);
        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            //bool querriesHitTriggers = Physics.queriesHitTriggers;
            //Physics.queriesHitTriggers = false;

            //Vector3 RaycastOrigin = m_transform.position + Vector3.up * charCollider.radius;
            float radius = ColliderRadius * 0.9f;
            //radius = 0.1f;
            Vector3 origin = m_transform.position + Vector3.up * (ColliderCenter.y - ColliderHeight / 2 + m_skinWidth);
            Vector3 sphereCastOrigin = origin + Vector3.up * radius;
            groundAngle = 0;

            float groundDistance = 10;
            groundHit = new RaycastHit();
            groundHit.point = m_transform.position - Vector3.up * airborneThreshold;
            groundHit.normal = m_transform.up;


            if (Physics.Raycast(origin, Vector3.down, out groundHit, airborneThreshold, m_collisionsLayerMask))
            {
                groundDistance = Vector3.Project(m_transform.position - groundHit.point, transform.up).magnitude;
                groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);

                if (Physics.SphereCast(sphereCastOrigin, radius, Vector3.down, out groundHit, ColliderRadius + airborneThreshold, m_collisionsLayerMask))
                {
                    groundAngle = Vector3.Angle(groundHit.normal, m_transform.up);

                    if (groundAngle > m_SlopeLimit)
                    {
                        // Retrieve a vector pointing down the slope
                        Vector3 r = Vector3.Cross(groundHit.normal, -transform.up);
                        Vector3 v = Vector3.Cross(r, groundHit.normal);

                        //Get a position slightly above the controller position to raycast down the slope from to avoid clipping
                        Vector3 flushOrigin = groundHit.point + groundHit.normal * tinyOffset;

                        // Properties of the flushHit ground
                        if (Physics.Raycast(flushOrigin, v, out groundHit, radius * 2, m_collisionsLayerMask))//Perform Raycast
                        {
                            float cos = 1 / Mathf.Cos(groundAngle);
                            float smallerRadius = radius * 0.9f;
                            float hypS = smallerRadius * cos;
                            float hypB = radius * cos;

                            Vector3 circleCenterSmall = groundHit.point + groundHit.normal * smallerRadius;
                            Vector3 pointOnSurface = circleCenterSmall + hypS * transform.up * -1;
                            Vector3 circleCenterBig = pointOnSurface + hypB * transform.up;

                            groundHit.distance = Vector3.Distance(origin, circleCenterBig);

                            if (groundHit.distance < tinyOffset * tinyOffset)
                                groundHit.distance = 0;

                            groundHit.point = circleCenterBig - groundHit.normal * radius;

                        }

                        //againstWall = true;
                    }


                    if (groundDistance > groundHit.distance - m_skinWidth)
                        groundDistance = groundHit.distance - m_skinWidth;

                }  // End of SphereCast

            }  //  End of Raycast.



            if (groundDistance < 0.05f && groundAngle < 85)
            {
                m_grounded = true;

                if(groundDistance > 0)
                {
                    //Vector3 groundedPos = m_transform.position - m_transform.up * groundDistance;
                    //m_rigidbody.MovePosition(groundHit.point);
                    var averagePosition = GetAverageRaycast(charCollider.radius - m_skinWidth, charCollider.radius - m_skinWidth);
                    //m_rigidbody.MovePosition(averagePosition);
                    m_rigidbody.MovePosition(Vector3.MoveTowards(m_rigidbody.position, averagePosition, m_deltaTime * 4));
                }

            }
            else
            {
                if(groundDistance > airborneThreshold)
                {
                    m_grounded = false;
                    groundAngle = 0;
                    groundHit.point = m_transform.position + Vector3.up * airborneThreshold;
                    groundHit.normal = m_transform.up;
                }

            }




            //m_rigidbody.useGravity = !Grounded;
            //Physics.queriesHitTriggers = querriesHitTriggers;


            //  Draw Sphere cast
            if (DebugGroundCheck) DebugDraw.Sphere(groundHit.point + Vector3.up * radius, radius, Grounded ? Color.green : Color.grey);
            if (DebugGroundCheck) Debug.DrawLine(RaycastOrigin, groundHit.point, Grounded ? Color.green : Color.grey);
            if (DebugGroundCheck) DebugDraw.DrawMarker(groundHit.point, 0.1f, Grounded ? Color.green : Color.grey);



            DebugUI.Log(this, "GroundDistance", MathUtil.Round(groundDistance), RichTextColor.Brown);
            DebugUI.Log(this, "GroundAngle", MathUtil.Round(groundAngle), RichTextColor.Brown);
        }



        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {

            if(groundAngle > m_SlopeLimit)
            {
                //Grab the direction that the controller is moving in
                Vector3 absoluteMoveDirection = Vector3.ProjectOnPlane(m_transform.position - previousPosition, groundHit.normal);

                // Retrieve a vector pointing down the slope
                Vector3 r = Vector3.Cross(groundHit.normal, -m_transform.up);
                Vector3 v = Vector3.Cross(r, groundHit.normal);

                //Check the angle between the move direction of the controller and a vector down the slope. If less than 90 degrees then the player is moving down the slope return false
                float angle = Vector3.Angle(absoluteMoveDirection, v);

                //if (angle <= 90.0f)
                //    return false;

                //// Calculate where to place the controller on the slope, or at the bottom, based on the desired movement distance
                //Vector3 resolvedPosition = Math3d.ProjectPointOnLine(initialPosition, r, transform.position);
                //Vector3 direction = Math3d.ProjectVectorOnPlane(n, resolvedPosition - transform.position);

                //transform.position += direction;
            }




            //MotionTrajectory(m_targetVelocity);

            //float castRadius = charCollider.radius + m_skinWidth;

            //Vector3 p1 = RaycastOrigin + charCollider.center + Vector3.up * (charCollider.height * 0.5f - castRadius);
            //Vector3 p2 = RaycastOrigin + charCollider.center - Vector3.up * (charCollider.height * 0.5f - castRadius);

            //var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, probedColiders, m_collisionsLayerMask);
            //totalColliderHits += hits;
            //if (hits > 0)
            //{
            //    colliderBuffer.Clear();
            //    var startIndex = totalColliderHits > hits ? totalColliderHits - hits : 0;
            //    var centerMass = m_transform.TransformPoint(m_rigidbody.centerOfMass);
            //    for (int i = startIndex; i < hits; i++)
            //    {
            //        colliderBuffer[i] = probedColiders[i];
            //        var collision = colliderBuffer[i];
            //        Vector3 direction;
            //        float distance;
            //        if (Physics.ComputePenetration(charCollider, m_transform.position, m_transform.rotation,
            //                                        collision, collision.transform.position, collision.transform.rotation,
            //                                        out direction, out distance))
            //        {
            //            Vector3 penetrationVector = direction * distance;
            //            Vector3 moveDirectionProjected = Vector3.Project(m_moveDirection, -direction);
            //            //  m_transform.position = m_transform.position + penetrationVector;
            //            m_rigidbody.MovePosition(m_transform.position + penetrationVector);
            //            m_moveDirection -= moveDirectionProjected;

            //            Debug.DrawRay(centerMass, m_moveDirection, Color.red);
            //        }

            //        var closestPoint = colliderBuffer[i].ClosestPointOnBounds(centerMass);


            //        Debug.DrawLine(centerMass, closestPoint, Color.black);
            //        DebugDraw.Sphere(closestPoint, 0.05f, Color.black);

            //        //Rigidbody rb = collision.attachedRigidbody;
            //        //if (rb != null)
            //        //{
            //        //    if (rb.mass > m_Mass)
            //        //    {

            //        //    }
            //        //}
            //        //colliderBuffer[i].
            //    }
            //}

            //If walk into wall.





            m_GroundSlopeDir = Vector3.zero;

            if (Grounded)
            {
                //  Find the vector that represents the slope.
                Vector3 groundRight = Vector3.Cross(groundHit.normal, Vector3.down);
                m_GroundSlopeDir = Vector3.Cross(groundRight, groundHit.normal);

                // Slopes
                Vector3 slopeCheckOffset = m_transform.forward * (charCollider.radius + m_skinWidth);
                RaycastHit slopeHit1;
                RaycastHit slopeHit2;
                if (Physics.Raycast(RaycastOrigin + slopeCheckOffset, Vector3.down, out slopeHit1, m_collisionsLayerMask)) {
                    if (DebugCollisions) Debug.DrawLine(RaycastOrigin + slopeCheckOffset, slopeHit1.point, Grounded ? Color.green : Color.gray);

                    float forwardAngle = Vector3.Angle(slopeHit1.normal, Vector3.up);
                    if (Physics.Raycast(RaycastOrigin - slopeCheckOffset, Vector3.down, out slopeHit2, m_collisionsLayerMask)) {
                        if (DebugCollisions) Debug.DrawLine(RaycastOrigin - slopeCheckOffset, slopeHit2.point, Grounded ? Color.green : Color.gray);

                        float backAngle = Vector3.Angle(slopeHit2.normal, Vector3.up);
                        float[] groundAngles = { groundAngle, forwardAngle, backAngle };
                        Array.Sort(groundAngles);
                        groundAngle = groundAngles[1];
                    } else {
                        groundAngle = (groundAngle + forwardAngle) / 2;
                    }
                }

                if (groundAngle > 0)
                {
                    //  What to do if ground angle is greater than slope limit.
                    if (groundAngle > m_SlopeLimit)
                    {
                        //  sliding is true.
                        Moving = false;
                        var localDir = m_transform.InverseTransformDirection(m_GroundSlopeDir);
                        m_targetAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
                        var targetDirection = Vector3.Project(m_moveDirection, m_GroundSlopeDir).normalized * m_SlopeForceDown;
                        m_moveDirection = Vector3.Lerp(m_moveDirection, targetDirection, m_deltaTime * m_rotationSpeed);
                        m_rigidbody.AddForce(m_moveDirection, ForceMode.Impulse);
                        CharacterDebug.Log("<color=red> ** Sliding  </color>", "Sliding");
                    }
                    //else {
                    //    Vector3 slopeDirection = Vector3.Cross(m_transform.right, groundHit.normal);
                    //    float direction = Mathf.Sign(slopeDirection.y);
                    //    m_moveDirection = Vector3.Project(m_moveDirection, slopeDirection).normalized * (direction > 0 ? m_SlopeForceUp : m_SlopeForceDown);
                    //}


                    //float slopeStartAngle = 0;
                    //float slopeEndAngle = m_SlopeLimit;
                    //float angle = 90 - Vector3.Angle(m_transform.forward, groundHit.normal);
                    //angle -= slopeStartAngle;
                    //float range = slopeEndAngle - slopeStartAngle;
                    //float slopeDamper = 1f - Mathf.Clamp(angle / range, 0f, 1f);
                    //CharacterDebug.Log("<color=green>anle  </color>", angle);
                    //CharacterDebug.Log("<color=green>slopeDamper  </color>", slopeDamper);
                }






                //if (Physics.RaycastNonAlloc(m_transform.position + Vector3.up * m_MaxStepHeight, m_transform.forward, colliderBuffer, charCollider.radius + 1, m_collisionsLayerMask) > 0) {

                //}

                //float offset = charCollider.radius + m_skinWidth;
                //Vector3 groundAverage = GetAverageRaycast(offset, offset, 2);
                //if (groundAverage != m_rigidbody.position) {
                //    m_rigidbody.MovePosition(new Vector3(m_transform.position.x, groundAverage.y + 0.1f, m_transform.position.z));
                //}

            }


            //CharacterDebug.Log("<color=green>Ground Angle</color>", groundAngle);

        }


    
        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            
            
            if (m_grounded)
            {
                m_velocity.y = 0;
                float velocityMag = m_targetVelocity.magnitude;
                Vector3 groundNormal = groundHit.normal;

                //if (Vector3.Dot(m_moveDirection, groundNormal) >= 0){
                //    //  If greater than 0, than we are going down a slope.
                //} else {
                //    //  We are going up the slope if it is negative.
                //}

                Vector3 directionTangent = GetDirectionTangentToSurface(m_velocity, groundNormal) * velocityMag;


                // Reorient target velocity.
                Vector3 inputRight = Vector3.Cross(m_targetVelocity, m_transform.up);
                m_targetVelocity = Vector3.Cross(groundNormal, inputRight).normalized * velocityMag;// m_moveDirection.magnitude;

                //m_velocity = Vector3.Lerp(m_velocity, m_targetVelocity, 1f - Mathf.Exp(-15 * m_deltaTime));

                m_velocity = Vector3.SmoothDamp(m_velocity, m_targetVelocity, ref velocitySmooth, 0.1f);
            }
            else
            {
                m_velocity += Gravity * m_deltaTime * m_gravityModifier;
            }




            //if(m_grounded) m_rigidbody.velocity = Vector3.ProjectOnPlane(m_rigidbody.velocity, groundHit.normal * m_groundStickiness);




        }


        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            //if (LookDirection.sqrMagnitude > 0f)
            //{
            //    Vector3 smoothLookDirection = Vector3.Slerp(m_transform.forward, LookDirection, 1 - Mathf.Exp(-10 * deltaTime)).normalized;
            //    currentRotation = Quaternion.LookRotation(m_transform.TransformDirection(smoothLookDirection), m_transform.up);
            //    m_rigidbody.MoveRotation(currentRotation);
            //}




            var start = 135f * Mathf.Sign(m_targetAngle);
            float percentage = Mathf.Clamp(m_targetAngle, -Mathf.Abs(start), Mathf.Abs(start)) / start;

            float moveAmount = Mathf.Clamp01(Mathf.Abs(m_moveDirection.x) + Mathf.Abs(m_moveDirection.z));
            moveAmount = MathUtil.Round(moveAmount);
            float rotationSpeed = Mathf.Lerp(0, m_rotationSpeed * 2, moveAmount);

            
            float currentAngle = Mathf.SmoothDampAngle(m_targetAngle, 0, ref rotationAngleSmooth, 0.12f);
            //float currentAngle = Mathf.Lerp(m_targetAngle, 0, percentage);

            CharacterDebug.Log("<b><color=white>*** CurrentAngle </color></b>", MathUtil.Round(currentAngle, 5));
            CharacterDebug.Log("<b><color=orange>*** Percentage </color></b>", 1 - Mathf.Exp(-10 * deltaTime));

            Quaternion currentRotation = Quaternion.AngleAxis(currentAngle, m_transform.up);
            m_angularVelocity = m_transform.up * currentAngle;


            //  Update angular velocity.
            m_rigidbody.angularDrag = Mathf.SmoothDamp(m_rigidbody.angularDrag, moveAmount > 0 ? 0.05f : m_Mass, ref angularDragSmooth, 0.16f);
            m_rigidbody.angularVelocity = Vector3.Slerp(m_rigidbody.angularVelocity, m_angularVelocity, m_deltaTime * rotationSpeed);
            //CharacterDebug.Log("<b><color=yellow>*** Angular V </color></b>", m_angularVelocity);
            //CharacterDebug.Log("<b><color=yellow>*** Current R </color></b>", currentRotation);
            currentRotation = Quaternion.Slerp(m_transform.rotation, currentRotation * m_transform.rotation, (m_rotationSpeed * m_deltaTime) * (m_rotationSpeed * m_deltaTime));
            m_rigidbody.MoveRotation(currentRotation);



            //if(m_targetAngle > 0)
            //{
            //    //  Get angular velocity.
            //    m_targetRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
            //    rotationAxis.Normalize();
            //    m_angularVelocity = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_rotationSpeed;
            //    //  Set the rigidbody angular velocity.
            //    m_rigidbody.angularVelocity = m_angularVelocity;



            //    var step = m_rotationSpeed * m_deltaTime;
            //    //  Calculate rotation based on velocity.
            //    Quaternion velocityRotation = Quaternion.LookRotation(m_velocity, Vector3.up);
            //    float eulerY = m_transform.rotation.eulerAngles.y;
            //    eulerY = Mathf.LerpAngle(eulerY, velocityRotation.eulerAngles.y, step);

            //    //  Calculate rotation based on lookRotation.

            //    Quaternion rotationAdjusted = Quaternion.RotateTowards(m_transform.rotation, LookRotation, step);

            //}

            //if (LookDirection.sqrMagnitude > 0f)
            //{
            //    Vector3 smoothLookDirection = Vector3.Slerp(m_transform.forward, LookDirection, 1 - Mathf.Exp(-10 * deltaTime)).normalized;
            //    currentRotation = Quaternion.LookRotation(smoothLookDirection, m_transform.up);
            //}
            ////if (Grounded)
            ////{
            ////    var hits = Physics.RaycastNonAlloc(m_transform.position + m_transform.rotation * ColliderCenter, -m_transform.up, probedHits, ColliderHeight, m_collisionsLayerMask);
            ////    totalRaycastHits += hits;
            ////    if (hits > 0)
            ////    {
            ////        float groundReorientSharpness = 10f;
            ////        Vector3 smoothedNormal = Vector3.Slerp(m_transform.up, LookDirection, 1 - Mathf.Exp(-groundReorientSharpness * deltaTime)).normalized;  //  VectorB (LookDirection) should be the closest hit from the RaycastNonAlloac
            ////        currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), smoothedNormal) * currentRotation;
            ////    }
            ////}



            //
            //  AddRelativeTorque adds torque according to its Inertia Tensors. Therefore, the desired angular
            //  velocity must be transformed according to the Inertia Tensor, to get the required Torque.
            //

            //// Rotate about Y principal axis
            //Vector3 desiredAngularVelInY = new Vector3(0, Mathf.PI, 0); //  1/2 revs per second 
            //Vector3 torque = rigidbodyCached.inertiaTensorRotation * Vector3.Scale(rigidbodyCached.inertiaTensor, desiredAngularVelInY);
            //rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);
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
            //  Set rigidbody velocity.
            m_rigidbody.velocity = m_velocity;
        }


        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            
            m_animator.SetBool(HashID.Grounded, Grounded);
            m_animator.SetBool(HashID.Moving, Moving);


            float lookAngle = 0;
            float maxLookAngle = 90;
            if (LookAngle > maxLookAngle || LookAngle < -maxLookAngle) lookAngle = Mathf.Lerp(maxLookAngle, 0, m_deltaTime * 4);
            else lookAngle = LookAngle;
            lookAngle = MathUtil.Round(lookAngle);
            //CharacterDebug.Log("<b><color=orange>*** lookAngle</color></b>", lookAngle);
            m_animator.SetFloat(HashID.LookAngle, lookAngle);




            m_Speed = InputVector.normalized.sqrMagnitude * (Movement == MovementTypes.Adventure ? 1 : 0);
            m_animator.SetFloat(HashID.Speed, Speed);

            //float rotation = GetAngleFromForward(m_transform.forward) - deltaAngle;
            //deltaAngle = 0;
            //rotation *= 0.01f;
            //rotation = Mathf.Clamp(rotation / Time.deltaTime, -1f, 1f);
            //rotation = MathUtil.Round(rotation, 8);
            //CharacterDebug.Log("<b><color=orange>*** rotation 1</color></b>", rotation);
            //m_animator.SetFloat(HashID.Rotation, rotation);
            //CharacterDebug.Log("<b><color=orange>*** rotation 1</color></b>", m_targetAngle);
            m_animator.SetFloat(HashID.Rotation, m_targetAngle);
            //m_animator.SetFloat(HashID.Rotation, m_targetAngle, 0.1f, m_deltaTime);



            
            m_AnimationMonitor.SetForwardInputValue(m_inputVector.z);
            m_AnimationMonitor.SetHorizontalInputValue(m_inputVector.x);

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
            }

            else {
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

            groundHit = new RaycastHit();
            groundHit.point = m_transform.position - Vector3.up * airborneThreshold;
            groundHit.normal = m_transform.up;

            Physics.SphereCast(origin, radius, Vector3.down, out groundHit, airborneThreshold * 2, m_collisionsLayerMask);

            return groundHit;
        }


        public Vector3 GetDirectionTangentToSurface(Vector3 normal)
        {
            Vector3 tangent;
            Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
            Vector3 t2 = Vector3.Cross(normal, Vector3.up);
            if (t1.sqrMagnitude > t2.sqrMagnitude) {
                tangent = t1;
            } else {
                tangent = t2;
            }
            return tangent;
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




        /// <summary>
        /// Get the average raycast position.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetZ"></param>
        /// <param name="rayCount"></param>
        /// <returns></returns>
        protected Vector3 GetAverageRaycast( float offsetX, float offsetZ, int rayCount = 2 )
        {
            int maxRays = 4;
            offsetX *= 2;
            offsetZ *= 2;
            rayCount = Mathf.Clamp(rayCount, 2, maxRays);
            int totalRays = rayCount * rayCount + 1;
            Vector3[] combinedCast = new Vector3[totalRays];
            int average = 0;
            Vector3 rayOrigin = m_transform.TransformPoint(0 - offsetX * 0.5f, m_MaxStepHeight + m_skinWidth, 0 - offsetZ * 0.5f);
            float rayLength = m_MaxStepHeight * 2;


            float xSpacing = offsetX / (rayCount - 1);
            float zSpacing = offsetZ / (rayCount - 1);

            bool raycastHit = false;
            Vector3 hitPoint = Vector3.zero;
            Vector3 raycast = m_transform.TransformPoint(0, m_MaxStepHeight, 0);

            if (DebugCollisions) Debug.DrawRay(raycast, MoveDirection.normalized, Color.blue);

            RaycastHit hit;
            int index = 0;
            for (int z = 0; z < rayCount; z++) {
                for (int x = 0; x < rayCount; x++) {
                    raycastHit = false;
                    hitPoint = Vector3.zero;
                    raycast = rayOrigin + (m_transform.forward * zSpacing * z) + (m_transform.right * xSpacing * x);
                    //raycast += MoveDirection.normalized * Time.deltaTime;
                    if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_collisionsLayerMask)) {
                        hitPoint = hit.point;
                        average++;
                        raycastHit = true;
                    }
                    combinedCast[index] = hitPoint;
                    index++;
                    if (DebugCollisions) Debug.DrawRay(raycast, Vector3.down * rayLength, (raycastHit ? Color.green : Color.red));
                }
            }


            hitPoint = Vector3.zero;
            raycastHit = false;
            raycast = m_transform.TransformPoint(0, m_MaxStepHeight, 0);
            //originRaycast += MoveDirection.normalized * Time.deltaTime;
            if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_collisionsLayerMask)) {
                hitPoint = hit.point;
                average++;
                raycastHit = true;
            }

            combinedCast[totalRays - 1] = hitPoint;
            if (DebugCollisions) DebugDraw.Circle(raycast, Vector3.up * rayLength, 0.2f, (raycastHit ? Color.blue : Color.red));



            average = Mathf.Clamp(average, 1, int.MaxValue);

            Vector3 averageHitPosition = Vector3.zero;
            float xTotal = 0f, yTotal = 0f, zTotal = 0f;
            for (int i = 0; i < combinedCast.Length; i++) {
                xTotal += combinedCast[i].x;
                yTotal += combinedCast[i].y;
                zTotal += combinedCast[i].z;
            }
            averageHitPosition.Set(xTotal / average, yTotal / average, zTotal / average);

            if (DebugCollisions) DebugDraw.DrawMarker(averageHitPosition, 0.2f, Color.blue);

            return averageHitPosition;
        }








        #region Public Functions



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
        protected float GetAngleFromForward(Vector3 worldDirection)
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


        protected Vector3 GetAcceleration(Vector3 finalVelocity, Vector3 initialVelocity, Vector3 distance)
        {
            Vector3 vf = new Vector3(finalVelocity.x * finalVelocity.x, finalVelocity.y * finalVelocity.y, finalVelocity.z * finalVelocity.z);
            Vector3 vi = new Vector3(initialVelocity.x * initialVelocity.x, initialVelocity.y * initialVelocity.y, initialVelocity.z * initialVelocity.z);
            //Vector3 a = (vf - vi) / (2 * distance);
            distance *= 2;
            Vector3 a = vf - vi;
            a.Set((a.x / distance.x), (a.x / distance.x), (a.x / distance.x));
            return a;
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




        protected void MotionTrajectory(Vector3 m_targetVelocity)
        {
            if (colliderBuffer == null) colliderBuffer = new List<Collider>();
            Vector3 transformUp = transform.up;
            Vector3 origin = m_transform.position;

            Vector3 stepVelocity = m_targetVelocity;
            stepVelocity /= motionTrajectoryResolution;

            for (int i = 0; i < motionTrajectoryResolution; i++)
            {
                bool hasHit = false;
                Vector3 previousOrigin = origin;
                origin += stepVelocity;
                m_targetPosition = origin;



                float radius = ColliderRadius + m_skinWidth * 2;
                Vector3 capsuleOrigin = origin + ColliderCenter + Vector3.up * m_skinWidth;
                Vector3 segment1 = GetCapsulePoint(capsuleOrigin, transformUp);
                Vector3 segment2 = GetCapsulePoint(capsuleOrigin, -transformUp);
                if (CheckCapsule(segment1, segment2, radius, m_collisionsLayerMask))
                {
                    colliderBuffer.Clear();

                    int hits = Physics.OverlapCapsuleNonAlloc(segment1, segment2, radius, probedColiders, m_collisionsLayerMask);
                    if (hits > 0)
                    {
                        for (int k = 0; k < hits; k++)
                        {

                            colliderBuffer.Add(probedColiders[k]);

                            var collision = colliderBuffer[k];
                            Vector3 direction;
                            float distance;
                            if (Physics.ComputePenetration(charCollider, m_transform.position, m_transform.rotation,
                                                            collision, collision.transform.position, collision.transform.rotation,
                                                            out direction, out distance))
                            {
                                Vector3 penetrationVector = direction * distance;
                                Vector3 moveDirectionProjected = Vector3.Project(m_moveDirection, -direction);
                                //  m_transform.position = m_transform.position + penetrationVector;
                                //m_rigidbody.MovePosition(m_transform.position + penetrationVector);
                                m_moveDirection -= moveDirectionProjected;

                                Debug.DrawLine(origin, penetrationVector, Color.black);
                            }

                            var closestPoint = colliderBuffer[k].ClosestPointOnBounds(origin);

                            //Debug.LogFormat("index: {0} | buffer: {1} | nonAlac: {2}", k, colliderBuffer.Capacity, probedColiders.Length);
                            Debug.DrawLine(origin, closestPoint, Color.black);
                            DebugDraw.Sphere(closestPoint, 0.05f, Color.black);

                            hasHit = true;
                        }

                    }
                }

                motionTrajectory[i] = origin;

                if (hasHit)
                {
                    //m_targetPosition = origin;
                    //DebugDraw.DrawCapsule(segment1, segment2, radius, Color.cyan);
                }
                else
                {
                    break;
                }



            }
        }





        #endregion










        #region Debugging








        protected abstract void DrawGizmos();


        protected virtual void DrawOnGUI()
        {
            if (debugger.states.showDebugUI == false) return;
            DebugAttributes();

            GUI.color = CharacterControllerUtility.DebugTextColor;
            Rect rect = CharacterControllerUtility.CharacterControllerRect;
            GUI.BeginGroup(rect, GUI.skin.box);

            GUI.Label(rect, CharacterDebug.Write());

            GUI.EndGroup();
        }


        protected void OnGUI()
        {
            if (Application.isPlaying && Debugger.debugMode)
            {
                DrawOnGUI();
            }

        }



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




//Debug.Log("Horizontal: " + m_HorizontalMovement + " Forward: " + m_ForwardMovement + " Euler: " + m_LookRotation.eulerAngles);
//if (m_Controller.Movement == RigidbodyCharacterController.MovementType.Combat || m_Controller.Movement == RigidbodyCharacterController.MovementType.Adventure) {
//m_LookRotation = m_CameraTransform.rotation;
//} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.TopDown) {
//var direction = (Vector3)PlayerInput.GetMousePosition() - m_Camera.WorldToScreenPoint(m_transform.position);
//// Convert the XY direction to an XYZ direction with Y equal to 0.
//direction.z = direction.y;
//direction.y = 0;
//m_LookRotation = Quaternion.LookRotation(direction);
//} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.RPG) {
//if (PlayerInput.IsDisabledButtonDown(false)) {
//m_LookRotation = m_CameraTransform.rotation;
//if (PlayerInput.IsDisabledButtonDown(true)) {
//m_ForwardMovement = 1;
//}
//} else if (!PlayerInput.IsDisabledButtonDown(true)) {
//if (m_ForwardMovement != 0 || m_HorizontalMovement != 0) {
//m_LookRotation = m_CameraTransform.rotation;
//}
//m_HorizontalMovement = 0;
//}
//} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.Pseudo3D) {
//var direction = (Vector3)PlayerInput.GetMousePosition() - m_Camera.WorldToScreenPoint(m_transform.position + m_CapsuleCollider.center);
//m_LookRotation = Quaternion.LookRotation(direction);
//} else { // Point and Click.
//m_LookRotation = m_PointClickLookRotation.Get();
//}
