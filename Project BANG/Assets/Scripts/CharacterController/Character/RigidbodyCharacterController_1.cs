//namespace CharacterController
//{
//    using UnityEngine;
//    using System;
//    using System.Collections.Generic;

//    using MathUtil = MathUtilities;

//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
//    public abstract class RigidbodyCharacterController_1 : MonoBehaviour
//    {
//        protected const float tinyOffset = .0001f;

//        public enum MovementTypes { Adventure, Combat };




//        #region Inspector properties



//        //  Locomotion variables
//        [SerializeField, Group("Motor")] protected bool m_UseRootMotion = true;
//        protected bool m_UseRootMotionRotation;
//        [SerializeField, Group("Motor")] protected float m_RootMotionSpeedMultiplier = 1;
//        [SerializeField, Group("Motor")] protected float m_RootMotionRotationMultiplier = 1;
//        [SerializeField, Group("Motor")] protected Vector3 m_GroundAcceleration = new Vector3(0.18f, 0, 0.18f);
//        [SerializeField, Group("Motor")] protected float m_Acceleration = 0.12f;
//        [SerializeField, Group("Motor")] protected float m_AirborneAcceleration = 0.2f;
//        [SerializeField, Group("Motor")] protected float m_MotorDamping = 0.3f;
//        [SerializeField, Group("Motor")] protected float m_AirborneDamping = 0.3f;
//        [SerializeField, Group("Motor")] protected float m_GroundSpeed = 1f;
//        [SerializeField, Group("Motor")] protected float m_AirborneSpeed = 0.5f;
//        [SerializeField, Group("Motor")] protected float m_RotationSpeed = 4f;
//        [SerializeField, Group("Motor")] protected float m_SlopeForceUp = 1f;
//        [SerializeField, Group("Motor")] protected float m_SlopeForceDown = 1.25f;



//        //  -- Physics variables

//        [Group("Physics")]
//        [SerializeField] protected float m_DetectObjectHeight = 0.4f;
//        [Group("Physics")]
//        [SerializeField] protected float m_Mass = 100;
//        [Group("Physics")]
//        [SerializeField] protected float m_SkinWidth = 0.08f;
//        [Group("Physics"), Range(0, 90)]
//        [SerializeField] protected float m_SlopeLimit = 45f;
//        [Group("Physics")]
//        [SerializeField] protected float m_MaxStepHeight = 0.4f;
//        [Group("Physics")]
//        [SerializeField] protected float m_GravityModifier = 0.4f;
//        [Group("Physics")]
//        [SerializeField] protected float m_GroundStickiness = 6f;

//        //  -- Collision detection
//        [Group("Collisions")]
//        [SerializeField] protected LayerMask m_CollisionsLayerMask;
//        [Group("Collisions")]
//        [SerializeField] protected int m_MaxCollisionCount = 100;




//        //  -- Animation
//        [Group("Animation")]
//        [SerializeField] protected float m_IdleRotationMultiplier = 2f;


//        #endregion

//        [SerializeField]
//        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();


//        protected CapsuleCollider charCollider;
//        protected float timeScale = 1;


//        protected MovementTypes m_MovementType = MovementTypes.Adventure;

//        protected float previousRotationAngle, rotationAngle;
//        protected Vector3 inputVector, relativeInputVector;
//        protected Vector3 moveDirection, m_Velocity, angularVelocity;
//        protected Quaternion targetRotation = Quaternion.identity, currentRotation = Quaternion.identity;



//        protected float groundAngle;
//        protected Vector3 m_GroundSlopeDir;


//        protected RaycastHit groundHit;
//        [SerializeField, Group("Collisions")]
//        protected Collider[] probedColiders;
//        [SerializeField, Group("Collisions")]
//        protected RaycastHit[] probedHits;
//        protected List<Collider> colliderBuffer = new List<Collider>();
//        protected int totalColliderHits, totalRaycastHits;

//        protected PhysicMaterial characterMaterial;

//        protected Vector3 previousForward;
//        protected Vector3 previousPosition, currentPosition, targetPosition;
//        protected Vector3 previousVelocity, currentVelocity, targetVelocity;

//        protected int motionTrajectoryResolution = 5;
//        protected Vector3[] motionTrajectory;
//        protected float deltaAngle;
//        protected Vector3 targetAngularVelocity;
//        protected float m_Speed;


//        protected Vector3 moveDirectionSmooth, m_ExternalForceSmooth;
//        protected Vector3 velocitySmooth;
//        protected Vector3 externalForceSmooth;
//        protected float rotationAngleSmooth, angularDragSmooth;

//        float castDistance = 10;
//        float airborneThreshold = 0.3f;


//        protected Animator m_Animator;
//        protected AnimatorMonitor m_AnimationMonitor;
//        protected LayerManager m_LayerManager;
//        protected Rigidbody m_Rigidbody;
//        protected GameObject m_GameObject;
//        protected Transform m_Transform;
//        protected float m_DeltaTime, deltaTime, fixedDeltaTime;



//        #region Parameters for Editor

//        //  For Editor.
//        //  Debug parameters.


//        //protected bool m_Debug;
//        public CharacterControllerDebugger Debugger { get { return debugger; } }
//        protected bool DebugGroundCheck { get { return Debugger.states.showGroundCheck; } }
//        protected bool DebugCollisions { get { return Debugger.states.showCollisions; } }
//        protected bool DebugMotion { get { return Debugger.states.showMotion; } }

//        [SerializeField, HideInInspector]
//        private bool displayMovement, displayPhysics, displayAnimations, displayActions;

//        #endregion





//        #region Properties

//        public MovementTypes Movement { get { return m_MovementType; } }

//        public bool Moving { get; set; }

//        public bool Grounded { get; set; }

//        public bool RotateToLookDir { get; set; }
//        //  We want the InputVector to remain as is so other scripts can get the correct input vector.
//        public Vector3 InputVector { get; set; }

//        public Vector3 RelativeInputVector
//        {
//            get
//            {
//                relativeInputVector = m_Transform.InverseTransformDirection(InputVector);
//                return relativeInputVector;
//                //return Quaternion.Inverse(m_Transform.rotation) * InputVector;
//            }
//        }

//        public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

//        public Quaternion LookRotation { get; set; } = default;

//        public float Speed { get { return Mathf.Abs(m_Speed); } set { m_Speed = Mathf.Abs(value); } }

//        public float RotationSpeed { get { return m_RotationSpeed; } set { m_RotationSpeed = value; } }

//        public bool UseRootMotion { get { return m_UseRootMotion; } set { m_UseRootMotion = value; } }

//        public Vector3 Gravity { get; protected set; }

//        public Vector3 Velocity { get { return m_Velocity; } set { m_Velocity = value; } }

//        public CapsuleCollider Collider { get { return charCollider; } protected set { charCollider = value; } }

