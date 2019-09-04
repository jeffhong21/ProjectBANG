//namespace CharacterController
//{
//    using UnityEngine;
//    using System;

//    using MathUtil = MathUtilities;

//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
//    public abstract class RigidbodyCharacterController : MonoBehaviour
//    {
//        public enum MovementTypes { Adventure, Combat };




//        #region Inspector properties
//        //  Locomotion variables
//        [Group("Motor")]
//        [SerializeField] protected bool m_UseRootMotion = true;
//        [Group("Motor")]
//        [SerializeField] protected bool m_UseRootMotionRotation = false;
//        [Group("Motor")]
//        [SerializeField] protected float m_RootMotionSpeedMultiplier = 1;
//        [Group("Motor")]
//        [SerializeField] protected float m_RootMotionRotationMultiplier = 1;
//        [Group("Motor")]
//        [SerializeField] protected float m_GroundAcceleration = 0.12f;
//        [Group("Motor")]
//        [SerializeField] protected float m_AirborneAcceleration = 0.2f;
//        [Group("Motor")]
//        [SerializeField] protected float m_MotorDamping = 0.3f;
//        [Group("Motor")]
//        [SerializeField] protected float m_AirborneDamping = 0.3f;
//        [Group("Motor")]
//        [SerializeField] protected float m_GroundSpeed = 1f;
//        [Group("Motor")]
//        [SerializeField] protected float m_AirborneSpeed = 0.5f;
//        [Group("Motor")]
//        [SerializeField] protected float m_RotationSpeed = 10f;
//        [Group("Motor")]
//        [SerializeField] protected float m_SlopeForceUp = 1f;
//        [Group("Motor")]
//        [SerializeField] protected float m_SlopeForceDown = 1.25f;



//        //  -- Physics variables

//        [Group("Physics")]
//        [SerializeField] protected float m_DetectObjectHeight = 0.4f;

//        [Group("Physics")]
//        protected float m_Mass = 100;
//        [Group("Physics")]
//        protected float m_SkinWidth = 0.08f;
//        [Range(0, 90)]
//        protected float m_SlopeLimit = 45f;

//        protected float m_MaxStepHeight = 0.4f;



//        [Group("Gravity")]
//        protected float m_GravityModifier = 0.4f;
//        [Group("Gravity")]
//        protected float m_GroundStickiness = 6f;

//        //  -- Collision detection






//        protected LayerMask m_CollisionsLayerMask;

//        protected int m_MaxCollisionCount = 100;




//        //  -- Animation
//        [Group("Animation")]
//        [SerializeField] protected float m_IdleRotationMultiplier = 2f;


//        #endregion

//        [SerializeField]
//        protected CharacterControllerDebugger Debugger = new CharacterControllerDebugger();


//        protected CapsuleCollider m_Collider;
//        protected float m_TimeScale = 1;


//        protected MovementTypes m_MovementType = MovementTypes.Adventure;
//        protected bool m_Moving, m_Grounded = true;
//        protected float m_RotationAngle;
//        protected Vector3 m_InputVector;
//        protected Vector3 m_MoveDirection, m_Velocity, m_AngularVelocity;
//        protected Quaternion m_LookRotation = Quaternion.identity, m_TargetRotation = Quaternion.identity;



//        protected float m_GroundAngle;
//        protected Vector3 m_GroundSlopeDir;


//        protected RaycastHit m_GroundHit;
//        protected Collider[] m_Collisions;

//        protected Vector3 m_ColliderCenter;
//        protected float m_ColliderHeight;
//        protected Collider[] m_LinkedColliders = new Collider[0];
//        protected PhysicMaterial m_PhysicsMaterial;


//        protected Vector3 previousPosition, currentPosition, targetPosition;
//        protected Vector3 previousVelocity, currentVelocity, targetVelocity;
//        protected int motionTrajectoryResolution;
//        protected Vector3[] motionTrajectory;
//        protected float deltaAngle;

//        protected float m_Speed;


//        protected Vector3 moveDirectionSmooth, m_ExternalForceSmooth;
//        protected Vector3 velocitySmooth;
//        protected Vector3 externalForceSmooth;
//        protected float rotationVelocitySmooth, angularDragSmooth;




//        protected Animator m_Animator;
//        protected AnimatorMonitor m_AnimationMonitor;
//        protected LayerManager m_Layers;
//        protected Rigidbody m_Rigidbody;
//        protected GameObject m_GameObject;
//        protected Transform m_Transform;
//        protected float m_DeltaTime, deltaTime, fixedDeltaTime;



//        #region Parameters for Editor

//        //  For Editor.
//        //  Debug parameters.
//        public bool DebugMode { get { return m_Debug; } set { m_Debug = value; } }

//        protected bool m_Debug;
//        protected bool DebugGroundCheck { get { return Debugger.states.showGroundCheck; } }
//        protected bool DebugCollisions { get { return Debugger.states.showCollisions; } }
//        protected bool DebugMotion { get { return Debugger.states.showMotion; } }

//        [SerializeField, HideInInspector]
//        private bool displayMovement, displayPhysics, displayAnimations, displayActions;

//        #endregion





//        #region Properties

//        public MovementTypes Movement { get { return m_MovementType; } }

//        public bool Moving { get { return m_Moving; } set { m_Moving = value; } }

//        public bool Grounded { get { return m_Grounded; } set { m_Grounded = value; } }

//        public Vector3 InputVector
//        {
//            get
//            {
//                if (m_InputVector.sqrMagnitude > 1) m_InputVector.Normalize();
//                return m_InputVector;
//            }
//            set { m_InputVector = value; }
//        }

//        public Vector3 MoveDirection { get { return m_MoveDirection; } protected set { m_MoveDirection = value; } }

//        public Quaternion LookRotation { get { return m_LookRotation; } set { m_LookRotation = value; } }

//        public float Speed { get { return Mathf.Abs(m_Speed); } set { m_Speed = Mathf.Abs(value); } }

//        public float RotationSpeed { get { return m_RotationSpeed; } set { m_RotationSpeed = value; } }

//        public bool UseRootMotion { get { return m_UseRootMotion; } set { m_UseRootMotion = value; } }

//        public Vector3 Gravity { get; protected set; }

//        public Vector3 Velocity { get { return m_Velocity; } set { m_Velocity = value; } }

//        public CapsuleCollider Collider { get { return m_Collider; } protected set { m_Collider = value; } }

//        public RaycastHit GroundHit { get { return m_GroundHit; } }

//        public Vector3 raycastOrigin { get { return m_Transform.position + Vector3.up * m_SkinWidth; } }

//        public Vector3 RootMotionVelocity { get; set; }

//        public Quaternion RootMotionRotation { get; set; }

