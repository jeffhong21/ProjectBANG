//namespace CharacterController
//{
//    using UnityEngine;
//    using System;

//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
//    public class RigidbodyCharacterController : MonoBehaviour
//    {
//        public enum MovementType { Adventure, Combat };

//        //  Locomotion variables
//        [SerializeField, HideInInspector]
//        protected bool m_UseRootMotion = true;
//        [SerializeField, HideInInspector]
//        protected float m_RootMotionSpeedMultiplier = 1;
//        [SerializeField, HideInInspector]
//        protected float m_Acceleration = 0.1f;
//        [SerializeField, HideInInspector]
//        protected float m_AirborneAcceleration = 0.2f;     //  NEED TO ADD
//        [SerializeField, HideInInspector]
//        protected float m_MotorDamping = 0.2f;
//        [SerializeField, HideInInspector]
//        protected float m_MovementSpeed = 1f;
//        [SerializeField, HideInInspector]
//        protected float m_AirSpeed = 0.5f;                       //  NEED TO ADD
//        [SerializeField, HideInInspector]
//        protected float m_RotationSpeed = 10f;
//        [SerializeField, HideInInspector]
//        protected float m_SlopeForceUp = 1f;
//        [SerializeField, HideInInspector]
//        protected float m_SlopeForceDown = 1.25f;
//        [SerializeField, HideInInspector, Range(-1, 1)]
//        [Tooltip("A -1 to 1 threshold for when the character should stop moving if another object collides with the character. " +
//        "A value of 1 indicates the object is directly in front of the character's move direction while a value of -1 indicates the " +
//        "object is directly behind the character's move direction")]
//        protected float m_StopMovementThreshold = 0.5f;


//        //  -- Physics variables
//        [SerializeField, HideInInspector]
//        protected float m_DetectObjectHeight = 0.4f;
//        [SerializeField, HideInInspector]
//        protected float m_Mass = 100;
//        [SerializeField, HideInInspector]
//        protected float m_SkinWidth = 0.08f;
//        [SerializeField, HideInInspector, Range(0, 90)]
//        protected float m_SlopeLimit = 45f;
//        [SerializeField, HideInInspector]
//        protected float m_MaxStepHeight = 0.25f;
//        [SerializeField, HideInInspector]
//        protected float m_GroundStickiness = 6f;
//        [SerializeField, HideInInspector]
//        protected float m_ExternalForceDamping = 0.1f;
//        [SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
//        protected float m_StepOffset = 0.15f;
//        [SerializeField, HideInInspector]
//        protected float m_StepSpeed = 4f;
//        [SerializeField, HideInInspector]
//        protected float m_GravityModifier = 2f;


//        //  -- Collision detection
//        [SerializeField, HideInInspector]
//        protected bool mDetectHorizontalCollision = true;
//        [SerializeField, HideInInspector]
//        protected int m_HorizontalCollisionCount = 10;
//        [SerializeField, HideInInspector]
//        protected int m_VerticalCollisionCount = 10;
//        [SerializeField, HideInInspector]
//        protected LayerMask m_ColliderLayerMask;
//        [SerializeField, HideInInspector]
//        protected int m_MaxCollisionCount = 50;


//        //  -- Animation
//        [Header("Animations")]
//        [SerializeField, HideInInspector]
//        protected float m_IdleRotationMultiplier = 2f;
//        [SerializeField, HideInInspector]
//        protected string m_MovingStateName = "Movement.Movement";
//        [SerializeField, HideInInspector]
//        protected string m_AirborneStateName = "Fall";



//        protected float m_TimeScale = 1;
//        //  How collision info every X frames.
//        protected int m_CollisionCastInterval = 3;
//        protected int m_CollisionCastCount;
//        //  Internal variable to to notify if Move() was called from outside, so it doesn't get called twice.
//        protected bool m_UpdateMove = true;
//        protected RaycastHit[] m_Collisions;





//        protected MovementType m_MovementType = MovementType.Adventure;
//        [SerializeField, DisplayOnly]
//        protected bool m_Moving;
//        protected bool m_Aiming;
//        protected float m_InputAngle;
//        protected Vector3 m_Velocity, m_PreviousVelocity;
//        [SerializeField, DisplayOnly]
//        protected Vector3 m_InputVector;
//        protected Vector3 m_LookDirection;
//        protected Quaternion m_LookRotation;
//        [SerializeField, DisplayOnly]
//        protected Vector3 m_MoveDirection;


