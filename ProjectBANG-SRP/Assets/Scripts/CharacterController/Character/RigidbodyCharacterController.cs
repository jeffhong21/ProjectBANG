namespace CharacterController
{
    using UnityEngine;
    using System;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class RigidbodyCharacterController : MonoBehaviour
    {
        public enum MovementType { Adventure, Combat };



        #region Inspector properties
        //  Locomotion variables
        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [Tooltip("The anim speed multiplier allows the overall " +
            "speed of walking/running to be tweaked in the inspector, which " +
            "affects the movement speed because of the root motion.")]
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_GroundAcceleration = 0.12f;
        [SerializeField, HideInInspector]
        protected float m_AirborneAcceleration = 0.2f;                   //  NEED TO ADD
        [SerializeField, HideInInspector]
        protected float m_MotorDamping = 0.3f;
        [SerializeField, HideInInspector]
        protected float m_AirborneDamping = 0.3f;
        [SerializeField, HideInInspector]
        protected float m_GroundSpeed = 1f;
        [SerializeField, HideInInspector]
        protected float m_AirborneSpeed = 0.5f;                          //  NEED TO ADD
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 10f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceUp = 1f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceDown = 1.25f;
        [Tooltip("A -1 to 1 threshold for when the character should stop moving if another object collides with the character. " +
        "A value of 1 indicates the object is directly in front of the character's move direction while a value of -1 indicates the " +
        "object is directly behind the character's move direction")]
        [SerializeField, HideInInspector, Range(-1, 1)]
        protected float m_StopMovementThreshold = 0.5f;


        //  -- Physics variables
        [SerializeField, HideInInspector]
        protected float m_DetectObjectHeight = 0.4f;
        [SerializeField, HideInInspector]
        protected float m_Mass = 100;
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector, Range(0, 90)]
        protected float m_SlopeLimit = 45f;
        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.4f;
        [SerializeField, HideInInspector]
        protected float m_ExternalForceDamping = 0.1f;
        [SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
        protected float m_StepOffset = 0.15f;
        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;

        [SerializeField, HideInInspector]
        protected float m_GravityModifier = 0.4f;
        [SerializeField, HideInInspector]
        protected float m_GroundStickiness = 6f;

        //  -- Collision detection
        [SerializeField, HideInInspector]
        protected bool mDetectHorizontalCollision = true;
        [SerializeField, HideInInspector]
        protected int m_HorizontalCollisionCount = 10;      //  Do not need
        [SerializeField, HideInInspector]
        protected int m_VerticalCollisionCount = 10;        // Do not need
        [SerializeField, HideInInspector]
        protected LayerMask m_ColliderLayerMask;
        [SerializeField, HideInInspector]
        protected int m_MaxCollisionCount = 50;


        //  -- Animation
        [SerializeField, HideInInspector]
        protected float m_IdleRotationMultiplier = 2f;
        [SerializeField, HideInInspector]
        protected string m_MovingStateName = "Movement.Movement";
        [SerializeField, HideInInspector]
        protected string m_AirborneStateName = "Fall.Fall";

        #endregion

        protected float m_TimeScale = 1;

        protected MovementType m_MovementType = MovementType.Adventure;
        protected bool m_Moving, m_Grounded = true, m_Aiming;
        protected float m_DeltaYRotation;
        protected Vector3 m_MoveDirection, m_PreviousPosition, m_ExternalForce, m_Velocity, m_AngularVelocity;
        protected Quaternion m_LookRotation;




        protected float m_GroundAngle;

        protected float m_SlopeAngle;
        protected RaycastHit m_GroundHit;
        protected RaycastHit[] m_Collisions;

        protected CapsuleCollider m_CapsuleCollider;
        protected float m_ColliderHeight, m_ColliderCenterY;
        protected Collider[] m_LinkedColliders = new Collider[0];
        protected PhysicMaterial m_GroundIdleFrictionMaterial, m_GroundedMovingFrictionMaterial, m_AirFrictionMaterial,
                                 m_StepFrictionMaterial, m_SlopeFrictionMaterial;


        protected bool m_CheckGround = true;
        protected bool m_CheckMovement = true;
        protected bool m_SetPhysicsMaterial = true;
        protected bool m_UpdateRotation = true;
        protected bool m_UpdateMovement = true;
        protected bool m_UpdateAnimator = true;
        protected bool m_Move = true;


        private float m_StartAngle, m_StartAngleSmooth;


        private Vector3 m_VelocitySmooth, m_ExternalForceSmooth;
        private float m_RotationSmoothDamp;




        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimationMonitor;
        protected LayerManager m_Layers;
        protected Rigidbody m_Rigidbody;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected float m_DeltaTime, deltaTime, fixedDeltaTime;



        #region Parameters for Editor

        //  For Editor.
        //  Debug parameters.
        public bool DebugMode { get { return m_Debug; } set { m_Debug = value; } }
        [SerializeField, HideInInspector]
        protected bool m_Debug, m_DebugCollisions, m_DrawDebugLine;
        [SerializeField, HideInInspector]
        protected bool displayMovement = true, displayPhysics = true, displayAnimations = true, displayActions = true;

        #endregion





        #region Properties

        public MovementType Movement { get { return m_MovementType; } }

        public bool Moving { get { return m_Moving; } set { m_Moving = value; } }

        public bool Aiming { get { return m_Aiming; } set { m_Aiming = value; } }

        public bool Grounded { get { return m_Grounded; } set { m_Grounded = value; } }

        public float RotationSpeed { get { return m_RotationSpeed; } set { m_RotationSpeed = value; } }

        public Vector3 InputVector { get; set; }

        public Vector3 RelativeInputVector { get; private set; }

        public Vector3 Velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        public Quaternion LookRotation { get { return m_LookRotation; } set { m_LookRotation = value; } }

        public Vector3 RootMotionVelocity { get; set; }

        public Quaternion RootMotionRotation { get; set; }

        public bool UseRootMotion { get { return m_UseRootMotion; } set { m_UseRootMotion = value; } }

        public Vector3 Gravity { get; private set; }

        public RaycastHit GroundHit { get { return m_GroundHit; } }

        public Vector3 RaycastOrigin { get { return m_Transform.position + Vector3.up * m_SkinWidth; } }

        #endregion



        protected virtual void Awake()
        {
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();

            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_CapsuleCollider == null)
                m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_Layers = GetComponent<LayerManager>();

            m_GameObject = gameObject;
            m_Transform = transform;

            m_DeltaTime = Time.deltaTime;

            if (m_Layers == null)
                m_Layers = m_GameObject.AddComponent<LayerManager>();

            Gravity = Physics.gravity;

            deltaTime = Time.deltaTime;
            fixedDeltaTime = Time.fixedDeltaTime;
        }



        protected virtual void OnEnable()
        {
            m_CapsuleCollider.enabled = true;
        }


        protected virtual void OnDisable()
        {
            m_CapsuleCollider.enabled = false;
        }


        protected void Start()
        {
            m_LookRotation = m_Transform.rotation;
            m_PreviousPosition = m_Transform.position;

            m_Rigidbody.mass = m_Mass;
            //m_Rigidbody.useGravity = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenterY = m_CapsuleCollider.center.y;

            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_Animator.applyRootMotion = m_UseRootMotion;


            m_ColliderLayerMask = m_Layers.SolidLayers;
            m_Collisions = new RaycastHit[m_MaxCollisionCount];



            // slides the character through walls and edges
            m_GroundedMovingFrictionMaterial = new PhysicMaterial
            {
                name = "GroundedMovingFrictionMaterial",
                staticFriction = .25f,
                dynamicFriction = .25f,
                frictionCombine = PhysicMaterialCombine.Multiply
            };

            // prevents the collider from slipping on ramps
            m_GroundIdleFrictionMaterial = new PhysicMaterial
            {
                name = "GroundIdleFrictionMaterial",
                staticFriction = 1f,
                dynamicFriction = 1f,
                frictionCombine = PhysicMaterialCombine.Maximum
            };

            // air physics 
            m_AirFrictionMaterial = new PhysicMaterial
            {
                name = "AirFrictionMaterial",
                staticFriction = 0f,
                dynamicFriction = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum
            };
        }


        protected virtual void Update()
        {
            m_TimeScale = Time.timeScale;
            if (Math.Abs(m_TimeScale) < float.Epsilon) return;
            m_DeltaTime = deltaTime;

        }


        protected virtual void FixedUpdate()
        {
            if (Math.Abs(m_TimeScale) < float.Epsilon) return;
            m_DeltaTime = fixedDeltaTime;

            //  Moves the character according to the input.
            if (m_Move) Move();
            //  Perform checks to determine if the character is on the ground.
            if (m_CheckGround) CheckGround();
            //  Ensure the current movement direction is valid.
            if (m_CheckMovement) CheckMovement();
            //  Set the physic material based on the grounded and stepping state
            if (m_SetPhysicsMaterial) SetPhysicsMaterial();


            //  Update the rotation forces.
            if (m_UpdateRotation) UpdateRotation();
            //  Apply any movement.
            if (m_UpdateMovement) UpdateMovement();
            // Update the Animator.
            if (m_Animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
                if (m_UpdateAnimator) UpdateAnimator();

        }


        private void LateUpdate()
        {
            m_Animator.applyRootMotion = m_UseRootMotion;
            m_PreviousPosition = m_Rigidbody.position;


        }


        protected virtual void OnAnimatorMove()
        {
            AnimatorMove();
        }








        /// <summary>
        /// Move charatcer based on input values.
        /// </summary>
        protected virtual void Move()
        {
            if (InputVector.sqrMagnitude > 1)
                InputVector.Normalize();
            RelativeInputVector = m_Transform.TransformDirection(InputVector);

            m_Velocity = RelativeInputVector * m_DeltaTime;
            m_MoveDirection = m_Velocity * m_DeltaTime;
            





            Moving = Mathf.Abs((m_Rigidbody.position - m_PreviousPosition).sqrMagnitude) > 0;
        }






        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            float groundDistance = 10f;
            float checkHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
            Vector3 rayOrigin = m_Transform.position + Vector3.up * checkHeight;

            float rayDist = m_CapsuleCollider.radius + 1;
            //rayDist = 1;
            //  GroundHit Normal is used by the Character footstep.
            //RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers))
            {
                groundDistance = Vector3.Project(m_Transform.position - m_GroundHit.point, m_Transform.up).magnitude;
            }

            if (DebugMode) Debug.DrawRay(rayOrigin, Vector3.down * m_GroundHit.distance, m_Grounded ? Color.green : Color.red);

            // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
            //float smallerRadius = m_CapsuleCollider.radius - (m_SkinWidth * m_SkinWidth);
            float smallerRadius = m_CapsuleCollider.radius * 0.9f;
            Vector3 sphereCastOrigin = m_Transform.position + Vector3.up * m_CapsuleCollider.radius;
            if (Physics.SphereCast(sphereCastOrigin, smallerRadius, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers))
            {
                // check if sphereCast distance is small than the ray cast distance
                if (groundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
                    groundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
            }

            if (DebugMode) DebugDraw.Sphere(sphereCastOrigin + Vector3.down * m_GroundHit.distance, smallerRadius, m_Grounded ? Color.gray : Color.red);



            groundDistance = (float)Math.Round(groundDistance, 2);
            float groundCheckDistance = 0.2f;
            //  If character is grounded, set check distance lower.
            if (m_Rigidbody.velocity.y > -0.001f && m_Rigidbody.velocity.y <= 0f)
                groundCheckDistance *= 0.5f;

            //  Character is grounded.
            if (groundDistance < 0.05f)
            {
                m_GroundAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;
                m_Grounded = true;
            }
            else
            {
                if(groundDistance >= groundCheckDistance)
                {
                    m_GroundAngle = 0;
                    m_Grounded = false;
                }



            }

            if (DebugMode) DebugDraw.DrawMarker(m_GroundHit.point, 0.1f, Color.green);
            m_Animator.SetBool(HashID.Grounded, m_Grounded);


        }


        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {



            m_Moving = InputVector != Vector3.zero;
            if (m_Moving == false) return;


            float direction = Mathf.Clamp(InputVector.z, -1, 1);
            if (direction < 0.01f && direction > 0.01f) direction = 1;
            float rayLength = 1f + m_SkinWidth;
            float dstBetweenRays = 0.4f;

            float colliderRadius = m_CapsuleCollider.radius - m_SkinWidth;
            float colliderHeight = m_CapsuleCollider.height - (colliderRadius * 2);
            int horizontalRayCount = mDetectHorizontalCollision ? Mathf.RoundToInt(colliderHeight / dstBetweenRays) : 3;
            float horizontalRaySpacing = colliderHeight / (horizontalRayCount - 1);


            bool hitDetected = false;
            RaycastHit hit;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector3 rayOrigin = m_Rigidbody.position;
                rayOrigin += Vector3.up * colliderRadius;
                rayOrigin += m_Transform.forward * colliderRadius;
                rayOrigin += Vector3.up * (horizontalRaySpacing * i);

                if (Physics.Raycast(rayOrigin, m_Transform.forward * direction, out hit, rayLength, m_ColliderLayerMask))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (i == 0 && slopeAngle <= m_SlopeLimit)
                    {
                        //float distanceToSlope = 0;
                        //if (Math.Abs(slopeAngle - m_GroundAngle) > float.Epsilon)
                        //{
                        //    distanceToSlope = hit.distance - colliderRadius;
                        //}

                        //Vector3 slopeDirection = Vector3.Cross(m_Transform.right, hit.normal).normalized;
                        ////slopeDirection = slopeDirection - slopeDirection * distanceToSlope;
                        //Vector3 targetVelocity = m_Velocity;
                        //targetVelocity = Vector3.Project(targetVelocity, slopeDirection);

                        ////if (DebugMode) Debug.DrawRay(rayOrigin, targetVelocity, Color.blue);
                        ////if (DebugMode) DebugDraw.Arrow(rayOrigin, targetVelocity, Color.blue);
                        //m_Velocity = targetVelocity;
                    }

                    if (slopeAngle > m_SlopeLimit)
                    {
                        rayLength = hit.distance;
                        m_Moving = false;
                    }
                    hitDetected = true;

                    //m_Velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.x) * Mathf.Sign(m_Velocity.x);
                    //m_Velocity.z = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.z) * Mathf.Sign(m_Velocity.z);
                }


                if (DebugMode) Debug.DrawRay(rayOrigin, m_Transform.forward * direction * rayLength, hitDetected == true ? Color.blue : Color.grey);
            }


            //VerticalCollisions();
        }


        private Vector3 GetAverageRaycast(float offsetX, float offsetZ, int rayCount = 2)
        {
            int maxRays = 4;
            offsetX *= 2;
            offsetZ *= 2;
            rayCount = Mathf.Clamp(rayCount, 2, maxRays);
            int totalRays = rayCount * rayCount + 1;
            Vector3[] combinedCast = new Vector3[totalRays];
            int average = 0;
            Vector3 rayOrigin = m_Transform.TransformPoint(0 - offsetX * 0.5f, m_MaxStepHeight + m_SkinWidth, 0 - offsetZ * 0.5f);
            float rayLength = m_MaxStepHeight * 2;


            float xSpacing = offsetX / (rayCount - 1);
            float zSpacing = offsetZ / (rayCount - 1);

            bool raycastHit = false;
            Vector3 hitPoint = Vector3.zero;
            Vector3 raycast = m_Transform.TransformPoint(0, m_MaxStepHeight, 0);

            if (m_DebugCollisions) Debug.DrawRay(raycast, RelativeInputVector.normalized, Color.blue);

            RaycastHit hit;
            int index = 0;
            for (int z = 0; z < rayCount; z++)
            {
                for (int x = 0; x < rayCount; x++)
                {
                    raycastHit = false;
                    hitPoint = Vector3.zero;
                    raycast = rayOrigin + (m_Transform.forward * zSpacing * z) + (m_Transform.right * xSpacing * x);
                    //raycast += RelativeInputVector.normalized * Time.deltaTime;
                    if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_Layers.SolidLayers))
                    {
                        hitPoint = hit.point;
                        average++;
                        raycastHit = true;
                    }
                    combinedCast[index] = hitPoint;
                    index++;
                    if (m_DebugCollisions) Debug.DrawRay(raycast, Vector3.down * rayLength, (raycastHit ? Color.green : Color.red));
                }
            }

            
            hitPoint = Vector3.zero;
            raycastHit = false;
            raycast = m_Transform.TransformPoint(0, m_MaxStepHeight, 0);
            //originRaycast += RelativeInputVector.normalized * Time.deltaTime;
            if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_Layers.SolidLayers))
            {
                hitPoint = hit.point;
                average++;
                raycastHit = true;
            }

            combinedCast[totalRays - 1] = hitPoint;
            if (m_DebugCollisions) DebugDraw.Circle(raycast, Vector3.up * rayLength, 0.2f, (raycastHit ? Color.blue : Color.red));



            average = Mathf.Clamp(average, 1, int.MaxValue);

            Vector3 averageHitPosition = Vector3.zero;
            float xTotal = 0f, yTotal = 0f, zTotal = 0f;
            for (int i = 0; i < combinedCast.Length; i++)
            {
                xTotal += combinedCast[i].x;
                yTotal += combinedCast[i].y;
                zTotal += combinedCast[i].z;
            }
            averageHitPosition.Set(xTotal / average, yTotal / average, zTotal / average);

            if (m_DebugCollisions) DebugDraw.DrawMarker(averageHitPosition, 0.2f, Color.blue);

            return averageHitPosition;
        }





        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            Vector3 lookDirection = m_LookRotation * m_Transform.forward;
            Vector3 axisSign = Vector3.Cross(lookDirection, m_Transform.forward);
            m_DeltaYRotation = Vector3.Angle(m_Transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f) * m_DeltaTime;
            m_DeltaYRotation = (float)Math.Round(m_DeltaYRotation, 2);

            Vector3 localDir = m_Transform.InverseTransformDirection(lookDirection);
            float targetAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
            if (Moving == false)
            {
                targetAngle = Mathf.Atan2(InputVector.x, Mathf.Abs(InputVector.z)) * Mathf.Rad2Deg;
                targetAngle *= InputVector.x * m_IdleRotationMultiplier;
                //targetAngle += m_Transform.eulerAngles.y;
            }




            if (InputVector == Vector3.zero)
                targetAngle *= (1.01f - (Mathf.Abs(targetAngle) / 180));
            targetAngle = (float)Math.Round(targetAngle, 2);
            CharacterDebug.Log("targetAngle", targetAngle);

            targetAngle *= m_RotationSpeed * m_DeltaTime;
            Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, m_Transform.up) * m_Transform.rotation;
            m_Rigidbody.MoveRotation(targetRotation.normalized);


            //if (m_UseRootMotion)
            //{
            //    float angleInDegrees;
            //    Vector3 rotationAxis;
            //    m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
            //    Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
            //    m_Rigidbody.angularVelocity = angularDisplacement;

            //    CharacterDebug.Log("angularDisplacement", angularDisplacement);
            //}


        }

        public Vector3 ComputeTorque(Quaternion desiredRotation)
        {
            //q will rotate from our current rotation to desired rotation
            Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
            //convert to angle axis representation so we can do math with angular velocity
            Vector3 axis;
            float axisMagnitude;
            q.ToAngleAxis(out axisMagnitude, out axis);
            axis.Normalize();
            //w is the angular velocity we need to achieve
            Vector3 targetAngularVelocity = axis * axisMagnitude * Mathf.Deg2Rad / Time.fixedDeltaTime;
            targetAngularVelocity -= m_Rigidbody.angularVelocity;
            //to multiply with inertia tensor local then rotationTensor coords
            Vector3 wl = transform.InverseTransformDirection(targetAngularVelocity);
            Vector3 Tl;
            Vector3 wll = wl;
            wll = m_Rigidbody.inertiaTensorRotation * wll;
            wll.Scale(m_Rigidbody.inertiaTensor);
            Tl = Quaternion.Inverse(m_Rigidbody.inertiaTensorRotation) * wll;
            Vector3 T = transform.TransformDirection(Tl);
            return T;
        }


        /// <summary>
        /// Apply any movement.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            if (m_UseRootMotion)
            {
                m_Velocity = RootMotionVelocity / m_DeltaTime;
                m_Velocity.y = Grounded ? 0 : m_Rigidbody.velocity.y;

                RootMotionVelocity = Vector3.zero;
            }
            else
            {
                Vector3 targetVelocity = m_Velocity * (Grounded ? m_GroundSpeed : m_AirborneSpeed);
                float acceleration = Moving ? m_GroundAcceleration : m_MotorDamping;
                if (Grounded == false) acceleration = Moving ? m_AirborneAcceleration : m_AirborneDamping;
                m_Velocity = Vector3.SmoothDamp(m_Velocity, targetVelocity, ref m_VelocitySmooth, acceleration);
            }


            if (m_Grounded)
            {
                m_Velocity += (Gravity * m_GravityModifier) * m_DeltaTime;
                m_Velocity = Vector3.ProjectOnPlane(m_Velocity, m_GroundHit.normal * m_GroundStickiness);
            }
            else
            {
                Vector3 verticalVelocity = (m_Rigidbody.position - m_PreviousPosition) * m_DeltaTime;
                verticalVelocity = Vector3.Project(verticalVelocity, Gravity);
                m_Velocity += verticalVelocity;
            }


            m_Rigidbody.velocity = m_Velocity;

            DebugDraw.Arrow(RaycastOrigin, m_Rigidbody.velocity, Color.green);

            ////  Add extrernal forces
            //if (m_ExternalForce.sqrMagnitude > 0.2f)
            //{
            //    m_Velocity += m_ExternalForce;
            //    //m_Rigidbody.AddForce(m_ExternalForce, ForceMode.Impulse);
            //}
            ////  Smooth out external force.
            //m_ExternalForce = Vector3.SmoothDamp(m_ExternalForce, Vector3.zero, ref m_ExternalForceSmooth, m_ExternalForceDamping);


            if (Grounded)
            {
                float offset = m_CapsuleCollider.radius + m_SkinWidth;
                Vector3 groundAverage = GetAverageRaycast(offset, offset, 2);
                if (groundAverage != m_Rigidbody.position)
                {
                    m_Rigidbody.MovePosition(new Vector3(m_Transform.position.x, groundAverage.y + 0.1f, m_Transform.position.z));
                }
            }
        }







        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            //m_Animator.SetFloat(HashID.Rotation, (m_DeltaYRotation * Mathf.Deg2Rad));
            //  1 means left foot is up.
            m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
            m_Animator.SetFloat(HashID.Rotation, m_DeltaYRotation);
            m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);
            m_Animator.SetBool(HashID.Moving, m_Moving);

            //  -----------
            //  Does a character action virtual the controllers update animator.
            //  -----------

            //  Movement Input
            m_AnimationMonitor.SetForwardInputValue(InputVector.z);
            m_AnimationMonitor.SetHorizontalInputValue(InputVector.x);
        }



        protected virtual void AnimatorMove()
        {
            if (m_UseRootMotion)
            {
                RootMotionVelocity = m_Animator.deltaPosition * m_RootMotionSpeedMultiplier;

                float angleInDegrees;
                Vector3 rotationAxis;
                m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
                angleInDegrees = (angleInDegrees * m_RootMotionSpeedMultiplier * Mathf.Deg2Rad) / m_DeltaTime;
                RootMotionRotation = Quaternion.AngleAxis(angleInDegrees, rotationAxis);
            }



        }



        protected virtual void SetPhysicsMaterial()
        {
            //change the physics material to very slip when not grounded or maxFriction when is
            if(!m_Grounded && Mathf.Abs(m_Rigidbody.velocity.y) > 0)
                m_CapsuleCollider.material = m_AirFrictionMaterial;

            else if (m_Grounded && m_Moving)
                m_CapsuleCollider.material = m_GroundedMovingFrictionMaterial;

            else if (m_Grounded && !m_Moving)
                m_CapsuleCollider.material = m_GroundIdleFrictionMaterial;

            else
                m_CapsuleCollider.material = m_GroundIdleFrictionMaterial;
        }


        // Scale the capsule collider to 'mlp' of the initial value
        protected void ScaleCapsule(float mlp)
        {
            //if (capsule.height != originalHeight * mlp)
            //{
            //    capsule.height = Mathf.MoveTowards(capsule.height, originalHeight * mlp, Time.deltaTime * 4);
            //    capsule.center = Vector3.MoveTowards(capsule.center, originalCenter * mlp, Time.deltaTime * 2);
            //}
        }

        public virtual float GetColliderHeightAdjustment()
        {
            return m_CapsuleCollider.height;
        }


        #region Public Functions


        public void SetMovementType(MovementType movementType)
        {
            m_MovementType = movementType;
        }


        //  If true, the character looks independently of the camera.  AI Agents do not need to use camera rotation.
        public bool IndependentLook()
        {
            if (m_Moving || m_Aiming)
            {
                return false;
            }
            //if(m_Aiming) return true;
            return true;
        }



        public virtual void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            Vector3 inputVector = Vector3.zero;
            switch (m_MovementType)
            {
                case (MovementType.Adventure):

                    inputVector.x = Mathf.Clamp(horizontalMovement, -1, 1);
                    inputVector.y = 0;
                    inputVector.z = Mathf.Clamp(forwardMovement, -1, 1);

                    m_DeltaYRotation = Mathf.Atan2(inputVector.x, inputVector.z);
                    //m_Velocity.x = turnAmount;
                    m_LookRotation = lookRotation;
                    break;

                case (MovementType.Combat):

                    inputVector.x = Mathf.Clamp(horizontalMovement, -1, 1);
                    inputVector.y = 0;
                    inputVector.z = Mathf.Clamp(forwardMovement, -1, 1);

                    //float turnAmount = Mathf.Atan2(InputVector.x, InputVector.z);
                    m_LookRotation = lookRotation;
                    break;
            }

            InputVector = inputVector;


            //   ---
        }



        protected void VerticalCollision()
        {
            //  We want the cos angle since we know "adjacent" and "hypotenuse".
            float startAngle = Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg;
            float checkHeight = m_CapsuleCollider.height / 2;
            float rayDistance = 2;
            float verticalAngle = 15;

            Quaternion startRayRotation = Quaternion.AngleAxis(-startAngle, Vector3.up);
            Vector3 startRay = startRayRotation * m_Transform.forward;
            Vector3 startDir = startRay;

            float angleAmount = (startAngle * 2) / (m_VerticalCollisionCount - 1);

            bool detectEdge = false;

            Vector3 raycastOrigin = m_Rigidbody.position + Vector3.up * checkHeight;

            for (int i = 0; i < m_VerticalCollisionCount; i++)
            {
                if (i == 0) continue;
                startDir = Quaternion.AngleAxis(angleAmount, Vector3.up) * startDir;
                Vector3 hitDirection = startDir;

                Quaternion angleRotation = Quaternion.AngleAxis(verticalAngle, m_Transform.right);
                hitDirection = Vector3.ClampMagnitude(angleRotation * hitDirection, rayDistance);


                if (Physics.Raycast(raycastOrigin, hitDirection + Vector3.up * checkHeight, rayDistance, m_ColliderLayerMask) == false)
                {

                    Vector3 start = m_Rigidbody.position + (m_Transform.forward * m_CapsuleCollider.radius);
                    start.y += m_DetectObjectHeight;
                    float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;
                    if (Physics.Raycast(start, Vector3.down, maxDetectEdgeDistance, m_ColliderLayerMask) == false)
                    {
                        detectEdge = true;
                    }
                }

                if (m_DebugCollisions) Debug.DrawRay(raycastOrigin, hitDirection * rayDistance, detectEdge == true ? Color.red : Color.grey);
            }


        }



        public bool DetectEdge()
        {
            if (!m_Grounded)
                return false;


            bool detectEdge = false;
            Vector3 start = m_Rigidbody.position + (m_Transform.forward * m_CapsuleCollider.radius);
            start.y = start.y + m_DetectObjectHeight;
            //start.y = start.y + 0.05f + (Mathf.Tan(m_SlopeAngle) * start.magnitude);

            Vector3 dir = Vector3.down;
            float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;



            if (Physics.Raycast(m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward, 2, m_Layers.SolidLayers) == false)
            {

                if (Physics.Raycast(start, dir, maxDetectEdgeDistance, m_Layers.SolidLayers) == false)
                {
                    detectEdge = true;
                }
            }



            //if (Debug && hitObject == false) Debug.DrawRay(m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward * 2, hitObject ? Color.red : Color.green);
            if (DebugMode) Debug.DrawRay(start, dir * maxDetectEdgeDistance, detectEdge ? Color.green : Color.gray);

            return detectEdge;
        }



        public bool DetectObject(Vector3 dir, out RaycastHit raycastHit, float maxDistance, LayerMask layerMask, int rayCount = 1, float maxAngle = 0)
        {
            bool detectObject = false;
            Vector3 start = m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight);

            if (rayCount < 1) rayCount = 1;

            if (Physics.Raycast(start, dir, out raycastHit, maxDistance, layerMask))
            {
                detectObject = true;
            }

            if (DebugMode) Debug.DrawRay(start, dir * maxDistance, detectObject ? Color.red : Color.green);
            return detectObject;
        }









        public void SetPosition(Vector3 position)
        {
            m_Rigidbody.MovePosition(position);
        }


        public void SetRotation(Quaternion rotation)
        {
            m_Rigidbody.MoveRotation(rotation.normalized);
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Moving = false;
        }


        #endregion










        #region Debugging



        protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);
        protected Color _Magenta = new Color(0.75f, 0, 0.75f, 0.9f);


        protected virtual void DebugAttributes()
        {
            CharacterDebug.Log("Moving", Moving);
            CharacterDebug.Log("Grounded", Grounded);

            CharacterDebug.Log("m_DeltaYRotation", m_DeltaYRotation);
            CharacterDebug.Log("m_SlopeAngle", m_SlopeAngle);
            CharacterDebug.Log("m_Velocity", m_Velocity);

            //CharacterDebug.Log("rb_AngularVelocity", m_Rigidbody.angularVelocity);
            //CharacterDebug.Log("rb_Velocity", m_Rigidbody.velocity.y);
        }



        protected virtual void DrawGizmos()
        {

        }


        protected virtual void DrawOnGUI()
        {
            DebugAttributes();

            GUI.color = CharacterControllerUtility.DebugTextColor;
            Rect rect = CharacterControllerUtility.CharacterControllerRect;
            GUI.BeginGroup(rect, GUI.skin.box);

            GUI.Label(rect, CharacterDebug.Write());

            GUI.EndGroup();
        }


        protected void OnGUI()
        {
            if (Application.isPlaying && DebugMode && Time.timeScale != 0)
            {
                DrawOnGUI();
            }

        }

        protected void OnDrawGizmos()
        {
            if (DebugMode && Application.isPlaying)
            {
                DrawGizmos();
            }

        }


        #endregion





        //private void DescendSlope(Vector3 velocity)
        //{
        //    float moveDirection = Mathf.Sign(InputVector.z);
        //    Vector3 rayOrigin = m_Transform.position + Vector3.up * checkHeight;
        //    rayOrigin += moveDirection * m_Transform.forward * (m_CapsuleCollider - m_SkinWidth);
        //    float rayLength = 2f + m_SkinWidth;
        //    RaycastHit hit;
        //    if(Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength, m_Layers.SolidLayers))
        //    {
        //        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        //        float maxDescendAngle = 65f;
        //        if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
        //        {
        //            if(MAthf.Sign(hit.normal.z) == moveDirection)
        //            {
        //                if(hit.distance - m_SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.z))
        //                {
        //                    float moveDistance = Mathf.Abs(m_Velocity.z);
        //                    float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        //                    m_Velocity.x = Mathf.Cos(slopeAngle * MAthf.Deg2Rad) * moveDistance * Mathf.Sign(m_Velocity.z);
        //                    m_velocity.y -= descendVelocityY;
        //                    m_SlopeAngle = slopeAngle;
        //                }
        //            }
        //        }
        //    }
        //}

    }

}