//        public Vector3 LookDirection
//        {
//            get
//            {
//                var lookDirection = m_Transform.InverseTransformDirection(LookRotation * m_Transform.forward);
//                return lookDirection;
//            }
//        }

//        public float LookAngle
//        {
//            get
//            {
//                var lookDirection = m_Transform.InverseTransformDirection(LookRotation * m_Transform.forward);
//                var axisSign = Vector3.Cross(lookDirection, m_Transform.forward);
//                return Vector3.Angle(m_Transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f);
//            }
//        }

//        public Vector3 ColliderCenter { get { return Collider.center; } }
//        public float ColliderHeight { get { return Collider.height * m_Transform.lossyScale.x; } }
//        public float ColliderRadius { get { return Collider.radius * m_Transform.lossyScale.x; } }


//        #endregion



//        protected virtual void Awake()
//        {
//            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
//            m_Animator = GetComponent<Animator>();

//            m_Rigidbody = GetComponent<Rigidbody>();
//            m_Layers = GetComponent<LayerManager>();

//            m_Collider = GetComponent<CapsuleCollider>();
//            if (m_Collider == null)
//            {
//                for (int index = 0; index < transform.childCount; index++)
//                {
//                    GameObject childObject = transform.GetChild(index).gameObject;
//                    if (childObject.layer == LayerManager.CharacterCollider)
//                    {
//                        if (childObject.GetComponent<CapsuleCollider>() != null)
//                        {
//                            m_Collider = childObject.GetComponent<CapsuleCollider>();
//                        }
//                        else
//                        {
//                            m_Collider = childObject.AddComponent<CapsuleCollider>();
//                            m_Collider.radius = 0.3f;
//                            m_Collider.height = 1.8f;
//                            m_Collider.center = new Vector3(0, m_Collider.height / 2, 0);
//                        }
//                        break;
//                    }
//                }

//                if (m_Collider == null)
//                {
//                    var colliderObject = new GameObject("Colliders", typeof(CapsuleCollider));
//                    colliderObject.transform.parent = transform;
//                    colliderObject.layer = LayerManager.CharacterCollider;
//                    m_Collider = colliderObject.GetComponent<CapsuleCollider>();
//                    m_Collider.radius = 0.3f;
//                    float colliderHeight = (float)Math.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2, 2);
//                    m_Collider.height = colliderHeight;
//                    m_Collider.center = new Vector3(0, m_Collider.height / 2, 0);
//                }
//            }

//            m_CollisionsLayerMask = m_Layers.SolidLayers;
//            m_Collisions = new Collider[m_MaxCollisionCount];

//            m_GameObject = gameObject;
//            m_Transform = transform;

//            m_DeltaTime = Time.deltaTime;

//            Gravity = Physics.gravity;

//            deltaTime = Time.deltaTime;
//            fixedDeltaTime = Time.fixedDeltaTime;

//            //  Initialize debugger;
//            Debugger.Initialize(this);
//        }


//        protected void Start()
//        {
//            m_LookRotation = m_Transform.rotation;


//            m_Rigidbody.mass = m_Mass;
//            m_Rigidbody.useGravity = false;
//            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

//            m_ColliderHeight = m_Collider.height;
//            m_ColliderCenter = m_Collider.center;

//            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
//            m_Animator.applyRootMotion = m_UseRootMotion;

//            m_PhysicsMaterial = new PhysicMaterial();

//            previousPosition = m_Rigidbody.position;
//            previousVelocity = m_Rigidbody.velocity;
//        }


//        protected virtual void Update()
//        {
//            m_TimeScale = Time.timeScale;
//            if (Math.Abs(m_TimeScale) < float.Epsilon) return;
//            m_DeltaTime = deltaTime;


//            previousPosition = m_Rigidbody.position;
//            previousVelocity = m_Rigidbody.velocity;
//        }


//        protected virtual void FixedUpdate()
//        {
//            if (Math.Abs(m_TimeScale) < float.Epsilon) return;
//            m_DeltaTime = fixedDeltaTime;



//        }


//        protected virtual void LateUpdate()
//        {
//            //for (int i = 0; i < m_Collisions.Length; i++) {
//            //    m_Collisions[i] = null;
//            //}


//        }


//        protected virtual void OnAnimatorMove()
//        {
//            AnimatorMove();
//        }








//        /// <summary>
//        /// Move charatcer based on input values.
//        /// </summary>
//        protected virtual void Move()
//        {
//            float speedMultiplier = 1;
//            //  Set the input vector, move direction and rotation angle based on the movement type.
//            switch (m_MovementType)
//            {
//                case (MovementTypes.Adventure):
//                    m_RotationAngle = GetAngleFromForward(m_InputVector);

//                    m_InputVector = m_Transform.InverseTransformDirection(m_InputVector);
//                    //m_MoveDirection = m_Transform.forward * m_InputVector.z;
//                    m_MoveDirection = Vector3.SmoothDamp(m_MoveDirection, m_Transform.forward * m_InputVector.z, ref moveDirectionSmooth, 0.1f);

//                    m_InputVector.z = Mathf.Abs(m_MoveDirection.x) + Mathf.Abs(m_MoveDirection.z);
//                    if (Mathf.Abs(m_RotationAngle) > 0.1f) m_InputVector.z += Mathf.Abs(m_RotationAngle);
//                    m_InputVector.x = 0;


//                    break;

//                case (MovementTypes.Combat):
//                    m_RotationAngle = GetAngleFromForward(m_LookRotation * m_Transform.forward);
//                    //m_MoveDirection = m_Transform.TransformDirection(m_InputVector);
//                    m_MoveDirection = Vector3.SmoothDamp(m_MoveDirection, m_Transform.TransformDirection(m_InputVector), ref moveDirectionSmooth, 0.1f);

//                    speedMultiplier = 0;
//                    break;
//            }

//            m_Speed = m_InputVector.normalized.sqrMagnitude * speedMultiplier;
//            Moving = Mathf.Clamp01(Mathf.Abs(InputVector.x) + Mathf.Abs(InputVector.z)) > 0.1f;


//            if (m_UseRootMotion)
//            {
//                Quaternion moveRotation = Quaternion.AngleAxis(MathUtil.Round(m_RotationAngle, 6), m_Transform.up);
//                targetVelocity = moveRotation * (RootMotionVelocity / m_DeltaTime);

//                //previousPosition = m_Transform.TransformPoint(targetPosition);
//                targetPosition = m_Transform.position + Quaternion.Inverse(m_Transform.rotation) * m_Transform.TransformDirection(RootMotionVelocity / m_DeltaTime);
//                //targetPosition = Vector3.Lerp(previousPosition, targetPosition, deltaAngle / m_RotationAngle);
//            }
//            else
//            {
//                targetVelocity = m_MoveDirection * (Grounded ? m_GroundSpeed : m_AirborneSpeed);
//                targetPosition = m_Transform.position + Quaternion.Inverse(m_Transform.rotation) * m_Transform.TransformDirection(targetVelocity);
//            }