//        [SerializeField, DisplayOnly]
//        protected bool m_Grounded = true;
//        protected Vector3 m_Gravity;
//        protected RaycastHit m_GroundHit, m_StepHit;
//        protected float m_GroundDistance;
//        protected float m_AirbornThreshold = 0.3f;
//        protected Vector3 m_GroundNormal;
//        protected float m_GroundCheckHeight;
//        protected float m_SlopeAngle;
//        protected float m_Stickiness;


//        protected CapsuleCollider m_CapsuleCollider;
//        protected Collider[] m_LinkedColliders = new Collider[0];
//        protected PhysicMaterial m_GroundIdleFrictionMaterial;
//        protected PhysicMaterial m_GroundedMovingFrictionMaterial;
//        protected PhysicMaterial m_StepFrictionMaterial;
//        protected PhysicMaterial m_SlopeFrictionMaterial;
//        protected PhysicMaterial m_AirFrictionMaterial;



//        private float m_StartAngle, m_StartAngleSmooth;


//        private Vector3 m_VelocitySmooth;
//        private float m_RotationSmoothDamp;




//        protected Animator m_Animator;
//        protected AnimatorMonitor m_AnimationMonitor;
//        protected LayerManager m_Layers;
//        protected Rigidbody m_Rigidbody;
//        protected GameObject m_GameObject;
//        protected Transform m_Transform;
//        protected float m_DeltaTime;

//        //  For Editor.
//        //  Debug parameters.
//        [SerializeField, HideInInspector]
//        protected bool m_Debug;
//        [SerializeField, HideInInspector]
//        protected bool m_DebugCollisions;
//        [SerializeField, HideInInspector]
//        protected bool m_DrawDebugLine;
//        [SerializeField, HideInInspector]
//        protected bool displayMovement = true, displayPhysics = true, displayAnimations = true, displayActions = true;






//        #region Properties

//        public MovementType Movement
//        {
//            get { return m_MovementType; }
//        }

//        public bool Moving
//        {
//            get { return m_Moving; }
//            set { m_Moving = value; }
//        }

//        public bool Aiming
//        {
//            get
//            {
//                if (m_Aiming && Grounded)
//                    return true;
//                return false;
//            }
//            set { m_Aiming = value; }
//        }

//        public bool Grounded
//        {
//            get { return m_Grounded; }
//            set { m_Grounded = value; }
//        }

//        public float RotationSpeed
//        {
//            get { return m_RotationSpeed; }
//            set { m_RotationSpeed = value; }
//        }

//        public Vector3 InputVector
//        {
//            get { return m_InputVector; }
//            set { m_InputVector = value; }
//        }

//        public Vector3 MoveDirection
//        {
//            get { return m_MoveDirection; }
//        }

//        public Vector3 LookDirection
//        {
//            get { return m_LookDirection; }
//            set { m_LookDirection = value == Vector3.zero ? m_Transform.forward : value; }
//        }

//        public Vector3 Velocity
//        {
//            get { return m_Velocity; }
//            set { m_Velocity = value; }
//        }

//        public Quaternion LookRotation
//        {
//            get { return m_LookRotation; }
//            set { m_LookRotation = value; }
//        }

//        public bool UseRootMotion
//        {
//            get { return m_UseRootMotion; }
//            set { m_UseRootMotion = value; }
//        }

//        public Vector3 RaycastOrigin
//        {
//            get { return m_Transform.position + Vector3.up * m_SkinWidth; }
//        }

//        public RaycastHit GroundHit
//        {
//            get { return m_GroundHit; }
//        }

//        public float GroundDistance
//        {
//            get { return m_GroundDistance; }
//        }

//        #endregion



//        protected virtual void Awake()
//        {
//            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
//            m_Animator = GetComponent<Animator>();

//            m_Rigidbody = GetComponent<Rigidbody>();
//            if (m_CapsuleCollider == null)
//                m_CapsuleCollider = GetComponent<CapsuleCollider>();
//            m_Layers = GetComponent<LayerManager>();

//            m_GameObject = gameObject;
//            m_Transform = transform;

//            m_DeltaTime = Time.deltaTime;

//            if (m_Layers == null)
//                m_Layers = m_GameObject.AddComponent<LayerManager>();

//            m_Gravity = Physics.gravity;


//        }