//        public RaycastHit GroundHit { get { return groundHit; } }



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
//        public Vector3 RaycastOrigin { get { return m_Transform.position + Vector3.up * m_SkinWidth; } }

//        #endregion



//        protected virtual void Awake()
//        {
//            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
//            m_Animator = GetComponent<Animator>();

//            m_Rigidbody = GetComponent<Rigidbody>();
//            m_LayerManager = GetComponent<LayerManager>();

//            charCollider = GetComponent<CapsuleCollider>();
//            if (charCollider == null)
//            {
//                charCollider = gameObject.AddComponent<CapsuleCollider>();
//                charCollider.radius = 0.3f;
//                charCollider.height = MathUtil.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2);
//                charCollider.center = new Vector3(0, charCollider.height / 2, 0);
//            }



//            m_CollisionsLayerMask = m_LayerManager.SolidLayers;


//            probedColiders = new Collider[m_MaxCollisionCount];
//            probedHits = new RaycastHit[m_MaxCollisionCount];

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
//            m_Rigidbody.mass = m_Mass;
//            m_Rigidbody.useGravity = false;
//            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
//            //m_Rigidbody.isKinematic = true;


//            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
//            m_Animator.applyRootMotion = m_UseRootMotion;

//            characterMaterial = new PhysicMaterial() { name = "Character Physics Material" };
//        }


//        private void OnEnable()
//        {
//            motionTrajectory = new Vector3[motionTrajectoryResolution];

//            LookRotation = Quaternion.LookRotation(m_Transform.forward);

//            previousForward = m_Transform.forward;
//            previousPosition = m_Rigidbody.position;
//            previousVelocity = m_Rigidbody.velocity;
//        }


//        protected virtual void Update()
//        {
//            timeScale = Time.timeScale;
//            if (Math.Abs(timeScale) < float.Epsilon) return;
//            m_DeltaTime = deltaTime;


//            previousPosition = m_Rigidbody.position;
//            previousVelocity = m_Rigidbody.velocity;

//            previousRotationAngle = rotationAngle;
//        }


//        protected virtual void FixedUpdate()
//        {
//            if (Math.Abs(timeScale) < float.Epsilon) return;
//            m_DeltaTime = fixedDeltaTime;



//        }


//        protected virtual void LateUpdate()
//        {

//        }


//        protected virtual void OnAnimatorMove()
//        {
//            AnimatorMove();
//        }




//        protected void MotionTrajectory(Vector3 targetVelocity)
//        {
//            if (colliderBuffer == null) colliderBuffer = new List<Collider>();
//            Vector3 transformUp = transform.up;
//            Vector3 origin = m_Transform.position;

//            Vector3 stepVelocity = targetVelocity;
//            stepVelocity /= motionTrajectoryResolution;

//            for (int i = 0; i < motionTrajectoryResolution; i++)
//            {
//                bool hasHit = false;
//                Vector3 previousOrigin = origin;
//                origin += stepVelocity;
//                targetPosition = origin;



//                float radius = ColliderRadius + m_SkinWidth * 2;
//                Vector3 capsuleOrigin = origin + ColliderCenter + Vector3.up * m_SkinWidth;
//                Vector3 segment1 = GetCapsulePoint(capsuleOrigin, transformUp);
//                Vector3 segment2 = GetCapsulePoint(capsuleOrigin, -transformUp);
//                if (CheckCapsule(segment1, segment2, radius, m_CollisionsLayerMask))
//                {
//                    colliderBuffer.Clear();

//                    int hits = Physics.OverlapCapsuleNonAlloc(segment1, segment2, radius, probedColiders, m_CollisionsLayerMask);
//                    if (hits > 0)
//                    {
//                        for (int k = 0; k < hits; k++)
//                        {

//                            colliderBuffer.Add(probedColiders[k]);

//                            var collision = colliderBuffer[k];
//                            Vector3 direction;
//                            float distance;
//                            if (Physics.ComputePenetration(charCollider, m_Transform.position, m_Transform.rotation,
//                                                            collision, collision.transform.position, collision.transform.rotation,
//                                                            out direction, out distance))
//                            {
//                                Vector3 penetrationVector = direction * distance;
//                                Vector3 moveDirectionProjected = Vector3.Project(moveDirection, -direction);
//                                //  m_transform.position = m_transform.position + penetrationVector;
//                                //m_Rigidbody.MovePosition(m_Transform.position + penetrationVector);
//                                moveDirection -= moveDirectionProjected;

//                                Debug.DrawLine(origin, penetrationVector, Color.black);
//                            }

//                            var closestPoint = colliderBuffer[k].ClosestPointOnBounds(origin);

//                            //Debug.LogFormat("index: {0} | buffer: {1} | nonAlac: {2}", k, colliderBuffer.Capacity, probedColiders.Length);
//                            Debug.DrawLine(origin, closestPoint, Color.black);
//                            DebugDraw.Sphere(closestPoint, 0.05f, Color.black);

//                            hasHit = true;
//                        }

//                    }
//                }

//                motionTrajectory[i] = origin;

//                if (hasHit)
//                {
//                    //targetPosition = origin;
//                    //DebugDraw.DrawCapsule(segment1, segment2, radius, Color.cyan);
//                }
//                else
//                {
//                    break;
//                }



//            }
//        }




//        public void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
//        {
//            InputVector.Set(horizontalMovement, 0, forwardMovement);
//            LookRotation = lookRotation;
//            //  Set the input vector, move direction and rotation angle based on the movement type.
//            switch (m_MovementType)
//            {
//                case (MovementTypes.Adventure):
//                    //  Get the correct target rotation based on input.
//                    rotationAngle = GetAngleFromForward(inputVector);
//                    inputVector = m_Transform.InverseTransformDirection(inputVector);
//                    inputVector.z = Mathf.Clamp01(inputVector.z + Mathf.Abs(rotationAngle) * Mathf.Deg2Rad);
//                    //currentVelocity += Vector3.Scale(inputVector, m_GroundAcceleration);
//                    moveDirection = m_Transform.forward * Mathf.Abs(inputVector.z);
//                    //moveDirection = Vector3.SmoothDamp(moveDirection, m_Transform.forward * Mathf.Abs(inputVector.z), ref moveDirectionSmooth, 0.18f);

//                    break;

//                case (MovementTypes.Combat):
//                    //  Get the correct target rotation based on look rotation.
//                    rotationAngle = GetAngleFromForward(LookRotation * m_Transform.forward);
//                    //relativeInputVector = inputVector;
//                    //moveDirection = m_Transform.TransformDirection(InputVector);
//                    moveDirection = Vector3.SmoothDamp(moveDirection, m_Transform.TransformDirection(inputVector), ref moveDirectionSmooth, 0.1f);

//                    break;
//            }
//        }


//        /// <summary>
//        /// Move charatcer based on input values.
//        /// </summary>
//        protected virtual void Move()
//        {
//            currentPosition = m_Transform.position;