//            if (DebugMotion) Debug.DrawRay(m_Transform.position + Vector3.up * 0.2f, targetVelocity, Color.blue);
//            if (DebugMotion) Debug.DrawLine(m_Transform.position + Vector3.up * 0.1f, targetPosition + Vector3.up * 0.1f, Color.green);
//            if (DebugMotion) DebugDraw.Sphere(targetPosition + Vector3.up * 0.1f, 0.1f, Color.green);

//            CharacterDebug.Log("<b><color=magenta>*** m_RotationAngle 1</color></b>", MathUtil.Round(m_RotationAngle, 5));
//            CharacterDebug.Log("<b><color=magenta>*** deltaAngle 1</color></b>", MathUtil.Round(deltaAngle / m_RotationAngle, 5));
//        }


//        /// <summary>
//        /// Perform checks to determine if the character is on the ground.
//        /// </summary>
//        protected virtual void CheckGround()
//        {
//            m_Grounded = false;
//            m_GroundAngle = 0;

//            float groundDistance = 0;
//            float groundAngle = 0;
//            m_GroundHit.point = m_Transform.position - m_Transform.up * 0.6f;
//            m_GroundHit.normal = m_Transform.up;

//            //Vector3 raycastOrigin = m_Transform.position + Vector3.up * m_Collider.radius;
//            float radius = m_Collider.radius - m_SkinWidth;
//            float raycastLength = m_Collider.radius * 2 + m_SkinWidth;

//            if (Physics.Raycast(raycastOrigin, Vector3.down, out m_GroundHit, raycastLength, m_Layers.SolidLayers))
//            {
//                groundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, transform.up).magnitude;
//                groundAngle = Vector3.Angle(m_GroundHit.normal, Vector3.up);

//                if (Physics.SphereCast(raycastOrigin, radius, Vector3.down, out m_GroundHit, raycastLength, m_Layers.SolidLayers))
//                {

//                    groundAngle = Vector3.Angle(m_GroundHit.normal, Vector3.up);

//                    if (groundDistance > m_GroundHit.distance - m_Collider.radius - m_SkinWidth)
//                        groundDistance = m_GroundHit.distance - m_Collider.radius - m_SkinWidth;
//                }

//                groundDistance = (float)Math.Round(groundDistance, 2);
//                if (groundDistance < 0.3f && groundAngle < 85)
//                {
//                    m_Grounded = true;
//                    //var averagePosition = GetAverageRaycast(m_Collider.radius + m_SkinWidth, m_Collider.radius + m_SkinWidth);
//                    //m_Rigidbody.MovePosition(Vector3.MoveTowards(m_Rigidbody.position, averagePosition, m_DeltaTime * 4));
//                }


//            }
//            m_GroundAngle = groundAngle;


//            //  Draw Sphere cast
//            if (DebugGroundCheck) DebugDraw.Sphere(raycastOrigin + Vector3.down * m_GroundHit.distance, radius, m_Grounded ? Color.green : Color.grey);
//            if (DebugGroundCheck) if (Grounded) Debug.DrawLine(raycastOrigin, m_GroundHit.point, m_Grounded ? Color.green : Color.grey);
//            if (DebugGroundCheck) DebugDraw.DrawMarker(m_GroundHit.point, 0.1f, Color.green);

//            CharacterDebug.Log("<color=green>Ground Hit Distance</color>", groundDistance);

//        }


//        /// <summary>
//        /// Ensure the current movement direction is valid.
//        /// </summary>
//        protected virtual void CheckMovement()
//        {

//            //float castRadius = m_Collider.radius + m_SkinWidth;

//            //Vector3 p1 = raycastOrigin + m_Collider.center + Vector3.up * (m_Collider.height * 0.5f - castRadius);
//            //Vector3 p2 = raycastOrigin + m_Collider.center - Vector3.up * (m_Collider.height * 0.5f - castRadius);

//            //var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, m_Collisions, m_CollisionsLayerMask);

//            //if (hits > 0) {
//            //    var centerMass = m_Transform.TransformPoint(m_Rigidbody.centerOfMass);
//            //    for (int i = 0; i < hits; i++)
//            //    {
//            //        var collision = m_Collisions[i];
//            //        Vector3 direction;
//            //        float distance;
//            //        if (Physics.ComputePenetration( m_Collider, m_Transform.position, m_Transform.rotation,
//            //                                        collision, collision.transform.position, collision.transform.rotation,
//            //                                        out direction, out distance))
//            //        {
//            //            Vector3 penetrationVector = direction * distance;
//            //            Vector3 moveDirectionProjected = Vector3.Project(m_MoveDirection, -direction);
//            //            //  m_transform.position = m_transform.position + penetrationVector;
//            //            m_Rigidbody.MovePosition(m_Transform.position + penetrationVector);
//            //            m_MoveDirection -= moveDirectionProjected;
//            //        }

//            //        var closestPoint = m_Collisions[i].ClosestPointOnBounds(centerMass);


//            //        Debug.DrawLine(centerMass, closestPoint, Color.green);
//            //        DebugDraw.Sphere(closestPoint, 0.05f, Color.green);

//            //        Rigidbody rb = collision.attachedRigidbody;
//            //        if(rb != null) {
//            //            if(rb.mass > m_Mass) {

//            //            }
//            //        }
//            //        //m_Collisions[i].
//            //    }
//            //}
//            //
//            //  If walk into wall.
//            //

//            //RaycastHit collisionHit;
//            //if (Physics.Raycast(m_Transform.position + Vector3.up * m_MaxStepHeight, m_Transform.forward, out collisionHit, 1.5f + m_Collider.radius, m_Layers.SolidLayers)) {
//            //    Vector3 groundNormal = m_GroundHit.normal;
//            //    Vector3 collisionNormal = collisionHit.normal;
//            //    Vector3.OrthoNormalize(ref groundNormal, ref collisionNormal);

//            //    Vector3 lookDir = m_LookRotation * m_Transform.forward;
//            //    float side = Vector3.Cross(lookDir, collisionNormal).y < 0 ? 1 : -1;
//            //    Vector3 desiredVector = Vector3.Cross(m_Transform.up, collisionNormal) * side;
//            //    m_CollisionRotation = Quaternion.FromToRotation(m_Transform.forward, desiredVector);

//            //    float angle = Vector3.Angle(m_Transform.forward, desiredVector);
//            //    Debug.DrawRay(m_Transform.position + Vector3.up * m_MaxStepHeight, desiredVector, Color.red);
//            //} else {
//            //    m_CollisionRotation = Quaternion.identity;
//            //}

//            m_GroundSlopeDir = Vector3.zero;