//        protected virtual void OnEnable()
//        {
//            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//            m_Rigidbody.mass = m_Mass;
//            m_Animator.applyRootMotion = m_UseRootMotion;

//            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

//        }


//        protected virtual void OnDisable()
//        {

//        }


//        protected void Start()
//        {
//            m_ColliderLayerMask = m_Layers.SolidLayers;

//            m_Collisions = new RaycastHit[m_MaxCollisionCount];


//            // slides the character through walls and edges
//            m_GroundedMovingFrictionMaterial = new PhysicMaterial
//            {
//                name = "GroundedMovingFrictionMaterial",
//                staticFriction = .25f,
//                dynamicFriction = .25f,
//                frictionCombine = PhysicMaterialCombine.Multiply
//            };

//            // prevents the collider from slipping on ramps
//            m_GroundIdleFrictionMaterial = new PhysicMaterial
//            {
//                name = "GroundIdleFrictionMaterial",
//                staticFriction = 1f,
//                dynamicFriction = 1f,
//                frictionCombine = PhysicMaterialCombine.Maximum
//            };

//            // air physics 
//            m_AirFrictionMaterial = new PhysicMaterial
//            {
//                name = "AirFrictionMaterial",
//                staticFriction = 0f,
//                dynamicFriction = 0f,
//                frictionCombine = PhysicMaterialCombine.Minimum
//            };
//        }


//        protected virtual void Update()
//        {
//            m_TimeScale = Time.timeScale;
//            if (m_TimeScale == 0) return;
//            m_DeltaTime = Time.deltaTime;


//            CheckGround();

//            CheckMovement();

//            SetPhysicsMaterial();

//            Move();

//            UpdateAnimator();



//        }


//        protected virtual void FixedUpdate()
//        {
//            if (m_TimeScale == 0) return;
//            m_DeltaTime = Time.fixedDeltaTime;


//            UpdateRotation();

//            UpdateMovement();
//        }


//        private void LateUpdate()
//        {
//            m_PreviousVelocity = m_Velocity;
//        }


//        protected virtual void OnAnimatorMove()
//        {
//            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
//            // which affects the movement speed because of the root motion.
//            m_Animator.speed = m_RootMotionSpeedMultiplier;

//            AnimatorMove();
//        }




//        public void SetMovementType(MovementType movementType)
//        {
//            m_MovementType = movementType;
//        }


//        //  If true, the character looks independently of the camera.  AI Agents do not need to use camera rotation.
//        public bool IndependentLook()
//        {
//            if (m_Moving || m_Aiming)
//            {
//                return false;
//            }
//            //if(m_Aiming) return true;
//            return true;
//        }


//        #region Ground Check

//        private void DrawCheckGroundGizmo()
//        {
//            Gizmos.color = m_Grounded ? Color.green : Color.red;


//            //Gizmos.color = groundRayHit ? Color.green : Color.red;
//            Gizmos.DrawRay(groundCastOrigin, Vector3.down * groundCheckMaxDistance);
//            //Gizmos.color = groundSphereHit ? darkGreen : darkRed;
//            Gizmos.DrawRay(sphereCastOrigin, Vector3.down * sphereCastHitDistance);
//            Gizmos.DrawWireSphere(sphereCastOrigin + Vector3.down * sphereCastHitDistance, sphereCastRadius);
//            if (m_Grounded)
//            {
//                UnityEditor.Handles.color = groundRayHit ? Color.green : Color.red;
//                UnityEditor.Handles.DrawSolidDisc(groundHitPoint, m_GroundNormal, 0.05f);

//            }
//        }

//        Vector3 groundCastOrigin;
//        float groundCheckMaxDistance;
//        bool groundRayHit;
//        Vector3 groundHitPoint;
//        Vector3 sphereCastOrigin;
//        float sphereCastMaxDistance, sphereCastHitDistance, sphereCastRadius;
//        bool groundSphereHit;

//        #endregion




//        protected virtual void CheckGround()
//        {
//            m_GroundDistance = 10;

