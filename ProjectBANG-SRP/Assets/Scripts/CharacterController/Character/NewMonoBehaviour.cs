//namespace CharacterController
//{
//    using UnityEngine;
//    using System;
//    using System.Collections;
//    using System.Text;

//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
//    public class CharacterLocomotion : MonoBehaviour
//    {
//        public event Action<bool> OnAim = delegate {};
//        public enum MovementType { Adventure, Combat };

//        //  Locomotion variables
//        [SerializeField, HideInInspector]
//        protected bool m_UseRootMotion = true;
//        [SerializeField, HideInInspector]
//        protected float m_RootMotionSpeedMultiplier = 1;
//        [SerializeField, HideInInspector]
//        protected float m_Acceleration = 0.12f;
//        [SerializeField, HideInInspector]
//        protected float m_MotorDamping = 0.2f;       
//        [SerializeField, HideInInspector]
//        protected float m_MovementSpeed = 1f;
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
//        protected int m_HorizontalCollisionCount = 10;
//        protected int m_VerticalCollisionCount = 10;
//        protected LayerMask m_ColliderLayerMask;
//        protected int m_MaxCollisionCount = 50;



//        //  --  Character Actions
//        [SerializeField, HideInInspector]
//        protected CharacterAction[] m_Actions;
//        [SerializeField, HideInInspector]
//        protected CharacterAction m_ActiveAction;





//        protected MovementType m_MovementType = MovementType.Adventure;
//        [SerializeField, DisplayOnly]
//        protected bool m_Moving;
//        protected bool m_Aiming;
//        [SerializeField, DisplayOnly]
//        protected float m_InputMagnitude;
//        protected float m_InputAngle;
//        protected Vector3 m_Velocity, m_VerticalVelocity;
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



//        private bool m_CheckGround = true, m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;


//        private Animator m_Animator;
//        private AnimatorMonitor m_AnimationMonitor;
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
//        protected bool m_DrawDebugLine;
//        [SerializeField, HideInInspector]
//        private bool displayMovement = true, displayPhysics = true, displayActions = true;








//        #region Properties

//        public MovementType Movement
//        {
//            get { return m_MovementType; }
//        }

//        public bool Moving{
//            get { return m_Moving;}
//            set { m_Moving = value; }
//        }

//        public bool Aiming{
//            get{
//                if (m_Aiming && Grounded)
//                    return true;
//                return false;
//            }
//            set { m_Aiming = value; }
//        }

//        public bool Grounded{
//            get { return m_Grounded; }
//            set { m_Grounded = value; }
//        }

//        public float RotationSpeed{
//            get { return m_RotationSpeed; }
//            set { m_RotationSpeed = value; }
//        }

//        public Vector3 InputVector{
//            get { return m_InputVector; }
//            set { m_InputVector = value; }
//        }

//        public Vector3 MoveDirection{
//            get { return m_MoveDirection; }
//        }

//        public Vector3 LookDirection{
//            get { return m_LookDirection; }
//            set { m_LookDirection = value == Vector3.zero ? m_Transform.forward : value; }
//        }

//        public Vector3 Velocity{
//            get { return m_Velocity; }
//            set { m_Velocity = value; }
//        }

//        public Quaternion LookRotation{
//            get { return m_LookRotation; }
//            set { m_LookRotation = value; }
//        }

//        public CharacterAction[] CharActions{
//            get { return m_Actions; }
//            set { m_Actions = value; }
//        }

//        public bool UseRootMotion
//        {
//            get { return m_UseRootMotion; }
//            set { m_UseRootMotion = value; }
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



//        protected void Awake()
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



//		protected void OnEnable()
//		{
//            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//            m_Rigidbody.mass = m_Mass;
//            m_Animator.applyRootMotion = m_UseRootMotion;

//            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
//            EventHandler.RegisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);
//		}


//		protected void OnDisable()
//		{
//            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
//            EventHandler.UnregisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);
//		}


//        protected void Start()
//        {
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


//        private void Update()
//		{
//            if (Time.timeScale == 0) return;
//            m_DeltaTime = Time.deltaTime;

//            //m_InputMagnitude = m_InputVector.magnitude;
//            //if (m_InputMagnitude < 0.01f || m_InputMagnitude > 0.99f)
//            //    m_InputMagnitude = Mathf.Round(m_InputMagnitude);
//            //if (m_InputMagnitude > 1)
//            //    m_InputVector.Normalize();

//            m_Moving = m_InputVector != Vector3.zero;




