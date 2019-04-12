namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;


    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
    {
        public event Action<bool> OnAim = delegate {};
        public enum MovementType {FreeMovement, Combat };


        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_MovementSpeed = 3.75f;
        [SerializeField, HideInInspector]
        protected float m_StrafeSpeed = 2.5f;
        [SerializeField, HideInInspector]
        protected float m_SpeedChangeMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 4f;
        [SerializeField, HideInInspector]
        protected float m_AimRotationSpeed = 8f;
        [SerializeField, HideInInspector]
        protected bool m_AlignToGround = true;
        [SerializeField, HideInInspector]
        protected float m_AlignToGroundDepthOffset = 0.5f;
        [SerializeField, HideInInspector]
        protected Vector3 m_GroundSpeed = new Vector3(1, 0, 1);
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector]
        protected float m_SlopeLimit = 45f;

        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.65f;
        [SerializeField, HideInInspector, Range(0, 0.3f) ]
        protected float m_StepOffset = 0.15f;
        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;
        [SerializeField, HideInInspector]
        protected float m_Acceleration = 20f;



        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;




        [Header("-- Debug --")]
        [SerializeField, DisplayOnly]
        private MovementType m_MovementType = MovementType.FreeMovement;
        [SerializeField, DisplayOnly]
        private bool m_Grounded = true;
        private bool m_Moving, m_Aiming, m_Running = true;
        [SerializeField, DisplayOnly]
        private float m_Speed;//, m_Direction;
        //[SerializeField, DisplayOnly]
        private float m_MoveAmount, m_TurnAmount;
        //[SerializeField, DisplayOnly]
        private Vector3 m_Velocity, m_RootMotionVelocity, m_MoveDirection;
        //[SerializeField, DisplayOnly]
        private Vector3 m_InputVector, m_RelativeInputVector;
        [SerializeField, DisplayOnly]
        private Quaternion m_LookRotation;


        private Vector3 m_LookDirection, m_LookAtPoint;





        private CapsuleCollider m_CapsuleCollider;
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;
        private LayerManager m_Layers;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;
        private float m_FixedDeltaTime;

        bool m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;

        private RaycastHit m_GroundHit;
        private RaycastHit m_StepHit;
        private float m_SlopeAngle;
        private Vector3 m_StepRayStart, m_StepRayDirection;

        [SerializeField] bool m_DrawDebugLine;
        [SerializeField] 
        private Vector3 RIGIDBODY_VELOCITY;




        #region Properties

        public bool Moving{
            get{
                m_Moving = Mathf.Abs(InputVector.x) < 1 && Mathf.Abs(InputVector.z) < 1;
                return m_Moving;
            }
            set { m_Moving = value; }
        }

        public bool Running{
            get { return m_Running; }
            set { m_Running = value; }
        }

        public bool Aiming{
            get{
                if (m_Aiming && Grounded)
                    return true;
                return false;
            }
        }

        public float TurnAmount{
            get { return m_TurnAmount; }
            set { m_TurnAmount = value; }
        }

        public float RotationSpeed{
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public float AimRotationSpeed{
            get { return m_AimRotationSpeed; }
            set { m_AimRotationSpeed = value; }
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

        public Vector3 LookAtPoint{
            get { return m_LookAtPoint; }
            set { m_LookAtPoint = value; }
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

        }


        protected void OnEnable()
		{
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;


            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}

		protected void OnDisable()
		{
            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}

        //[SerializeField]
        //float angleOut, direction;

		private void Update()
		{
            if (m_DeltaTime == 0) return;

            if (m_InputVector.magnitude > 1)
                m_InputVector.Normalize();
            m_MoveAmount = Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z);
            //  Check if moving.
            m_Moving = m_InputVector.sqrMagnitude > 0.2f;



            //m_Grounded = CheckGround();
            m_Grounded = CheckGround_2();




            //  Start Stop Actions.
            StartStopActions();
            //
            m_UpdateRotation = m_UpdateMovement = m_UpdateAnimator = m_Move = m_CheckMovement = true;




            for (int i = 0; i < m_Actions.Length; i++)
            {
                //  Next, if current Action is active, update overrides.
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
                    //  Call Action Update.
                    m_Actions[i].UpdateAction();
                }
            }

            //if (m_Move)
                //Move();
            if (m_UpdateAnimator)
                UpdateAnimator();

            RIGIDBODY_VELOCITY = m_Rigidbody.velocity;
		}


		private void FixedUpdate()
        {
            if (m_UpdateRotation)
                UpdateRotation();
            if (m_UpdateMovement)
                UpdateMovement();
            
            if(!m_Grounded)
                m_Rigidbody.AddForce((Physics.gravity * 2) - Physics.gravity);
        }


        //  Should the character look independetly of the camera?  AI Agents do not need to use camera rotation.
        public bool IndependentLook()
        {
            if(m_Moving || m_Aiming){
                return false;
            }

            //if(m_Aiming){
            //    return true;
            //}

            return true;
        }


        private bool CheckGround_2()
        {
            //m_MoveDirection = m_Transform.forward * m_InputVector.z + m_Transform.right * m_InputVector.x;

            if (m_Grounded){
                m_MoveDirection = m_Moving ? Vector3.Cross(m_Transform.right, m_GroundHit.normal) : Vector3.zero;
                m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;
            }else{
                m_MoveDirection = Vector3.zero;
                m_SlopeAngle = 0;
            }


            if(Physics.SphereCast(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, m_CapsuleCollider.radius * 0.9f, 
                                  -Vector3.up, out m_GroundHit, m_CapsuleCollider.radius + m_SkinWidth, m_Layers.SolidLayer))
            {
                if (m_InputVector.sqrMagnitude > 0.1f && m_Grounded)
                {
                    m_StepRayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius * 2);  //+ m_SkinWidth);
                    m_StepRayDirection = Vector3.down * (m_MaxStepHeight - m_StepOffset);

                    if (m_DrawDebugLine) Debug.DrawRay(m_StepRayStart, m_StepRayDirection, Color.yellow);

                    if (Physics.Raycast(m_StepRayStart, m_StepRayDirection, out m_StepHit, m_MaxStepHeight - m_StepOffset, m_Layers.SolidLayer) && !m_StepHit.collider.isTrigger ){
                        if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_StepOffset + m_SkinWidth)){
                            //m_MoveDirection = (m_StepHit.point - m_Transform.position).normalized;
                            m_MoveDirection = Vector3.Lerp(m_MoveDirection, (m_StepHit.point - m_Transform.position).normalized, m_StepSpeed * m_DeltaTime );
                            m_Rigidbody.velocity += (m_MoveDirection + (Vector3.up * m_StepSpeed)) * m_StepSpeed * m_DeltaTime;
                            m_Transform.position += (m_MoveDirection + (Vector3.up * m_StepSpeed)) * m_StepSpeed * m_DeltaTime;
                        }
                    }
                }

                if (m_DrawDebugLine) Debug.DrawRay(m_Transform.position, m_MoveDirection, Color.cyan);

                if(m_AlignToGround){
                    if (Mathf.Abs(Vector3.Angle(m_GroundHit.normal, Vector3.up)) < 85f)
                        m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundHit.normal);
                } else {
                    Vector3 targetPosition = m_Transform.position;
                    targetPosition.y = m_GroundHit.point.y;
                    m_Transform.position = targetPosition;
                }

                //Vector3 targetPosition = m_Transform.position;
                //targetPosition.y = m_GroundHit.point.y;
                //m_Transform.position = targetPosition;
                
                return true;
            }

            return false;
        }




        private bool CheckGround()
        {
            Color _stepColor = Color.blue;


            if(m_Grounded) {
                m_MoveDirection = m_Moving ? Vector3.Cross(m_Transform.right, m_GroundHit.normal) : Vector3.zero;
                m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;
            } else {
                m_MoveDirection = Vector3.zero;
                m_SlopeAngle = 0;
            }

            //  -- DEBUG DRAW RAY -- 
            if(m_DrawDebugLine) Debug.DrawRay(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down, Color.white);


            //if (Physics.Raycast(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down, out m_GroundHit, m_AlignToGroundDepthOffset + m_SkinWidth, m_Layers.SolidLayer))
            if (Physics.SphereCast(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, m_CapsuleCollider.radius * 0.9f,
                      -Vector3.up, out m_GroundHit, m_CapsuleCollider.radius + m_SkinWidth, m_Layers.SolidLayer))
            {


                var rayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius * 2);  //+ m_SkinWidth);
                var rayEnd = Vector3.down * (m_MaxStepHeight - m_StepOffset);
                //  -- DEBUG DRAW RAY -- 
                if (m_DrawDebugLine) Debug.DrawRay(rayStart, rayEnd, Color.yellow);

                if(m_InputVector.sqrMagnitude > 0.1f && m_Grounded)
                {
                    if (Physics.Raycast(rayStart, rayEnd, out m_StepHit, m_MaxStepHeight - m_StepOffset, m_Layers.SolidLayer))
                    {
                        if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_StepOffset + m_SkinWidth))
                        {
                            m_MoveDirection = (m_StepHit.point - m_Transform.position).normalized * m_StepSpeed ;//* (m_Speed > 1 ? m_Speed : 1);
                            m_Rigidbody.velocity = m_MoveDirection + Vector3.up * 1; // * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1);// + (Vector3.up * m_Step);
                            //m_Transform.position += m_MoveDirection;
                            _stepColor = Color.magenta;
                        }
                        else
                        {
                            _stepColor = Color.cyan;
                        }

                    }
                }
                //  -- DEBUG DRAW RAY -- 
                if (m_DrawDebugLine) Debug.DrawRay( m_Transform.position, m_MoveDirection * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1), _stepColor);

                Vector3 targetPosition = m_Transform.position;
                targetPosition.y = m_GroundHit.point.y;
                m_Transform.position = targetPosition;

                return true;
            }


            return false;
        }


        //  Update the rotation forces.
        private void UpdateRotation()
        {
            //m_TurnAmount += m_TurnAmount * m_RotationSpeed;
            if(m_Moving)
            {
                m_TurnAmount = Mathf.Rad2Deg * m_TurnAmount * m_RotationSpeed;
                m_TurnAmount = Mathf.Lerp(0, m_TurnAmount, m_DeltaTime);
                m_LookRotation *= Quaternion.Euler(0, m_TurnAmount, 0);
            }
            else{
                m_TurnAmount = Mathf.Lerp(m_TurnAmount, 0, m_DeltaTime);
            }


            m_LookRotation = Quaternion.Lerp(m_Transform.rotation, m_LookRotation, (m_Aiming ? m_AimRotationSpeed : m_RotationSpeed) * m_DeltaTime);
            m_Transform.rotation = m_LookRotation;
            //m_Rigidbody.MoveRotation(m_LookRotation.normalized);

        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            if (m_SlopeAngle >= m_SlopeLimit) return;

            ////m_MoveDirection = m_InputVector.normalized.x * m_Transform.right * m_StrafeSpeed + m_InputVector.normalized.z * m_Transform.forward * m_MovementSpeed;
            //m_MoveDirection = m_InputVector.x * m_Transform.right + m_InputVector.z * m_Transform.forward;
            //m_MoveDirection.z = m_MoveDirection.z * m_MovementSpeed;
            //m_MoveDirection.x = m_MoveDirection.x * m_StrafeSpeed;

            //m_Rigidbody.drag = m_MoveAmount > 0.1f ? 0 : 5;

            m_Velocity = m_MoveDirection * m_MoveAmount;
            m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
            //m_Velocity.y = 0;
            m_Rigidbody.AddForce(m_Velocity, ForceMode.VelocityChange);
        }



		private void Move()
		{
            if (m_UseRootMotion)
            {
                m_RootMotionVelocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
                m_RootMotionVelocity.y = 0;
                m_Rigidbody.velocity = m_RootMotionVelocity;
            }
            else{
                switch (m_MovementType)
                {
                    case MovementType.FreeMovement:
                        //m_TurnAmount = Mathf.Atan2(m_InputVector.x, m_InputVector.z);
                        //m_Transform.rotation = Quaternion.Euler(0, m_TurnAmount * (m_Aiming ? m_AimRotationSpeed : m_RotationSpeed) * m_DeltaTime, 0);
                        break;
                    case MovementType.Combat:



                        break;
                }
            }
		}


		private void UpdateAnimator()
        {
            //  Is Moving.
            m_Animator.SetBool(HashID.Moving, m_Moving);


            // Rotation
            if(m_Moving){
                if(Mathf.Abs(m_TurnAmount) > 0.5f)
                    m_Animator.SetFloat(HashID.TurnAmount, Mathf.Clamp(m_TurnAmount, -1, 1), 0.2f, m_DeltaTime);  // Mathf.Clamp(m_TurnAmount, -1, 1)
                else
                    m_Animator.SetFloat(HashID.TurnAmount, 0, 0.2f, m_DeltaTime);
            }
            //  Movement Input
            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
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

        protected void OnDrawGizmos()
        {
            if(m_DrawDebugLine && Application.isPlaying)
            {
                //if (Aiming){
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawSphere(m_LookAtPoint, 0.1f);
                //    Gizmos.DrawLine(transform.position + (Vector3.up * 1.35f), m_LookAtPoint);
                //}

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(m_Transform.position, m_MoveDirection);
                GizmosUtils.DrawString("Move Direction", m_Transform.position + m_MoveDirection, Color.white);

                //Gizmos.color = Color.cyan;
                //Gizmos.DrawRay(m_Transform.position + (Vector3.up * 1.35f), m_LookDirection);

                //Gizmos.color = Color.magenta;
                //Gizmos.DrawSphere(m_LookAtPoint, 0.2f);
            }

            //if (m_DrawDebugLine)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, m_Velocity.normalized );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.forward );
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.right );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            //}
            //if(Application.isPlaying){
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawSphere(m_LookDirection, 0.1f);
            //    Gizmos.DrawLine(transform.position + (transform.up * 1.35f), m_LookDirection);
            //}

        }







    }

}