//            if (Grounded)
//            {
//                //  Find the vector that represents the slope.
//                Vector3 groundRight = Vector3.Cross(m_GroundHit.normal, Vector3.down);
//                m_GroundSlopeDir = Vector3.Cross(groundRight, m_GroundHit.normal);

//                // Slopes
//                Vector3 slopeCheckOffset = m_Transform.forward * (m_Collider.radius + m_SkinWidth);
//                RaycastHit slopeHit1;
//                RaycastHit slopeHit2;
//                if (Physics.Raycast(raycastOrigin + slopeCheckOffset, Vector3.down, out slopeHit1, m_Layers.SolidLayers))
//                {
//                    if (DebugCollisions) Debug.DrawLine(raycastOrigin + slopeCheckOffset, slopeHit1.point, m_Grounded ? Color.green : Color.gray);

//                    float forwardAngle = Vector3.Angle(slopeHit1.normal, Vector3.up);
//                    if (Physics.Raycast(raycastOrigin - slopeCheckOffset, Vector3.down, out slopeHit2, m_Layers.SolidLayers))
//                    {
//                        if (DebugCollisions) Debug.DrawLine(raycastOrigin - slopeCheckOffset, slopeHit2.point, m_Grounded ? Color.green : Color.gray);

//                        float backAngle = Vector3.Angle(slopeHit2.normal, Vector3.up);
//                        float[] groundAngles = { m_GroundAngle, forwardAngle, backAngle };
//                        Array.Sort(groundAngles);
//                        m_GroundAngle = groundAngles[1];
//                    }
//                    else
//                    {
//                        m_GroundAngle = (m_GroundAngle + forwardAngle) / 2;
//                    }
//                }

//                //if(m_GroundAngle > 0) {
//                //    //  What to do if ground angle is greater than slope limit.
//                //    if (m_GroundAngle > m_SlopeLimit) {
//                //        //  sliding is true.
//                //        m_Moving = false;
//                //        var localDir = m_Transform.InverseTransformDirection(m_GroundSlopeDir);
//                //        m_RotationAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
//                //        var targetDirection = Vector3.Project(m_MoveDirection, m_GroundSlopeDir).normalized * m_SlopeForceDown;
//                //        m_MoveDirection = Vector3.Lerp(m_MoveDirection, targetDirection, m_DeltaTime * m_RotationSpeed);
//                //        m_Rigidbody.AddForce(m_MoveDirection, ForceMode.Impulse);
//                //        CharacterDebug.Log("<color=red> ** Sliding  </color>", "Sliding");
//                //    }
//                //    //else {
//                //    //    Vector3 slopeDirection = Vector3.Cross(m_Transform.right, m_GroundHit.normal);
//                //    //    float direction = Mathf.Sign(slopeDirection.y);
//                //    //    m_MoveDirection = Vector3.Project(m_MoveDirection, slopeDirection).normalized * (direction > 0 ? m_SlopeForceUp : m_SlopeForceDown);
//                //    //}


//                //    //float slopeStartAngle = 0;
//                //    //float slopeEndAngle = m_SlopeLimit;
//                //    //float angle = 90 - Vector3.Angle(m_Transform.forward, m_GroundHit.normal);
//                //    //angle -= slopeStartAngle;
//                //    //float range = slopeEndAngle - slopeStartAngle;
//                //    //float slopeDamper = 1f - Mathf.Clamp(angle / range, 0f, 1f);
//                //    //CharacterDebug.Log("<color=green>anle  </color>", angle);
//                //    //CharacterDebug.Log("<color=green>slopeDamper  </color>", slopeDamper);
//                //}






//                //if (Physics.RaycastNonAlloc(m_Transform.position + Vector3.up * m_MaxStepHeight, m_Transform.forward, m_Collisions, m_Collider.radius + 1, m_CollisionsLayerMask) > 0) {

//                //}

//                float offset = m_Collider.radius + m_SkinWidth;
//                Vector3 groundAverage = GetAverageRaycast(offset, offset, 2);
//                if (groundAverage != m_Rigidbody.position)
//                {
//                    m_Rigidbody.MovePosition(new Vector3(m_Transform.position.x, groundAverage.y + 0.1f, m_Transform.position.z));
//                }

//            }
//            else
//            {


//            }


//            CharacterDebug.Log("<color=green>Ground Angle</color>", m_GroundAngle);

//        }





//        /// <summary>
//        /// Update the character’s position values.
//        /// </summary>
//        protected virtual void UpdateMovement()
//        {
//            //m_Velocity = Vector3.zero;

//            if (m_UseRootMotion)
//            {
//                m_Velocity = RootMotionVelocity / m_DeltaTime;
//                m_Velocity = Vector3.Project((RootMotionVelocity / m_DeltaTime), m_MoveDirection);
//                m_Velocity.y = Grounded ? 0 : m_Rigidbody.velocity.y;
//                //m_MoveDirection = RootMotionVelocity / m_DeltaTime;
//            }
//            else
//            {
//                targetVelocity = m_MoveDirection * (Grounded ? m_GroundSpeed : m_AirborneSpeed);
//                float acceleration;
//                if (Moving) acceleration = Grounded ? m_GroundAcceleration : m_AirborneAcceleration;
//                else acceleration = Grounded ? m_MotorDamping : m_AirborneDamping;
//                m_Velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref velocitySmooth, acceleration);
//            }



//            // // Drag
//            // currentVelocity *= (1f / (1f + (Drag * deltaTime)));

//            if (m_Grounded)
//            {

//                //m_Velocity = m_TargetRotation * m_Velocity;

//                if (m_GroundAngle > m_SlopeLimit)
//                {
//                    Vector3 drag = m_Velocity * (m_GroundAngle - m_SlopeLimit) * m_DeltaTime;
//                    if (m_MoveDirection.sqrMagnitude > 0)
//                    {
//                        drag *= (1f - Vector3.Dot(m_MoveDirection.normalized, m_Velocity.normalized));
//                    }

//                    if (drag.sqrMagnitude > m_Velocity.sqrMagnitude) m_Velocity = Vector3.zero;
//                    else m_Velocity -= drag;
//                }

//                m_Velocity = Vector3.SmoothDamp(m_Velocity, m_Velocity + m_MoveDirection, ref velocitySmooth, 0.1f);
//                m_Velocity.y = 0;
//                m_Velocity = Vector3.ProjectOnPlane(m_Velocity, m_GroundHit.normal * m_GroundStickiness);
//            }
//            else
//            {
//                m_Velocity -= Vector3.Project(m_MoveDirection, m_Transform.forward);
//                m_Velocity += (Gravity * m_GravityModifier) * m_DeltaTime;
//                //Vector3 verticalVelocity = (m_Velocity - m_PreviousPosition) * m_DeltaTime;
//                //verticalVelocity = Vector3.Project(verticalVelocity, Gravity);
//                //m_Velocity += verticalVelocity;
//            }


