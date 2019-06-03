namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Text;

    //[DisallowMultipleComponent]
    //[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : RigidbodyController
    {
        public event Action<bool> OnAim = delegate {};
        public enum MovementType { Adventure, Combat };

        //  Locomotion variables
        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_Acceleration = 0.12f;
        [SerializeField, HideInInspector]
        protected float m_MotorDamping = 0.2f;       
        [SerializeField, HideInInspector]
        protected float m_MovementSpeed = 1f;
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 10f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceUp = 1f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceDown = 1.25f;
        [SerializeField, HideInInspector]
        protected float m_StopMovementThreshold = 0.2f;  


        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField, HideInInspector]
        protected CharacterAction m_ActiveAction;





        private MovementType m_MovementType = MovementType.Adventure;
        private bool m_Moving;
        private bool m_Aiming;
        private float m_InputMagnitude;
        private float m_InputAngle;
        private Vector3 m_Velocity, m_VerticalVelocity;
        [SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        private Vector3 m_LookDirection;
        private Quaternion m_LookRotation;
        [SerializeField, DisplayOnly]
        private Vector3 m_MoveDirection;




        private float m_StartAngle, m_StartAngleSmooth;


        private Vector3 m_VelocitySmooth;



        private bool m_CheckGround = true, m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;


        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;

        //  For Editor.
        [SerializeField, HideInInspector]
        private bool displayMovement = true, displayPhysics = true, displayActions = true;





        #region Properties

        public bool Moving{
            get { return m_Moving;}
            set { m_Moving = value; }
        }

        public bool Aiming{
            get{
                if (m_Aiming && Grounded)
                    return true;
                return false;
            }
            set { m_Aiming = value; }
        }

        public bool Grounded{
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public float RotationSpeed{
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public Vector3 InputVector{
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 MoveDirection{
            get { return m_MoveDirection; }
        }

        public Vector3 LookDirection{
            get { return m_LookDirection; }
            set { m_LookDirection = value; }
        }

        public Vector3 Velocity{
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public Quaternion LookRotation{
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public CharacterAction[] CharActions{
            get { return m_Actions; }
            set { m_Actions = value; }
        }

        public bool UseRootMotion
        {
            get { return m_UseRootMotion; }
            set { m_UseRootMotion = value; }
        }




        #endregion



        protected override void Awake()
        {
            base.Awake();
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();
        }



		protected void OnEnable()
		{
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_Rigidbody.mass = m_Mass;
            m_Animator.applyRootMotion = m_UseRootMotion;

            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}


		protected void OnDisable()
		{
            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}





        private void Update()
		{
            m_DeltaTime = Time.deltaTime;

            m_Animator.applyRootMotion = m_UseRootMotion;
            ////  Start Stop Actions.
            //StartStopActions();


            m_UpdateAnimator = true;
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];
                StopStartAction(charAction);

                if(charAction.IsActive){
                    if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
                }

                //  Call Action Update.
                charAction.UpdateAction();
            }


            UpdateAnimator();

            //  -----
            //  Debug messages
            if(m_Debug) DebugMessages();
            //  -----
        }


		private void FixedUpdate()
		{
            m_DeltaTime = Time.fixedDeltaTime;
            m_InputMagnitude = m_InputVector.magnitude;
            if (m_InputMagnitude < 0.01f || m_InputMagnitude > 0.99f)
                m_InputMagnitude = Mathf.Round(m_InputMagnitude);
            if (m_InputMagnitude > 1)
                m_InputVector.Normalize();

            m_Moving = m_InputMagnitude > m_StopMovementThreshold;
            //m_Moving = m_InputMagnitude > 0.2f;



            m_CheckMovement = true;
            m_CheckGround = true;
            m_UpdateRotation = true;
            m_UpdateMovement = true;
            //m_UpdateAnimator = true;
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];

                if (charAction.IsActive)
                {
                    if (m_CheckGround) m_CheckGround = charAction.CheckGround();
                    if (m_CheckMovement) m_CheckMovement = charAction.CheckMovement();
                    if (m_UpdateRotation) m_UpdateRotation = charAction.UpdateRotation();
                    if (m_UpdateMovement) m_UpdateMovement = charAction.UpdateMovement();

                    //if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
                }
            }  //  end of for loop

            SetPhysicsMaterial();

            CheckGround();

            CheckMovement();

            UpdateRotation();

            UpdateMovement();

            //UpdateAnimator();
        }


		//  Should the character look independetly of the camera?  AI Agents do not need to use camera rotation.
		public bool IndependentLook()
        {
            if(m_Moving || m_Aiming){
                return false;
            }
            //if(m_Aiming) return true;
            return true;
        }


        #region Ground Check

        private void DrawCheckGroundGizmo()
        {
            Gizmos.color = m_Grounded ? Color.green : Color.red;

            //Gizmos.color = groundRayHit ? Color.green : Color.red;
            Gizmos.DrawRay(groundCastOrigin, Vector3.down * groundCheckMaxDistance);
            //Gizmos.color = groundSphereHit ? darkGreen : darkRed;
            Gizmos.DrawRay(sphereCastOrigin, Vector3.down * sphereCastHitDistance);
            Gizmos.DrawWireSphere(sphereCastOrigin + Vector3.down * sphereCastHitDistance, sphereCastRadius);
            if (m_Grounded)
            {
                UnityEditor.Handles.color = groundRayHit ? Color.green : Color.red;
                UnityEditor.Handles.DrawSolidDisc(groundHitPoint, m_GroundNormal, 0.05f);

            }
        }

        Vector3 groundCastOrigin;
        float groundCheckMaxDistance;
        bool groundRayHit;
        Vector3 groundHitPoint;
        Vector3 sphereCastOrigin;
        float sphereCastMaxDistance, sphereCastHitDistance, sphereCastRadius;
        bool groundSphereHit;
        Color darkGreen = new Color(0, 0.5f, 0, 1);
        Color darkRed = new Color(0.5f, 0, 0, 1);

        #endregion

        protected override void CheckGround()
        {
            //  First, do anything that is independent of a character action.


            //  Does the current active character action override this method.
            if (m_CheckGround == true)
            {
                m_GroundDistance = 10;

                m_GroundCheckHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
                groundCastOrigin = m_Rigidbody.position + Vector3.up * m_GroundCheckHeight;
                groundCheckMaxDistance = m_CapsuleCollider.radius;
                groundRayHit = false;
                if (Physics.Raycast(groundCastOrigin, Vector3.down,
                                    out m_GroundHit, groundCheckMaxDistance, m_Layers.GroundLayer))
                {
                    if(m_GroundHit.transform != m_Transform)
                    {
                        groundRayHit = true;
                        //m_GroundDistance = m_Transform.position.y - m_GroundHit.point.y;
                        m_GroundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, m_Transform.up).magnitude;
                        m_GroundNormal = m_GroundHit.normal;
                    }

                }

                sphereCastOrigin = m_Rigidbody.position + Vector3.up * m_CapsuleCollider.radius;
                //sphereCastRadius = m_CapsuleCollider.radius - m_SkinWidth;
                sphereCastRadius = m_CapsuleCollider.radius * 0.9f;
                sphereCastMaxDistance = m_CapsuleCollider.radius + 2;
                sphereCastHitDistance = sphereCastMaxDistance;
                groundSphereHit = false;
                if (Physics.SphereCast(sphereCastOrigin, sphereCastRadius,
                                       Vector3.down, out m_GroundHit, sphereCastMaxDistance, m_Layers.GroundLayer))
                {
                    sphereCastHitDistance = m_GroundHit.distance;
                    groundSphereHit = true;
                    // check if sphereCast distance is small than the ray cast distance
                    if (m_GroundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
                        m_GroundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
                }




                m_GroundDistance = (float)Math.Round(m_GroundDistance, 2);
                var groundCheckDistance = 0.2f;

                if(m_GroundDistance < 0.05f)
                {
                    Vector3 horizontalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity);
                    m_Stickiness = m_GroundStickiness * horizontalVelocity.magnitude * m_AirbornThreshold;
                    m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundNormal) - 90;

                    //m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundNormal);

                    m_Grounded = true;
                }
                else
                {
                    if (m_GroundDistance >= groundCheckDistance)
                    {
                        m_InputVector = Vector3.zero;
                        m_GroundNormal = m_Transform.up;
                        m_SlopeAngle = 0;
                        m_Grounded = false;
                        m_Rigidbody.AddForce(m_Gravity * m_GravityModifier);
                    }
                       
                }

                if (m_Debug) Debug.DrawRay(m_GroundHit.point, m_GroundNormal, Color.magenta);

                groundHitPoint = m_Grounded ? m_GroundHit.point : Vector3.zero;
            }

        }



        //  Ensure the current movement direction is valid.
        protected override void CheckMovement()
        {
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_CheckMovement == true)
            {
                ////  Start walk angle
                Vector3 axisSign = Vector3.Cross(m_LookDirection, m_Transform.forward);
                m_InputAngle = Vector3.Angle(m_Transform.forward, m_LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
                m_InputAngle = (float)Math.Round(m_InputAngle, 2);
                //m_StartAngle = m_InputAngle;
                if (m_InputMagnitude <= 0.2f)
                    m_StartAngle = m_InputAngle;
                else if (m_InputMagnitude > 0.2f && Mathf.Abs(m_InputAngle) < 10)
                    m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
                m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);






                if (m_Grounded)
                {
                    float slopeForce;

                    if (m_SlopeAngle > 0){
                        slopeForce = m_SlopeForceUp;
                    }
                    else if (m_SlopeAngle > 0){
                        slopeForce = m_SlopeForceDown;
                    }
                    else{
                        slopeForce = 1;
                    }

                    m_MoveDirection = m_Rigidbody.rotation * Vector3.Cross(m_Transform.right, m_GroundNormal);

                    if (m_SlopeAngle > 0 || m_SlopeAngle < 0){
                        Vector3 slopeDirection = new Vector3(0,
                                    (1 - Mathf.Cos(m_SlopeAngle * Mathf.Deg2Rad) * m_CapsuleCollider.radius),
                                    (Mathf.Sin(m_SlopeAngle * Mathf.Deg2Rad) * m_CapsuleCollider.radius)) * m_CapsuleCollider.radius;
                        m_MoveDirection = Quaternion.Inverse(m_Rigidbody.rotation) * Vector3.RotateTowards(m_MoveDirection, slopeDirection, 10 * m_DeltaTime, 1f) * slopeForce;

                    }
                    Debug.DrawRay(m_Rigidbody.position + m_DebugHeightOffset, m_MoveDirection, Color.white);

                    m_Velocity = m_MoveDirection.normalized * m_MovementSpeed;



                }
                //  If airborne.
                else{
                    float m_AirSpeed = 6;
                    m_MoveDirection = (Quaternion.Inverse(m_Transform.rotation) * m_Transform.forward).normalized * m_AirSpeed;
                    m_Velocity = Vector3.Project(m_MoveDirection, m_Gravity * m_GravityModifier);
                }

                m_VerticalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity * m_GravityModifier);
                m_Velocity = Quaternion.Inverse(m_Rigidbody.rotation) * m_Velocity + m_VerticalVelocity;
                


            }
        }




        //  Update the rotation forces.
        protected override void UpdateRotation()
        {
            //  First, do anything that is independent of a character action.
            //  -----------

            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_UpdateRotation == true)
            {
                switch (m_MovementType)
                {
                    case (MovementType.Adventure):

                        if (m_LookDirection == Vector3.zero)
                            m_LookDirection = m_Transform.forward;

                        if (m_InputMagnitude > 0.2f && m_LookDirection.sqrMagnitude > 0.2f)
                        {
                            Vector3 local = m_Transform.InverseTransformDirection(m_LookDirection);
                            float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
                            if (m_InputVector == Vector3.zero)
                                angle *= (1.01f - (Mathf.Abs(angle) / 180)) * 1;

                            //m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up) * m_Rigidbody.rotation);

                            Quaternion rotation = Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up);
                            if (m_Grounded)
                            {
                                Vector3 d = transform.position - m_Animator.pivotPosition;
                                m_Rigidbody.MovePosition(m_Animator.pivotPosition + rotation * d);
                            }
                            m_Rigidbody.MoveRotation(rotation * m_Rigidbody.rotation);
                        }
                        return;
                    case (MovementType.Combat):

                        return;
                }

            }
        }



        //  Apply any movement.
        protected override void UpdateMovement()
        {
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_UpdateMovement == true)
            {
                switch (m_MovementType)
                {
                    case (MovementType.Adventure):

                        m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);

                        //if (m_Grounded)
                        //{
                        //    //m_Velocity = Vector3.SmoothDamp(m_Velocity, m_MoveDirection, ref m_VelocitySmooth, m_Acceleration);
                        //    //m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;

                        //    m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);
                        //    //m_Rigidbody.velocity = m_Velocity * m_DeltaTime + m_VerticalVelocity;
                        //}
                        //else
                        //{
                        //    //m_Rigidbody.AddForce(m_Gravity * m_GravityModifier);
                        //    m_Rigidbody.AddForce(m_VerticalVelocity);
                        //    //m_Rigidbody.velocity = m_Velocity;
                        //}

                        return;
                    case (MovementType.Combat):

                        return;
                }

            }
        }



        protected override void Move()
		{
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if(m_Move == true)
            {
                if (m_UseRootMotion)
                {
                    float angleInDegrees;
                    Vector3 rotationAxis;
                    m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
                    Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
                    m_Rigidbody.angularVelocity = angularDisplacement;



                    Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime);
                    velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                    //velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y * m_CapsuleCollider.height;

                    m_Rigidbody.velocity = velocity;
                    if (m_Grounded){
                        m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundNormal * m_Stickiness);
                    }


                }
                else
                {
                    m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                    m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_VelocitySmooth, m_Acceleration);
                    //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_MovementSpeed);
                    m_Rigidbody.velocity = m_Velocity * m_InputMagnitude;
                }
            }

		}


        //private void OnAnimatorMove()
        //{
        //    m_Move = true;
        //    for (int i = 0; i < m_Actions.Length; i++)
        //    {
        //        if (m_Actions[i].IsActive)
        //        {
        //            if (m_Move) m_Move = m_Actions[i].Move();
        //        }
        //    }
        //    Move();
        //}

        private void LateUpdate()
        {
            m_Move = true;
            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i].IsActive){
                    if (m_Move) m_Move = m_Actions[i].Move();
                }
            }
            Move();
        }



        protected void UpdateAnimator()
        {
            //  First, do anything that is independent of a character action.
            //  -----------

            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            m_Animator.speed = m_RootMotionSpeedMultiplier;

            //m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
            m_Animator.SetBool(HashID.Moving, m_Moving);
            m_Animator.SetFloat(HashID.InputMagnitude, m_InputMagnitude);
            m_Animator.SetFloat(HashID.InputAngle, (m_InputAngle * Mathf.Deg2Rad));
            //var localVelocity = Quaternion.Inverse(m_Transform.rotation) * (m_Rigidbody.velocity / m_Acceleration);
            if (m_Animator.pivotWeight > 0.75f){
                m_Animator.SetBool(HashID.StopLeftUp, true);
            }
            else if (m_Animator.pivotWeight < 0.25f){
                m_Animator.SetBool(HashID.StopRightUp, true);
            }
            else{
                m_Animator.SetBool(HashID.StopLeftUp, false);
                m_Animator.SetBool(HashID.StopRightUp, false);
            }
            m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);


            //  -----------
            //  Does a character action override the controllers update animator.
            //  -----------
            if (m_UpdateAnimator == true)
            {
                if(m_Grounded){
                    //  Movement Input
                    //m_Animator.applyRootMotion = true;
                    m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
                    m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
                }
                else{
                    //m_AnimationMonitor.SetForwardInputValue(0);
                    //m_AnimationMonitor.SetHorizontalInputValue(0);
                    //m_Animator.applyRootMotion = false;
                    m_Animator.SetFloat(HashID.ForwardInput, 0);
                    m_Animator.SetFloat(HashID.HorizontalInput, 0);
                }
            }
        }



        protected override void SetPhysicsMaterial()
        {
            // change the physics material to very slip when not grounded or maxFriction when is
            if (m_Grounded && m_InputMagnitude == 0)
                m_CapsuleCollider.material = m_GroundIdleFrictionMaterial;
            else if (m_Grounded && m_InputMagnitude != 0)
                m_CapsuleCollider.material = m_GroundedMovingFrictionMaterial;
            else
                m_CapsuleCollider.material = m_AirFrictionMaterial;
        }



        #region Public Functions

        public void SetPosition(Vector3 position){
            m_Rigidbody.MovePosition(position);
        }


        public void SetRotation(Quaternion rotation){
            m_Rigidbody.MoveRotation(rotation.normalized);
        }


		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            throw new NotImplementedException(string.Format("<color=yellow>{0}</color> MoveCharacter not Implemented.", GetType()));
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Moving = false;
        }


        #endregion



        #region Actions


        private void StopStartAction(CharacterAction charAction)
        {
            //  First, check if current Action can Start or Stop.

            //  Current Action is Active.
            if (charAction.enabled && charAction.IsActive)
            {
                //  Check if can stop Action is StopType is NOT Manual.
                if (charAction.StopType != ActionStopType.Manual)
                {
                    if (charAction.CanStopAction())
                    {
                        //  Start the Action and update the animator.
                        charAction.StopAction();
                        //  Reset Active Action.
                        if (m_ActiveAction = charAction)
                            m_ActiveAction = null;
                        //  Move on to the next Action.
                        return;
                    }
                }
            }
            //  Current Action is NOT Active.
            else
            {
                //  Check if can start Action is StartType is NOT Manual.
                if (charAction.enabled && charAction.StartType != ActionStartType.Manual)
                {
                    if (m_ActiveAction == null)
                    {
                        if (charAction.CanStartAction())
                        {
                            //  Start the Action and update the animator.
                            charAction.StartAction();
                            //charAction.UpdateAnimator();
                            //  Set active Action if not concurrent.
                            if (charAction.IsConcurrentAction() == false)
                                m_ActiveAction = charAction;
                            //  Move onto the next Action.
                            return;
                        }
                    }
                    else if (charAction.IsConcurrentAction())
                    {
                        if (charAction.CanStartAction())
                        {
                            //  Start the Action and update the animator.
                            charAction.StartAction();
                            //charAction.UpdateAnimator();
                            //  Move onto the next Action.
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }


        }


        public T GetAction<T>() where T : CharacterAction
        {
            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i] is T){
                    return (T)m_Actions[i];
                }
            }
            return null;
        }


        public bool TryStartAction(CharacterAction action)
        {
            if (action == null) return false;

            int index = Array.IndexOf(m_Actions, action);
            //  If there is an active action and current action is non concurrent.
            if(m_ActiveAction != null && action.IsConcurrentAction() == false){
                int activeActionIndex = Array.IndexOf(m_Actions, m_ActiveAction);
                //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
                if(index < activeActionIndex){
                    if (action.CanStartAction()){
                        //  Stop the current active action.
                        TryStopAction(m_ActiveAction);
                        //  Set the active action.
                        m_ActiveAction = m_Actions[index];
                        //m_ActiveActions[index] = m_Actions[index];
                        action.StartAction();
                        action.UpdateAnimator();
                        return true;
                    }
                } 
            }
            //  If there is an active action and current action is concurrent.
            else if (m_ActiveAction != null && action.IsConcurrentAction())
            {
                if (action.CanStartAction()){
                    //m_ActiveActions[index] = m_Actions[index];
                    action.StartAction();
                    action.UpdateAnimator();
                    return true;
                }
            }
            //  If there is no active action.
            else if (m_ActiveAction == null){
                if (action.CanStartAction())
                {
                    m_ActiveAction = m_Actions[index];
                    //m_ActiveActions[index] = m_Actions[index];
                    action.StartAction();
                    action.UpdateAnimator();
                    return true;
                }
            }


            return false;
        }


        public void TryStopAllActions()
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].IsActive)
                {
                    TryStopAction(m_Actions[i]);
                }
            }
        }


        public void TryStopAction(CharacterAction action)
        {
            if (action == null) return; 

            if (action.CanStopAction()){
                int index = Array.IndexOf(m_Actions, action);
                if (m_ActiveAction == action)
                    m_ActiveAction = null;


                action.StopAction();
                ActionStopped();
            }
        }


        public void TryStopAction(CharacterAction action, bool force)
        {
            if (action == null) return; 
            if(force){
                int index = Array.IndexOf(m_Actions, action);
                if (m_ActiveAction == action)
                    m_ActiveAction = null;


                action.StopAction();
                ActionStopped();
                return;
            }

            TryStopAction(action);
        }

        #endregion



        #region OnAction execute

        private void OnAimActionStart(bool aim)
        {
            m_Aiming = aim;
            m_MovementType = m_Aiming ? MovementType.Combat : MovementType.Adventure;
            //  Call On Aim Delegate.
            OnAim(aim);

            //CameraController.Instance.FreeRotation = aim;
            //Debug.LogFormat("Camera Free Rotation is {0}", CameraController.Instance.FreeRotation);
        }


        private void OnActionActive(CharacterAction action, bool activated)
        {
            int index = Array.IndexOf(m_Actions, action);
            if (action == m_Actions[index])
            {
                if (m_Actions[index].enabled)
                {
                    if(activated)
                    {
                        //Debug.LogFormat(" {0} is starting.", action.GetType().Name);

                    }
                    else
                    {

                    }
                }
            }

        }


        public void ActionStopped()
        {

        }

        #endregion








        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            DrawCheckGroundGizmo();

            if (m_Rigidbody == null) return;
            //  Move direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(m_Rigidbody.position + m_DebugHeightOffset, m_Transform.InverseTransformDirection(m_MoveDirection));
            //GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_MoveDirection, Color.white);
            //  Velocioty
            //Gizmos.color = Color.cyan;
            //Gizmos.DrawRay(m_Rigidbody.position + m_DebugHeightOffset, m_Transform.InverseTransformDirection(m_Velocity));
        }









        private void DebugMessages()
        {
            //debugMessages.AppendFormat("Ground Distance: {0} \n", m_GroundDistance);
            //debugMessages.AppendFormat("Grounded: {0} \n", m_Grounded);
            //debugMessages.AppendFormat("deltaAngle: {0} \n", deltaAngle);
            debugMsgs = new string[]
            {
                string.Format("Ground Distance: {0}", m_GroundDistance),
                string.Format("Grounded: {0}", m_Grounded),
                string.Format("m_InputMagnitude: {0}", m_InputMagnitude ),
                //string.Format("VelocityDot: {0}", (float)Math.Round(Vector3.Dot(m_VerticalVelocity, m_Gravity), 2) ),
                string.Format("SlopeAngle: {0}", m_SlopeAngle),
                string.Format("RigidbodyVelY: {0}", (float)Math.Round(m_Rigidbody.velocity.y, 2)),

                //string.Format("Next State (short): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).shortNameHash),
                //string.Format("Next State (full): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).fullPathHash),


                //string.Format("Current State (short): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash),

                //string.Format("Current State (full): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash),
                //string.Format("In Transition?: {0}\n", m_Animator.IsInTransition(0)),
                //string.Format("Transition Norm Time: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).normalizedTime),
                //string.Format("Transition Duration: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).duration),
            };
        }


        string[] debugMsgs;
        Camera mainCamera;
        private void OnGUI()
        {
            if (Application.isPlaying && m_Debug)
            {
                if (mainCamera == null)
                {
                    mainCamera = Camera.current;
                }


                GUI.color = CharacterControllerUtility.DebugTextColor;
                Rect rect = CharacterControllerUtility.CharacterControllerRect;


                GUI.BeginGroup(rect, GUI.skin.box);

                //GUI.Label(rect, debugMessages.ToString(0, debugMessages.Length));

                for (int i = 0; i < debugMsgs.Length; i++)
                {
                    rect.y = 16 * i;
                    //GUI.Label(rect, debugMsgs[i]);
                    GUI.Label(rect, debugMsgs[i], CharacterControllerUtility.GuiStyle);
                }

                GUI.EndGroup();
            }

        }






    }

}