//            m_GroundCheckHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
//            groundCastOrigin = m_Rigidbody.position + Vector3.up * m_GroundCheckHeight;
//            groundCheckMaxDistance = m_CapsuleCollider.radius;
//            groundRayHit = false;
//            if (Physics.Raycast(groundCastOrigin, Vector3.down, out m_GroundHit, groundCheckMaxDistance, m_Layers.SolidLayers))
//            {
//                if (m_GroundHit.transform != m_Transform)
//                {
//                    groundRayHit = true;
//                    //m_GroundDistance = m_Transform.position.y - m_GroundHit.point.y;
//                    m_GroundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, m_Transform.up).magnitude;
//                    m_GroundNormal = m_GroundHit.normal;
//                }

//            }

//            sphereCastOrigin = m_Rigidbody.position + Vector3.up * m_CapsuleCollider.radius;
//            //sphereCastRadius = m_CapsuleCollider.radius - m_SkinWidth;
//            sphereCastRadius = m_CapsuleCollider.radius * 0.9f;
//            sphereCastMaxDistance = m_CapsuleCollider.radius + 2;
//            sphereCastHitDistance = sphereCastMaxDistance;
//            groundSphereHit = false;
//            if (Physics.SphereCast(sphereCastOrigin, sphereCastRadius,
//                                    Vector3.down, out m_GroundHit, sphereCastMaxDistance, m_Layers.SolidLayers))
//            {
//                sphereCastHitDistance = m_GroundHit.distance;
//                groundSphereHit = true;
//                // check if sphereCast distance is small than the ray cast distance
//                if (m_GroundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
//                    m_GroundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
//            }


//            m_GroundDistance = (float)Math.Round(m_GroundDistance, 2);
//            var groundCheckDistance = 0.2f;

//            //  Character is grounded.
//            if (m_GroundDistance < 0.05f)
//            {
//                Vector3 horizontalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity);
//                m_Stickiness = m_GroundStickiness * horizontalVelocity.magnitude * m_AirbornThreshold;
//                m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundNormal) - 90;

//                m_Grounded = true;
//            }
//            else
//            {
//                if (m_GroundDistance >= groundCheckDistance)
//                {
//                    m_InputVector = Vector3.zero;
//                    m_GroundNormal = m_Transform.up;
//                    m_SlopeAngle = 0;

//                    //m_Rigidbody.AddForce(m_Gravity * m_GravityModifier);


//                    m_Grounded = false;
//                }

//            }

//            //if (m_Debug) Debug.DrawRay(m_GroundHit.point, m_GroundNormal, Color.magenta);
//            groundHitPoint = m_Grounded ? m_GroundHit.point : Vector3.zero;
//        }


//        //  Ensure the current movement direction is valid.
//        protected virtual void CheckMovement()
//        {
//            m_Moving = m_InputVector != Vector3.zero;
//            if (m_Moving == false) return;


//            float direction = Mathf.Clamp(m_InputVector.z, -1, 1);
//            if (direction == 0) direction = 1;
//            float rayLength = 2f + m_SkinWidth;
//            float dstBetweenRays = 0.4f;


//            float colliderHeight = m_CapsuleCollider.height - (m_SkinWidth * 2);
//            int horizontalRayCount = mDetectHorizontalCollision ? Mathf.RoundToInt(colliderHeight / dstBetweenRays) : 3;
//            float horizontalRaySpacing = colliderHeight / (horizontalRayCount - 1);


//            RaycastHit hit;
//            for (int i = 0; i < horizontalRayCount; i++)
//            {
//                Vector3 rayOrigin = RaycastOrigin + m_Transform.forward * (m_CapsuleCollider.radius - m_SkinWidth);
//                rayOrigin += Vector3.up * (horizontalRaySpacing * i);

//                bool hitDetected = Physics.Raycast(rayOrigin, m_Transform.forward * direction, out hit, rayLength, m_ColliderLayerMask);
//                if (hitDetected && hit.collider != null)
//                {
//                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
//                    if (i == 0 && slopeAngle <= m_SlopeLimit)
//                    {

//                    }
//                    if (slopeAngle > m_SlopeLimit)
//                    {
//                        if (hit.distance < Mathf.Abs(direction))
//                        {
//                            m_Moving = false;
//                        }
//                        rayLength = hit.distance;
//                    }
//                }



//                if (m_Debug) Debug.DrawRay(rayOrigin, m_Transform.forward * direction * rayLength, hitDetected == true ? Color.blue : Color.grey);
//            }


//            m_Animator.SetBool(HashID.Moving, m_Moving);
//        }



