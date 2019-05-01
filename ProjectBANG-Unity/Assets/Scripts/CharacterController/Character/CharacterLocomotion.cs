namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;


    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
    {
        public event Action<bool> OnAim = delegate {};
        public enum MovementType { FreeMovement, Combat };

        //  Locomotion variables
        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_Acceleration = 0.12f;
        [SerializeField, HideInInspector]
        protected float m_MotorDamping = 0.2f;             //  TODO:  Need to add to editor
        [SerializeField, HideInInspector]
        protected float m_MovementSpeed = 1f;
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 10f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceDown = 1.25f;

        protected float m_StopMovementThreshold = 0.5f;     //  TODO:  Need to add to editor


        //  Physics variables
        [SerializeField, HideInInspector]
        protected float m_Mass = 100;
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector, Range(0, 90)]
        protected float m_SlopeLimit = 45f;
        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.25f;
        [SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
        protected float m_StepOffset = 0.15f;

        [SerializeField, HideInInspector]
        protected float m_AlignToGroundDepthOffset = 0.5f;


        //[SerializeField, HideInInspector]
        protected float m_GravityModifier = 2f;
        [SerializeField, HideInInspector]
        protected bool m_AlignToGround = true;


        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;




        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;




        [Header("-- Debug --")]
        [SerializeField] bool m_DrawDebugLine;
        //[SerializeField, DisplayOnly]
        private MovementType m_MovementType = MovementType.Combat;
        [SerializeField, DisplayOnly]
        private bool m_Grounded = true;
        //[SerializeField, DisplayOnly]
        private bool m_Aiming, m_Moving;
        //[SerializeField, DisplayOnly]
        private float m_InputMagnitude, m_YawRotation;
        //[SerializeField, DisplayOnly]
        private Vector3 m_Velocity;
        [SerializeField, DisplayOnly]
        private Vector3 m_InputVector, m_RelativeInputVector;
        private Vector3 m_LookDirection;
        private Quaternion m_LookRotation;

        [SerializeField, DisplayOnly]
        private Vector3 GroundNormal;


        private CapsuleCollider m_CapsuleCollider;
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;
        private LayerManager m_Layers;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;
        private float m_FixedDeltaTime;

        private bool m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;


        private RaycastHit m_GroundHit, m_StepHit;
        private bool m_OnStep;
        [SerializeField, DisplayOnly]
        private float m_SlopeAngle;
        private Vector3 m_StepRayStart, m_StepRayDirection;

        private Vector3 m_velocitySmooth;

        [SerializeField] 
        private Vector3 RIGIDBODY_VELOCITY;





        #region Properties

        public bool Moving{
            get{
                //m_Moving = Mathf.Abs(InputVector.x) < 1 && Mathf.Abs(InputVector.z) < 1;
                return m_Moving;
            }
            set { m_Moving = value; }
        }

        public bool Aiming{
            get{
                if (m_Aiming && Grounded)
                    return true;
                return false;
            }
        }

        public float TurnAmount{
            get { return m_YawRotation; }
            set { m_YawRotation = value; }
        }

        public float RotationSpeed{
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }


        public bool AimState{
            get { return m_Aiming; }
            set { m_Aiming = value; }
        }



        public Vector3 InputVector{
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 RelativeInputVector{
            get { return m_RelativeInputVector; }
        }

        public Quaternion LookRotation{
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public bool Grounded{
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public Vector3 Velocity{
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }


        public CharacterAction[] CharActions{
            get { return m_Actions; }
            set { m_Actions = value; }
        }

        public Vector3 LookDirection{
            get { return m_LookDirection; }
            set { m_LookDirection = value; }
        }




        #endregion



        protected void Awake()
        {
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Layers = GetComponent<LayerManager>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_GameObject = gameObject;
            m_Transform = transform;

            m_DeltaTime = Time.deltaTime;
            m_FixedDeltaTime = Time.fixedDeltaTime;

            if(m_Rigidbody == null) m_Rigidbody = m_GameObject.AddComponent<Rigidbody>();
            if(m_Layers == null) m_Layers = m_GameObject.AddComponent<LayerManager>();

            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_Rigidbody.mass = m_Mass;
        }


        protected void OnEnable()
		{
            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}


		protected void OnDisable()
		{
            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}





        private void Update()
		{
            //if (m_DeltaTime == 0) return;

            m_InputMagnitude = m_InputVector.magnitude;
            if (m_InputMagnitude > 1)
                m_InputVector.Normalize();
            m_RelativeInputVector = m_Transform.TransformDirection(m_InputVector);
            m_RelativeInputVector = Quaternion.Inverse(m_Transform.rotation) * m_RelativeInputVector;
            //m_RelativeInputVector = m_Transform.right * m_InputVector.x + m_Transform.forward * m_InputVector.z;
            m_Moving = m_InputMagnitude > 0.0f;




            //  Start Stop Actions.
            StartStopActions();



            //  Check if grounded.
            CheckGround();
            if (m_CheckMovement)
                CheckMovement();
            if (m_UpdateRotation)
                UpdateRotation();
            if (m_UpdateMovement)
                UpdateMovement();
            if (m_UpdateAnimator)
                UpdateAnimator();

            //  Is Moving.
            //m_Moving = m_InputMagnitude > m_StopMovementThreshold;
            m_Moving = m_InputMagnitude > 0.0f;
            m_Animator.SetBool(HashID.Moving, m_Moving);

            RIGIDBODY_VELOCITY = m_Rigidbody.velocity;
		}


		private void FixedUpdate()
        {
            //if (m_Grounded)
            //{
            //    if (m_InputMagnitude > 0)
            //    {
            //        if (Mathf.Abs(m_SlopeAngle) > 0)
            //        {
            //              Going down a slope
            //            if(m_SlopeAngle < 0)
            //            m_Rigidbody.AddForce(Vector3.up * m_SlopeForceDown * m_SlopeAngle * m_DeltaTime + Physics.gravity, ForceMode.VelocityChange);
            //            //  Going up a slope.
            //            else
            //            m_Rigidbody.AddForce(Vector3.up * m_SlopeForceDown * m_SlopeAngle * m_DeltaTime + Physics.gravity, ForceMode.VelocityChange);
            //        }

            //    }
            //}

            if (m_InputMagnitude > 0)
            {
                if (!m_Grounded)
                    m_Rigidbody.AddForce((m_Transform.up + Physics.gravity) * m_DeltaTime, ForceMode.VelocityChange);
            }


            //if (m_SlopeAngle >= m_SlopeLimit)
            //return;
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


        private void CheckGround()
        {
            if (m_CapsuleCollider != null)
            {
                float distance = 10f;
                //  Position of the SphereCast origin starting at the base of the capsule.
                Vector3 pos = m_Transform.position + Vector3.up * (m_CapsuleCollider.radius);

                if (Physics.Raycast(m_Transform.position + Vector3.up * (m_CapsuleCollider.height / 2), Vector3.down,
                                   out m_GroundHit, m_CapsuleCollider.height / 2 + 2f, m_Layers.GroundLayer))
                {
                    distance = m_Transform.position.y - m_GroundHit.point.y;
                }
                if (Physics.SphereCast(m_Transform.position + Vector3.up * (m_CapsuleCollider.radius), m_CapsuleCollider.radius * 0.9f,
                                       -Vector3.up, out m_GroundHit, m_CapsuleCollider.radius + 2f, m_Layers.GroundLayer))
                {
                    // check if sphereCast distance is small than the ray cast distance
                    if (distance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
                        distance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
                }
                var groundDistance = (float)Math.Round(distance, 2);
                if(groundDistance <= 0.05f){
                    m_SlopeAngle = (float)Math.Round(Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90, 2);
                    m_Grounded = true;
                }
                else
                {
                    m_SlopeAngle = 0;
                    m_Grounded = false;
                    //m_Rigidbody.AddForce((m_Transform.up + Physics.gravity) * m_DeltaTime, ForceMode.VelocityChange);
                }
            }


            //if(Physics.SphereCast(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, m_CapsuleCollider.radius * 0.9f, 
            //                      -Vector3.up, out m_GroundHit, m_CapsuleCollider.radius + m_SkinWidth, m_Layers.GroundLayer))
            //{
            //    m_SlopeAngle = (float)Math.Round(Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90, 2);
            //    m_Grounded =  true;
            //}
            //else{
            //    m_SlopeAngle = 0;
            //    m_Grounded = false;
            //}

            GroundNormal = m_GroundHit.normal;
        }


        //  Ensure the current movement direction is valid.
        private void CheckMovement()
        {
            
            //if ((m_Moving && m_InputVector.z == 0) &&
            //    (m_Velocity.x > 0 && m_InputVector.x < 0) ||
            //    (m_Velocity.x < 0 && m_InputVector.x > 0) )
            //{
            //    //Debug.LogFormat("InputX: {0} | Velocity.x {1}", m_InputVector.x, m_Velocity.x);

            //}

            //if ((m_Moving && m_InputVector.z == 0) &&
            //    (m_Velocity.x > 0 && m_InputVector.x < 0))
            //{
            //    Debug.LogFormat("InputX: {0} | Velocity.x {1}", m_InputVector.x, m_Velocity.x);

            //}


            if(m_Grounded)
            {
                if (m_InputMagnitude > 0.1f)
                {
                    m_OnStep = false;
                    m_StepRayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius * 2);  //+ m_SkinWidth);
                    m_StepRayDirection = Vector3.down;

                    if (Physics.Raycast(m_StepRayStart, m_StepRayDirection, out m_StepHit, m_MaxStepHeight - m_SkinWidth, m_Layers.GroundLayer) && !m_StepHit.collider.isTrigger)
                    {
                        if(m_StepHit.normal == Vector3.up)
                        {
                            if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_MaxStepHeight + m_SkinWidth))
                            {
                                m_Velocity = Vector3.Lerp(m_Velocity + (m_StepHit.point - m_Transform.position), m_Velocity, m_StepSpeed * m_DeltaTime);
                                m_Transform.position += (Vector3.up * (m_StepOffset)) * m_InputMagnitude + (m_Velocity * m_DeltaTime);
                                m_Transform.position += m_Velocity * m_DeltaTime;

                                m_Rigidbody.AddForce(Vector3.up * m_SlopeForceDown * Time.deltaTime, ForceMode.VelocityChange);
                                //m_Velocity = m_Velocity + Vector3.up * m_SlopeForceDown;

                                m_OnStep = true;
                            }
                        }
                    }
                }
                //  Not grounded.
                else {
                    m_Velocity = Vector3.zero;
                }
            }
        }


        float smoothYawRotation, m_yawRotationVelocity;
        //  Update the rotation forces.
        private void UpdateRotation()
        {
            if (m_LookDirection == Vector3.zero)
                m_LookDirection = m_Transform.forward;

            m_LookDirection.y = 0;
            m_LookDirection.Normalize();
            m_LookRotation = Quaternion.LookRotation(m_LookDirection, Vector3.up);


            if (m_Moving){
                var deltaYaw = m_YawRotation * Mathf.Rad2Deg * m_DeltaTime;
                m_YawRotation = Mathf.SmoothDamp(m_YawRotation, deltaYaw, ref m_yawRotationVelocity, 0.15f);
                if (Mathf.Abs(m_YawRotation) < 1f)
                    m_YawRotation = Mathf.SmoothDamp(m_YawRotation, 0, ref m_yawRotationVelocity, 0.1f);
                m_YawRotation = (float)Math.Round(m_YawRotation, 4);
            }
            else{
                //m_YawRotation = m_YawRotation * m_RotationSpeed * Mathf.Rad2Deg * m_DeltaTime;
                //if (Mathf.Abs(m_YawRotation) < 0.8f)
                //    m_YawRotation = 0;
                //else
                //    m_YawRotation = Mathf.Clamp(m_YawRotation, -1, 1);
                //m_YawRotation = (float)Math.Round(m_YawRotation, 4);
                m_YawRotation = 0;
            }


            if(m_Moving){
                m_LookRotation = Quaternion.Slerp(m_Transform.rotation, m_LookRotation, m_RotationSpeed * m_DeltaTime);
                //m_Transform.rotation = m_LookRotation;
                m_Rigidbody.MoveRotation(m_LookRotation.normalized);
            }
        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            ////  Keep the character glued to the ground.
            //if (Mathf.Abs(Vector3.Angle(m_GroundHit.normal, Vector3.up)) < m_SlopeLimit)
                //m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundHit.normal);

            m_Rigidbody.drag = 0;
            m_Velocity = Vector3.SmoothDamp(m_Velocity, m_RelativeInputVector, ref m_velocitySmooth, m_Acceleration);
            //m_Velocity = Quaternion.Inverse(m_Transform.rotation) * m_Velocity;
            m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
            //m_Rigidbody.AddForce(m_Velocity, ForceMode.VelocityChange);
            //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_velocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);

            if(m_Grounded)
            {
                //if(m_InputMagnitude > m_StopMovementThreshold){
                //    m_Rigidbody.drag = 0;
                //    m_Velocity = Vector3.SmoothDamp(m_Velocity, m_RelativeInputVector, ref m_velocitySmooth, m_Acceleration);
                //    //m_Velocity = Quaternion.Inverse(m_Transform.rotation) * m_Velocity;
                //    m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                //    //m_Rigidbody.AddForce(m_Velocity, ForceMode.VelocityChange);
                //    //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_velocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);
                //}
                //else{
                //    //m_Rigidbody.drag = 10;
                //    m_Velocity = Vector3.SmoothDamp(m_Velocity, Vector3.zero, ref m_velocitySmooth, m_MotorDamping);
                //}
            }
        }



		private void Move()
		{
            m_Animator.applyRootMotion = m_UseRootMotion;

            if(m_UseRootMotion)
            {
                var velocity = (m_Animator.deltaPosition / m_DeltaTime);
                velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_velocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);
                //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_MovementSpeed);
                m_Rigidbody.velocity = velocity;
            }
		}


		private void UpdateAnimator()
        {
            // Rotation
            m_Animator.SetFloat(HashID.Rotation, m_YawRotation, 0.1f, m_DeltaTime);
            //var localVelocity = Quaternion.Inverse(m_Transform.rotation) * (m_Rigidbody.velocity / m_Acceleration);
            //  Movement Input
            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);


            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            m_Animator.speed = m_RootMotionSpeedMultiplier;
        }



        private void OnAnimatorMove()
        {
            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i].IsActive){
                    if (m_Move) m_Move = m_Actions[i].Move();
                }
            }
            if (m_Move) Move();
        }




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









        #region Actions

        private void StartStopActions()
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                //  First, check if current Action can Start or Stop.
                var currentAction = m_Actions[i];
                //  Current Action is Active.
                if (m_Actions[i].IsActive)
                {
                    //  Check if can stop Action is StopType is NOT Manual.
                    if (m_Actions[i].StopType != ActionStopType.Manual)
                    {
                        if (m_Actions[i].CanStopAction())
                        {
                            //  Start the Action and update the animator.
                            m_Actions[i].StopAction();
                            //m_Actions[i].UpdateAnimator();
                            //  Reset Active Action.
                            if (m_ActiveAction = m_Actions[i])
                                m_ActiveAction = null;
                            //  Move on to the next Action.
                            continue;
                        }
                    }
                }
                //  Current Action is NOT Active.
                else
                {
                    //  Check if can start Action is StartType is NOT Manual.
                    if (m_Actions[i].StartType != ActionStartType.Manual)
                    {
                        if (m_ActiveAction == null)
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                m_Actions[i].UpdateAnimator();
                                //  Set active Action if not concurrent.
                                if (m_Actions[i].IsConcurrentAction() == false)
                                    m_ActiveAction = m_Actions[i];
                                //  Move onto the next Action.
                                continue;
                            }
                        }
                        else if (m_Actions[i].IsConcurrentAction())
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                //m_Actions[i].UpdateAnimator();
                                //  Move onto the next Action.
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }


                //  Next, if current Action is active, update overrides.
                m_CheckMovement = m_UpdateRotation = m_UpdateMovement = m_UpdateAnimator = m_Move = true;
                //  Call Action Update.
                m_Actions[i].UpdateAction();

                if (m_Actions[i].IsActive)
                {
                    if (m_CheckMovement)
                    {
                        m_CheckMovement = m_Actions[i].CheckMovement();
                    }
                    if (m_UpdateRotation)
                    {
                        m_UpdateRotation = m_Actions[i].UpdateRotation();
                    }
                    if (m_UpdateMovement)
                    {
                        m_UpdateMovement = m_Actions[i].UpdateMovement();
                    }
                    if (m_UpdateAnimator)
                    {
                        m_UpdateAnimator = m_Actions[i].UpdateAnimator();
                    }
                }


            }  //  end of for loop
            //  end of
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



        #region On Action execute

        private void OnAimActionStart(bool aim)
        {
            m_Aiming = aim;
            m_MovementType = m_Aiming ? MovementType.Combat : MovementType.FreeMovement;
            //  Call On Aim Delegate.
            OnAim(aim);

            //CameraController.Instance.FreeRotation = aim;
            //Debug.LogFormat("Camera Free Rotation is {0}", CameraController.Instance.FreeRotation);
        }


        private void OnActionActive(CharacterAction action, bool activated)
        {
            //int index = Array.IndexOf(m_Actions, action);
            //if(activated){
            //    m_ActiveActions[index] = m_Actions[index];
            //    Debug.LogFormat(" {0} is active.", action.GetType().Name);
            //}
            //else{
            //    m_ActiveActions[index] = null;
            //    Debug.LogFormat(" {0} is now not active.", action.GetType().Name);
            //}
        }


        public void ActionStopped()
        {

        }

        #endregion















		private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);
        private Color _Magenta = new Color(0.8f, 0, 0.8f, 0.8f);

        protected void OnDrawGizmos()
        {
            //float slopeCheckHeight = 0.5f;
            //Quaternion rotation = Quaternion.AngleAxis(m_SlopeLimit, -transform.right);
            ////  Hypotenuse
            //Gizmos.color = _Magenta;
            //float slopeCheckHypotenuse = slopeCheckHeight / Mathf.Cos(m_SlopeLimit);
            //Vector3 slopeAngleVector = slopeCheckHypotenuse * transform.forward;
            //Gizmos.DrawRay(transform.position + (transform.forward) * 0.3f, rotation * slopeAngleVector - (slopeAngleVector * 0.3f));

            //  Check distance
            //Gizmos.color = Color.magenta;
            //float slopeCheckDistance = Mathf.Tan(m_SlopeLimit) * slopeCheckHeight;
            //Vector3 slopeCheckVector = slopeCheckDistance * transform.forward;
            //Gizmos.DrawRay(transform.position + transform.up * slopeCheckHeight, slopeCheckVector );//- (transform.forward) * 0.3f);

            if(m_DrawDebugLine && Application.isPlaying)
            {
                //Gizmos.color = Color.blue;
                //Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_Velocity);
                //GizmosUtils.DrawString("m_Velocity", m_Transform.position + m_Velocity, Color.white);

                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_LookDirection);
                GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_LookDirection, Color.white);

                Gizmos.color = m_OnStep ? Color.yellow : Color.white;
                Gizmos.DrawRay(m_StepRayStart, m_StepRayDirection * (m_MaxStepHeight - m_SkinWidth));
                GizmosUtils.DrawString("Step", m_StepRayStart + Vector3.up * m_MaxStepHeight, Color.white);



            }
        }



        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent();
        Vector2 size;
        //Color debugTextColor = new Color(0, 1f, 1f, 1);
        Color debugTextColor = Color.black;

        GUIStyle textStyle = new GUIStyle();
        Rect location = new Rect();
        bool showDebugPanel;
        private void OnGUI()
        {
            //if (Application.isPlaying)
            //{
            //    GUI.color = debugTextColor;
            //    textStyle.fontStyle = FontStyle.Bold;

            //    content.text = "";
            //    content.text += string.Format("YawRotation: {0}\n", m_YawRotation);
            //    content.text += string.Format("Moving: {0} | InputMag {1}\n", m_Moving, m_InputMagnitude);
            //    content.text += string.Format("Slope Angle: {0}\n", m_SlopeAngle);
            //    content.text += string.Format("Is Grounded: {0}\n", m_Grounded);
            //    content.text += string.Format("RelativeInputVector: {0}\n", m_RelativeInputVector);
            //    content.text += string.Format("Velocity: {0}\n", m_Velocity);
            //    content.text += string.Format("Rigidbody Velocity: {0}\n", m_Rigidbody.velocity);

            //    size = new GUIStyle(GUI.skin.label).CalcSize(content);
            //    location.Set(5, 15, size.x * 1.5f , size.y * 2);
            //    GUILayout.BeginArea(location, GUI.skin.box);
            //    GUILayout.Label(content);
            //    //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
            //    GUILayout.EndArea();
            //}

        }



    }

}