//            //  We want an input vector that can be used regardless of movement type.
//            inputVector = InputVector;




//            //  Set the input vector, move direction and rotation angle based on the movement type.
//            switch (m_MovementType)
//            {
//                case (MovementTypes.Adventure):
//                    //  Get the correct target rotation based on input.
//                    rotationAngle = GetAngleFromForward(inputVector);
//                    inputVector = m_Transform.InverseTransformDirection(inputVector);
//                    inputVector.z = Mathf.Clamp01(inputVector.z + Mathf.Abs(rotationAngle) * Mathf.Deg2Rad);
//                    //currentVelocity += Vector3.Scale(inputVector, m_GroundAcceleration);
//                    moveDirection = m_Transform.forward * Mathf.Abs(inputVector.z);
//                    //moveDirection = Vector3.SmoothDamp(moveDirection, m_Transform.forward * Mathf.Abs(inputVector.z), ref moveDirectionSmooth, 0.18f);

//                    break;

//                case (MovementTypes.Combat):
//                    //  Get the correct target rotation based on look rotation.
//                    rotationAngle = GetAngleFromForward(LookRotation * m_Transform.forward);
//                    //relativeInputVector = inputVector;
//                    //moveDirection = m_Transform.TransformDirection(InputVector);
//                    moveDirection = Vector3.SmoothDamp(moveDirection, m_Transform.TransformDirection(inputVector), ref moveDirectionSmooth, 0.1f);

//                    break;
//            }


//            //  Is there enough movement to be considered moving.
//            Moving = moveDirection.sqrMagnitude > 0;
//            //Moving = Mathf.Clamp01(Mathf.Abs(InputVector.x) + Mathf.Abs(InputVector.z)) > 0.1f;


//            //  Get all the target values.
//            if (m_UseRootMotion)
//            {
//                targetRotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
//                targetAngularVelocity = new Vector3(0, rotationAngle, 0);
//                targetVelocity = targetRotation * (RootMotionVelocity / m_DeltaTime);
//                //targetVelocity = Vector3.Lerp(previousVelocity, moveRotation * (RootMotionVelocity / m_DeltaTime), m_DeltaTime * m_DeltaTime);
//                //previousVelocity = targetVelocity;
//                //previousPosition = m_Transform.TransformPoint(targetPosition);

//                targetPosition = m_Transform.position + Quaternion.Inverse(m_Transform.rotation) * m_Transform.TransformDirection(targetVelocity);
//                //targetPosition = Vector3.Lerp(previousPosition, _targetPosition, m_DeltaTime * m_DeltaTime);
//                //previousPosition = targetPosition;
//                //targetPosition = Vector3.Lerp(previousPosition, targetPosition, deltaAngle / rotationAngle);
//            }
//            else
//            {
//                targetRotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
//                targetAngularVelocity = new Vector3(0, rotationAngle, 0);
//                targetVelocity = moveDirection * (Grounded ? m_GroundSpeed : m_AirborneSpeed);
//                targetPosition = m_Transform.position + Quaternion.Inverse(m_Transform.rotation) * m_Transform.TransformDirection(targetVelocity);
//            }



//            Vector3 axisSign = Vector3.Cross(inputVector, m_Transform.forward);
//            // Calculate the angular delta in character rotation
//            float angularDelta = (axisSign.y >= 0 ? -1 : 1) * GetAngleFromForward(previousForward) - deltaAngle;
//            deltaAngle = 0f;
//            previousForward = transform.forward;
//            //angle = (angle * 0.01f) / m_DeltaTime;
//            angularDelta = angularDelta / deltaTime;
//            //angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);









//            //if (DebugMotion) Debug.DrawRay(m_Transform.position + Vector3.up * 1.5f, targetVelocity, Color.blue);
//            //if (DebugMotion) Debug.DrawLine(m_Transform.position + Vector3.up * 1.8f, targetPosition + Vector3.up *1.8f, Color.green);
//            //if (DebugMotion && Moving) DebugDraw.Sphere(targetPosition + Vector3.up * 1.8f, 0.1f, Color.green);

//            //CharacterDebug.Log("<b><color=magenta>*** RotationAngle </color></b>", MathUtil.Round(rotationAngle , 5));
//            //CharacterDebug.Log("<b><color=magenta>*** AngularDelta </color></b>", MathUtil.Round(angularDelta, 5));
//        }


//        /// <summary>
//        /// Perform checks to determine if the character is on the ground.
//        /// </summary>
//        protected virtual void CheckGround()
//        {
//            bool querriesHitTriggers = Physics.queriesHitTriggers;
//            Physics.queriesHitTriggers = false;

//            bool grounded = false;
//            //Vector3 RaycastOrigin = m_Transform.position + Vector3.up * charCollider.radius;
//            float radius = ColliderRadius * 0.9f;
//            Vector3 origin = m_Transform.position + Vector3.up * (ColliderCenter.y - ColliderHeight / 2 + m_SkinWidth);
//            Vector3 sphereCastOrigin = origin + Vector3.up * (radius + m_SkinWidth);
//            groundAngle = 0;

//            float groundDistance = 10;
//            //groundHit = new RaycastHit();
//            //groundHit.point = m_Transform.position - Vector3.up * airborneThreshold;
//            //groundHit.normal = m_Transform.up;


//            if (Physics.Raycast(origin, Vector3.down, out groundHit, ColliderRadius, m_CollisionsLayerMask))
//            {
//                groundDistance = Vector3.Project(m_Transform.position - groundHit.point, transform.up).magnitude;
//                groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);

//                if (Physics.SphereCast(sphereCastOrigin, radius, Vector3.down, out groundHit, ColliderRadius + 2, m_CollisionsLayerMask))
//                {
//                    groundAngle = Vector3.Angle(groundHit.normal, m_Transform.up);

//                    if (groundAngle > m_SlopeLimit)
//                    {
//                        // Retrieve a vector pointing down the slope
//                        Vector3 r = Vector3.Cross(groundHit.normal, -transform.up);
//                        Vector3 v = Vector3.Cross(r, groundHit.normal);

//                        //Get a position slightly above the controller position to raycast down the slope from to avoid clipping
//                        Vector3 flushOrigin = groundHit.point + groundHit.normal * tinyOffset;

//                        // Properties of the flushHit ground
//                        if (Physics.Raycast(flushOrigin, v, out groundHit, radius * 2, m_CollisionsLayerMask))//Perform Raycast
//                        {
//                            float cos = 1 / Mathf.Cos(groundAngle);
//                            float smallerRadius = radius * 0.9f;
//                            float hypS = smallerRadius * cos;
//                            float hypB = radius * cos;

//                            Vector3 circleCenterSmall = groundHit.point + groundHit.normal * smallerRadius;
//                            Vector3 pointOnSurface = circleCenterSmall + hypS * transform.up * -1;
//                            Vector3 circleCenterBig = pointOnSurface + hypB * transform.up;