//            m_Rigidbody.velocity = m_Velocity;

//            //m_Rigidbody.MovePosition(m_MoveDirection + m_Rigidbody.position);

//            //DebugDraw.Sphere(m_Transform.position + m_Velocity, 0.08f, Color.black);
//            //Debug.DrawRay(m_Transform.position, m_Velocity, Color.black);

//        }


//        /// <summary>
//        /// Update the character’s rotation values.
//        /// </summary>
//        protected virtual void UpdateRotation()
//        {


//            float moveAmount = Mathf.Clamp01(Mathf.Abs(m_MoveDirection.x) + Mathf.Abs(m_MoveDirection.z));
//            moveAmount = (float)Math.Round(moveAmount, 2);
//            float rotationSpeed = Mathf.Lerp(m_RotationSpeed * m_IdleRotationMultiplier, m_RotationSpeed, moveAmount);



//            if (m_UseRootMotionRotation)
//            {
//                RootMotionRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

//                m_TargetRotation = Quaternion.AngleAxis(angleInDegrees, rotationAxis.normalized);
//                m_AngularVelocity = rotationAxis.normalized * angleInDegrees;
//                m_RotationAngle = (float)Math.Round(angleInDegrees, 2);
//            }
//            else
//            {
//                m_RotationAngle *= rotationSpeed * m_DeltaTime;
//                m_RotationAngle = Mathf.SmoothDampAngle(m_RotationAngle, (m_RotationAngle * rotationSpeed * m_DeltaTime), ref rotationVelocitySmooth, 0.1f);
//                m_RotationAngle = (float)Math.Round(m_RotationAngle, 2);

//                m_TargetRotation = Quaternion.AngleAxis(m_RotationAngle, m_Transform.up.normalized);
//                m_AngularVelocity = m_Transform.up.normalized * m_RotationAngle;
//            }


//            //  Update angular velocity.
//            m_Rigidbody.angularDrag = Mathf.SmoothDamp(m_Rigidbody.angularDrag, moveAmount > 0 ? 0.05f : m_Mass, ref angularDragSmooth, 0.1f);
//            m_Rigidbody.angularVelocity = Vector3.Lerp(m_Rigidbody.angularVelocity, m_AngularVelocity, m_DeltaTime * rotationSpeed);


//            //  Update the rotations.
//            //m_LookRotation = Quaternion.Slerp(m_LookRotation, m_CollisionRotation, m_DeltaTime * m_RotationSpeed);
//            m_Rigidbody.MoveRotation(m_TargetRotation * m_Transform.rotation);






//            //
//            //  AddRelativeTorque adds torque according to its Inertia Tensors. Therefore, the desired angular
//            //  velocity must be transformed according to the Inertia Tensor, to get the required Torque.
//            //

//            //// Rotate about Y principal axis
//            //Vector3 desiredAngularVelInY = new Vector3(0, Mathf.PI, 0); //  1/2 revs per second 
//            //Vector3 torque = rigidbodyCached.inertiaTensorRotation * Vector3.Scale(rigidbodyCached.inertiaTensor, desiredAngularVelInY);
//            //rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);
//        }



//        /// <summary>
//        /// Apply rotation.
//        /// </summary>
//        protected virtual void ApplyRotation()
//        {

//        }


//        /// <summary>
//        /// Apply position values.
//        /// </summary>
//        protected virtual void ApplyMovement()
//        {

//        }


//        /// <summary>
//        /// Updates the animator.
//        /// </summary>
//        protected virtual void UpdateAnimator()
//        {

//            float lookAngle = 0;
//            float maxLookAngle = 90;
//            if (LookAngle > maxLookAngle || LookAngle < -maxLookAngle) lookAngle = Mathf.Lerp(maxLookAngle, 0, m_DeltaTime * 4);
//            else lookAngle = LookAngle;
//            lookAngle = MathUtil.Round(lookAngle);
//            CharacterDebug.Log("<b><color=magenta>*** lookAngle 1</color></b>", lookAngle);

//            float rotationAngle = GetAngleFromForward(m_Transform.forward) - deltaAngle;
//            deltaAngle = 0;
//            rotationAngle *= 0.01f;
//            rotationAngle = Mathf.Clamp(rotationAngle / Time.deltaTime, -1f, 1f);
//            rotationAngle = MathUtil.Round(rotationAngle, 8);
//            CharacterDebug.Log("<b><color=magenta>*** rotationAngle 1</color></b>", rotationAngle);

//            m_Animator.SetFloat(HashID.LookAngle, lookAngle);

//            m_Animator.SetBool(HashID.Grounded, Grounded);

//            m_Animator.SetFloat(HashID.Speed, Speed);

//            m_Animator.SetFloat(HashID.Rotation, rotationAngle);

//            //m_Animator.SetFloat(HashID.Rotation, Mathf.Clamp(m_RotationAngle, -2, 2), 0.1f, m_DeltaTime);

//            m_Animator.SetBool(HashID.Moving, Moving);


//            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
//            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);



//        }


//        /// <summary>
//        /// Anything that should be done in the OnAnimatorMove function.
//        /// </summary>
//        protected virtual void AnimatorMove()
//        {
//            Vector3 f = m_Animator.deltaRotation * Vector3.forward;
//            deltaAngle += Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;

//            if (m_UseRootMotion)
//            {
//                RootMotionVelocity = m_Animator.deltaPosition * m_RootMotionSpeedMultiplier;
//                if (m_Animator.hasRootMotion) m_Rigidbody.MovePosition(m_Animator.rootPosition);
//            }

//            if (m_UseRootMotionRotation)
//            {
//                //float angleInDegrees;
//                //Vector3 rotationAxis;
//                m_Animator.deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
//                angleInDegrees = (angleInDegrees * m_RootMotionRotationMultiplier * Mathf.Deg2Rad) / m_DeltaTime;
//                RootMotionRotation = Quaternion.AngleAxis(angleInDegrees, rotationAxis);

//                if (m_Animator.hasRootMotion) m_Rigidbody.MoveRotation(m_Animator.rootRotation);
//            }


//        }


//        /// <summary>
//        /// Set the collider's physics material.
//        /// </summary>
//        protected virtual void SetPhysicsMaterial()
//        {
//            //change the physics material to very slip when not grounded or maxFriction when is

//            //  Airborne.
//            if (!m_Grounded && Mathf.Abs(m_Rigidbody.velocity.y) > 0)
//            {
//                m_Collider.material.staticFriction = 0f;
//                m_Collider.material.dynamicFriction = 0f;
//                m_Collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
//            }
//            //  Grounded and is moving.
//            else if (m_Grounded && m_Moving)
//            {
//                m_Collider.material.staticFriction = 0.25f;
//                m_Collider.material.dynamicFriction = 0f;
//                m_Collider.material.frictionCombine = PhysicMaterialCombine.Multiply;
//            }
//            //  Grounded but not moving.
//            else if (m_Grounded && !m_Moving)
//            {
//                m_Collider.material.staticFriction = 1f;
//                m_Collider.material.dynamicFriction = 1f;
//                m_Collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
//            }