//            ////  Start Stop Actions.
//            for (int i = 0; i < m_Actions.Length; i++)
//            {
//                if (m_Actions[i].enabled == false)
//                    continue;
//                CharacterAction charAction = m_Actions[i];
//                StopStartAction(charAction);

//                //  Call Action Update.
//                charAction.UpdateAction();
//            }

//            SetPhysicsMaterial();

//        }


//        private void FixedUpdate()
//		{
//            if (Time.timeScale == 0) return;
//            m_DeltaTime = Time.fixedDeltaTime;


//            m_Velocity = Vector3.zero;

//            m_CheckGround = true;
//            m_CheckMovement = true;
//            m_UpdateRotation = true;
//            m_UpdateMovement = true;
//            m_UpdateAnimator = true;
//            for (int i = 0; i < m_Actions.Length; i++)
//            {
//                if (m_Actions[i].enabled == false)
//                    continue;
//                CharacterAction charAction = m_Actions[i];

//                if (charAction.IsActive)
//                {
//                    if (m_CheckGround) m_CheckGround = charAction.CheckGround();

//                    if (m_CheckMovement) m_CheckMovement = charAction.CheckMovement();

//                    if (m_UpdateRotation) m_UpdateRotation = charAction.UpdateRotation();

//                    if (m_UpdateMovement) m_UpdateMovement = charAction.UpdateMovement();

//                    if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
//                }
//            }  //  end of for loop



//            CheckGround();
//            CheckMovement();
//            UpdateRotation();
//            UpdateMovement();
//            UpdateAnimator();
//        }


//        private void OnAnimatorMove()
//        {
//            if(m_Grounded)
//            {
//                if (m_UseRootMotion)
//                {
//                    //m_Animator.ApplyBuiltinRootMotion();
//                }
//            }
//            else
//            {

//            }
//        }


//        private void LateUpdate()
//        {
//            m_Move = true;
//            for (int i = 0; i < m_Actions.Length; i++){
//                if (m_Actions[i].IsActive){
//                    if (m_Move) m_Move = m_Actions[i].Move();
//                }
//            }
//            Move();


//            //  -----
//            //  Debug messages
//            if (m_Debug) DebugMessages();
//            //  -----
//        }



//        public void SetMovementType(MovementType movementType)
//        {
//            m_MovementType = movementType;
//        }


//        //  Should the character look independetly of the camera?  AI Agents do not need to use camera rotation.
//        public bool IndependentLook()
//        {
//            if(m_Moving || m_Aiming){
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
//            //  First, do anything that is independent of a character action.


//            //  Does the current active character action virtual this method.
//            if (m_CheckGround == true)
//            {
//                m_GroundDistance = 10;

//                m_GroundCheckHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
//                groundCastOrigin = m_Rigidbody.position + Vector3.up * m_GroundCheckHeight;
//                groundCheckMaxDistance = m_CapsuleCollider.radius;
//                groundRayHit = false;
//                if (Physics.Raycast(groundCastOrigin, Vector3.down,
//                                    out m_GroundHit, groundCheckMaxDistance, m_Layers.SolidLayers))
//                {
//                    if(m_GroundHit.transform != m_Transform)
//                    {
//                        groundRayHit = true;
//                        //m_GroundDistance = m_Transform.position.y - m_GroundHit.point.y;
//                        m_GroundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, m_Transform.up).magnitude;
//                        m_GroundNormal = m_GroundHit.normal;
//                    }

//                }

//                sphereCastOrigin = m_Rigidbody.position + Vector3.up * m_CapsuleCollider.radius;
//                //sphereCastRadius = m_CapsuleCollider.radius - m_SkinWidth;
//                sphereCastRadius = m_CapsuleCollider.radius * 0.9f;
//                sphereCastMaxDistance = m_CapsuleCollider.radius + 2;
//                sphereCastHitDistance = sphereCastMaxDistance;
//                groundSphereHit = false;
//                if (Physics.SphereCast(sphereCastOrigin, sphereCastRadius,
//                                       Vector3.down, out m_GroundHit, sphereCastMaxDistance, m_Layers.SolidLayers))
//                {
//                    sphereCastHitDistance = m_GroundHit.distance;
//                    groundSphereHit = true;
//                    // check if sphereCast distance is small than the ray cast distance
//                    if (m_GroundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
//                        m_GroundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
//                }




//                m_GroundDistance = (float)Math.Round(m_GroundDistance, 2);
//                var groundCheckDistance = 0.2f;