//                            groundHit.distance = Vector3.Distance(origin, circleCenterBig);

//                            if (groundHit.distance < tinyOffset * tinyOffset)
//                                groundHit.distance = 0;

//                            groundHit.point = circleCenterBig - groundHit.normal * radius;

//                        }

//                        //againstWall = true;
//                    }


//                    if (groundDistance > groundHit.distance - radius - m_SkinWidth)
//                        groundDistance = groundHit.distance - radius - m_SkinWidth;

//                }  // End of SphereCast

//            }  //  End of Raycast.



//            if (groundDistance < 0.05f && groundAngle < 85)
//            {
//                grounded = true;

//                //Vector3 groundedPos = m_Transform.position - m_Transform.up * groundDistance;
//                //m_Rigidbody.MovePosition(groundedPos);
//                //var averagePosition = GetAverageRaycast(charCollider.radius + m_SkinWidth, charCollider.radius + m_SkinWidth);
//                //m_Rigidbody.MovePosition(Vector3.MoveTowards(m_Rigidbody.position, averagePosition, m_DeltaTime * 4));
//            }
//            else
//            {
//                grounded = false;
//                groundAngle = 0;
//                groundHit.point = m_Transform.position + Vector3.up * airborneThreshold;
//                groundHit.normal = m_Transform.up;
//            }



//            Grounded = grounded;
//            m_Rigidbody.useGravity = !Grounded;
//            Physics.queriesHitTriggers = querriesHitTriggers;


//            //  Draw Sphere cast
//            if (DebugGroundCheck) DebugDraw.Sphere(groundHit.point + Vector3.up * radius, radius, Grounded ? Color.green : Color.grey);
//            if (DebugGroundCheck) Debug.DrawLine(RaycastOrigin, groundHit.point, Grounded ? Color.green : Color.grey);
//            if (DebugGroundCheck) DebugDraw.DrawMarker(groundHit.point, 0.1f, Grounded ? Color.green : Color.grey);

//            CharacterDebug.Log("<color=green>Ground Hit Distance</color>", MathUtil.Round(groundDistance));
//            CharacterDebug.Log("<color=green>Ground Angle</color>", MathUtil.Round(groundAngle));


//        }



//        /// <summary>
//        /// Ensure the current movement direction is valid.
//        /// </summary>
//        protected virtual void CheckMovement()
//        {

//            if (groundAngle > m_SlopeLimit)
//            {
//                //Grab the direction that the controller is moving in
//                Vector3 absoluteMoveDirection = Vector3.ProjectOnPlane(m_Transform.position - previousPosition, groundHit.normal);

//                // Retrieve a vector pointing down the slope
//                Vector3 r = Vector3.Cross(groundHit.normal, -m_Transform.up);
//                Vector3 v = Vector3.Cross(r, groundHit.normal);

//                //Check the angle between the move direction of the controller and a vector down the slope. If less than 90 degrees then the player is moving down the slope return false
//                float angle = Vector3.Angle(absoluteMoveDirection, v);

//                //if (angle <= 90.0f)
//                //    return false;

//                //// Calculate where to place the controller on the slope, or at the bottom, based on the desired movement distance
//                //Vector3 resolvedPosition = Math3d.ProjectPointOnLine(initialPosition, r, transform.position);
//                //Vector3 direction = Math3d.ProjectVectorOnPlane(n, resolvedPosition - transform.position);

//                //transform.position += direction;
//            }




//            //MotionTrajectory(targetVelocity);

//            //float castRadius = charCollider.radius + m_SkinWidth;

//            //Vector3 p1 = RaycastOrigin + charCollider.center + Vector3.up * (charCollider.height * 0.5f - castRadius);
//            //Vector3 p2 = RaycastOrigin + charCollider.center - Vector3.up * (charCollider.height * 0.5f - castRadius);

//            //var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, probedColiders, m_CollisionsLayerMask);
//            //totalColliderHits += hits;
//            //if (hits > 0)
//            //{
//            //    colliderBuffer.Clear();
//            //    var startIndex = totalColliderHits > hits ? totalColliderHits - hits : 0;
//            //    var centerMass = m_Transform.TransformPoint(m_Rigidbody.centerOfMass);
//            //    for (int i = startIndex; i < hits; i++)
//            //    {
//            //        colliderBuffer[i] = probedColiders[i];
//            //        var collision = colliderBuffer[i];
//            //        Vector3 direction;
//            //        float distance;
//            //        if (Physics.ComputePenetration(charCollider, m_Transform.position, m_Transform.rotation,
//            //                                        collision, collision.transform.position, collision.transform.rotation,
//            //                                        out direction, out distance))
//            //        {
//            //            Vector3 penetrationVector = direction * distance;
//            //            Vector3 moveDirectionProjected = Vector3.Project(moveDirection, -direction);
//            //            //  m_transform.position = m_transform.position + penetrationVector;
//            //            m_Rigidbody.MovePosition(m_Transform.position + penetrationVector);
//            //            moveDirection -= moveDirectionProjected;

//            //            Debug.DrawRay(centerMass, moveDirection, Color.red);
//            //        }

//            //        var closestPoint = colliderBuffer[i].ClosestPointOnBounds(centerMass);


//            //        Debug.DrawLine(centerMass, closestPoint, Color.black);
//            //        DebugDraw.Sphere(closestPoint, 0.05f, Color.black);

//            //        //Rigidbody rb = collision.attachedRigidbody;
//            //        //if (rb != null)
//            //        //{
//            //        //    if (rb.mass > m_Mass)
//            //        //    {

//            //        //    }
//            //        //}
//            //        //colliderBuffer[i].
//            //    }
//            //}

//            //If walk into wall.





//            m_GroundSlopeDir = Vector3.zero;

//            if (Grounded)
//            {
//                //  Find the vector that represents the slope.
//                Vector3 groundRight = Vector3.Cross(groundHit.normal, Vector3.down);
//                m_GroundSlopeDir = Vector3.Cross(groundRight, groundHit.normal);

//                // Slopes
//                Vector3 slopeCheckOffset = m_Transform.forward * (charCollider.radius + m_SkinWidth);
//                RaycastHit slopeHit1;
//                RaycastHit slopeHit2;
//                if (Physics.Raycast(RaycastOrigin + slopeCheckOffset, Vector3.down, out slopeHit1, m_CollisionsLayerMask))
//                {
//                    if (DebugCollisions) Debug.DrawLine(RaycastOrigin + slopeCheckOffset, slopeHit1.point, Grounded ? Color.green : Color.gray);

//                    float forwardAngle = Vector3.Angle(slopeHit1.normal, Vector3.up);
//                    if (Physics.Raycast(RaycastOrigin - slopeCheckOffset, Vector3.down, out slopeHit2, m_CollisionsLayerMask))
//                    {
//                        if (DebugCollisions) Debug.DrawLine(RaycastOrigin - slopeCheckOffset, slopeHit2.point, Grounded ? Color.green : Color.gray);