//        //  Update the rotation forces.
//        protected virtual void UpdateRotation()
//        {
//            Vector3 axisSign = Vector3.Cross(m_LookDirection, m_Transform.forward);
//            m_InputAngle = Vector3.Angle(m_Transform.forward, m_LookDirection) * (axisSign.y >= 0 ? -1f : 1f) * m_DeltaTime;
//            m_InputAngle = (float)Math.Round(m_InputAngle, 2);

//            //  Add any LookRotation from OnAnimatorUpdate()
//            m_Rigidbody.MoveRotation(m_Transform.rotation * m_LookRotation.normalized);
//            m_LookRotation = Quaternion.identity;

//            //  Get the turn amount from the input vector.
//            Vector3 local = m_Transform.InverseTransformDirection(m_LookDirection);
//            float turnAmount = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
//            if (m_InputVector == Vector3.zero)
//                turnAmount *= (1.01f - (Mathf.Abs(turnAmount) / 180)) * m_IdleRotationMultiplier;

//            if (m_Moving && m_LookDirection.sqrMagnitude > 0.2f)
//            {
//                m_LookRotation = Quaternion.AngleAxis(turnAmount * m_DeltaTime * m_RotationSpeed, m_Transform.up);
//                if (m_Grounded)
//                {
//                    Vector3 d = transform.position - m_Animator.pivotPosition;
//                    m_Rigidbody.MovePosition(m_Animator.pivotPosition + m_LookRotation * d);
//                }
//                m_Rigidbody.MoveRotation(m_LookRotation * m_Rigidbody.rotation);
//            }


//            m_Animator.SetFloat(HashID.Rotation, (m_InputAngle * Mathf.Deg2Rad));

//        }


//        //  Apply any movement.
//        protected virtual void UpdateMovement()
//        {

//            if (m_Grounded)
//            {
//                //m_MoveDirection = Vector3.Cross(m_Transform.right, m_GroundNormal);
//                //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_SkinWidth) + m_Transform.right * m_SkinWidth, m_MoveDirection * 1, Color.cyan);

//                //m_Velocity = m_MoveDirection.normalized * m_MovementSpeed * m_InputVector.z;
//            }
//            //  If airborne.
//            else
//            {
//                //float m_AirSpeed = 6;
//                //m_MoveDirection = (Quaternion.Inverse(m_Transform.rotation) * m_Transform.forward) * m_AirSpeed;
//                //m_Velocity = Vector3.Project(m_MoveDirection, m_Gravity * m_GravityModifier);

//                //m_Rigidbody.AddForce(m_Gravity * m_GravityModifier);
//            }


//            //m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);
//        }



//        protected virtual void AnimatorMove()
//        {
//            if (m_UseRootMotion)
//            {
//                //float angleInDegrees;
//                //Vector3 rotationAxis;
//                //m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
//                //Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
//                //m_Rigidbody.angularVelocity = angularDisplacement;


//                //m_Velocity = (m_Animator.deltaPosition / m_DeltaTime);
//                ////m_Velocity = m_Velocity.normalized + (m_Animator.deltaPosition / m_DeltaTime);
//                //m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
//                //m_Rigidbody.velocity = m_Velocity;

//                m_Velocity += m_Animator.deltaPosition;
//                m_LookRotation *= m_Animator.deltaRotation;
//            }

//        }


//        protected virtual void Move()
//        {
//            #region Slope
//            //float directionZ = Mathf.Sign(m_Velocity.z);

//            //RaycastHit hitInfo;
//            //if (Physics.Raycast(m_Transform.position + Vector3.up * m_SkinWidth, m_Transform.forward, out hitInfo, 1, m_ColliderLayerMask))
//            //{
//            //    m_SlopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
//            //    if (m_Grounded && m_SlopeAngle <= m_SlopeLimit)
//            //    {
//            //        float distanceToSlopeStart = 0;
//            //        if (m_SlopeAngle > 0)
//            //        {
//            //            distanceToSlopeStart = hitInfo.distance - m_SkinWidth;
//            //            m_Velocity.z -= distanceToSlopeStart * directionZ;
//            //        }

//            //        float moveDistance = Mathf.Abs(m_Velocity.z);
//            //        float climbVelocityY = Mathf.Sin(m_SlopeAngle * Mathf.Deg2Rad) * moveDistance;

//            //        if (m_Velocity.y <= climbVelocityY)
//            //        {
//            //            m_Velocity.y = climbVelocityY;
//            //            m_Velocity.z = Mathf.Cos(m_SlopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(m_Velocity.z);
//            //        }