//                //  Character is grounded.
//                if(m_GroundDistance < 0.05f)
//                {
//                    Vector3 horizontalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity);
//                    m_Stickiness = m_GroundStickiness * horizontalVelocity.magnitude * m_AirbornThreshold;
//                    m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundNormal) - 90;

//                    m_Animator.applyRootMotion = m_UseRootMotion;
//                    m_Grounded = true;
//                }
//                else
//                {
//                    if (m_GroundDistance >= groundCheckDistance)
//                    {
//                        m_InputVector = Vector3.zero;
//                        m_GroundNormal = m_Transform.up;
//                        m_SlopeAngle = 0;
//                        m_Rigidbody.AddForce(m_Gravity * m_GravityModifier);

//                        m_Animator.applyRootMotion = false;
//                        m_Grounded = false;
//                    }
                       
//                }

//                //if (m_Debug) Debug.DrawRay(m_GroundHit.point, m_GroundNormal, Color.magenta);

//                groundHitPoint = m_Grounded ? m_GroundHit.point : Vector3.zero;
//            }

//            if (m_Grounded)
//            {
//                DetectEdge();
//            }
//        }




//        //  Ensure the current movement direction is valid.
//        protected virtual void CheckMovement()
//        {
//            if (m_CheckMovement == false) return;



//            //  We want the cos angle since we know "adjacent" and "hypotenuse".
//            float stopMovementAngle = Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg;
//            float checkDistance = 2;


//            float nearestHitDistance = 0;
//            float hitDetectionDotProduct = -1;
//            RaycastHit detectionHitInfo;


//            //  Check look direction.
//            if (DetectionRaycast(Mathf.Acos(1) * Mathf.Rad2Deg, out detectionHitInfo, checkDistance, m_DetectObjectHeight, m_Layers.SolidLayers))
//            {
//                if (detectionHitInfo.distance <= nearestHitDistance)
//                {
//                    nearestHitDistance = detectionHitInfo.distance;
//                    hitDetectionDotProduct = Vector3.Dot(detectionHitInfo.normal, Vector3.up);
//                }
//            }
//            //  Check left side
//            if (DetectionRaycast(-stopMovementAngle, out detectionHitInfo, checkDistance, m_DetectObjectHeight, m_Layers.SolidLayers))
//            {
//                if (detectionHitInfo.distance <= nearestHitDistance){
//                    nearestHitDistance = detectionHitInfo.distance;
//                    hitDetectionDotProduct = Vector3.Dot(detectionHitInfo.normal, Vector3.up);
//                }
//            }
//            //  Check right side./
//            if (DetectionRaycast(stopMovementAngle, out detectionHitInfo, checkDistance, m_DetectObjectHeight, m_Layers.SolidLayers))
//            {
//                if (detectionHitInfo.distance <= nearestHitDistance){
//                    nearestHitDistance = detectionHitInfo.distance;
//                    hitDetectionDotProduct = Vector3.Dot(detectionHitInfo.normal, Vector3.up);
//                }
//            }



//        }



//        protected bool DetectionRaycast(float stopMovementAngle, out RaycastHit hitInfo, float checkDistance, float checkHeight = 0.4f, int layerMask = 1 << 27)
//        {
//            //checkHeight = Mathf.Clamp(checkHeight, 0, m_CapsuleCollider.height);
//            //  TODO: check if stopMovementAngle exceedes 180.

//            Vector3 hitDetectionStartRay = m_Transform.position + Vector3.up * checkHeight;
//            Quaternion rayRotation = Quaternion.AngleAxis(stopMovementAngle, m_Transform.up) * m_Transform.rotation;
//            Vector3 hitDetectionEndRay = rayRotation * m_Transform.InverseTransformDirection(m_Transform.forward);

//            bool hitObject = false;
//            if (Physics.Raycast(hitDetectionStartRay, hitDetectionEndRay + Vector3.up * checkHeight, out hitInfo, checkDistance, layerMask)){
//                if (hitInfo.collider != null){
//                    hitObject = true;
//                }
//            }
//            Debug.DrawRay(hitDetectionStartRay, hitDetectionEndRay * checkDistance, hitObject == true ? Color.red : Color.blue);
//            return hitObject;
//        }






//        //  Update the rotation forces.
//        protected virtual void UpdateRotation()
//        {
//            if (m_UpdateRotation == false) return;

//            switch (m_MovementType)
//            {
//                case (MovementType.Adventure):