//                        float backAngle = Vector3.Angle(slopeHit2.normal, Vector3.up);
//                        float[] groundAngles = { groundAngle, forwardAngle, backAngle };
//                        Array.Sort(groundAngles);
//                        groundAngle = groundAngles[1];
//                    }
//                    else
//                    {
//                        groundAngle = (groundAngle + forwardAngle) / 2;
//                    }
//                }

//                if (groundAngle > 0)
//                {
//                    //  What to do if ground angle is greater than slope limit.
//                    if (groundAngle > m_SlopeLimit)
//                    {
//                        //  sliding is true.
//                        Moving = false;
//                        var localDir = m_Transform.InverseTransformDirection(m_GroundSlopeDir);
//                        rotationAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
//                        var targetDirection = Vector3.Project(moveDirection, m_GroundSlopeDir).normalized * m_SlopeForceDown;
//                        moveDirection = Vector3.Lerp(moveDirection, targetDirection, m_DeltaTime * m_RotationSpeed);
//                        m_Rigidbody.AddForce(moveDirection, ForceMode.Impulse);
//                        CharacterDebug.Log("<color=red> ** Sliding  </color>", "Sliding");
//                    }
//                    //else {
//                    //    Vector3 slopeDirection = Vector3.Cross(m_Transform.right, groundHit.normal);
//                    //    float direction = Mathf.Sign(slopeDirection.y);
//                    //    moveDirection = Vector3.Project(moveDirection, slopeDirection).normalized * (direction > 0 ? m_SlopeForceUp : m_SlopeForceDown);
//                    //}


//                    //float slopeStartAngle = 0;
//                    //float slopeEndAngle = m_SlopeLimit;
//                    //float angle = 90 - Vector3.Angle(m_Transform.forward, groundHit.normal);
//                    //angle -= slopeStartAngle;
//                    //float range = slopeEndAngle - slopeStartAngle;
//                    //float slopeDamper = 1f - Mathf.Clamp(angle / range, 0f, 1f);
//                    //CharacterDebug.Log("<color=green>anle  </color>", angle);
//                    //CharacterDebug.Log("<color=green>slopeDamper  </color>", slopeDamper);
//                }






//                //if (Physics.RaycastNonAlloc(m_Transform.position + Vector3.up * m_MaxStepHeight, m_Transform.forward, colliderBuffer, charCollider.radius + 1, m_CollisionsLayerMask) > 0) {

//                //}

//                //float offset = charCollider.radius + m_SkinWidth;
//                //Vector3 groundAverage = GetAverageRaycast(offset, offset, 2);
//                //if (groundAverage != m_Rigidbody.position) {
//                //    m_Rigidbody.MovePosition(new Vector3(m_Transform.position.x, groundAverage.y + 0.1f, m_Transform.position.z));
//                //}

//            }


//            //CharacterDebug.Log("<color=green>Ground Angle</color>", groundAngle);

//        }



//        /// <summary>
//        /// Update the character’s position values.
//        /// </summary>
//        protected virtual void UpdateMovement()
//        {

//            Vector3 velocityAdjusted = moveDirection.normalized;
//            if (Grounded)
//            {
//                //float moveDirScale = Mathf.Abs(inputVector.x) + Mathf.Abs(inputVector.z);// + moveDirection.magnitude;
//                float moveDirScale = moveDirection.magnitude;
//                Vector3 groundNormal = groundHit.normal;

//                if (Vector3.Dot(moveDirection, groundNormal) >= 0)
//                {
//                    //  If greater than 0, than we are going down a slope.

//                }
//                else
//                {
//                    //  We are going up the slope if it is negative.

//                }
//                //CharacterDebug.Log("<color=cyan>MoveDir Dot</color>", Vector3.Dot(moveDirection, groundNormal));


//                velocityAdjusted = GetDirectionTangentToSurface(velocityAdjusted, groundNormal) * moveDirScale;

//                // Calculate target velocity
//                Vector3 inputRight = Vector3.Cross(moveDirection, m_Transform.up);
//                Vector3 reorientedInput = Vector3.Cross(groundNormal, inputRight).normalized * moveDirScale;// moveDirection.magnitude;
//                Vector3 targetMovementVelocity = reorientedInput * 10; // MaxStableMoveSpeed;

//                //CharacterDebug.Log("<color=cyan>targetMovementVelocity</color>", targetMovementVelocity);


//                // Smooth movement Velocity
//                velocityAdjusted = Vector3.Lerp(velocityAdjusted, targetMovementVelocity, 1f - Mathf.Exp(-15 * m_DeltaTime));  //  1f - Mathf.Exp(-StableMovementSharpness * deltaTime));

//                //CharacterDebug.Log("<color=blue>velocityAdjusted1</color>", velocityAdjusted);
//            }
//            else
//            {
//                //if (inputVector.sqrMagnitude > 0f)
//                //{
//                //    //float movementContrarity = Mathf.Max(0f, Vector3.Dot(-_moveInputVector.normalized, currentVelocity.normalized));
//                //    //float chosenAccelerationSpeed = Mathf.Lerp(AirAccelerationSpeed, AirAccelerationSpeed * AirAccelerationBias, movementContrarity);
//                //    Vector3 addedVelocity = moveDirection * m_AirborneAcceleration * deltaTime;
//                //    //// TODO: Remove velocity going against all current obstruction normals
//                //    //if (Motor.GroundingStatus.FoundAnyGround)
//                //    //{
//                //    //    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
//                //    //    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
//                //    //}
//                //}

//                //  Gravity
//                velocityAdjusted += Gravity * m_DeltaTime;
//                //  Drag.
//                //velocityAdjusted *= (1f / (1f + (m_Rigidbody.drag * m_DeltaTime)));
//            }

//            //targetVelocity = GetDirectionTangentToSurface(targetVelocity, groundHit.normal);
//            //Debug.DrawRay(m_Transform.position + Vector3.up * 0.5f, targetVelocity, Color.blue);
//            //if (velocityAdjusted != Vector3.zero)
//            //{
//            //    Debug.DrawRay(m_Transform.position + Vector3.up * 0.8f, velocityAdjusted, Color.green);
//            //}
//            //else
//            //{
//            //    Debug.DrawRay(m_Transform.position + Vector3.up * 0.8f, m_Transform.forward, Color.grey);
//            //}

//            if (!m_Rigidbody.isKinematic)
//            {
//                m_Rigidbody.velocity = Vector3.Lerp(velocityAdjusted, targetVelocity, m_DeltaTime);
//            }

//            CharacterDebug.Log("<color=blue>targetVelocity</color>", targetVelocity);
//            CharacterDebug.Log("<color=blue>velocityAdjusted1</color>", velocityAdjusted);