//            else
//            {
//                m_Collider.material.staticFriction = 1f;
//                m_Collider.material.dynamicFriction = 1f;
//                m_Collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
//            }

//            //m_Collider.material = m_AirFrictionMaterial;
//        }







//        public Vector3 GetDirectionTangentToSurface(Vector3 normal)
//        {
//            Vector3 tangent;
//            Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
//            Vector3 t2 = Vector3.Cross(normal, Vector3.up);
//            if (t1.sqrMagnitude > t2.sqrMagnitude)
//            {
//                tangent = t1;
//            }
//            else
//            {
//                tangent = t2;
//            }
//            return tangent;
//        }


//        public void CollisionsOverlap(Vector3 position)
//        {
//            float castRadius = m_Collider.radius + m_SkinWidth;

//            Vector3 p1 = position + m_Collider.center + Vector3.up * (m_Collider.height * 0.5f - castRadius);
//            Vector3 p2 = position + m_Collider.center - Vector3.up * (m_Collider.height * 0.5f - castRadius);

//            //DebugDraw.Capsule(position + m_Collider.center, Vector3.up, castRadius, m_Collider.height, Color.white);

//            var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, m_Collisions, m_CollisionsLayerMask);

//            if (hits > 0)
//            {
//                for (int i = 0; i < hits; i++)
//                {
//                    Debug.DrawLine(p1, m_Collisions[i].ClosestPoint(p1), Color.red);
//                    //m_Collisions[i].
//                }
//            }

//        }




//        /// <summary>
//        /// Get the average raycast position.
//        /// </summary>
//        /// <param name="offsetX"></param>
//        /// <param name="offsetZ"></param>
//        /// <param name="rayCount"></param>
//        /// <returns></returns>
//        protected Vector3 GetAverageRaycast(float offsetX, float offsetZ, int rayCount = 2)
//        {
//            int maxRays = 4;
//            offsetX *= 2;
//            offsetZ *= 2;
//            rayCount = Mathf.Clamp(rayCount, 2, maxRays);
//            int totalRays = rayCount * rayCount + 1;
//            Vector3[] combinedCast = new Vector3[totalRays];
//            int average = 0;
//            Vector3 rayOrigin = m_Transform.TransformPoint(0 - offsetX * 0.5f, m_MaxStepHeight + m_SkinWidth, 0 - offsetZ * 0.5f);
//            float rayLength = m_MaxStepHeight * 2;


//            float xSpacing = offsetX / (rayCount - 1);
//            float zSpacing = offsetZ / (rayCount - 1);

//            bool raycastHit = false;
//            Vector3 hitPoint = Vector3.zero;
//            Vector3 raycast = m_Transform.TransformPoint(0, m_MaxStepHeight, 0);

//            if (DebugCollisions) Debug.DrawRay(raycast, MoveDirection.normalized, Color.blue);

//            RaycastHit hit;
//            int index = 0;
//            for (int z = 0; z < rayCount; z++)
//            {
//                for (int x = 0; x < rayCount; x++)
//                {
//                    raycastHit = false;
//                    hitPoint = Vector3.zero;
//                    raycast = rayOrigin + (m_Transform.forward * zSpacing * z) + (m_Transform.right * xSpacing * x);
//                    //raycast += MoveDirection.normalized * Time.deltaTime;
//                    if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_Layers.SolidLayers))
//                    {
//                        hitPoint = hit.point;
//                        average++;
//                        raycastHit = true;
//                    }
//                    combinedCast[index] = hitPoint;
//                    index++;
//                    if (DebugCollisions) Debug.DrawRay(raycast, Vector3.down * rayLength, (raycastHit ? Color.green : Color.red));
//                }
//            }


//            hitPoint = Vector3.zero;
//            raycastHit = false;
//            raycast = m_Transform.TransformPoint(0, m_MaxStepHeight, 0);
//            //originRaycast += MoveDirection.normalized * Time.deltaTime;
//            if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_Layers.SolidLayers))
//            {
//                hitPoint = hit.point;
//                average++;
//                raycastHit = true;
//            }

//            combinedCast[totalRays - 1] = hitPoint;
//            if (DebugCollisions) DebugDraw.Circle(raycast, Vector3.up * rayLength, 0.2f, (raycastHit ? Color.blue : Color.red));



//            average = Mathf.Clamp(average, 1, int.MaxValue);

//            Vector3 averageHitPosition = Vector3.zero;
//            float xTotal = 0f, yTotal = 0f, zTotal = 0f;
//            for (int i = 0; i < combinedCast.Length; i++)
//            {
//                xTotal += combinedCast[i].x;
//                yTotal += combinedCast[i].y;
//                zTotal += combinedCast[i].z;
//            }
//            averageHitPosition.Set(xTotal / average, yTotal / average, zTotal / average);

//            if (DebugCollisions) DebugDraw.DrawMarker(averageHitPosition, 0.2f, Color.blue);

//            return averageHitPosition;
//        }








//        #region Public Functions

//        // Scale the capsule collider to 'mlp' of the initial value
//        protected void ScaleCapsule(float scale)
//        {
//            scale = Mathf.Abs(scale);
//            if (m_Collider.height < m_ColliderHeight * scale || m_Collider.height > m_ColliderHeight * scale)
//            {
//                m_Collider.height = Mathf.MoveTowards(m_Collider.height, m_ColliderHeight * scale, Time.deltaTime * 4);
//                m_Collider.center = Vector3.MoveTowards(m_Collider.center, m_ColliderCenter * scale, Time.deltaTime * 2);
//            }
//        }

//        public virtual float GetColliderHeightAdjustment()
//        {
//            return m_Collider.height;
//        }


//        // Gets angle around y axis from a world space direction
//        protected float GetAngleFromForward(Vector3 worldDirection)
//        {
//            Vector3 local = transform.InverseTransformDirection(worldDirection);
//            return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
//        }







//        //        [Tooltip("A -1 to 1 threshold for when the character should stop moving if another object collides with the character. " +
//        //"A value of 1 indicates the object is directly in front of the character's move direction while a value of -1 indicates the " +
//        //"object is directly behind the character's move direction")]
//        //        [Range(-1, 1)]
//        //        protected float m_StopMovementThreshold = 0.5f;
//        //        protected int m_VerticalCollisionCount = 10;        // Do not need
//        //        protected void VerticalCollision()
//        //        {
//        //            //  We want the cos angle since we know "adjacent" and "hypotenuse".
//        //            float startAngle = Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg;
//        //            float checkHeight = m_Collider.height / 2;
//        //            float rayDistance = 2;
//        //            float verticalAngle = 15;