//                    Vector3 axisSign = Vector3.Cross(m_LookDirection, m_Transform.forward);
//                    m_InputAngle = Vector3.Angle(m_Transform.forward, m_LookDirection) * (axisSign.y >= 0 ? -1f : 1f) * m_DeltaTime;
//                    m_InputAngle = (float)Math.Round(m_InputAngle, 2);

//                    if (m_Moving && m_LookDirection.sqrMagnitude > 0.2f)
//                    {
//                        Vector3 local = m_Transform.InverseTransformDirection(m_LookDirection);
//                        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
//                        if (m_InputVector == Vector3.zero)
//                            angle *= (1.01f - (Mathf.Abs(angle) / 180)) * 1;

//                        //m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up) * m_Rigidbody.rotation);

//                        Quaternion rotation = Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up);
//                        if (m_Grounded){
//                            Vector3 d = transform.position - m_Animator.pivotPosition;
//                            m_Rigidbody.MovePosition(m_Animator.pivotPosition + rotation * d);
//                        }
//                        m_Rigidbody.MoveRotation(rotation * m_Rigidbody.rotation);
//                    }
//                    return;


//                case (MovementType.Combat):

//                    return;
//            }
//        }



//        //  Apply any movement.
//        protected virtual void UpdateMovement()
//        {
//            if (m_UpdateMovement == false) return;

//            switch (m_MovementType)
//            {
//                case (MovementType.Adventure):

//                    if (m_Grounded)
//                    {
//                        //  Get the start move angle
//                        if (m_Moving && Mathf.Abs(m_InputAngle) < 10)
//                            m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
//                        else if (!m_Moving)
//                            m_StartAngle = m_InputAngle;
//                        m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);
//                        //   ---


//                        m_MoveDirection = Vector3.Cross(m_Transform.right, m_GroundNormal);

//                        if (m_SlopeAngle > 0 || m_SlopeAngle < 0)
//                        {
//                            Vector3 slopeDirection = new Vector3(0,
//                                        (1 - Mathf.Cos(m_SlopeAngle * Mathf.Deg2Rad) * m_CapsuleCollider.radius),
//                                        (Mathf.Sin(m_SlopeAngle * Mathf.Deg2Rad) * m_CapsuleCollider.radius)) * m_CapsuleCollider.radius;
//                            m_MoveDirection = Quaternion.Inverse(m_Rigidbody.rotation) * Vector3.RotateTowards(m_MoveDirection, slopeDirection, 10 * m_DeltaTime, 1f);
//                        }

//                        m_Velocity = m_MoveDirection.normalized * m_MovementSpeed;
//                        //m_Velocity = m_MoveDirection.normalized;

//                    }
//                    //  If airborne.
//                    else
//                    {
//                        float m_AirSpeed = 6;
//                        m_MoveDirection = (Quaternion.Inverse(m_Transform.rotation) * m_Transform.forward) * m_AirSpeed;
//                        m_Velocity = Vector3.Project(m_MoveDirection, m_Gravity * m_GravityModifier);
//                    }
//                    m_VerticalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity * m_GravityModifier);
//                    m_Velocity = Quaternion.Inverse(m_Rigidbody.rotation) * m_Velocity + m_VerticalVelocity;


//                    m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);

//                    return;
//                case (MovementType.Combat):

//                    return;
//            }


//        }



//        protected virtual void Move()
//		{
//            if(m_Move == true)
//            {
//                if (m_UseRootMotion)
//                {
//                    float angleInDegrees;
//                    Vector3 rotationAxis;
//                    m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
//                    Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
//                    m_Rigidbody.angularVelocity = angularDisplacement;


//                    //velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y * m_CapsuleCollider.height;
//                    Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime);
//                    velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
//                    m_Rigidbody.velocity = velocity;
//                }
//                else
//                {
//                    m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
//                    m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_VelocitySmooth, m_Acceleration);
//                    //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_MovementSpeed);
//                    m_Rigidbody.velocity = m_Velocity * m_InputVector.sqrMagnitude;
//                }


//                if (m_Grounded){
//                    m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundNormal * m_Stickiness);
//                }
//            }

//		}






//        protected void UpdateAnimator()
//        {
//            //  First, do anything that is independent of a character action.
//            //  -----------

//            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
//            // which affects the movement speed because of the root motion.
//            m_Animator.speed = m_RootMotionSpeedMultiplier;