//            //Vector3 horizontalDir = Vector3.Normalize(Vector3.Cross(moveDirection, groundHit.normal));
//            //Vector3 forwardDir = Vector3.Normalize(Vector3.Cross(groundHit.normal, horizontalDir)) * ColliderRadius;

//            //float velScale = Vector3.Dot(targetVelocity, forwardDir);
//            //float fwdScale = Vector3.Dot(currentVelocity, forwardDir);
//            //float velocityScale = velScale - fwdScale;

//            //if (Grounded && moveDirection != Vector3.zero)
//            //{
//            //    currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocitySmooth, 0.1f);
//            //    currentVelocity += (Gravity * m_GravityModifier + Gravity) * m_DeltaTime;
//            //    currentVelocity.y = groundHit.distance > 0 ? m_Rigidbody.velocity.y : 0;
//            //    currentVelocity = Vector3.ProjectOnPlane(currentVelocity, groundHit.normal) * m_GroundStickiness;
//            //}
//            //else
//            //{
//            //    Vector3 verticalVelocity = (currentVelocity - previousVelocity) * m_DeltaTime;
//            //    verticalVelocity = Vector3.Project(verticalVelocity, Gravity);
//            //    currentVelocity += verticalVelocity;
//            //}


//            //m_Rigidbody.velocity = m_Velocity;
//            //m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, groundHit.normal) * m_GroundStickiness;







//            //if (m_UseRootMotion)
//            //{
//            //    m_Velocity = RootMotionVelocity / m_DeltaTime;
//            //    m_Velocity = Vector3.Project((RootMotionVelocity / m_DeltaTime), moveDirection);
//            //    m_Velocity.y = Grounded ? 0 : m_Rigidbody.velocity.y;
//            //    //moveDirection = RootMotionVelocity / m_DeltaTime;
//            //}
//            //else
//            //{
//            //    targetVelocity = moveDirection * (Grounded ? m_GroundSpeed : m_AirborneSpeed);
//            //    float acc;
//            //    if (Moving) acc = Grounded ? m_Acceleration : m_AirborneAcceleration;
//            //    else acc = Grounded ? m_MotorDamping : m_AirborneDamping;
//            //    m_Velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref velocitySmooth, acc);
//            //}

//            //// // Drag
//            //// currentVelocity *= (1f / (1f + (Drag * deltaTime)));

//            //if (Grounded)
//            //{
//            //    //m_Velocity = targetRotation * m_Velocity;

//            //    if (groundAngle > m_SlopeLimit)
//            //    {
//            //        Vector3 drag = m_Velocity * (groundAngle - m_SlopeLimit) * m_DeltaTime;
//            //        if (moveDirection.sqrMagnitude > 0)
//            //        {
//            //            drag *= (1f - Vector3.Dot(moveDirection.normalized, m_Velocity.normalized));
//            //        }

//            //        if (drag.sqrMagnitude > m_Velocity.sqrMagnitude) m_Velocity = Vector3.zero;
//            //        else m_Velocity -= drag;
//            //    }

//            //    m_Velocity = Vector3.SmoothDamp(m_Velocity, m_Velocity + moveDirection, ref velocitySmooth, 0.1f);
//            //    m_Velocity.y = 0;
//            //    //m_Velocity += (Gravity + Gravity * m_GravityModifier) * m_DeltaTime;
//            //    m_Velocity = Vector3.ProjectOnPlane(m_Velocity, groundHit.normal * m_GroundStickiness);

//            //}
//            //else
//            //{
//            //    m_Velocity -= Vector3.Project(moveDirection, m_Transform.forward);
//            //    m_Velocity += (Gravity + Gravity * m_GravityModifier) * m_DeltaTime;
//            //    //Vector3 verticalVelocity = (m_Velocity - m_PreviousPosition) * m_DeltaTime;
//            //    //verticalVelocity = Vector3.Project(verticalVelocity, Gravity);
//            //    //m_Velocity += verticalVelocity;
//            //}


//            //m_Rigidbody.velocity = m_Velocity;
//            ////m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, groundHit.normal * m_GroundStickiness);
//        }


//        /// <summary>
//        /// Update the character’s rotation values.
//        /// </summary>
//        protected virtual void UpdateRotation()
//        {

//            if (LookDirection.sqrMagnitude > 0f)
//            {
//                Vector3 smoothLookDirection = Vector3.Slerp(m_Transform.forward, LookDirection, 1 - Mathf.Exp(-10 * deltaTime)).normalized;
//                currentRotation = Quaternion.LookRotation(smoothLookDirection, m_Transform.up);
//            }
//            //if (Grounded)
//            //{
//            //    var hits = Physics.RaycastNonAlloc(m_Transform.position + m_Transform.rotation * ColliderCenter, -m_Transform.up, probedHits, ColliderHeight, m_CollisionsLayerMask);
//            //    totalRaycastHits += hits;
//            //    if (hits > 0)
//            //    {
//            //        float groundReorientSharpness = 10f;
//            //        Vector3 smoothedNormal = Vector3.Slerp(m_Transform.up, LookDirection, 1 - Mathf.Exp(-groundReorientSharpness * deltaTime)).normalized;  //  VectorB (LookDirection) should be the closest hit from the RaycastNonAlloac
//            //        currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), smoothedNormal) * currentRotation;
//            //    }
//            //}

//            //Quaternion currentRotation = transform.rotation;
//            //int maxAngleDifference = 35;
//            //Quaternion previousRotation = m_Transform.rotation;
//            ////int rotationCollisionCheckCount = 10;
//            //int intervals = Mathf.CeilToInt(Quaternion.Angle(m_Transform.rotation, targetRotation) / maxAngleDifference);
//            //float subdivided = 1f / intervals;
//            //CharacterDebug.Log("<color=green> Angle Subdivided</color>", subdivided);
//            //for (int i = 1; i <= intervals; i++)
//            //{
//            //    currentRotation = Quaternion.Slerp(currentRotation, targetRotation, subdivided * i);
//            //    if (Physics.CheckCapsule(m_Transform.position + Vector3.up * (ColliderRadius), m_Transform.position + Vector3.up * (ColliderHeight - ColliderRadius), ColliderRadius - m_SkinWidth * 2))
//            //    {
//            //        currentRotation = previousRotation;
//            //        break;
//            //    }
//            //    else
//            //    {
//            //        previousRotation = currentRotation;
//            //    }

//            //    Debug.DrawRay(RaycastOrigin, currentRotation * m_Transform.forward, Color.yellow, 1);
//            //}



//            float moveAmount = Mathf.Clamp01(Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z));
//            moveAmount = MathUtil.Round(moveAmount);
//            float rotationSpeed = Mathf.Lerp(0, Moving ? m_RotationSpeed : m_RotationSpeed * m_IdleRotationMultiplier, moveAmount);



//            var currentAngle = Mathf.SmoothDampAngle(rotationAngle, (rotationAngle * rotationSpeed * m_DeltaTime), ref rotationAngleSmooth, 0.24f);
//            currentAngle = MathUtil.Round(currentAngle);


