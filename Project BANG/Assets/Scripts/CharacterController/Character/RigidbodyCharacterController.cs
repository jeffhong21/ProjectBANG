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
        [SerializeField, Group("Motor")] protected float m_rootMotionSpeedMultiplier = 1;
        [SerializeField, Group("Motor")] protected float m_rootMotionRotationMultiplier = 1;
        [SerializeField, Group("Motor")] protected Vector3 m_acceleration = new Vector3(0.18f, 0, 0.18f);
        [Range(0, 10)]
        [SerializeField, Group("Motor")] protected float m_desiredSpeed = 4f;
        [Range(0, 1)]
        [SerializeField, Group("Motor")] protected float m_motorDamping = 0.3f;
        [SerializeField, Group("Motor")] protected float m_rotationSpeed = 4f;


        
        //  -- Physics variables
        [Group("Physics")]
        [SerializeField] protected float m_mass = 100;
        [Group("Physics")]
        [SerializeField] protected float m_skinWidth = 0.08f;
        [Group("Physics"), Range(0, 85)]
        [SerializeField] protected float m_slopeLimit = 45f;
        [Group("Physics")]
        [SerializeField] protected float m_maxStepHeight = 0.4f;
        [Group("Physics")]
        [SerializeField] protected float m_gravityModifier = 1f;
        [Group("Physics")]
        [SerializeField] protected float m_groundStickiness = 2f;

        //  -- Collision detection
        [Group("Collisions")] //[Adnc.Utility.SortLayer]
        [SerializeField] protected LayerMask m_collisionsLayerMask;
        [Group("Collisions")]
        [SerializeField] protected int m_maxCollisionCount = 100;







        #endregion

        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();



        protected CapsuleCollider m_collider;
        protected float m_timeScale = 1;
        protected MovementTypes m_MovementType = MovementTypes.Adventure;

        protected bool m_moving, m_grounded = true;
        protected float m_previousMoveAngle, m_moveAngle;
        protected Vector3 m_inputVector, m_inputDirection;
        protected Vector3 m_moveDirection, m_velocity , m_angularVelocity;
        protected Vector3 m_movePosition;
        protected Quaternion m_moveRotation = Quaternion.identity, m_lookRotation = Quaternion.identity;
        protected Vector3 m_gravity;
        protected Vector3 m_lookDirection, m_smoothLookDirection;

        //  root motion variables.
        protected Vector3 m_rootMotionVelocity;
        protected Quaternion m_rootMotionRotation;
        protected Vector3 m_deltaPosition;
        protected Quaternion m_deltaRotation;
        protected float m_deltaAngle;

        protected float m_groundAngle;

        protected float m_spherecastRadius = 0.1f;
        protected RaycastHit m_groundHit;
        [SerializeField, Group("Collisions")]
        protected Collider[] m_probedColliders;



        protected PhysicMaterial m_physicsMaterial;


        protected Vector3 m_previousPosition;
        protected Vector3 m_previousVelocity;
        protected Quaternion m_previousRotation;


        protected float m_Speed;
        protected float m_dragVelocity;

        protected Vector3 moveDirectionSmooth;
        protected Vector3 velocitySmooth;
        protected float rotationAngleSmooth, angularDragSmooth;


        float m_airborneThreshold = 0.3f;


        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_layerManager;
        protected Rigidbody m_rigidbody;
        protected GameObject m_gameObject;
        protected Transform m_transform;
        protected float m_deltaTime;


        [Group("Movement")][Range(0, 10)]
        [SerializeField] protected int m_sampleRate = 4;
        [Group("Movement")] [Range(0, 10)]
        [SerializeField] protected float m_positionBias = 1.2f;
        [Group("Movement")][Range(0, 10)]
        [SerializeField] protected float m_directionBias = 1.2f;
        [SerializeField] protected Trajectory m_motionPath;


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

        public Vector3 InputVector { get { return m_inputVector;} set{ m_inputVector = value; } }
        //public Vector3 InputVector { get; set; }

        public Vector3 InputDirection{ get{ return m_inputDirection; } }

        public Vector3 MoveDirection { get { return m_moveDirection; } set { m_moveDirection = value; } }

        public Vector3 LookDirection { get { return m_lookDirection; } }

        public Quaternion LookRotation { get { return m_lookRotation; } set { m_lookRotation = value; } }

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

        public Quaternion MoveRotation { get { return m_moveRotation; } set { m_moveRotation = value; } }

        public Vector3 Velocity { get { return m_velocity; } set { m_velocity = value; } }



        public CapsuleCollider Collider { get{ return m_collider; } protected set { m_collider = value; }}

        public RaycastHit GroundHit { get { return m_groundHit; } }



        public Vector3 RootMotionVelocity { get { return m_rootMotionVelocity; } set { m_rootMotionVelocity = value; } }

        public Quaternion RootMotionRotation { get { return m_rootMotionRotation; } set { m_rootMotionRotation = value; } }





        //public float LookAngle{
        //    get{
        //        var lookDirection = m_transform.InverseTransformDirection(LookRotation * m_transform.forward);
        //        var axisSign = Vector3.Cross(lookDirection, m_transform.forward);
        //        return Vector3.Angle(m_transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f);
        //    }
        //}


 

        protected Vector3 m_colliderCenter;
        protected float m_colliderHeight, m_colliderRadius;

        #endregion





        protected virtual void AnimatorMove()
        {
            Vector3 fwd = m_animator.deltaRotation * Vector3.forward;
            m_deltaAngle += Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;


            if (m_useRootMotion) {
                m_deltaPosition = m_animator.deltaPosition * m_rootMotionSpeedMultiplier;
                m_rootMotionVelocity = m_deltaPosition / m_deltaTime;
                //if (m_animator.hasRootMotion) m_rigidbody.MovePosition(m_animator.rootPosition);
            }

            if (m_useRootMotion) {
                //m_previousRotation *= m_animator.deltaRotation;

                m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = (angle * m_rootMotionSpeedMultiplier * Mathf.Deg2Rad) / m_deltaTime;
                m_rootMotionRotation = Quaternion.AngleAxis(angle, axis);

                //if (m_animator.hasRootMotion) m_rigidbody.MoveRotation(m_animator.rootRotation);
            }


        }


        protected virtual void InternalMove()
        {
            if (m_inputVector.sqrMagnitude > 1) m_inputVector.Normalize();

            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (m_MovementType)
            {
                case (MovementTypes.Adventure):
                    //  Get the correct target rotation based on input.
                    m_moveAngle = m_transform.AngleFromForward(m_lookDirection);
                    m_inputDirection = m_transform.InverseTransformDirection(m_inputVector);
                    m_inputDirection.z = Mathf.Clamp01(m_inputDirection.z + (Mathf.Abs(m_moveAngle) * Mathf.Deg2Rad));

                    break;

                case (MovementTypes.Combat):
                    //  Get the correct target rotation based on look rotation.
                    m_moveAngle = m_transform.AngleFromForward(m_lookDirection);
                    m_inputDirection = m_transform.TransformDirection(m_inputVector);

                    break;
            }



            //m_previousVelocity = m_useRootMotion ? m_rootMotionVelocity : (m_transform.position - m_previousPosition) / m_deltaTime;
            //m_previousPosition = m_transform.position;

            m_previousPosition = m_useRootMotion ? m_deltaPosition : (m_transform.position - m_previousPosition);
            m_previousVelocity = m_previousPosition / m_deltaTime;

        }


        protected virtual void Move()
        {

            if (m_inputVector.sqrMagnitude >= 0.2f) {
                Vector3 moveDirection = m_lookRotation * m_transform.forward;
                Vector3 forward = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
                Vector3 linearVelocity = Quaternion.FromToRotation(Vector3.forward, forward) * m_inputVector;
                linearVelocity = linearVelocity.Multiply(m_desiredSpeed);

                m_moveDirection = Quaternion.Inverse(m_transform.rotation) * linearVelocity;
            } else {
                m_moveDirection.Zero();
            }

            //  --------------------------
            //  Project motion trajectory.
            //  --------------------------
            PredictTrajectory(m_moveDirection, m_positionBias, m_directionBias, false);
            //  --------------------------


            m_lookDirection = m_lookRotation * m_transform.forward;

            m_velocity = m_previousPosition / m_deltaTime;
            //  Is there enough movement to be considered moving.
            m_moving = m_inputDirection.sqrMagnitude > 0;
        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            if (m_grounded && !m_moving) return;

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
                    m_colliderRadius + m_airborneThreshold, m_collisionsLayerMask))
                {
                    groundAngle = Vector3.Angle(m_groundHit.normal, m_transform.up);
                    if (groundDistance > m_groundHit.distance - m_skinWidth)
                        groundDistance = m_groundHit.distance - m_skinWidth;

                }  // End of SphereCast

            }  //  End of Raycast.



            if (groundDistance < 0.05f && m_groundAngle < 85) {
                m_grounded = true;
                m_groundAngle = groundAngle;
                //m_velocity += Vector3.ProjectOnPlane(m_groundHit.point - m_transform.position, m_transform.up * m_groundStickiness);
                //ScaleCapsule(1);
            }
            else {
                if (groundDistance > m_airborneThreshold){
                    if (m_grounded) {
                        Vector3 initialFallVelocity = Vector3.Project(m_previousVelocity, m_gravity);
                        initialFallVelocity += m_gravity * m_gravityModifier * m_deltaTime;
                        m_velocity += initialFallVelocity;
                    }
                    m_groundAngle = 0;
                    m_groundHit.point = m_transform.position - Vector3.up * m_airborneThreshold;
                    m_groundHit.normal = m_transform.up;
                    m_grounded = false;
                }

                //ScaleCapsule(0.5f);
            }

            //float percentage = (groundDistance > 1) ? 1 : groundDistance / 1;
            //float scale = Mathf.Lerp(1, 0.5f, percentage.Cubed());
            //ScaleCapsule(0.5f);
        }






        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {
            //  Set maxDepentration velocity.
            m_rigidbody.maxDepenetrationVelocity = 0.01f;

            //  --------------------------
            //  Get the position based off of the motion path.
            //  --------------------------
            //Vector3 position = m_motionPath.Next(0).RelativeToTransformWorld(m_transform);
            //var worldSpacePos = m_transform.TransformPoint(position);
            //var positionDifference = m_transform.localPosition + position - worldSpacePos;
            //position = worldSpacePos + positionDifference;


            float castRadius = m_collider.radius + m_skinWidth;
            Vector3 castPosition = transform.position.WithYZ(castRadius, castRadius + m_skinWidth);
            int hits = Physics.OverlapSphereNonAlloc(castPosition, castRadius, m_probedColliders, m_collisionsLayerMask);
            if (hits > 0)
            {
                Vector3 checkPosition = m_transform.position.WithY(m_maxStepHeight);

                for (int i = 0; i < hits; i++)
                {
                    Collider collision = m_probedColliders[i];
                    Vector3 closestPoint = collision.ClosestPoint(m_transform.position);

                    //  --------------------------
                    //  Calculate penetration (if the rigidbody is kinematic.)
                    //  --------------------------
                    if (m_rigidbody.isKinematic)
                    {
                        bool overlapped = Physics.ComputePenetration(m_collider, m_transform.position, m_transform.rotation,
                                                                    collision, collision.transform.position, collision.transform.rotation,
                                                                    out Vector3 direction, out float distance);
                        if (overlapped) {
                            Vector3 penetrationVector = direction * distance;
                            Vector3 moveDirectionProjected = Vector3.Project(m_velocity, -penetrationVector);

                            m_velocity = m_velocity.Add(penetrationVector);
                            m_velocity = m_velocity.Subtract(moveDirectionProjected);      //  TODO: optimize me

                            //m_velocity = Vector3.ClampMagnitude(m_velocity, 10);

                            if (DebugCollisions) {
                                DebugDrawer.DrawLabel(closestPoint + penetrationVector, "Penetration Vector " + penetrationVector.ToString());
                                Debug.DrawRay(closestPoint, penetrationVector, Color.red);
                                DebugDrawer.DrawLabel(closestPoint + moveDirectionProjected, "Velocity Projected " + moveDirectionProjected.ToString());
                                Debug.DrawRay(closestPoint, moveDirectionProjected, Color.cyan);
                            }
                            //Debug.Break();
                        }
                    }


                    //  --------------------------
                    //  Check if a collision has occured.
                    //  --------------------------
                    Vector3 closestPointDir = closestPoint - m_transform.position;
                    float colliderAngle = Vector3.Dot(m_transform.forward, closestPointDir.normalized);
                    if(colliderAngle > 0)
                    {
                        Vector3 normal = m_transform.position - closestPoint;   //  Normal vector
                        Vector3 v2 = Vector3.Cross(normal, Vector3.up);         //  Wall angle

						Vector3 slideDir = normal * Vector3.Dot(m_moveDirection, normal);
						Vector3 wallSlideDir = (m_moveDirection - slideDir).normalized;
						m_velocity = Vector3.Project(m_velocity, wallSlideDir);
						Vector3 tangent = m_velocity;
                        Vector3.OrthoNormalize(ref normal, ref tangent);

                        //Debug.DrawRay(checkPosition, m_velocity, Color.blue);
                        //Debug.DrawRay(checkPosition.AddY(.1f), tangent, Color.green);


                    }

                    //  ---------------
                    // ----- Debugging
                    //  ---------------
                    if (DebugCollisions) {
                        Debug.DrawRay(checkPosition, checkPosition - closestPoint, Color.yellow);
                        DebugDraw.Sphere(closestPoint, 0.05f, Color.yellow);
                        DebugUI.Log(this, collision.name, Vector3.Dot(m_transform.forward, (closestPoint - m_transform.position).normalized), RichTextColor.LightBlue);
                    }
                }


                //if(m_velocity.sqrMagnitude > 100)
                //{
                //    Debug.Log("<b><color=red>[Velocity Exceede Speed]</color></b> Velocity Magnitude: " + m_rigidbody.velocity.magnitude);
                //}

                //if(DebugCollisions) DebugDraw.DrawCapsule(p1, p2, castRadius, Color.gray);
                if (DebugCollisions) DebugDraw.Sphere(castPosition, castRadius, Color.gray);
            }


        }





        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            Vector3 velocity = m_velocity;

            if (m_grounded)
            {
                Vector3 drag = velocity * (m_motorDamping * m_deltaTime);
                if (m_inputDirection.sqrMagnitude > 0)
                    drag *= (1f - Vector3.Dot(m_inputDirection.normalized, velocity.normalized));
                if (drag.sqrMagnitude > velocity.sqrMagnitude)
                    velocity = Vector3.zero;
                else velocity -= drag;
                //velocity += m_inputDirection.Multiply(m_acceleration.Multiply(m_deltaTime));
                velocity.x += m_inputDirection.x * (m_deltaTime * m_acceleration.x);
                velocity.y += m_inputDirection.y * (m_deltaTime * m_acceleration.y);
                velocity.z += m_inputDirection.z * (m_deltaTime * m_acceleration.z);
                if (velocity.sqrMagnitude > m_desiredSpeed * m_desiredSpeed)
                    velocity = velocity.normalized * m_desiredSpeed;

                float velocitySpeed = Vector3.Dot(m_velocity, m_transform.forward);
                Vector3 groundNormal = m_groundHit.normal;

                //if (Vector3.Dot(m_moveDirection, groundNormal) >= 0)
                //{
                //    //  If greater than 0, than we are going down a slope.
                //}
                //else
                //{
                //    //  We are going up the slope if it is negative.
                //}

                Vector3 directionTangent = GetDirectionTangentToSurface(velocity, groundNormal) * velocitySpeed;

                if(DebugGroundCheck) DebugDraw.Arrow(m_transform.position.WithY(0.1f), directionTangent, Color.magenta);
                // Reorient target velocity.
                Vector3 inputRight = Vector3.Cross(velocity, m_transform.up);
                velocity = Vector3.Cross(groundNormal, inputRight).normalized * velocitySpeed;// m_moveDirection.magnitude;

                //m_velocity = Vector3.Lerp(m_velocity, m_velocity, 1f - Mathf.Exp(-15 * m_deltaTime));
                
            } else {

                Vector3 verticalVelocity = Vector3.Project(m_previousVelocity, m_gravity);


                verticalVelocity += m_gravity * m_gravityModifier * m_deltaTime ;
                DebugDraw.Arrow(m_transform.position, verticalVelocity, Color.red);
                //if (m_rigidbody.velocity.y < 0)
                //    verticalVelocity = verticalVelocity.Multiply(1.5f);


                velocity += verticalVelocity;
            }


           m_velocity = velocity;
        }



        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            float t = m_deltaTime;
            //m_smoothLookDirection = Vector3.Slerp(m_transform.forward, m_lookDirection, smooth).normalized;

            m_moveRotation = Quaternion.AngleAxis(m_moveAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
            



        }



        /// <summary>
        /// Apply rotation.
        /// </summary>
        protected virtual void ApplyRotation()
        {

            //m_moveRotation.ToAngleAxis(out float angle, out Vector3 axis);
            //m_angularVelocity = axis.normalized * angle;
            //m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, m_angularVelocity, m_deltaTime * m_rotationSpeed);
            //AddRelativeTorque adds torque according to its Inertia Tensors. Therefore, the desired angular
            //velocity must be transformed according to the Inertia Tensor, to get the required Torque.

            //Vector3 torque = m_rigidbody.inertiaTensorRotation * Vector3.Scale(m_rigidbody.inertiaTensor, m_angularVelocity);
            //m_rigidbody.AddRelativeTorque(torque, ForceMode.Impulse);

            //m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, torque, m_deltaTime * m_rotationSpeed);

            //DebugUI.Log(this, "Angle", angle, RichTextColor.Magenta);

            float t = m_deltaTime * m_rotationSpeed;
            m_rigidbody.MoveRotation(m_moveRotation);
        }


        /// <summary>
        /// Apply position values.
        /// </summary>
        protected virtual void ApplyMovement()
        {
            //m_velocity = Vector3.ClampMagnitude(m_velocity, m_desiredSpeed);
            //if (m_grounded) m_velocity.y = 0;
            //m_rigidbody.drag = Mathf.SmoothDamp(m_rigidbody.drag, m_moving ? 0 : m_mass, ref m_dragVelocity, m_moving ? m_acceleration.z : m_motorDamping);
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

            m_animatorMonitor.SetForwardInputValue(m_inputDirection.z);
            m_animatorMonitor.SetHorizontalInputValue(m_inputDirection.x);

        }





        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected virtual void SetPhysicsMaterial()
        {
            //change the physics material to very slip when not grounded or maxFriction when is

            //  Airborne.
            if (!Grounded && Mathf.Abs(m_rigidbody.velocity.y) > 0) {
                m_collider.material.staticFriction = 0f;
                m_collider.material.dynamicFriction = 0f;
                m_collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            //  Grounded and is moving.
            else if (Grounded && Moving) {
                m_collider.material.staticFriction = 0.25f;
                m_collider.material.dynamicFriction = 0f;
                m_collider.material.frictionCombine = PhysicMaterialCombine.Multiply;
            }
            //  Grounded but not moving.
            else if (Grounded && !Moving) {
                m_collider.material.staticFriction = 1f;
                m_collider.material.dynamicFriction = 1f;
                m_collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            } else {
                m_collider.material.staticFriction = 1f;
                m_collider.material.dynamicFriction = 1f;
                m_collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
            }

            //m_collider.material = m_AirFrictionMaterial;
        }






        protected void InternalMovementTest()
        {
            return;
            m_useRootMotion = false;


            //  --------------------------
            //  IsKinematic
            //  --------------------------
            //  Calculkate drag.

            //  Get input direction.
            if (m_inputVector.sqrMagnitude > 1) m_inputVector.Normalize();
            m_inputDirection = Quaternion.Inverse(m_transform.rotation) * m_transform.InverseTransformDirection(m_inputVector);
            float fwdSpeed = 0;


            m_moveAngle = Mathf.Atan2(m_inputDirection.x, m_inputDirection.z) * Mathf.Rad2Deg;
            //var additionarlRot = Mathf.Lerp(0, m_rotationSpeed, Mathf.Abs(m_inputVector.x)) * m_deltaTime;
            m_moveRotation = Quaternion.AngleAxis(m_moveAngle * m_rotationSpeed * m_deltaTime, m_transform.up);


            //  Add acceleration to move direction.
            Vector3 acceleration = m_acceleration;
            acceleration.x = 0;
            acceleration.y = 0;
            acceleration.z = m_inputDirection.z * m_acceleration.z;

            m_moveDirection = acceleration;


            //m_moveDirection.x += acceleration.x;
            //m_moveDirection.y += acceleration.y;
            //m_moveDirection.z += acceleration.z;

            //if (m_moveAngle > 0 || m_moveAngle < 0) m_moveDirection = m_moveRotation * m_moveDirection;

            //m_moveDirection = Quaternion.Inverse(m_transform.rotation) * m_moveDirection;
            //  Calculate drag
            //Vector3 drag = m_moveDirection * (1 - m_motorDamping);
            //if (m_inputVector.sqrMagnitude > 0) {
            //    fwdSpeed = Vector3.Dot(m_inputDirection.normalized, m_moveDirection.normalized);
            //    drag *= (1f - fwdSpeed).Squared();
            //    //drag *= (1f - Vector3.Dot(m_inputVector, m_moveDirection.normalized));
            //}

            ////  Add drag to move direction.
            //if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
            //    m_moveDirection = Vector3.zero;
            //else
            //    m_moveDirection -= drag;
            //  Cap the move direction to desired speed.
            //if (m_moveDirection.sqrMagnitude > m_desiredSpeed * m_desiredSpeed)
            //    m_moveDirection = m_moveDirection.normalized * m_desiredSpeed * m_deltaTime;

            ////  Calculate rotation.
            //if (m_moveAngle > 0 || m_moveAngle < 0) {
            //    m_moveAngle = Mathf.Atan2(m_inputDirection.x, m_inputDirection.z) * Mathf.Rad2Deg;
            //    //var additionarlRot = Mathf.Lerp(0, m_rotationSpeed, Mathf.Abs(m_inputVector.x)) * m_deltaTime;
            //    m_moveRotation = Quaternion.AngleAxis(m_moveAngle * m_rotationSpeed * m_deltaTime, m_transform.up);
            //}

            //m_moveDirection = Quaternion.Inverse(m_transform.rotation) * m_moveDirection;
            m_moveDirection = Vector3.Project(m_moveDirection, m_transform.forward);
            m_movePosition = m_transform.position + m_moveDirection;

            float t = m_deltaTime * m_rotationSpeed;
            m_rigidbody.MoveRotation(Quaternion.Slerp(m_transform.rotation, m_moveRotation * m_transform.rotation, t * t));
            m_rigidbody.MovePosition(m_movePosition);


            //RigidbodyRotateAround(m_moveDirection, m_transform.up, m_moveAngle);



            DebugUI.Log(this, "m_velocity kin", m_velocity, RichTextColor.LightBlue);
            DebugUI.Log(this, "fwdSpeed", fwdSpeed, RichTextColor.LightBlue);
            //DebugUI.Log(this, "drag", drag, RichTextColor.LightBlue);
            DebugDraw.Arrow(m_transform.position.WithY(0.05f), m_transform.forward, Color.blue);

            DebugDraw.Arrow(m_transform.position.WithY(0.1f), m_moveDirection, Color.yellow);
            DebugDrawer.DrawPoint(m_movePosition);


            //m_velocity = (m_transform.position - m_previousPosition) / m_deltaTime;
            //m_previousVelocity = m_velocity;
            //m_previousPosition = m_transform.position;

            //var acceleration = new Vector3(0, 0, m_inputDirection.z * m_acceleration.z);
            //m_moveDirection = acceleration;
            //var finalVelocity = m_previousVelocity + acceleration;
            ////  Draw Forward.
            //m_moveDirection = m_transform.position + m_moveDirection;

            ////  velocity = move direction
            //var n1 = finalVelocity - (m_transform.position - m_moveDirection) * (1 - ((1 - m_motorDamping) * (1 - m_motorDamping))).Squared();
            //var n2 = 1 + (1 - (1 - m_motorDamping) * (1 - m_motorDamping));
            ////var n3 = 1 + m_motorDamping * m_deltaTime;
            //m_velocity = n1 / (n2 * n2);
            //m_movePosition = m_transform.position + m_velocity * m_deltaTime;


            //m_velocity = (m_transform.position - m_previousPosition) / m_deltaTime;
            //m_previousVelocity = m_velocity;
            //m_previousPosition = m_transform.position;


            //var acceleration = MathUtil.Multiply(m_inputVector, m_acceleration);
            //acceleration.x = 0;
            //acceleration.y = 0;
            ////acceleration.z *= (1 - (1 / ((1 - m_acceleration.z) * (1 - m_acceleration.z))));
            //m_moveDirection += acceleration;

            //var damp = (1 - m_motorDamping);
            //var n1 = m_moveDirection * (1 - damp * damp);
            //var n2 = (1 / damp * damp) * m_deltaTime;
            //var drag = n1 / (n2 * n2);

            ////  Add drag to move direction.
            //if (drag.sqrMagnitude > m_moveDirection.sqrMagnitude)
            //    m_moveDirection = Vector3.zero;
            //else
            //    m_moveDirection -= drag;
            //m_movePosition = m_transform.position + m_moveDirection;
            //m_velocity = Quaternion.Inverse(m_transform.rotation) * (m_transform.position + m_moveDirection);

        }







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



        protected virtual Vector3 GetLinearVelocity(bool world = false)
        {
            float moveAmount = Mathf.Abs(m_inputVector.x) + Mathf.Abs(m_inputVector.z);
            if (moveAmount >= 0.2f || m_inputVector.sqrMagnitude >= 0.2f)
            {
                Vector3 moveDirection = m_lookRotation * m_transform.forward;
                Vector3 forward = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
                Vector3 linearVelocity = Quaternion.FromToRotation(Vector3.forward, forward) * m_inputVector;
                linearVelocity = linearVelocity.Multiply(m_desiredSpeed);

                return world ? linearVelocity : Quaternion.Inverse(m_transform.rotation) * linearVelocity;
            }

            return Vector3.zero;
        }



        public RaycastHit GetSphereCastGroundHit()
        {
            float radius = 0.1f;
            Vector3 origin = m_transform.position + Vector3.up * (m_colliderCenter.y - m_colliderHeight / 2 + m_skinWidth);
            origin += Vector3.up * radius;

            m_groundHit = new RaycastHit();
            m_groundHit.point = m_transform.position - Vector3.up * m_airborneThreshold;
            m_groundHit.normal = m_transform.up;

            Physics.SphereCast(origin, radius, Vector3.down, out m_groundHit, m_airborneThreshold * 2, m_collisionsLayerMask);

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








        #region Public Functions






        // Scale the capsule collider to 'mlp' of the initial value
        protected void ScaleCapsule( float scale )
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
            for (int i = 0; i < m_probedColliders.Length; i++)
            {
                if (m_probedColliders[i] == null) break;
                m_probedColliders[i] = default;
            }
        }




        #endregion










        #region Debugging





        protected virtual void DebugAttributes()
        {
            if (debugger.states.showDebugUI == false) return;

            DebugUI.Log(this, "m_grounded", m_grounded, RichTextColor.White);


            DebugUI.Log(this, "m_moveDirection", m_moveDirection, RichTextColor.Green);
            DebugUI.Log(this, "m_moveAngle", m_moveAngle, RichTextColor.Green);
            DebugUI.Log(this, "m_moveRotation", m_moveRotation, RichTextColor.Green);

            DebugUI.Log(this, "m_velocity", m_velocity, RichTextColor.Cyan);
            DebugUI.Log(this, "r_velocity", m_rigidbody.velocity, RichTextColor.Cyan);
            DebugUI.Log(this, "fwd_speed", Vector3.Dot(m_rigidbody.velocity, m_transform.forward), RichTextColor.Cyan);

            //DebugUI.Log(this, "r_angularVelocity", m_rigidbody.angularVelocity, RichTextColor.Magenta);

            DebugUI.Log(this, "transformPos", m_transform.position, RichTextColor.Orange);
            DebugUI.Log(this, "rbPois", m_rigidbody.position, RichTextColor.Orange);
            DebugUI.Log(this, "rootPos", m_animator.rootPosition, RichTextColor.Orange);


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