//            m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
//            m_Animator.SetBool(HashID.Moving, m_Moving);
//            m_Animator.SetFloat(HashID.InputAngle, (m_InputAngle * Mathf.Deg2Rad));
//            //var localVelocity = Quaternion.Inverse(m_Transform.rotation) * (m_Rigidbody.velocity / m_Acceleration);

//            //  1 means left foot is up. 
//            m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);


//            //  -----------
//            //  Does a character action virtual the controllers update animator.
//            //  -----------
//            if (m_UpdateAnimator == true)
//            {
//                if(m_Grounded){
//                    //  Movement Input
//                    //m_Animator.applyRootMotion = true;
//                    m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
//                    m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
//                }
//                else{
//                    //m_AnimationMonitor.SetForwardInputValue(0);
//                    //m_AnimationMonitor.SetHorizontalInputValue(0);
//                    //m_Animator.applyRootMotion = false;
//                    m_Animator.SetFloat(HashID.ForwardInput, 0);
//                    m_Animator.SetFloat(HashID.HorizontalInput, 0);
//                }
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

//        //}




//        #region Public Functions

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



//            if (Physics.Raycast(m_Transform.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward,  2, m_Layers.SolidLayers ) == false )
//            {

//                if (Physics.Raycast(start, dir, maxDetectEdgeDistance, m_Layers.SolidLayers) == false){
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

//            if (Physics.Raycast(start, dir, out raycastHit, maxDistance, layerMask)){
//                detectObject = true;
//            }

//            if (m_Debug) Debug.DrawRay(start, dir * maxDistance, detectObject ? Color.red : Color.green);
//            return detectObject;
//        }


//        public void SetPosition(Vector3 position){
//            m_Rigidbody.MovePosition(position);
//        }


//        public void SetRotation(Quaternion rotation){
//            m_Rigidbody.MoveRotation(rotation.normalized);
//        }


//		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
//        {
//            throw new NotImplementedException(string.Format("<color=yellow>{0}</color> MoveCharacter not Implemented.", GetType()));
//        }


//        public void StopMovement()
//        {
//            m_Rigidbody.velocity = Vector3.zero;
//            m_Moving = false;
//        }


//        #endregion



//        #region Actions


//        protected void StopStartAction(CharacterAction charAction)
//        {
//            //  First, check if current Action can Start or Stop.

//            //  Current Action is Active.
//            if (charAction.enabled && charAction.IsActive)
//            {
//                //  Check if can stop Action is StopType is NOT Manual.
//                if (charAction.StopType != ActionStopType.Manual)
//                {
//                    if (charAction.CanStopAction())
//                    {
//                        //  Start the Action and update the animator.
//                        charAction.StopAction();
//                        //  Reset Active Action.
//                        if (m_ActiveAction = charAction)
//                            m_ActiveAction = null;
//                        //  Move on to the next Action.
//                        return;
//                    }
//                }
//            }
//            //  Current Action is NOT Active.
//            else
//            {
//                //  Check if can start Action is StartType is NOT Manual.
//                if (charAction.enabled && charAction.StartType != ActionStartType.Manual)
//                {
//                    if (m_ActiveAction == null)
//                    {
//                        if (charAction.CanStartAction())
//                        {
//                            //  Start the Action and update the animator.
//                            charAction.StartAction();
//                            //charAction.UpdateAnimator();
//                            //  Set active Action if not concurrent.
//                            if (charAction.IsConcurrentAction() == false)
//                                m_ActiveAction = charAction;
//                            //  Move onto the next Action.
//                            return;
//                        }
//                    }
//                    else if (charAction.IsConcurrentAction())
//                    {
//                        if (charAction.CanStartAction())
//                        {
//                            //  Start the Action and update the animator.
//                            charAction.StartAction();
//                            //charAction.UpdateAnimator();
//                            //  Move onto the next Action.
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                }
//            }


//        }


//        public T GetAction<T>() where T : CharacterAction
//        {
//            for (int i = 0; i < m_Actions.Length; i++){
//                if (m_Actions[i] is T){
//                    return (T)m_Actions[i];
//                }
//            }
//            return null;
//        }


//        public bool TryStartAction(CharacterAction action)
//        {
//            if (action == null) return false;