//            //        m_Velocity.z += distanceToSlopeStart * directionZ;
//            //    }

//            //    if (m_SlopeAngle > m_SlopeLimit)
//            //    {
//            //        m_Velocity.z = (hitInfo.distance - m_SkinWidth) * directionZ;

//            //        if (Vector3.Angle(m_Transform.forward, m_GroundNormal) - 90 > 0)
//            //        {
//            //            m_Velocity.x = m_Velocity.y / Mathf.Tan(m_SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(Velocity.x);
//            //            m_Velocity.y = Mathf.Tan(m_SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(Velocity.x);
//            //        }
//            //    }
//            //}
//            //Debug.DrawRay(m_Transform.position + (Vector3.up * m_SkinWidth), m_Transform.forward * 1, Color.red);
//            #endregion
//            switch (m_MovementType)
//            {
//                case (MovementType.Adventure):


//                    //m_Rigidbody.MoveRotation(m_Transform.rotation * m_LookRotation.normalized);
//                    Vector3 moveDirection = Vector3.Cross(m_Transform.right, m_GroundNormal);
//                    m_Velocity = Vector3.Project(m_Velocity, moveDirection);
//                    Vector3 targetVelocity = m_Velocity.normalized * (Grounded ? m_MovementSpeed : m_AirSpeed);

//                    if (m_Moving)
//                        m_Velocity = Vector3.SmoothDamp(m_Velocity, targetVelocity, ref m_VelocitySmooth, Grounded ? m_Acceleration : m_AirborneAcceleration);
//                    else
//                        m_Velocity = Vector3.SmoothDamp(m_Velocity, Vector3.zero, ref m_VelocitySmooth, m_MotorDamping);
//                    m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;

//                    if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_SkinWidth), m_Velocity, Color.yellow);
//                    //m_Rigidbody.velocity = m_Velocity;
//                    m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);
//                    break;

//                case (MovementType.Combat):

//                    break;
//            }

//            if (m_Grounded)
//            {
//                m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundNormal * m_Stickiness);
//            }
//        }



//        public virtual void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
//        {
//            //m_InputVector.x = Mathf.Clamp(horizontalMovement, -1, 1);
//            //m_InputVector.z = Mathf.Clamp(forwardMovement, -1, 1);

//            m_InputVector.x = horizontalMovement;
//            m_InputVector.z = forwardMovement;
//            if (m_InputVector.sqrMagnitude > 1)
//                m_InputVector.Normalize();

//            m_LookRotation = lookRotation;

//            switch (m_MovementType)
//            {
//                case (MovementType.Adventure):
//                    m_Velocity.z = m_InputVector.z;
//                    //m_Velocity.y = m_Rigidbody.velocity.y;

//                    float turnAmount = Mathf.Atan2(m_InputVector.x, m_InputVector.z);
//                    //m_Velocity.x = turnAmount;

//                    break;

//                case (MovementType.Combat):
//                    m_Velocity.z = m_InputVector.z;
//                    m_Velocity.x = m_InputVector.x;
//                    //m_Velocity.y = m_Rigidbody.velocity.y;

//                    m_Velocity = lookRotation * m_Velocity;
//                    break;
//            }

//            m_Velocity = m_Transform.rotation * m_Velocity;



//            //  -- Get the start move angle
//            //if (m_Moving && Mathf.Abs(m_InputAngle) < 10)
//            //    m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
//            //else if (!m_Moving)
//            //    m_StartAngle = m_InputAngle;
//            //m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);
//            ////  Set the animator parameter.
//            //m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
//            //   ---
//        }




//        protected void UpdateAnimator()
//        {

//            //  1 means left foot is up. 
//            m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);
//            m_Animator.SetBool(HashID.Moving, m_Moving);

//            //  -----------
//            //  Does a character action virtual the controllers update animator.
//            //  -----------
//            if (m_Grounded)
//            {
//                //  Movement Input
//                m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
//                m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
//            }
//            else
//            {
//                m_Animator.SetFloat(HashID.ForwardInput, 0);
//                m_Animator.SetFloat(HashID.HorizontalInput, 0);
//            }
//        }



