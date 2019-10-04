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
        public enum MovementTypes { Adventure, Combat };

        # region Parameter clases
        [Serializable]
        public class PhysicsSettings
        {
            [Serializable]
            public class ColliderSettings{
                public float colliderHeight = 2f;
                public float colliderRadius = 0.4f;
            }
            public ColliderSettings colliderSettings = new ColliderSettings();

        }

        [Serializable]
        public class AdvanceSettings{
            [Tooltip("<Not Implemented>")]
            public QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            [Tooltip("<Not Implemented>")]
            public float timeScale = 1;
        }

        [SerializeField]
        protected PhysicsSettings m_physicsSettings = new PhysicsSettings();
        [SerializeField]
        protected AdvanceSettings m_advanceSettings = new AdvanceSettings();
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
        [Tooltip("Maximum fall speed of character.")]
        protected float m_termijnalVelocity = -100f;

        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();


        // --- Kinematic




        #endregion





        protected CapsuleCollider m_collider;
        [SerializeField, HideInInspector]
        protected MovementTypes m_MovementType = MovementTypes.Adventure;

        protected Vector3 m_currentPosition { get { return m_transform.position; } }

        protected bool m_moving, m_grounded = true;
        protected float m_targetAngle, m_viewAngle;
        protected Vector3 m_inputVector, m_inputDirection;
        protected Vector3 m_moveDirection, m_angularVelocity;
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
        protected Collider[] m_probedColliders;



        protected PhysicMaterial m_physicsMaterial;

        protected float m_previousAngle;
        protected Vector3 m_previousPosition;
        protected Vector3 m_velocity;
        protected Quaternion m_previousRotation;

        protected float m_currentVerticalVelocity;
        protected float m_moveAmount;
        protected float m_forwardSpeed;
        protected float m_lateralSpeed;
        protected float m_currentSpeed;

        public float gravity { get; protected set; }
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

        public float Speed { get { return Mathf.Abs(m_currentSpeed); } set { m_currentSpeed = Mathf.Abs(value); } }

        public float RotationSpeed { get { return m_rotationSpeed; } set { m_rotationSpeed = value; } }

        public bool UseRootMotion { get { return m_useRootMotionPosition; } set { m_useRootMotionPosition = value; } }



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
                m_deltaRotation = Quaternion.AngleAxis(angle, axis);
            }


        }



        protected virtual void InternalMove()
        {

            //m_velocity = m_useRootMotionPosition ? m_deltaPosition / m_deltaTime : (m_transform.position - m_previousPosition) / m_deltaTime;
            //m_previousPosition = m_transform.position;

            //m_velocity = m_rigidbody.velocity;


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
            m_rigidbody.MovePosition(m_movePosition);
        }


        


        protected virtual void Move()
        {
            if (m_inputVector.sqrMagnitude > 1)
                m_inputVector.Normalize();
            
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType)
            {
                case (MovementTypes.Adventure):
                    //  Get the correct input direction.
                    float moveAmount = Mathf.Clamp01(Mathf.Abs(m_inputVector.x) + Mathf.Abs(m_inputVector.z));
                    m_inputDirection =  m_transform.forward * moveAmount;

                    //  Set forward and lateral speeds.
                    m_forwardSpeed = moveAmount;
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
            DebugUI.Log(this, "m_inputDirection1", m_inputDirection, RichTextColor.Orange);
            //DebugUI.Log(this, "m_inputDirection2", Quaternion.Inverse(m_transform.rotation) *  m_transform.TransformDirection(m_inputVector), RichTextColor.Orange);

            m_moveAmount = m_inputDirection.magnitude;
            m_moveDirection = m_inputDirection * m_desiredSpeed * m_moveAmount;

            //  Calculate move vector
            Vector3 drag = m_moveDirection * MathUtil.SmoothStop3(m_motorDamping);
            if (m_inputDirection.sqrMagnitude > 0)
                drag *= (1 - Vector3.Dot(m_inputDirection.normalized, m_moveDirection.normalized));
            if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
                m_moveDirection = Vector3.zero;
            else
                m_moveDirection -= drag;


            m_moveDirection += m_inputDirection * (m_motorAcceleration * m_deltaTime);
            if (m_moveDirection.sqrMagnitude > m_desiredSpeed * m_desiredSpeed)
                m_moveDirection = m_moveDirection.normalized * m_desiredSpeed;


            m_currentSpeed = m_moveDirection.magnitude;
            //  Is there enough movement to be considered moving.
            m_moving = m_moveAmount > 0;




            //if(m_moveAmount > 0)
            //    m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
            //else
            //    m_targetRotation = m_transform.rotation;
            m_targetRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);


            DebugUI.Log(this, "m_targetAngle", m_targetAngle, RichTextColor.Cyan);
            DebugUI.Log(this, "m_inputDirection", m_inputDirection, RichTextColor.Cyan);
        }









        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            if (m_grounded && !m_moving) return;

            RaycastHit groundHit;
            groundInfo.angle = 0;
            groundInfo.point = m_currentPosition;
            groundInfo.normal = m_transform.up;

            if (SphereGroundCast(m_currentPosition, Vector3.down, m_spherecastRadius, (k_groundedDistance + k_collisionOffset), out groundHit)) {

                float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);

                //if (groundAngle > 85) {
                //    if (SphereGroundCast(m_currentPosition, Vector3.down, m_colliderRadius + m_skinWidth, (k_groundedDistance + k_collisionOffset), out groundHit)) {
           
                //        DebugDrawer.DrawSphere(groundHit.point - Vector3.up * (m_colliderRadius + m_skinWidth), m_colliderRadius + m_skinWidth, Color.green);
                //    }
                //    else {
                //        m_grounded = false;
                //        Debug.Break();
                //        return;
                //    }
                //}

                DebugDrawer.DrawSphere(groundHit.point - Vector3.up * m_spherecastRadius, m_spherecastRadius, Color.green);

                if (!m_grounded) {
                    m_rigidbody.position = groundHit.point;
                }
                m_grounded = true;
                groundInfo.angle = groundAngle;
                groundInfo.point = groundHit.point;
                groundInfo.normal = groundHit.normal;
            }
            else {
                m_grounded = false;

            }

            
            if (m_grounded)
            {
                groundInfo.grounded = m_grounded;
                m_verticalVelocity = Vector3.zero;
                m_currentVerticalVelocity = 0;
            }


            


        }






        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {
            Vector3 moveDirection = m_moveDirection.normalized;
            float moveSpeed = m_currentSpeed;
            Vector3 virtualPosition = m_transform.position;

            Vector3 transformCenter = m_currentPosition + m_colliderCenter;


            RaycastHit capsuleHit;
            if (CapsuleCast(m_currentPosition, moveDirection, moveSpeed, out capsuleHit)) {

                var distance = Vector3.Distance(m_currentPosition, moveDirection * moveSpeed) - m_colliderRadius;
                float hitDistance = Mathf.Max(capsuleHit.distance - k_collisionOffset, 0.0f);
                // Note: remainingDistance is more accurate is we use hitDistance, but using hitInfoCapsule.distance gives a tiny 
                // bit of dampening when sliding along obstacles
                var remainingDistance = Mathf.Max(distance - capsuleHit.distance, 0.0f);
                //DebugDraw.DrawCapsule(GetBottomCapsulePoint(capsuleHit.point.WithY(capsuleOriginHeight)), GetBottomCapsulePoint(capsuleHit.point.WithY(capsuleOriginHeight)), m_colliderRadius, Color.white);

                var hitDirection = capsuleHit.point - virtualPosition;
                virtualPosition += hitDirection;

                Debug.DrawLine(transformCenter, capsuleHit.point, Color.red);

                //if(remainingDistance <= m_colliderRadius){
                //    m_rigidbody.position = Vector3.Lerp(m_rigidbody.position, virtualPosition, m_deltaTime * 2);
                //}
            }
            else {
                // No collision, move direction is valid.
            }

            
        }




        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            Vector3 moveDirection = m_moveDirection.normalized;
            float moveSpeed = m_currentSpeed;
            Vector3 virtualPosition = m_transform.position;

            float capsuleOriginHeight = m_colliderHeight * 0.5f + m_skinWidth;
            var p1 = GetBottomCapsulePoint(m_currentPosition.WithY(capsuleOriginHeight), moveDirection);
            var p2 = GetTopCapsulePoint(m_currentPosition.WithY(capsuleOriginHeight), moveDirection);
            int overlapCount = Physics.OverlapCapsuleNonAlloc(p1, p2, m_collider.radius + m_skinWidth, m_probedColliders, GetCollisionMask(), m_advanceSettings.queryTrigger);

            Vector3 localPosition = Vector3.zero;
            bool overlap = false;
            if (overlapCount > 0)  //   || m_probedColliders.Length >= 0
            {
                for (int i = 0; i < overlapCount; i++)
                 {
                    Collider collision = m_probedColliders[i];
                    if (collision == m_collider) { continue; }
                    if(collision == null) { break; }

                    Vector3 direction;
                    float distance;
                    Transform colliderTransform = collision.transform;

                    if (ComputePenetration(virtualPosition, moveDirection,
                                            collision, colliderTransform.position, colliderTransform.rotation,
                                            out direction, out distance)) {
                        localPosition += direction * (distance + k_collisionOffset);
                        overlap = true;
                    }

                }
            }


            if (m_grounded)
            {



                //m_currentSpeed = m_moveDirection.magnitude;
                //if (Vector3.Dot(m_moveDirection, groundInfo.normal) >= 0) {
                //    //  If greater than 0, than we are going down a slope.
                //}
                //else {
                //    //  We are going up the slope if it is negative.
                //}

                m_moveDirection = GetDirectionTangentToSurface(m_moveDirection, groundInfo.normal) ;

                //// Reorient target velocity.
                //Vector3 inputRight = Vector3.Cross(m_moveDirection, m_transform.up);
                //Vector3 targetMovementVelocity = Vector3.Cross(groundInfo.normal, inputRight).normalized * m_currentSpeed;// m_moveDirection.magnitude;

                //m_moveDirection = Vector3.Lerp(m_moveDirection, targetMovementVelocity * m_deltaTime, 1f - Mathf.Exp(-5 * m_deltaTime));
            }
            else {
                //Vector3 verticalVelocity = Vector3.Project(m_velocity, m_gravity);
                m_currentVerticalVelocity += (gravity * fallTime);
                m_verticalVelocity.y = m_currentVerticalVelocity;
            }


            if(overlap) m_moveDirection += localPosition.normalized;
            m_moveDirection += m_verticalVelocity * m_deltaTime ;
        }



        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {

            m_moveRotation = Quaternion.AngleAxis(m_targetAngle * m_rotationSpeed * m_deltaTime, m_transform.up);

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

            m_currentSpeed = 2;
            m_animator.SetFloat(HashID.Speed, m_currentSpeed);

            //if (Mathf.Approximately(m_targetAngle, 0)) m_targetAngle = 0;
            //m_animator.SetFloat(HashID.Rotation, m_targetAngle);


            //if (Mathf.Approximately(m_viewAngle, 0)) m_viewAngle = 0;
            //m_animator.SetFloat(HashID.LookAngle, m_viewAngle);



            //m_animator.SetFloat(HashID.Rotation, m_targetAngle);

            m_animatorMonitor.SetForwardInputValue(m_forwardSpeed * 2);
            m_animatorMonitor.SetHorizontalInputValue(m_lateralSpeed * 2);

            //m_animatorMonitor.SetForwardInputValue(m_inputVector.z * 2);
            //m_animatorMonitor.SetHorizontalInputValue(m_inputVector.x * 2);
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

        public LayerMask GetCollisionMask()
        {
            return m_collisionsLayerMask;
        }


        public bool SphereGroundCast(Vector3 position, Vector3 direction, float radius, float distance, out RaycastHit sphereCastHit)
        {
            // Cast further than the distance we need, to try to take into account small edge cases (e.g. Casts fail 
            // when moving almost parallel to an obstacle for small distances).
            var extraDistance = radius;
            var spherePosition = position.WithY(extraDistance);
            Debug.DrawLine(spherePosition, spherePosition + direction * (distance + extraDistance), Color.white);
            DebugDrawer.DrawSphere(spherePosition, radius, Color.white);
            return Physics.SphereCast(spherePosition, radius, direction,out sphereCastHit, distance + extraDistance, GetCollisionMask(), m_advanceSettings.queryTrigger);
        }


        public bool CapsuleCast(Vector3 position, Vector3 direction, float distance, out RaycastHit capsuleHit, bool useSkinWidth = false)
        {
            float capsuleOriginHeight = m_colliderHeight * 0.5f;
            float extraDistance = m_colliderRadius;
            float skinWidth = useSkinWidth ? m_skinWidth : 0;
            

            return Physics.CapsuleCast( GetBottomCapsulePoint(position.WithY(capsuleOriginHeight)),
                                        GetTopCapsulePoint(position.WithY(capsuleOriginHeight)),
                                        m_colliderRadius + skinWidth,
                                        direction, out capsuleHit,
                                        distance + extraDistance,
                                        GetCollisionMask(), m_advanceSettings.queryTrigger);
        }


        public bool CheckCapsule(Vector3 currentPosition, Vector3? offsetPosition = null)
        {
            Vector3 offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            return Physics.CheckCapsule(GetCapsulePoint(currentPosition, m_transform.up) + offset,
                                        GetCapsulePoint(currentPosition, -m_transform.up) + offset,
                                        m_colliderRadius + m_skinWidth,
                                        GetCollisionMask(), m_advanceSettings.queryTrigger);
                
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



        private float GetGroundAngle()
        {

            return 0;
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


        public bool RaycastCheckGrounded(float? positionOffset = null)
        {

            if (Physics.Raycast(m_currentPosition + m_colliderCenter,
                                            -m_transform.up, m_airborneThreshold * m_colliderHeight, GetCollisionMask(), m_advanceSettings.queryTrigger)) {
                return true;
            }
            var offset = positionOffset != null ? positionOffset.Value : m_colliderRadius;
            var xRayOffset = new Vector3(m_colliderRadius, 0f, 0f);
            var zRayOffset = new Vector3(0f, 0f, m_colliderRadius);

            for (var i = 0; i < 4; i++) {
                var sign = 1f;
                Vector3 rayOffset;
                if (i % 2 == 0) {
                    rayOffset = xRayOffset;
                    sign = i - 1f;
                }
                else {
                    rayOffset = zRayOffset;
                    sign = i - 2f;
                }
                Debug.DrawRay(m_currentPosition + m_colliderCenter + sign * rayOffset,
                              new Vector3(0, -m_airborneThreshold * m_colliderHeight, 0), Color.blue);

                if (Physics.Raycast(m_currentPosition + m_colliderCenter + sign * rayOffset,
                                            -m_transform.up, m_airborneThreshold * m_colliderHeight,
                                            GetCollisionMask(), m_advanceSettings.queryTrigger)) {
                    return true;
                }
            }
            return false;
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
        //        if (Physics.Raycast(slopeCastOrigin, slopeVector, out m_groundHit, (m_colliderRadius * 2) + m_skinWidth, m_collisionsLayerMask)) {
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
        //                if (Physics.Raycast(secondaryOrigin, transformDown, out hit, m_colliderRadius * 2, m_collisionsLayerMask)) {
        //                    // Remove the tolerance from the distance travelled
        //                    hit.distance -= k_collisionOffset + k_collisionOffset;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}


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

            DebugUI.Log(this, "fallTime", fallTime, RichTextColor.Green);
            //DebugUI.Log(this, "InputDirection", InputDirection, RichTextColor.Green);

            DebugUI.Log(this, "m_viewAngle", m_viewAngle, RichTextColor.White);




            DebugUI.Log(this, "m_verticalVelocity", m_verticalVelocity, RichTextColor.Cyan);
            DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.White);
            DebugUI.Log(this, "fwd_speed", Vector3.Dot(m_rigidbody.velocity, m_transform.forward), RichTextColor.White);

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
//    Vector3 rayOrigin = m_transform.TransformPoint(0 - offsetX * 0.5f, m_MaxStepHeight + m_skinWidth, 0 - offsetZ * 0.5f);
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
//            if (Physics.Raycast(raycast, Vector3.down, out hit, rayLength, m_collisionsLayerMask)) {
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
//    if (Physics.Raycast(raycast, Vector3.down, out hit, 0.4f, m_collisionsLayerMask)) {
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