//        //            Quaternion startRayRotation = Quaternion.AngleAxis(-startAngle, Vector3.up);
//        //            Vector3 startRay = startRayRotation * m_Transform.forward;
//        //            Vector3 startDir = startRay;

//        //            float angleAmount = (startAngle * 2) / (m_VerticalCollisionCount - 1);

//        //            bool detectEdge = false;

//        //            Vector3 raycastOrigin = m_Rigidbody.position + Vector3.up * checkHeight;

//        //            for (int i = 0; i < m_VerticalCollisionCount; i++)
//        //            {
//        //                if (i == 0) continue;
//        //                startDir = Quaternion.AngleAxis(angleAmount, Vector3.up) * startDir;
//        //                Vector3 hitDirection = startDir;

//        //                Quaternion angleRotation = Quaternion.AngleAxis(verticalAngle, m_Transform.right);
//        //                hitDirection = Vector3.ClampMagnitude(angleRotation * hitDirection, rayDistance);


//        //                if (Physics.Raycast(raycastOrigin, hitDirection + Vector3.up * checkHeight, rayDistance, m_CollisionsLayerMask) == false)
//        //                {

//        //                    Vector3 start = m_Rigidbody.position + (m_Transform.forward * m_Collider.radius);
//        //                    start.y += m_DetectObjectHeight;
//        //                    float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;
//        //                    if (Physics.Raycast(start, Vector3.down, maxDetectEdgeDistance, m_CollisionsLayerMask) == false)
//        //                    {
//        //                        detectEdge = true;
//        //                    }
//        //                }

//        //                if (DebugCollisions) Debug.DrawRay(raycastOrigin, hitDirection * rayDistance, detectEdge == true ? Color.red : Color.grey);
//        //            }


//        //        }














//        #endregion










//        #region Debugging



//        protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);



//        private int GetASctiveCollisions()
//        {
//            int count = 0;
//            for (int i = 0; i < m_Collisions.Length; i++)
//            {
//                if (m_Collisions[i] != null) count++;
//            }
//            return count;
//        }

//        protected virtual void DebugAttributes()
//        {

//            CharacterDebug.Log("seperator", "----------");
//            CharacterDebug.Log("<color=cyan>Moving</color>", Moving);
//            CharacterDebug.Log("<color=cyan>Grounded</color>", Grounded);
//            CharacterDebug.Log("seperator", "----------");
//            CharacterDebug.Log("InputVector", InputVector);
//            CharacterDebug.Log("MoveDirection", MoveDirection);
//            CharacterDebug.Log("Velocity", Velocity);
//            CharacterDebug.Log("<color=blue>•rbVelocity</color>", m_Rigidbody.velocity);
//            CharacterDebug.Log("CollisionsCount", GetASctiveCollisions());
//            //CharacterDebug.Log("rb_AngularVelocity", m_Rigidbody.angularVelocity);
//            //CharacterDebug.Log("rb_Velocity", m_Rigidbody.velocity.y);
//        }



//        protected virtual void DrawGizmos()
//        {

//            if (DebugMotion)
//            {
//                //Gizmos.color = Color.white;
//                //Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, m_Transform.InverseTransformDirection(m_LookRotation * m_Transform.forward));
//                //GizmosUtils.DrawText(GUI.skin, "LookDirection", transform.position + Vector3.up * 1.5f + m_LookRotation * transform.forward, Color.green);

//                //if(m_Rigidbody.velocity != Vector3.zero) {
//                //    Gizmos.color = Color.green;
//                //    GizmosUtils.DrawArrow(raycastOrigin, m_Rigidbody.velocity);
//                //    GizmosUtils.DrawText(GUI.skin, "Velocity", raycastOrigin + transform.forward, Color.green);
//                //}
//                Gizmos.color = Color.green;
//                GizmosUtils.DrawWireSphere(m_Transform.position, 0.1f);
//                GizmosUtils.DrawMarker(m_Animator.pivotPosition, 0.1f, Color.blue);
//                GizmosUtils.DrawMarker(m_Animator.rootPosition, 0.1f, Color.magenta);

//                if (DebugCollisions) GizmosUtils.DrawWireCapsule(raycastOrigin + m_Collider.center, m_Collider.radius + m_SkinWidth, m_Collider.height - m_SkinWidth);

//                if (Grounded)
//                {
//                    if (Mathf.Abs(m_GroundAngle) > 0)
//                    {
//                        Gizmos.color = Color.black;
//                        GizmosUtils.DrawArrow(raycastOrigin, m_GroundSlopeDir);

//                    }
//                }


//            }




//            GizmosUtils.DrawText(GUI.skin, Grounded.ToString(), transform.position + Vector3.up * 2f, Grounded ? Color.black : Color.red);
//        }


//        protected virtual void DrawOnGUI()
//        {
//            DebugAttributes();

//            GUI.color = CharacterControllerUtility.DebugTextColor;
//            Rect rect = CharacterControllerUtility.CharacterControllerRect;
//            GUI.BeginGroup(rect, GUI.skin.box);

//            GUI.Label(rect, CharacterDebug.Write());

//            GUI.EndGroup();
//        }


//        protected void OnGUI()
//        {
//            if (Application.isPlaying && DebugMode)
//            {
//                DrawOnGUI();
//            }

//        }

//        protected void OnDrawGizmos()
//        {
//            if (DebugMode && Application.isPlaying)
//            {
//                DrawGizmos();
//            }

//        }


//        #endregion





//        //protected void DescendSlope(Vector3 velocity)
//        //{
//        //    float moveDirection = Mathf.Sign(InputVector.z);
//        //    Vector3 rayOrigin = m_Transform.position + Vector3.up * checkHeight;
//        //    rayOrigin += moveDirection * m_Transform.forward * (m_Collider - m_SkinWidth);
//        //    float rayLength = 2f + m_SkinWidth;
//        //    RaycastHit hit;
//        //    if(Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength, m_Layers.SolidLayers))
//        //    {
//        //        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
//        //        float maxDescendAngle = 65f;
//        //        if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
//        //        {
//        //            if(MAthf.Sign(hit.normal.z) == moveDirection)
//        //            {
//        //                if(hit.distance - m_SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.z))
//        //                {
//        //                    float moveDistance = Mathf.Abs(m_Velocity.z);
//        //                    float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
//        //                    m_Velocity.x = Mathf.Cos(slopeAngle * MAthf.Deg2Rad) * moveDistance * Mathf.Sign(m_Velocity.z);
//        //                    m_velocity.y -= descendVelocityY;
//        //                    m_SlopeAngle = slopeAngle;
//        //                }
//        //            }
//        //        }
//        //    }
//        //}