//        protected virtual void SetPhysicsMaterial()
//        {
//            // change the physics material to very slip when not grounded or maxFriction when is
//            if (m_Grounded && !m_Moving)
//                m_CapsuleCollider.material = m_GroundIdleFrictionMaterial;
//            else if (m_Grounded && m_Moving)
//                m_CapsuleCollider.material = m_GroundedMovingFrictionMaterial;
//            else
//                m_CapsuleCollider.material = m_AirFrictionMaterial;
//        }




//        //protected void OnCollisionEnter(Collision collision)
//        //{
//        //    //for (int i = 0; i < collision.contacts.Length; i++)
//        //    //{
//        //    //    ContactPoint contact = collision.contacts[i];
//        //    //    if(m_Debug) Debug.DrawRay(contact.point, contact.normal, Color.white, 1.25f);
//        //    //}
//        //}




//        #region Public Functions






//        protected void VerticalCollision()
//        {
//            //  We want the cos angle since we know "adjacent" and "hypotenuse".
//            float startAngle = Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg;
//            float checkHeight = m_CapsuleCollider.height / 2;
//            float rayDistance = 2;
//            float verticalAngle = 15;

//            Quaternion startRayRotation = Quaternion.AngleAxis(-startAngle, Vector3.up);
//            Vector3 startRay = startRayRotation * m_Transform.forward;
//            Vector3 startDir = startRay;

//            float angleAmount = (startAngle * 2) / (m_VerticalCollisionCount - 1);

//            bool detectEdge = false;

//            Vector3 raycastOrigin = m_Transform.position + Vector3.up * checkHeight;

//            for (int i = 0; i < m_VerticalCollisionCount; i++)
//            {
//                if (i == 0) continue;
//                startDir = Quaternion.AngleAxis(angleAmount, Vector3.up) * startDir;
//                Vector3 hitDirection = startDir;

//                Quaternion angleRotation = Quaternion.AngleAxis(verticalAngle, m_Transform.right);
//                hitDirection = Vector3.ClampMagnitude(angleRotation * hitDirection, rayDistance);


//                if (Physics.Raycast(raycastOrigin, hitDirection + Vector3.up * checkHeight, rayDistance, m_ColliderLayerMask) == false)
//                {

//                    Vector3 start = m_Transform.position + (m_Transform.forward * m_CapsuleCollider.radius);
//                    start.y += m_DetectObjectHeight;
//                    float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;
//                    if (Physics.Raycast(start, Vector3.down, maxDetectEdgeDistance, m_ColliderLayerMask) == false)
//                    {
//                        detectEdge = true;
//                    }
//                }

//                if (m_DebugCollisions) Debug.DrawRay(raycastOrigin, hitDirection * rayDistance, detectEdge == true ? Color.red : Color.grey);
//            }


//        }



//        public bool DetectEdge()
//        {
//            if (!m_Grounded)
//                return false;


//            bool detectEdge = false;
//            Vector3 start = m_Transform.position + (m_Transform.forward * m_CapsuleCollider.radius);
//            start.y = start.y + m_DetectObjectHeight;
//            //start.y = start.y + 0.05f + (Mathf.Tan(m_SlopeAngle) * start.magnitude);

//            Vector3 dir = Vector3.down;
//            float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;



//            if (Physics.Raycast(m_Transform.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward, 2, m_Layers.SolidLayers) == false)
//            {

//                if (Physics.Raycast(start, dir, maxDetectEdgeDistance, m_Layers.SolidLayers) == false)
//                {
//                    detectEdge = true;
//                }
//            }



//            //if (m_Debug && hitObject == false) Debug.DrawRay(m_Transform.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward * 2, hitObject ? Color.red : Color.green);
//            if (m_Debug) Debug.DrawRay(start, dir * maxDetectEdgeDistance, detectEdge ? Color.green : Color.gray);

//            return detectEdge;
//        }



//        public bool DetectObject(Vector3 dir, out RaycastHit raycastHit, float maxDistance, LayerMask layerMask, int rayCount = 1, float maxAngle = 0)
//        {
//            bool detectObject = false;
//            Vector3 start = m_Transform.position + (Vector3.up * m_DetectObjectHeight);

//            if (rayCount < 1) rayCount = 1;

//            if (Physics.Raycast(start, dir, out raycastHit, maxDistance, layerMask))
//            {
//                detectObject = true;
//            }

//            if (m_Debug) Debug.DrawRay(start, dir * maxDistance, detectObject ? Color.red : Color.green);
//            return detectObject;
//        }