//            int index = Array.IndexOf(m_Actions, action);
//            //  If there is an active action and current action is non concurrent.
//            if(m_ActiveAction != null && action.IsConcurrentAction() == false){
//                int activeActionIndex = Array.IndexOf(m_Actions, m_ActiveAction);
//                //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
//                if(index < activeActionIndex){
//                    if (action.CanStartAction()){
//                        //  Stop the current active action.
//                        TryStopAction(m_ActiveAction);
//                        //  Set the active action.
//                        m_ActiveAction = m_Actions[index];
//                        //m_ActiveActions[index] = m_Actions[index];
//                        action.StartAction();
//                        //action.UpdateAnimator();
//                        return true;
//                    }
//                } 
//            }
//            //  If there is an active action and current action is concurrent.
//            else if (m_ActiveAction != null && action.IsConcurrentAction())
//            {
//                if (action.CanStartAction()){
//                    //m_ActiveActions[index] = m_Actions[index];
//                    action.StartAction();
//                    //action.UpdateAnimator();
//                    return true;
//                }
//            }
//            //  If there is no active action.
//            else if (m_ActiveAction == null){
//                if (action.CanStartAction())
//                {
//                    m_ActiveAction = m_Actions[index];
//                    //m_ActiveActions[index] = m_Actions[index];
//                    action.StartAction();
//                    //action.UpdateAnimator();
//                    return true;
//                }
//            }


//            return false;
//        }


//        public void TryStopAllActions()
//        {
//            for (int i = 0; i < m_Actions.Length; i++)
//            {
//                if (m_Actions[i].IsActive)
//                {
//                    TryStopAction(m_Actions[i]);
//                }
//            }
//        }


//        public void TryStopAction(CharacterAction action)
//        {
//            if (action == null) return; 

//            if (action.CanStopAction()){
//                int index = Array.IndexOf(m_Actions, action);
//                if (m_ActiveAction == action)
//                    m_ActiveAction = null;


//                action.StopAction();
//                ActionStopped();
//            }
//        }


//        public void TryStopAction(CharacterAction action, bool force)
//        {
//            if (action == null) return; 
//            if(force){
//                int index = Array.IndexOf(m_Actions, action);
//                if (m_ActiveAction == action)
//                    m_ActiveAction = null;


//                action.StopAction();
//                ActionStopped();
//                return;
//            }

//            TryStopAction(action);
//        }

//        #endregion



//        #region OnAction execute

//        protected void OnAimActionStart(bool aim)
//        {
//            m_Aiming = aim;
//            m_MovementType = m_Aiming ? MovementType.Combat : MovementType.Adventure;
//            //  Call On Aim Delegate.
//            OnAim(aim);

//            //CameraController.Instance.FreeRotation = aim;
//            //Debug.LogFormat("Camera Free Rotation is {0}", CameraController.Instance.FreeRotation);
//        }


//        protected void OnActionActive(CharacterAction action, bool activated)
//        {
//            int index = Array.IndexOf(m_Actions, action);
//            if (action == m_Actions[index])
//            {
//                if (m_Actions[index].enabled)
//                {
//                    if(activated)
//                    {
//                        //Debug.LogFormat(" {0} is starting.", action.GetType().Name);

//                    }
//                    else
//                    {

//                    }
//                }
//            }

//        }


//        public void ActionStopped()
//        {

//        }

//        #endregion









//        protected string[] debugMsgs;
//        //Camera mainCamera;
//        protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);
//        protected Color _Magenta = new Color(0.75f, 0, 0.75f, 0.9f);




//        protected virtual void DrawGizmos()
//        {
//            DrawCheckGroundGizmo();

//            if (m_Rigidbody == null) return;
//            //  Move direction
//            Gizmos.color = Color.blue;
//            Gizmos.DrawRay(m_Rigidbody.position + debugHeightOffset, m_MoveDirection);

//            Gizmos.color = Color.cyan;
//            Gizmos.DrawSphere(m_CapsuleCollider.bounds.min, 0.05f);
//            Gizmos.DrawSphere(m_CapsuleCollider.bounds.max, 0.5f);
//            //GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_MoveDirection, Color.white);
//            //  Velocity
//            //Gizmos.color = Color.cyan;
//            //Gizmos.DrawRay(m_Rigidbody.position + debugHeightOffset, m_Transform.InverseTransformDirection(m_Velocity));
//        }



//        private void DebugMessages()
//        {
//            debugMsgs = new string[]
//            {
//                string.Format("InputAngle: {0}", m_InputAngle),
//                string.Format("StopMovement Rad2Deg: {0}", m_StopMovementThreshold * Mathf.Rad2Deg),
//                string.Format("StopMovement Rad2Deg: {0}", Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg),
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
//                for (int i = 0; i < debugMsgs.Length; i++){
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
//                DrawGizmos();
//            }

//        }



//        //------
//    }
//}