//        ///// <summary>
//        ///// Perform checks to determine if the character is on the ground.
//        ///// </summary>
//        //protected virtual void CheckGround1()
//        //{
//        //    float groundDistance = 10f;
//        //    float checkHeight = m_Collider.center.y - m_Collider.height / 2 + m_SkinWidth;
//        //    Vector3 rayOrigin = m_Transform.position + Vector3.up * checkHeight;

//        //    float rayDist = m_Collider.radius + 1;
//        //    //rayDist = 1;
//        //    //  GroundHit Normal is used by the Character footstep.
//        //    //RaycastHit hit;
//        //    if (Physics.Raycast(rayOrigin, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers)) {
//        //        groundDistance = Vector3.Project(m_Transform.position - m_GroundHit.point, m_Transform.up).magnitude;
//        //    }

//        //    if (DebugCollisions) Debug.DrawRay(rayOrigin, Vector3.down * m_GroundHit.distance, m_Grounded ? Color.green : Color.red);

//        //    // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
//        //    //float smallerRadius = m_Collider.radius - (m_SkinWidth * m_SkinWidth);
//        //    float smallerRadius = m_Collider.radius * 0.9f;
//        //    Vector3 sphereCastOrigin = m_Transform.position + Vector3.up * m_Collider.radius;
//        //    if (Physics.SphereCast(sphereCastOrigin, smallerRadius, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers)) {
//        //        // check if sphereCast distance is small than the ray cast distance
//        //        if (groundDistance > (m_GroundHit.distance - m_Collider.radius * 0.1f))
//        //            groundDistance = (m_GroundHit.distance - m_Collider.radius * 0.1f);
//        //    }

//        //    if (DebugCollisions) DebugDraw.Sphere(sphereCastOrigin + Vector3.down * m_GroundHit.distance, smallerRadius, m_Grounded ? Color.gray : Color.red);



//        //    groundDistance = (float)Math.Round(groundDistance, 2);
//        //    float groundCheckDistance = 0.2f;
//        //    //  If character is grounded, set check distance lower.
//        //    if (m_Rigidbody.velocity.y > -0.001f && m_Rigidbody.velocity.y <= 0f)
//        //        groundCheckDistance *= 0.5f;

//        //    //  Character is grounded.
//        //    if (groundDistance < 0.05f) {
//        //        //m_GroundAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;
//        //        m_Grounded = true;


//        //        ////  Move the player so he lines up with raised parts of the ground.
//        //        ////  Prob should go in CheckMovement.
//        //        //float offset = m_Collider.radius + m_SkinWidth;
//        //        //Vector3 groundAverage = GetAverageRaycast(offset, offset, 2);
//        //        //if (groundAverage != m_Rigidbody.position) {
//        //        //    m_Rigidbody.MovePosition(new Vector3(m_Transform.position.x, groundAverage.y + 0.1f, m_Transform.position.z));
//        //        //}



//        //    } else {
//        //        if (groundDistance >= groundCheckDistance) {
//        //            //m_GroundAngle = 0;
//        //            m_Grounded = false;
//        //        }
//        //        //m_Grounded = false;


//        //    }

//        //    if (DebugMode) DebugDraw.DrawMarker(m_GroundHit.point, 0.1f, Color.green);
//        //}




//        ///// <summary>
//        ///// Ensure the current movement direction is valid.
//        ///// </summary>
//        //protected virtual void CheckMovement()
//        //{
//        //    m_Moving = InputVector != Vector3.zero;
//        //    //if (m_Moving == false) return;


//        //    float direction = Mathf.Clamp(InputVector.z, -1, 1);
//        //    if (direction < 0.01f && direction > 0.01f) direction = 1;
//        //    float rayLength = 1f + m_SkinWidth;
//        //    float dstBetweenRays = 0.4f;

//        //    float colliderRadius = m_Collider.radius - m_SkinWidth;
//        //    float colliderHeight = m_Collider.height - (colliderRadius * 2);
//        //    int horizontalRayCount = m_DetectHorizontalCollision ? Mathf.RoundToInt(colliderHeight / dstBetweenRays) : 3;
//        //    float horizontalRaySpacing = colliderHeight / (horizontalRayCount - 1);


//        //    bool hitDetected = false;
//        //    RaycastHit hit;
//        //    for (int i = 0; i < horizontalRayCount; i++) {
//        //        Vector3 rayOrigin = m_Rigidbody.position;
//        //        rayOrigin += Vector3.up * colliderRadius;
//        //        rayOrigin += m_Transform.forward * colliderRadius;
//        //        rayOrigin += Vector3.up * (horizontalRaySpacing * i);

//        //        //Physics.RaycastNonAlloc(rayOrigin, m_Transform.forward * direction, m_Collisions, rayLength, m_CollisionsLayerMask);

//        //        if (Physics.Raycast(rayOrigin, m_Transform.forward * direction, out hit, rayLength, m_CollisionsLayerMask)) {
//        //            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
//        //            if (i == 0 && slopeAngle <= m_SlopeLimit) {
//        //                //float distanceToSlope = 0;
//        //                //if (Math.Abs(slopeAngle - m_GroundAngle) > float.Epsilon)
//        //                //{
//        //                //    distanceToSlope = hit.distance - colliderRadius;
//        //                //}

//        //                //Vector3 slopeDirection = Vector3.Cross(m_Transform.right, hit.normal).normalized;
//        //                ////slopeDirection = slopeDirection - slopeDirection * distanceToSlope;
//        //                //Vector3 targetVelocity = m_Velocity;
//        //                //targetVelocity = Vector3.Project(targetVelocity, slopeDirection);

//        //                ////if (DebugMode) Debug.DrawRay(rayOrigin, targetVelocity, Color.blue);
//        //                ////if (DebugMode) DebugDraw.Arrow(rayOrigin, targetVelocity, Color.blue);
//        //                //m_Velocity = targetVelocity;
//        //            }

//        //            if (slopeAngle > m_SlopeLimit) {
//        //                rayLength = hit.distance;
//        //                m_Moving = false;
//        //            }
//        //            hitDetected = true;

//        //            //m_Velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.x) * Mathf.Sign(m_Velocity.x);
//        //            //m_Velocity.z = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.z) * Mathf.Sign(m_Velocity.z);
//        //        }


//        //        //if (DebugCollisions && DebugMotion) Debug.DrawRay(rayOrigin, m_Transform.forward * direction * rayLength, hitDetected == true ? Color.blue : Color.grey);
//        //    }

//        //    //for (int i = 0; i < m_Collisions.Length; i++)
//        //    //{
//        //    //    Debug.DrawLine(raycastOrigin, m_Collisions[i].point, Color.red);
//        //    //}


//        //    //VerticalCollisions();
//        //}


//    }

//}