//        public void SetPosition(Vector3 position)
//        {
//            m_Rigidbody.MovePosition(position);
//        }


//        public void SetRotation(Quaternion rotation)
//        {
//            m_Rigidbody.MoveRotation(rotation.normalized);
//        }


//        public void StopMovement()
//        {
//            m_Rigidbody.velocity = Vector3.zero;
//            m_Moving = false;
//        }


//        #endregion













//        protected string[] debugMsgs;
//        //Camera mainCamera;
//        protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);
//        protected Color _Magenta = new Color(0.75f, 0, 0.75f, 0.9f);




//        protected virtual void DrawGizmos()
//        {
//            if (m_Rigidbody == null) return;

//            DrawCheckGroundGizmo();
//            //  Move direction
//            //Gizmos.color = Color.blue;
//            //Gizmos.DrawRay(m_Rigidbody.position + debugHeightOffset, m_MoveDirection);

//            Gizmos.color = Color.magenta;
//            Gizmos.DrawSphere(m_CapsuleCollider.bounds.min, 0.08f);

//            //GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_MoveDirection, Color.white);
//            //  Velocity
//            //Gizmos.color = Color.cyan;
//            //Gizmos.DrawRay(m_Rigidbody.position + debugHeightOffset, m_Transform.InverseTransformDirection(m_Velocity));
//        }



//        protected virtual void DebugMessages()
//        {
//            debugMsgs = new string[]
//            {
//                //string.Format("InputAngle: {0}", m_InputAngle),
//                //string.Format("StopMovement Rad2Deg: {0}", m_StopMovementThreshold * Mathf.Rad2Deg),
//                //string.Format("StopMovement Rad2Deg: {0}", Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg),
//                //string.Format("hitDetectionEndRayAngle: {0}", hitDetectionEndRayAngle ),
//                //string.Format("VelocityDot: {0}", (float)Math.Round(Vector3.Dot(m_VerticalVelocity, m_Gravity), 2) ),
//                string.Format("SlopeAngle: {0}", m_SlopeAngle)
//                //string.Format("RigidbodyVelY: {0}", (float)Math.Round(m_Rigidbody.velocity.y, 2)),
//            };
//        }




//        private void OnGUI()
//        {
//            if (Application.isPlaying && m_Debug && Time.timeScale != 0)
//            {
//                //if (mainCamera == null){
//                //    mainCamera = Camera.current;
//                //}
//                GUI.color = CharacterControllerUtility.DebugTextColor;
//                Rect rect = CharacterControllerUtility.CharacterControllerRect;
//                GUI.BeginGroup(rect, GUI.skin.box);
//                //GUI.Label(rect, debugMessages.ToString(0, debugMessages.Length));
//                for (int i = 0; i < debugMsgs.Length; i++)
//                {
//                    rect.y = 16 * i;
//                    //GUI.Label(rect, debugMsgs[i]);
//                    GUI.Label(rect, debugMsgs[i], CharacterControllerUtility.GuiStyle);
//                }
//                GUI.EndGroup();
//            }

//        }




//        protected void OnDrawGizmos()
//        {

//            #region Slope Check
//            //float slopeCheckHeight = 0.5f;
//            //Quaternion rotation = Quaternion.AngleAxis(m_SlopeLimit, -transform.right);
//            ////  Hypotenuse
//            //Gizmos.color = _Magenta;
//            //float slopeCheckHypotenuse = slopeCheckHeight / Mathf.Cos(m_SlopeLimit);
//            //Vector3 slopeAngleVector = slopeCheckHypotenuse * transform.forward;
//            //Gizmos.DrawRay(transform.position + (transform.forward) * 0.3f, rotation * slopeAngleVector - (slopeAngleVector * 0.3f));

//            ////Check distance
//            //Gizmos.color = Color.magenta;
//            //float slopeCheckDistance = Mathf.Tan(m_SlopeLimit) * slopeCheckHeight;
//            //Vector3 slopeCheckVector = slopeCheckDistance * transform.forward;
//            //Gizmos.DrawRay(transform.position + transform.up * slopeCheckHeight, slopeCheckVector );//- (transform.forward) * 0.3f);
//            #endregion

//            if (m_Debug && Application.isPlaying)
//            {
//                //  -----
//                //  Debug messages
//                if (m_Debug) DebugMessages();
//                //  -----

//                DrawGizmos();
//            }

//        }


//    }

//}