//            currentRotation = Quaternion.AngleAxis(currentAngle, m_Transform.up);
//            angularVelocity = m_Transform.up * currentAngle;


//            //  Update angular velocity.
//            m_Rigidbody.angularDrag = Mathf.SmoothDamp(m_Rigidbody.angularDrag, moveAmount > 0 ? 0.05f : m_Mass, ref angularDragSmooth, 0.16f);
//            m_Rigidbody.angularVelocity = Vector3.Slerp(m_Rigidbody.angularVelocity, angularVelocity, m_DeltaTime * rotationSpeed);




//            currentRotation = Quaternion.Slerp(m_Transform.rotation, currentRotation * m_Transform.rotation, (m_RotationSpeed * m_DeltaTime) * (m_RotationSpeed * m_DeltaTime));
//            m_Rigidbody.MoveRotation(currentRotation);





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

//            m_Animator.SetBool(HashID.Grounded, Grounded);
//            m_Animator.SetBool(HashID.Moving, Moving);


//            float lookAngle = 0;
//            float maxLookAngle = 90;
//            if (LookAngle > maxLookAngle || LookAngle < -maxLookAngle) lookAngle = Mathf.Lerp(maxLookAngle, 0, m_DeltaTime * 4);
//            else lookAngle = LookAngle;
//            lookAngle = MathUtil.Round(lookAngle);
//            //CharacterDebug.Log("<b><color=orange>*** lookAngle</color></b>", lookAngle);
//            m_Animator.SetFloat(HashID.LookAngle, lookAngle);




//            m_Speed = InputVector.normalized.sqrMagnitude * (Movement == MovementTypes.Adventure ? 1 : 0);
//            m_Animator.SetFloat(HashID.Speed, Speed);

//            float rotation = GetAngleFromForward(m_Transform.forward) - deltaAngle;
//            deltaAngle = 0;
//            rotation *= 0.01f;
//            rotation = Mathf.Clamp(rotation / Time.deltaTime, -1f, 1f);
//            rotation = MathUtil.Round(rotation, 8);
//            //CharacterDebug.Log("<b><color=orange>*** rotation 1</color></b>", rotation);
//            //m_Animator.SetFloat(HashID.Rotation, rotation);
//            //CharacterDebug.Log("<b><color=orange>*** rotation 1</color></b>", rotationAngle);
//            m_Animator.SetFloat(HashID.Rotation, rotationAngle);
//            //m_Animator.SetFloat(HashID.Rotation, rotationAngle, 0.1f, m_DeltaTime);




//            m_AnimationMonitor.SetForwardInputValue(inputVector.z);
//            m_AnimationMonitor.SetHorizontalInputValue(inputVector.x);

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
//                //if (m_Animator.hasRootMotion) m_Rigidbody.MovePosition(m_Animator.rootPosition);
//            }

//            if (m_UseRootMotionRotation)
//            {
//                //float angleInDegrees;
//                //Vector3 rotationAxis;
//                m_Animator.deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
//                angleInDegrees = (angleInDegrees * m_RootMotionSpeedMultiplier * Mathf.Deg2Rad) / m_DeltaTime;
//                RootMotionRotation = Quaternion.AngleAxis(angleInDegrees, rotationAxis);

//                //if (m_Animator.hasRootMotion) m_Rigidbody.MoveRotation(m_Animator.rootRotation);
//            }


//        }


//        /// <summary>
//        /// Set the collider's physics material.
//        /// </summary>
//        protected virtual void SetPhysicsMaterial()
//        {
//            //change the physics material to very slip when not grounded or maxFriction when is

//            //  Airborne.
//            if (!Grounded && Mathf.Abs(m_Rigidbody.velocity.y) > 0)
//            {
//                charCollider.material.staticFriction = 0f;
//                charCollider.material.dynamicFriction = 0f;
//                charCollider.material.frictionCombine = PhysicMaterialCombine.Minimum;
//            }
//            //  Grounded and is moving.
//            else if (Grounded && Moving)
//            {
//                charCollider.material.staticFriction = 0.25f;
//                charCollider.material.dynamicFriction = 0f;
//                charCollider.material.frictionCombine = PhysicMaterialCombine.Multiply;
//            }
//            //  Grounded but not moving.
//            else if (Grounded && !Moving)
//            {
//                charCollider.material.staticFriction = 1f;
//                charCollider.material.dynamicFriction = 1f;
//                charCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
//            }

//            else
//            {
//                charCollider.material.staticFriction = 1f;
//                charCollider.material.dynamicFriction = 1f;
//                charCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
//            }

//            //charCollider.material = m_AirFrictionMaterial;
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

//        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
//        {
//            float scale = direction.magnitude;
//            Vector3 temp = Vector3.Cross(surfaceNormal, direction);
//            Vector3 tangent = Vector3.Cross(temp, surfaceNormal);
//            tangent = tangent.normalized * scale;
//            return tangent;
//        }



//        public void CollisionsOverlap(Vector3 position)
//        {
//            float castRadius = charCollider.radius + m_SkinWidth;

//            Vector3 p1 = position + charCollider.center + Vector3.up * (charCollider.height * 0.5f - castRadius);
//            Vector3 p2 = position + charCollider.center - Vector3.up * (charCollider.height * 0.5f - castRadius);

//            //DebugDraw.Capsule(position + charCollider.center, Vector3.up, castRadius, charCollider.height, Color.white);

//            var hits = Physics.OverlapCapsuleNonAlloc(p1, p2, castRadius, probedColiders, m_CollisionsLayerMask);

//            if (hits > 0)
//            {
//                for (int i = 0; i < hits; i++)
//                {
//                    Debug.DrawLine(p1, colliderBuffer[i].ClosestPoint(p1), Color.red);
//                    //colliderBuffer[i].
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
//                    if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_CollisionsLayerMask))
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
//            if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_CollisionsLayerMask))
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
//            if (charCollider.height < ColliderHeight * scale || charCollider.height > ColliderHeight * scale)
//            {
//                charCollider.height = Mathf.MoveTowards(charCollider.height, ColliderHeight * scale, Time.deltaTime * 4);
//                charCollider.center = Vector3.MoveTowards(charCollider.center, ColliderCenter * scale, Time.deltaTime * 2);
//            }
//        }


//        public virtual float GetColliderHeightAdjustment()
//        {
//            return charCollider.height;
//        }


//        // Gets angle around y axis from a world space direction
//        protected float GetAngleFromForward(Vector3 worldDirection)
//        {
//            Vector3 local = transform.InverseTransformDirection(worldDirection);
//            return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
//        }



//        // Rotate a rigidbody around a point and axis by angle
//        protected void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle)
//        {
//            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
//            Vector3 d = transform.position - point;
//            m_Rigidbody.MovePosition(point + rotation * d);
//            m_Rigidbody.MoveRotation(rotation * transform.rotation);
//        }



