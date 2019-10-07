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
        protected const float k_collisionOffset = 0.001f;
        protected const float k_groundedDistance = 0.001f;
        protected const float k_wallAngle = 85f;
        protected readonly float k_gravity = -9.81f;

        public enum MovementTypes { Adventure, Explorer, Combat };

        #region Parameters
        [Serializable]
        public class MovementSettings
        {

            [Header("Root Motion Settings")]
            [Tooltip("Should root motion be used to move the character.")]
            public bool useRootMotionPosition;
            [Tooltip("If using root motion, applies a multiplier to the delta position.")]
            public float rootMotionSpeedMultiplier = 1;
            [Tooltip("Should root motion be used to rotate the character.")]
            public bool useRootMotionRotation;
            [Tooltip("If using root motion, applies a multiplier to the delta rotation.")]
            public float rootMotionRotationMultiplier = 1;
            // ----- 
            [Header("Ground Movement Settings")]
            [Tooltip("Rate at which the character accelerates while grounded."), Range(0.01f, 1)]
            public float groundAcceleration = 0.18f;
            [Tooltip("Max speed the character moves while grounded.")]
            public float groundSpeed = 4f;
            [Tooltip("Rate at which the character decelerates while on the ground."), Range(0.01f, 1)]
            public float groundDamping = 0.3f;
            [Tooltip("Multiplier to ground speed when moving backwards."), Range(0, 2)]
            public float backwardsMultiplier = 0.7f;
            [Tooltip("Max speed when grounded.")]
            public float groundMaxSpeed = 8f;
            // ----- 
            [Header("Airborne Settings")]
            [Tooltip("Rate at which the character accelerates while airborne."), Range(0.01f, 1)]
            public float airborneAcceleration = 0.12f;
            [Tooltip("Max speed the character moves while airborne.")]
            public float airborneSpeed = 1f;
            [Tooltip("Rate at which the character decelerates while on the airborne."), Range(0.01f, 1)]
            public float airborneDamping = 0.3f;
            [Tooltip("Max forward speed when airborne.")]
            public float airborneMaxSpeed = 2f;
            // ----- 
            [Header("Rotation Settings")]
            [Tooltip("Rotation speed when in motion.")]
            public float rotationSpeed = 120f;
            [Tooltip("Rotation speed when standing idle.")]
            public float idleRotationSpeed = 180f;

        }

        [Serializable]
        public class PhysicsSettings
        {
            [Tooltip("The mass of the character.  Is used to calculate how much force to add when colliding with another rigidbody.")]
            public float mass = 100;
            [Tooltip("NotYetImplemented")]
            public float skinWidth = 0.08f;
            [Tooltip("NotYetImplemented")]
            public float maxSlopeAngle = 45f;
            [Tooltip("NotYetImplemented")]
            public float maxStepHeight = 0.4f;
            [Tooltip("NotYetImplemented"), Range(1, 4)]
            public float gravityModifier = 1.5f;
            [Tooltip("Maximum downward velocity the character can reach when falling.")]
            public float terminalVelocity = 10;
            [Tooltip("NotYetImplemented")]
            public float groundStickiness = 2f;
            [Serializable]
            public class ColliderSettings{
                [Tooltip("Height of the character."), Min(0)]
                public float colliderHeight = 2f;
                [Tooltip("Radius of the character collider."), Min(0)]
                public float colliderRadius = 0.4f;
                [Tooltip("NotYetImplemented")]
                public PhysicMaterial physicMaterial;
                [Tooltip("Colliders to ignore when determing collision detection.")]
                public Collider[] ignoreColliders = new Collider[0];
            }
            public ColliderSettings colliderSettings = new ColliderSettings();
        }

        [Serializable]
        public class CollisionSettings
        {
            [Tooltip("NotYetImplemented")]
            public LayerMask collisionsMask;
            [Tooltip("Should character check for collisions in horizontal space.")]
            public bool detectHorizontalCollisions = true;
            [Tooltip("Should character check for collisions in vertical space.")]
            public bool detectVerticalCollisions = false;
            [Tooltip("NotYetImplemented")]
            public int maxCollisionsCount = 64;
            [Tooltip("The maximum number of iterations to detect collision overlaps.")]
            public int maxOverlapIterations = 20;
        }

        [Serializable]
        public class AdvanceSettings{
            [Tooltip("<Not Implemented>")]
            public QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            [Tooltip("<Not Implemented>"), Range(0, 4)]
            public float timeScale = 1;
        }

        [Serializable]
        public class DebugSettings
        {
            public bool showMotionVectors;
            public bool showGroundCheck;
            public bool showCollisions;
        }


        [SerializeField]
        protected MovementSettings m_motor = new MovementSettings();
        [SerializeField]
        protected PhysicsSettings m_physics = new PhysicsSettings();
        [SerializeField]
        protected CollisionSettings m_collision = new CollisionSettings();
        [SerializeField]
        protected AdvanceSettings m_advance = new AdvanceSettings();
        [SerializeField]
        protected DebugSettings m_debug = new DebugSettings();


        protected float m_stickyForce;


        public float moveSpeed { get => Vector3.Dot(m_moveDirection, m_moveDirection.normalized); }

        public float maxSpeed { get => m_grounded ? m_motor.groundMaxSpeed : m_motor.airborneMaxSpeed; }

        public Vector3 transformFwd { get => m_transform.forward; }

        public Vector3 transformUp { get => m_transform.up; }

        public Vector3 transformDown { get => -m_transform.up; }


        #endregion


        public class GroundInfo
        {
            public bool grounded;
            public Vector3 point;
            public Vector3 normal;
            public float angle;

            public bool onLedge;

            public GroundInfo()
            {
                point = Vector3.zero;
                normal = Vector3.zero;
                angle = 0;
            }

        }


        #region Inspector properties



        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();


        // --- Kinematic




        #endregion

        protected float m_moveSpeed;
        protected Vector3 m_velocity;
        protected PhysicMaterial m_physicsMaterial;
        protected Collider[] m_probedColliders;
        protected CapsuleCollider m_collider;
        [SerializeField, HideInInspector]
        protected MovementTypes m_MovementType = MovementTypes.Adventure;

        protected Vector3 m_currentPosition { get { return m_transform.position; } }

        protected bool m_moving, m_grounded = true;

        protected float m_targetAngle, m_viewAngle;
        protected Vector3 m_inputVector, m_inputDirection;
        protected Vector3 m_moveDirection;
        protected Vector3 m_movePosition;
        protected Quaternion m_targetRotation = Quaternion.identity, m_moveRotation = Quaternion.identity, m_lookRotation = Quaternion.identity;

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






        protected float m_previousAngle;
        protected Vector3 m_previousPosition;

        protected Quaternion m_previousRotation;

        protected float m_currentVerticalVelocity;
        protected float m_inputMagnitude;
        protected float m_forwardSpeed;
        protected float m_lateralSpeed;
        protected float m_currentSpeed;

        public float gravity { get => k_gravity * m_physics.gravityModifier; }
        public float fallTime { get; protected set; }
        public GroundInfo groundInfo = new GroundInfo();


        protected Vector3 m_verticalVelocity;
        


        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_layerManager;
        protected Rigidbody m_rigidbody;
        protected GameObject m_gameObject;
        protected Transform m_transform;
        protected float m_deltaTime;





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

        public float Speed { get { return Mathf.Abs(m_currentSpeed); } set { m_currentSpeed = Mathf.Abs(value); } }

        public float RotationSpeed { get { return m_motor.rotationSpeed; } set { m_motor.rotationSpeed = value; } }

        public bool UseRootMotion { get { return m_motor.useRootMotionPosition; } set { m_motor.useRootMotionPosition = value; } }



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
            m_animator = GetComponent<Animator>();
            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.applyRootMotion = true;

            //  Rigidbody settings
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.mass = m_physics.mass;
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


            //  Collider settings
            m_collider = GetComponent<CapsuleCollider>();
            if (m_collider == null) {
                m_collider = gameObject.AddComponent<CapsuleCollider>();
                m_collider.radius = 0.36f;
                m_collider.height = MathUtil.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2);
                m_collider.center = new Vector3(0, m_collider.height / 2, 0);
            }
            //  Colider properties.
            m_colliderCenter = m_collider.center;
            m_colliderHeight = m_collider.height * m_transform.lossyScale.x;
            m_colliderRadius = m_collider.radius * m_transform.lossyScale.x;

            //  Collision settings
            m_layerManager = GetComponent<LayerManager>();
            m_collision.collisionsMask = m_layerManager.SolidLayers;
            m_probedColliders = new Collider[m_collision.maxCollisionsCount];

            //  Cached variables.
            m_lookDirection = m_transform.forward;
            m_lookRotation = Quaternion.LookRotation(m_transform.forward);
            m_previousPosition = m_transform.position;
            m_velocity = Vector3.zero;


            m_previousRotation = m_transform.rotation;


            if (m_rigidbody.isKinematic) Debug.LogFormat("<b><color=yellow>[{0}]</color> is kinematic. </b>", this.gameObject.name);
        }



        protected virtual void AnimatorMove()
        {
            m_animator.applyRootMotion = true;



            if (m_motor.useRootMotionPosition) {
                //m_previousRotation *= m_animator.deltaRotation;

                m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = (angle * m_motor.rootMotionRotationMultiplier * Mathf.Deg2Rad) / m_deltaTime;
                m_deltaRotation = Quaternion.AngleAxis(angle, axis);
            }


        }



        protected virtual void InternalMove()
        {
            if (m_inputVector.sqrMagnitude > 1)
                m_inputVector.Normalize();


            //m_velocity = m_rigidbody.velocity;
            m_deltaRotation = m_transform.rotation * Quaternion.Inverse(m_previousRotation);
            m_previousRotation = m_transform.rotation;
            m_deltaRotation.ToAngleAxis(out float deltaAngle1, out Vector3 axis);
            DebugUI.Log(this, "deltaPosition", m_animator.deltaPosition, RichTextColor.Orange);



            m_lookDirection = m_lookRotation * m_transform.forward;
            m_previousAngle = m_viewAngle;
            m_viewAngle = m_transform.AngleFromForward(m_lookDirection);

            if (!m_grounded) fallTime += m_deltaTime;
            else fallTime = 0;
        }



        /// <summary>
        /// Apply rotation.
        /// </summary>
        protected virtual void ApplyRotation()
        {
            //m_targetRotation = Quaternion.Slerp(m_transform.rotation, m_targetRotation, 1 - Mathf.Exp(-m_motor.rotationSpeed) * m_deltaTime);
            //m_targetRotation = Quaternion.Slerp(m_transform.rotation, m_targetRotation, m_motor.rotationSpeed * m_deltaTime);
            m_rigidbody.MoveRotation(m_targetRotation * m_rigidbody.rotation);
        }


        /// <summary>
        /// Apply position values.
        /// </summary>
        protected virtual void ApplyMovement()
        {
            DebugUI.Log(this, "m_moveDirection", m_moveDirection, RichTextColor.Orange);

            if (!m_rigidbody.isKinematic) {
                m_velocity = m_moveDirection;

                m_rigidbody.velocity = m_velocity;

                return;
            }

            m_velocity = m_moveDirection;
            m_movePosition = m_rigidbody.position + m_moveDirection;
            m_rigidbody.MovePosition(m_movePosition);

            if (GetPenetrationInfo(m_rigidbody.position, out float penetrationDistance, out Vector3 penetrationDirection)) {
                m_rigidbody.MovePosition(m_rigidbody.position + penetrationDirection * penetrationDistance);
            }
        }


        protected Vector3 GetMovementVector()
        {
            if (m_motor.useRootMotionPosition) {
                return (m_animator.deltaPosition * m_motor.rootMotionSpeedMultiplier) / m_deltaTime;
            }

            m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_motor.groundSpeed, m_motor.groundAcceleration.Squared());
            return m_inputDirection * m_moveSpeed * m_inputMagnitude;
        }


        protected virtual void Move()
        {

            m_inputMagnitude = m_inputVector.magnitude;
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType)
            {
                case (MovementTypes.Adventure):
                    //  Get the correct input direction.
                    m_inputDirection =  m_transform.forward * m_inputMagnitude;

                    //  Set forward and lateral speeds.
                    m_forwardSpeed = m_inputMagnitude;
                    m_lateralSpeed = 0;

                    //  Get the correct target rotation angle.
                    m_targetAngle = m_transform.AngleFromForward(m_lookDirection);
                    //Vector3 local = Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(m_inputVector);
                    //m_targetAngle = m_transform.AngleFromForward(local);
                    break;
                case (MovementTypes.Combat):
                    //  Get the correct input direction.
                    m_inputDirection = m_transform.TransformDirection(m_inputVector);

                    //  Set forward and lateral speeds.
                    m_forwardSpeed = m_inputVector.z;
                    m_lateralSpeed = m_inputVector.x;

                    //  Get the correct target rotation angle.
                    m_targetAngle = m_transform.AngleFromForward(m_lookDirection);
                    break;
            }
            //DebugUI.Log(this, "m_inputDirection1", m_inputDirection, RichTextColor.Orange);
            //DebugUI.Log(this, "m_inputDirection2", Quaternion.Inverse(m_transform.rotation) *  m_transform.TransformDirection(m_inputVector), RichTextColor.Orange);

            m_moving = m_inputMagnitude > 0;
            //m_moveDirection = m_inputDirection * m_motor.groundSpeed * m_inputMagnitude;
            m_moveDirection = GetMovementVector() + m_velocity;
            //  Calculate move vector
            Vector3 drag = m_moveDirection * MathUtil.SmoothStop3(m_motor.groundDamping);
            if (m_inputDirection.sqrMagnitude > 0)
                drag *= (1 - Vector3.Dot(m_inputDirection.normalized, m_moveDirection.normalized));
            if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
                m_moveDirection = Vector3.zero;
            else
                m_moveDirection -= drag;

            m_moveDirection += m_inputDirection * (m_motor.groundAcceleration * m_deltaTime);
            if (m_moveDirection.sqrMagnitude > m_motor.groundSpeed * m_motor.groundSpeed)
                m_moveDirection = m_moveDirection.normalized * m_motor.groundSpeed;


            m_currentSpeed = m_moveDirection.magnitude;
            //  Is there enough movement to be considered moving.





            //if(m_inputMagnitude > 0)
            //    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_motor.rotationSpeed * m_deltaTime, m_transform.up);
            //else
            //    m_targetRotation = m_transform.rotation;
            m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_motor.rotationSpeed * m_deltaTime, m_transform.up);


            DebugUI.Log(this, "m_targetAngle", m_targetAngle, RichTextColor.Cyan);
            DebugUI.Log(this, "m_inputDirection", m_inputDirection, RichTextColor.Cyan);

            //if (!m_moving) {
            //    Vector3 drag = m_moveDirection * MathUtil.SmoothStop3(m_motor.groundDamping);
            //    if (m_inputDirection.sqrMagnitude > 0)
            //        drag *= (1 - Vector3.Dot(m_inputDirection.normalized, m_moveDirection.normalized));
            //    if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
            //        m_moveDirection = Vector3.zero;
            //    else
            //        m_moveDirection -= drag;
            //}
        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            m_stickyForce = 0;

            float distance = m_spherecastRadius + m_physics.skinWidth;
            RaycastHit groundHit;
            bool didCollide = false;

            if (SphereGroundCast(m_currentPosition, Vector3.down, m_spherecastRadius, distance, out groundHit, true, null, DebugGroundCheck, true)) {
                groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                didCollide = true;
            }

            if (!didCollide) {
                if (SphereGroundCast(m_currentPosition, Vector3.down, m_colliderRadius, distance + k_groundedDistance, out groundHit, true, null, DebugGroundCheck, true)) {
                    groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                    didCollide = true;
                }
            }

            if (didCollide) {
                float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                m_grounded = true;
                ////  Check if cast is hitting against a wall.
                //if (groundAngle > k_wallAngle) {
                //    if (SphereGroundCast(m_currentPosition, Vector3.down, m_spherecastRadius, m_colliderHeight * 0.5f + k_collisionOffset, out groundHit, true, m_colliderCenter, DebugGroundCheck, false)) {
                //        groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                //    }
                //    else {
                //        m_grounded = false;
                //    }
                //}
                ////  Check if on a ledge.



                float fwdVelocity = Vector3.ProjectOnPlane(m_moveDirection, Vector3.up * gravity).magnitude;
                m_stickyForce = groundHit.distance * m_physics.groundStickiness * fwdVelocity;
                //m_moveDirection = m_moveDirection - transformUp * stickyForce * m_deltaTime;
                UpdateGroundInfo(m_grounded, groundAngle, groundHit.point, groundHit.normal);
            }
            else {
                m_grounded = false;

                m_verticalVelocity = Vector3.zero;
                m_currentVerticalVelocity = 0;

                UpdateGroundInfo(false);
            }

        }





        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            if (m_grounded) {

                ////  If greater than 0, than we are going down a slope.
                //if (Vector3.Dot(m_moveDirection, groundInfo.normal) >= 0) {

                //}
                ////  We are going up the slope if it is negative.
                //else {

                //}

                //m_moveDirection = GetDirectionTangentToSurface(m_moveDirection, groundInfo.normal);
                //// Reorient target velocity.
                //Vector3 inputRight = Vector3.Cross(m_moveDirection, m_transform.up);
                //Vector3 targetMovementVelocity = Vector3.Cross(groundInfo.normal, inputRight).normalized * m_currentSpeed;// m_moveDirection.magnitude;
                //m_moveDirection = Vector3.Lerp(m_moveDirection, targetMovementVelocity * m_deltaTime, 1f - Mathf.Exp(-5 * m_deltaTime));

                //m_moveDirection -= m_transform.up * m_stickyForce * m_deltaTime;
            }
            else {
                //Vector3 verticalVelocity = Vector3.Project(m_velocity, m_gravity);
                m_currentVerticalVelocity += Mathf.Min(gravity * fallTime, m_physics.terminalVelocity);


                m_moveDirection.y += m_currentVerticalVelocity;
            }




        }



        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {

            m_moveRotation = Quaternion.AngleAxis(m_targetAngle * m_motor.rotationSpeed * m_deltaTime, m_transform.up);

            //if(m_lookDirection.sqrMagnitude > 0) {
            //    m_targetAngle = Mathf.Lerp(m_targetAngle, m_viewAngle, 1 - Mathf.Exp(-m_motor.rotationSpeed * m_deltaTime));

            //    //m_targetRotation = Quaternion.LookRotation(m_inputDirection, Vector3.up);

            //    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_motor.rotationSpeed * m_deltaTime, m_transform.up);
            //}


            //m_targetAngle = m_transform.AngleFromForward(m_lookDirection);
            //m_targetAngle = Vector3.SignedAngle(m_smoothLookDirection, m_lookDirection, m_transform.up);

            //m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_deltaTime, m_transform.up);
            //float t = m_motor.rotationSpeed * m_deltaTime;
            //m_targetRotation = Quaternion.LookRotation(m_smoothLookDirection, m_transform.up);
        }



        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {

            var velocity = m_moveDirection * m_deltaTime;
            if (m_collision.detectHorizontalCollisions && velocity != Vector3.zero) {
                if(DetectHorizontalCollisions(ref velocity, groundInfo.angle, true)){
                    if (GetPenetrationInfo(m_rigidbody.position, out float penetrationDistance, out Vector3 penetrationDirection)) {
                        velocity += penetrationDirection * penetrationDistance;
                    }
                }

            }

            //if (m_collision.detectVerticalCollisions) {
            //    collisionDetected = DetectHorizontalCollisions(ref m_moveDirection, groundInfo.angle, out hit, true);
            //}



            m_moveDirection = velocity;
        }




        private bool GetPenetrationInfo(Vector3 currentPosition, out float getDistance, out Vector3 getDirection, bool useSkinWidth = true, Vector3? offsetPosition = null)
        {
            getDistance = 0;
            getDirection = Vector3.zero;



            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            var origin = currentPosition.WithY(m_colliderCenter.y);
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var p1 = GetBottomCapsulePoint(origin, offset);
            var p2 = GetTopCapsulePoint(origin, offset);
            var radius = m_colliderRadius + skinWidth;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(p1, p2, radius, m_probedColliders, GetCollisionMask(), m_advance.queryTrigger);

            Vector3 localPosition = Vector3.zero;
            bool overlap = false;
            if (overlapCount > 0)  //   || m_probedColliders.Length >= 0
            {
                for (int i = 0; i < overlapCount; i++) {
                    Collider collision = m_probedColliders[i];
                    if (collision == m_collider) { continue; }
                    if (collision == null) { break; }

                    Vector3 direction;
                    float distance;
                    Transform colliderTransform = collision.transform;

                    if (ComputePenetration(currentPosition, offset,
                                            collision, colliderTransform.position, colliderTransform.rotation,
                                            out direction, out distance)) {
                        localPosition += direction * (distance + k_collisionOffset);
                        overlap = true;
                    }
                }
            }

            //if (overlap) m_moveDirection += localPosition.normalized;
            if (overlap) {
                getDistance = localPosition.magnitude;
                getDirection = localPosition.normalized;
            }
            //m_rigidbody.MovePosition(m_rigidbody.position + localPosition.normalized);

            return overlap;
        }







        protected bool DetectHorizontalCollisions(ref Vector3 moveDirection, float groundAngle, bool useSkinWidth = true)
        {
            bool climbingSlope = false;
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            float distanceOffset = m_colliderRadius + skinWidth;
            float distance = Vector3.Dot(moveDirection, moveDirection.normalized);

            RaycastHit hit;
            bool hitDetected = CapsuleCast(m_currentPosition, moveDirection, distance, out hit, useSkinWidth, DebugCollisions);

            if (hitDetected)
            {
                DebugDrawer.DrawPoint(hit.point, Color.green);
                //  Climb Slope.
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if(slopeAngle <= m_physics.maxSlopeAngle)
                {
                    Vector3 vectorToHitPoint = Vector3.zero;
                    if(Math.Abs(slopeAngle - groundAngle) > float.Epsilon)
                    {
                        //  Get the adjusted distance to the hit point
                        float distanceToSlopeStart = hit.distance - distanceOffset;
                        //  Set the direction vector to the hitPoint.
                        vectorToHitPoint = moveDirection.normalized * distanceToSlopeStart;
                        //  Adjust the move direction to account for accending slope.
                        moveDirection -= vectorToHitPoint;
                    }
                    //  Climb slope.
                    climbingSlope = ClimbSlope(ref moveDirection, distance, slopeAngle);
                    //  Add the slope start distance that was previous subtracted.
                    moveDirection += vectorToHitPoint;
                }

                if (slopeAngle > m_physics.maxSlopeAngle) {
                    distance = (hit.distance - distanceOffset) * m_currentSpeed;
                    if (climbingSlope) {
                        moveDirection.y = Mathf.Tan(groundAngle * Mathf.Deg2Rad) * distance;
                    }
                }
            }

            return hitDetected;
        }



        protected bool DetectVerticalCollisions(ref Vector3 moveDirection, out RaycastHit hit, bool useSkinWidth = true)
        {
            float distance = Vector3.Dot(moveDirection, moveDirection.normalized);
            bool hitDetected = CapsuleCast(m_currentPosition, moveDirection, moveSpeed, out hit, useSkinWidth);

            return hitDetected;
        }



        protected bool ClimbSlope(ref Vector3 moveDirection, float moveAmount, float slopeAngle)
        {
            //  Climb slope.
            float yDirection = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * m_colliderRadius;
            if (moveDirection.y <= yDirection) {
                moveAmount = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * m_colliderRadius;
                moveDirection = moveDirection.normalized * moveAmount;
                moveDirection.y = yDirection;

                return true;
            }
            return false;
        }



        protected void UpdateGroundInfo(bool grounded, float groundAngle = 0, Vector3? hitPoint = null, Vector3? groundNormal = null)
        {
            groundInfo.grounded = grounded;
            groundInfo.angle = grounded ? groundAngle : 0;
            groundInfo.point = grounded ? hitPoint.Value : m_currentPosition;
            groundInfo.normal = grounded ? groundNormal.Value : m_transform.up;
        }








        #region Public Functions

        public LayerMask GetCollisionMask()
        {
            return m_collision.collisionsMask;
        }


        Color green2 = new Color(0, 1, 0, 0.5f);
        Color red2 = new Color(1, 0, 0, 0.5f);
        Color grey2 = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public bool SphereGroundCast(Vector3 currentPosition, Vector3 direction,
                                    float radius, float distance, out RaycastHit hit,
                                    bool useSkinWidth = false, Vector3? positionOffset = null, bool debug = false, bool useSphere = false)
        {
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            Vector3 startPosition = currentPosition.WithY(radius + skinWidth);
            Vector3 spherePosition = positionOffset != null ? startPosition + positionOffset.Value : startPosition;
            float maxDistance = distance + skinWidth;

            bool result = Physics.SphereCast(spherePosition, radius, direction, out hit, maxDistance, GetCollisionMask(), m_advance.queryTrigger);

            if (debug) {
                var hitPoint = result ? hit.point : currentPosition;
                DebugDrawer.DrawSphere(spherePosition, radius, grey2);
                DebugDrawer.DrawLine(spherePosition, hitPoint, result ? Color.green : Color.red);
                if(useSphere)
                    DebugDrawer.DrawSphere(hitPoint, radius, result ? green2 : red2);
                else
                    DebugDrawer.DrawPoint(hitPoint, result ? Color.green : Color.red, radius);
                //DebugDrawer.DrawSphere(hit.point - direction * radius, radius, result ? Color.green : Color.red);
            }

            return result;
        }

        

        public bool CapsuleCast(Vector3 currentPosition, Vector3 direction, float distance, out RaycastHit capsuleHit, bool useSkinWidth = false, bool debug = false)
        {
            var origin = currentPosition.WithY(m_colliderCenter.y);
            var p1 = GetBottomCapsulePoint(origin);
            var p2 = GetTopCapsulePoint(origin);
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            float castRadius = m_colliderRadius - skinWidth;
            float maxDistance = distance + skinWidth;

            bool result = Physics.CapsuleCast(p1, p2, castRadius, direction, out capsuleHit, maxDistance, GetCollisionMask(), m_advance.queryTrigger);

            if (debug){
                var hitPos = direction * (capsuleHit.distance - (m_colliderRadius + skinWidth));
                //DebugDrawer.DrawCapsule(p1 + hitPos, p2 + hitPos, castRadius, result ? Color.green : Color.white);
                Debug.DrawRay(p1, direction * (capsuleHit.distance - (m_colliderRadius + skinWidth)), result ? Color.green : Color.grey);
                Debug.DrawRay(p2, direction * (capsuleHit.distance - (m_colliderRadius + skinWidth)), result ? Color.green : Color.grey);
            }

            return result;
        }




        public bool CheckCapsule(Vector3 currentPosition, Vector3? offsetPosition = null)
        {
            Vector3 offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            return Physics.CheckCapsule(GetCapsulePoint(currentPosition, m_transform.up) + offset,
                                        GetCapsulePoint(currentPosition, -m_transform.up) + offset,
                                        m_colliderRadius + m_physics.skinWidth,
                                        GetCollisionMask(), m_advance.queryTrigger);
                
        }


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


        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            float scale = direction.magnitude;
            Vector3 temp = Vector3.Cross(surfaceNormal, direction);
            Vector3 tangent = Vector3.Cross(temp, surfaceNormal);
            tangent = tangent.normalized * scale;
            return tangent;
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

        public Vector3 GetBottomCapsulePoint(Vector3 origin, Vector3? offsetPosition = null)
        {
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (Vector3.down * (pointsDist * .5f)) + offset;
        }

        public Vector3 GetTopCapsulePoint(Vector3 origin, Vector3? offsetPosition = null)
        {
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (Vector3.up * (pointsDist * .5f)) + offset;
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

        //     Physics.Raycast(nearPoint, down, out nearHit, m_colliderRadius * 2, m_collision.collisionsMask);
        //     Physics.Raycast(farPoint, down, out farHit, m_colliderRadius * 2, m_collision.collisionsMask);

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



        //private bool CheckSlope(Vector3 groundNormal)
        //{
        //    Vector3 transformDown = -m_transform.up;
        //    // If we are currently standing on ground that should be counted as a wall,
        //    // we are likely flush against it on the ground. Retrieve what we are standing on
        //    if (Vector3.Angle(m_groundHit.normal, Vector3.up) > 85) {
        //        //Vector3 slopeDirection = Vector3.Cross(m_groundHit.normal, m_transform.right);
        //        Vector3 slopeRight = Vector3.Cross(groundNormal, transformDown);
        //        Vector3 slopeVector = Vector3.Cross(slopeRight, m_groundHit.normal);


        //        Vector3 slopeCastOrigin = m_groundHit.point + (m_groundHit.normal.Multiply(k_collisionOffset));
        //        if (Physics.Raycast(slopeCastOrigin, slopeVector, out m_groundHit, (m_colliderRadius * 2) + m_physics.skinWidth, m_collision.collisionsMask)) {
        //            float groundAngle = Vector3.Angle(groundNormal, m_transform.up) * Mathf.Deg2Rad;
        //            Vector3 secondaryOrigin = m_transform.position + m_transform.up.Multiply(k_collisionOffset);

        //            if (!Mathf.Approximately(groundAngle, 0)) {
        //                float horizontal = Mathf.Sin(groundAngle) * m_colliderRadius;
        //                float vertical = (1.0f - Mathf.Cos(groundAngle)) * m_colliderRadius;

        //                // Retrieve a vector pointing up the slope
        //                Vector3 r2 = Vector3.Cross(groundNormal, transformDown);
        //                Vector3 v2 = -Vector3.Cross(r2, groundNormal);

        //                secondaryOrigin += Vector3.ProjectOnPlane(v2, m_transform.up).normalized * horizontal + m_transform.up * vertical;

        //                RaycastHit hit;
        //                if (Physics.Raycast(secondaryOrigin, transformDown, out hit, m_colliderRadius * 2, m_collision.collisionsMask)) {
        //                    // Remove the tolerance from the distance travelled
        //                    hit.distance -= k_collisionOffset + k_collisionOffset;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}






        #endregion










        #region Debugging





        protected virtual void DebugAttributes()
        {
            //if (debugger.states.showDebugUI == false) return;

            DebugUI.Log(this, "m_grounded", m_grounded, RichTextColor.White);

            DebugUI.Log(this, "fallTime", fallTime, RichTextColor.Green);
            //DebugUI.Log(this, "InputDirection", InputDirection, RichTextColor.Green);

            DebugUI.Log(this, "m_viewAngle", m_viewAngle, RichTextColor.White);




            DebugUI.Log(this, "m_verticalVelocity", m_verticalVelocity, RichTextColor.Cyan);
            DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.White);
            DebugUI.Log(this, "fwd_speed", Vector3.Dot(m_rigidbody.velocity, m_transform.forward), RichTextColor.White);
            DebugUI.Log(this, "moveSpeed", moveSpeed, RichTextColor.White);
            //DebugUI.Log(this, "r_angularVelocity", m_rigidbody.angularVelocity, RichTextColor.Magenta);

            DebugUI.Log(this, "groundInfo.angle", groundInfo.angle, RichTextColor.Brown);
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


        //protected virtual void CheckMovement()
        //{
        //    Vector3 moveDirection = m_moveDirection.normalized;
        //    float moveSpeed = Vector3.Dot(m_moveDirection, moveDirection);
        //    Vector3 virtualPosition = m_transform.position;

        //    Vector3 transformCenter = m_currentPosition + m_colliderCenter;


        //    RaycastHit capsuleHit;
        //    if (CapsuleCast(m_currentPosition, moveDirection, moveSpeed, out capsuleHit)) {

        //        var distance = Vector3.Distance(m_currentPosition, moveDirection * moveSpeed) - m_colliderRadius;
        //        float hitDistance = Mathf.Max(capsuleHit.distance - k_collisionOffset, 0.0f);
        //        // Note: remainingDistance is more accurate is we use hitDistance, but using hitInfoCapsule.distance gives a tiny 
        //        // bit of dampening when sliding along obstacles
        //        var remainingDistance = Mathf.Max(distance - capsuleHit.distance, 0.0f);
        //        //DebugDraw.DrawCapsule(GetBottomCapsulePoint(capsuleHit.point.WithY(capsuleOriginHeight)), GetBottomCapsulePoint(capsuleHit.point.WithY(capsuleOriginHeight)), m_colliderRadius, Color.white);

        //        var hitDirection = capsuleHit.point - virtualPosition;
        //        virtualPosition += hitDirection;

        //        Debug.DrawLine(transformCenter, capsuleHit.point, Color.red);

        //        //if(remainingDistance <= m_colliderRadius){
        //        //    m_rigidbody.position = Vector3.Lerp(m_rigidbody.position, virtualPosition, m_deltaTime * 2);
        //        //}
        //    }
        //    else {
        //        // No collision, move direction is valid.
        //    }


        //}

        //protected virtual void CheckMovement()
        //{
        //    //Vector3 moveDirection = m_moveDirection.normalized;

        //    //Vector3 virtualPosition = m_transform.position;

        //    //float capsuleOriginHeight = m_colliderHeight * 0.5f + m_physics.skinWidth;
        //    //var p1 = GetBottomCapsulePoint(m_currentPosition.WithY(capsuleOriginHeight), moveDirection);
        //    //var p2 = GetTopCapsulePoint(m_currentPosition.WithY(capsuleOriginHeight), moveDirection);
        //    //int overlapCount = Physics.OverlapCapsuleNonAlloc(p1, p2, m_collider.radius + m_physics.skinWidth, m_probedColliders, GetCollisionMask(), m_advance.queryTrigger);

        //    //Vector3 localPosition = Vector3.zero;
        //    //bool overlap = false;
        //    //if (overlapCount > 0)  //   || m_probedColliders.Length >= 0
        //    //{
        //    //    for (int i = 0; i < overlapCount; i++) {
        //    //        Collider collision = m_probedColliders[i];
        //    //        if (collision == m_collider) { continue; }
        //    //        if (collision == null) { break; }

        //    //        Vector3 direction;
        //    //        float distance;
        //    //        Transform colliderTransform = collision.transform;

        //    //        if (ComputePenetration(virtualPosition, moveDirection,
        //    //                                collision, colliderTransform.position, colliderTransform.rotation,
        //    //                                out direction, out distance)) {
        //    //            localPosition += direction * (distance + k_collisionOffset);
        //    //            overlap = true;
        //    //        }
        //    //    }
        //    //}

        //    ////if (overlap) m_moveDirection += localPosition.normalized;
        //    //if (overlap) m_rigidbody.MovePosition(m_rigidbody.position + localPosition.normalized);

        //    float penetrationDistance;
        //    Vector3 penetrationDirection;

        //    if (GetPenetrationInfo(m_rigidbody.position, out penetrationDistance, out penetrationDirection)) {
        //        m_moveDirection += penetrationDirection * penetrationDistance;
        //    }

        //    RaycastHit hit;
        //    bool collisionDetected = false;

        //    var velocity = m_moveDirection * m_deltaTime;
        //    if (m_collision.detectHorizontalCollisions) {
        //        collisionDetected = DetectHorizontalCollisions(ref velocity, groundInfo.angle, out hit, true);
        //    }

        //    //if (m_collision.detectVerticalCollisions) {
        //    //    collisionDetected = DetectHorizontalCollisions(ref m_moveDirection, groundInfo.angle, out hit, true);
        //    //}

        //    if (collisionDetected) {

        //    }

        //    m_moveDirection = velocity;
        //}
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



//    Vector3 drag = m_moveDirection * MathUtil.SmoothStop2(m_motor.groundDamping);
//    //if (m_inputDirection.sqrMagnitude > 0)
//    //    drag *= (1f - Vector3.Dot(m_lookDirection.normalized, m_moveDirection.normalized));
//    if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
//        m_moveDirection = Vector3.zero;
//    else
//        m_moveDirection -= drag;

//    Vector3 direction = m_transform.TransformDirection(m_inputVector);
//    m_moveDirection.x += direction.x * m_motor.groundAcceleration;
//    m_moveDirection.z += direction.z * m_motor.groundAcceleration;
//    m_moveDirection.y = 0;

//    if (m_moveDirection.sqrMagnitude > m_motor.groundSpeed * m_motor.groundSpeed)
//        m_moveDirection = m_moveDirection.normalized * m_motor.groundSpeed;


//    //m_movePosition = Vector3.Lerp(m_transform.position, m_transform.position + m_moveDirection, m_deltaTime);

//    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_motor.rotationSpeed * m_deltaTime, m_transform.up);
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




///// <summary>
///// Get the average raycast position.
///// </summary>
///// <param name="offsetX"></param>
///// <param name="offsetZ"></param>
///// <param name="rayCount"></param>
///// <returns></returns>
//protected Vector3 GetAverageRaycast(float offsetX, float offsetZ, int rayCount = 2)
//{
//    int maxRays = 4;
//    offsetX *= 2;
//    offsetZ *= 2;
//    rayCount = Mathf.Clamp(rayCount, 2, maxRays);
//    int totalRays = rayCount * rayCount + 1;
//    Vector3[] combinedCast = new Vector3[totalRays];
//    int average = 0;
//    Vector3 rayOrigin = m_transform.TransformPoint(0 - offsetX * 0.5f, m_MaxStepHeight + m_physics.skinWidth, 0 - offsetZ * 0.5f);
//    float rayLength = m_MaxStepHeight * 2;


//    float xSpacing = offsetX / (rayCount - 1);
//    float zSpacing = offsetZ / (rayCount - 1);

//    bool raycastHit = false;
//    Vector3 hitPoint = Vector3.zero;
//    Vector3 raycast = m_transform.TransformPoint(0, m_MaxStepHeight, 0);

//    if (DebugCollisions) Debug.DrawRay(raycast, MoveDirection.normalized, Color.blue);

//    RaycastHit hit;
//    int index = 0;
//    for (int z = 0; z < rayCount; z++) {
//        for (int x = 0; x < rayCount; x++) {
//            raycastHit = false;
//            hitPoint = Vector3.zero;
//            raycast = rayOrigin + (m_transform.forward * zSpacing * z) + (m_transform.right * xSpacing * x);
//            //raycast += MoveDirection.normalized * Time.deltaTime;
//            if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_collision.collisionsMask)) {
//                hitPoint = hit.point;
//                average++;
//                raycastHit = true;
//            }
//            combinedCast[index] = hitPoint;
//            index++;
//            if (DebugCollisions) Debug.DrawRay(raycast, Vector3.down * rayLength, (raycastHit ? Color.green : Color.red));
//        }
//    }


//    hitPoint = Vector3.zero;
//    raycastHit = false;
//    raycast = m_transform.TransformPoint(0, m_MaxStepHeight, 0);
//    //originRaycast += MoveDirection.normalized * Time.deltaTime;
//    if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_collision.collisionsMask)) {
//        hitPoint = hit.point;
//        average++;
//        raycastHit = true;
//    }

//    combinedCast[totalRays - 1] = hitPoint;
//    if (DebugCollisions) DebugDraw.Circle(raycast, Vector3.up * rayLength, 0.2f, (raycastHit ? Color.blue : Color.red));



//    average = Mathf.Clamp(average, 1, int.MaxValue);

//    Vector3 averageHitPosition = Vector3.zero;
//    float xTotal = 0f, yTotal = 0f, zTotal = 0f;
//    for (int i = 0; i < combinedCast.Length; i++) {
//        xTotal += combinedCast[i].x;
//        yTotal += combinedCast[i].y;
//        zTotal += combinedCast[i].z;
//    }
//    averageHitPosition.Set(xTotal / average, yTotal / average, zTotal / average);

//    if (DebugCollisions) DebugDraw.DrawMarker(averageHitPosition, 0.2f, Color.blue);

//    return averageHitPosition;
//}