//        public bool CheckCapsule(Vector3 segment1, Vector3 segment2, float radius, LayerMask mask)
//        {
//            //Vector3 transformUp = m_Transform.up;
//            //Vector3 start = GetCapsulePoint(m_Transform.position + ColliderCenter, transformUp);
//            //Vector3 end = GetCapsulePoint(m_Transform.position + ColliderCenter, -transformUp);
//            return Physics.CheckCapsule(segment1, segment2, radius, mask);
//        }


//        public Vector3 GetCapsulePoint(Vector3 origin, Vector3 direction)
//        {
//            var pointsDist = ColliderHeight - (ColliderRadius * 2f);
//            return origin + (direction * (pointsDist * .5f));
//        }


//        protected Vector3 GetAcceleration(Vector3 finalVelocity, Vector3 initialVelocity, Vector3 distance)
//        {
//            Vector3 vf = new Vector3(finalVelocity.x * finalVelocity.x, finalVelocity.y * finalVelocity.y, finalVelocity.z * finalVelocity.z);
//            Vector3 vi = new Vector3(initialVelocity.x * initialVelocity.x, initialVelocity.y * initialVelocity.y, initialVelocity.z * initialVelocity.z);
//            //Vector3 a = (vf - vi) / (2 * distance);
//            distance *= 2;
//            Vector3 a = vf - vi;
//            a.Set((a.x / distance.x), (a.x / distance.x), (a.x / distance.x));
//            return a;
//        }



//        protected void ClearNonAllocArrays()
//        {
//            if (totalColliderHits > 0)
//                for (int i = 0; i < totalColliderHits; i++)
//                    probedColiders[i] = null;

//            //if (totalRaycastHits > 0)
//            //    for (int i = 0; i < totalRaycastHits; i++)
//            //        probedHits[i] = 
//        }


//        #endregion










//        #region Debugging



//        protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);




//        protected virtual void DebugAttributes()
//        {

//            //CharacterDebug.Log("seperator", "----------");
//            CharacterDebug.Log("<color=brown>Moving</color>", Moving);
//            CharacterDebug.Log("<color=brown>Grounded</color>", Grounded);
//            //CharacterDebug.Log("seperator", "----------");

//            CharacterDebug.Log("MoveDirection", MoveDirection);
//            CharacterDebug.Log("MoveDirection", InputVector);



//            //CharacterDebug.Log("Velocity", Velocity);
//            //CharacterDebug.Log("<color=blue>•rbVelocity</color>", m_Rigidbody.velocity);
//            //CharacterDebug.Log("CollisionsCount", GetASctiveCollisions());
//            //CharacterDebug.Log("rb_AngularVelocity", m_Rigidbody.angularVelocity);
//            CharacterDebug.Log("rb_Velocity", m_Rigidbody.velocity);
//        }



//        protected abstract void DrawGizmos();


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
//            if (Application.isPlaying && Debugger.debugMode)
//            {
//                DrawOnGUI();
//            }

//        }



//        private void OnDrawGizmos()
//        {
//            if (Debugger.debugMode && Application.isPlaying)
//            {
//                //Gizmos.color = Color.white;
//                //Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, m_Transform.InverseTransformDirection(LookRotation * m_Transform.forward));
//                //GizmosUtils.DrawText(GUI.skin, "LookDirection", transform.position + Vector3.up * 1.5f + LookRotation * transform.forward, Color.green);

//                //if(m_Rigidbody.velocity != Vector3.zero) {
//                //    Gizmos.color = Color.green;
//                //    GizmosUtils.DrawArrow(RaycastOrigin, m_Rigidbody.velocity);
//                //    GizmosUtils.DrawText(GUI.skin, "Velocity", RaycastOrigin + transform.forward, Color.green);
//                //}
//                //Gizmos.color = Color.cyan;
//                //Gizmos.DrawWireSphere(m_Transform.position, ColliderRadius);
//                //Gizmos.color = Color.yellow;
//                //GizmosUtils.DrawWireCircle(m_Animator.pivotPosition, 0.1f);
//                //GizmosUtils.DrawMarker(m_Animator.rootPosition, 0.1f, Color.magenta);

//                //if (DebugCollisions) GizmosUtils.DrawWireCapsule(RaycastOrigin + charCollider.center, charCollider.radius + m_SkinWidth, charCollider.height - m_SkinWidth);

//                if (Grounded)
//                {

//                }
//                GizmosUtils.DrawText(GUI.skin, Grounded.ToString(), transform.position + Vector3.up * 2f, Grounded ? Color.black : Color.red);


//                if (motionTrajectory != null)
//                {
//                    Gizmos.color = Color.blue;
//                    for (int i = 0; i < motionTrajectoryResolution; i++)
//                    {
//                        if (motionTrajectory[i] != Vector3.zero)
//                        {
//                            Gizmos.DrawSphere(motionTrajectory[i], 0.08f);

//                            if (i + 1 < motionTrajectoryResolution)
//                                Gizmos.DrawLine(motionTrajectory[i], motionTrajectory[i + 1]);
//                        }

//                    }
//                }



//                //  Draw Gizmos
//                DrawGizmos();
//            }

//        }


//        #endregion





//    }

//}




////Debug.Log("Horizontal: " + m_HorizontalMovement + " Forward: " + m_ForwardMovement + " Euler: " + m_LookRotation.eulerAngles);
////if (m_Controller.Movement == RigidbodyCharacterController.MovementType.Combat || m_Controller.Movement == RigidbodyCharacterController.MovementType.Adventure) {
////m_LookRotation = m_CameraTransform.rotation;
////} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.TopDown) {
////var direction = (Vector3)PlayerInput.GetMousePosition() - m_Camera.WorldToScreenPoint(m_Transform.position);
////// Convert the XY direction to an XYZ direction with Y equal to 0.
////direction.z = direction.y;
////direction.y = 0;
////m_LookRotation = Quaternion.LookRotation(direction);
////} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.RPG) {
////if (PlayerInput.IsDisabledButtonDown(false)) {
////m_LookRotation = m_CameraTransform.rotation;
////if (PlayerInput.IsDisabledButtonDown(true)) {
////m_ForwardMovement = 1;
////}
////} else if (!PlayerInput.IsDisabledButtonDown(true)) {
////if (m_ForwardMovement != 0 || m_HorizontalMovement != 0) {
////m_LookRotation = m_CameraTransform.rotation;
////}
////m_HorizontalMovement = 0;
////}
////} else if (m_Controller.Movement == RigidbodyCharacterController.MovementType.Pseudo3D) {
////var direction = (Vector3)PlayerInput.GetMousePosition() - m_Camera.WorldToScreenPoint(m_Transform.position + m_CapsuleCollider.center);
////m_LookRotation = Quaternion.LookRotation(direction);
////} else { // Point and Click.
////m_LookRotation = m_PointClickLookRotation.Get();
////}
