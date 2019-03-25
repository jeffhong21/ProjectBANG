namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;


    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
    {
        public event Action<bool> OnAim = delegate {};
        public enum MovementType {Combat, Adventure, TopDown };


        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
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
        private bool m_Grounded = true;
        private bool m_Moving, m_Aiming, m_Running;
        //[SerializeField, DisplayOnly]
        private float m_Speed;
        //[SerializeField, DisplayOnly]
        private Vector3 m_Velocity, m_MoveDirection, m_LookDirection;
        [SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        private Vector3 m_LookAtPoint;
        //[SerializeField, DisplayOnly]
        private float rotationDifference , eulerY;
        private Quaternion m_LookRotation;


        [Header("-- Rigidbody Debug --")]
        [SerializeField, DisplayOnly] private Vector3 _rigidbodyVelocity;
        [SerializeField, DisplayOnly] private Vector3 _rigidbodyAngularVelocity;


        private CapsuleCollider m_CapsuleCollider;
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;
        private LayerManager m_Layers;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;


        bool m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true;

        private RaycastHit m_GroundHit;
        private RaycastHit m_StepHit;
        private float m_SlopeAngle;


        [SerializeField] bool m_DrawDebugLine;





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


        public float SpeedChangeMultiplier{
            get { return m_SpeedChangeMultiplier; }
            set { m_SpeedChangeMultiplier = value; }
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

        public bool Aiming{
            get{
                if(m_Aiming && Grounded)
                    return true;
                return false;
            }
        }

        public Vector3 InputVector{
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 RelativeInputVector
        {
            get { return m_InputVector.normalized; }
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


        public Vector3 LookDirection{
            get { return m_LookDirection; }
            set { m_LookDirection = value; }
        }

        public Vector3 LookAtPoint
        {
            get { return m_LookAtPoint; }
            set { m_LookAtPoint = value; }
        }

        public CharacterAction[] CharActions{
            get { return m_Actions; }
            set { m_Actions = value; }
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


            if(m_Rigidbody == null) m_Rigidbody = m_GameObject.AddComponent<Rigidbody>();
            if(m_Layers == null) m_Layers = m_GameObject.AddComponent<LayerManager>();




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


        public void PlayInteractAnimation()
        {
            var actionIntId = UnityEngine.Random.Range(0, 3);
            m_Animator.SetInteger(HashID.ActionIntData, actionIntId);
            if(actionIntId == 0) m_Animator.CrossFade("Pickup_Item.Pickup_Item_Ground", 0.2f, 0);
            else if (actionIntId == 1) m_Animator.CrossFade("Pickup_Item.Pickup_Item_Waist", 0.2f, 0);
            else if (actionIntId == 2) m_Animator.CrossFade("Pickup_Item.Pickup_Objects_Ground", 0.2f, 0);
                    
            //Debug.Log("Playing Animation " + Animator.StringToHash("Pickup_Item.Pickup_Item_Ground") + " | ID: " + actionIntId );
        }







		private void Update()
		{
            if (m_DeltaTime == 0) return;

            m_UpdateRotation = true;
            m_UpdateMovement = true;
            m_UpdateAnimator = true;

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
                if (m_Actions[i].IsActive)
                {
                    if (m_UpdateRotation){
                        m_UpdateRotation = m_Actions[i].UpdateRotation();
                    }
                    if (m_UpdateMovement){
                        m_UpdateMovement = m_Actions[i].UpdateMovement();
                    }
                    if (m_UpdateAnimator){
                        m_UpdateAnimator = m_Actions[i].UpdateAnimator();
                    }
                    //  Call Action Update.
                    m_Actions[i].UpdateAction();
                }
            }
		}


		private void FixedUpdate()
        {
            m_Grounded = CheckGround();

            if (m_UpdateRotation) UpdateRotation();
            if (m_UpdateMovement) UpdateMovement();
            if (m_UpdateAnimator) UpdateAnimator();


            _rigidbodyVelocity = m_Rigidbody.velocity;
            _rigidbodyAngularVelocity = m_Rigidbody.angularVelocity;
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

            Ray groundCheck = new Ray(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down);
            if(m_DrawDebugLine) Debug.DrawRay(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down, Color.white);

            if (Physics.Raycast(groundCheck, out m_GroundHit, m_AlignToGroundDepthOffset + m_SkinWidth, m_Layers.SolidLayer))
            {
                var rayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius * 2);  //+ m_SkinWidth);
                var rayEnd = Vector3.down * (m_MaxStepHeight - m_StepOffset);

                if (m_DrawDebugLine) Debug.DrawRay(rayStart, rayEnd, Color.yellow);

                if(m_InputVector.sqrMagnitude > 0.1f && m_Grounded)
                {
                    if (Physics.Raycast(rayStart, rayEnd, out m_StepHit, m_MaxStepHeight - m_StepOffset, m_Layers.SolidLayer))
                    {
                        if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_StepOffset + m_SkinWidth))
                        {
                            m_MoveDirection = (m_StepHit.point - m_Transform.position).normalized * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1);
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

                if (m_DrawDebugLine)
                    Debug.DrawRay( m_Transform.position, m_MoveDirection * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1), _stepColor);

                return true;
            }


            return false;
        }


        //  Update the rotation forces.
        private void UpdateRotation()
        {
            if (m_Aiming){
                m_LookDirection = m_Transform.forward * 10 - m_Transform.position;
                m_LookDirection.y = m_Transform.position.y;

                if (m_LookDirection != Vector3.zero)
                    m_LookRotation = Quaternion.LookRotation(m_LookDirection, m_Transform.up);
                else
                    m_LookRotation = m_Transform.rotation;
                
                rotationDifference = m_LookRotation.eulerAngles.y - m_Transform.eulerAngles.y;
                m_LookRotation = Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime);
                m_Rigidbody.MoveRotation(m_LookRotation);

                //if (m_DrawDebugLine) Debug.DrawRay(m_Transform.position, m_LookDirection, Color.red);
            }
            else if(m_InputVector != Vector3.zero && m_LookDirection.sqrMagnitude > 0.2f)
            {
                //eulerY = Mathf.Atan2(m_InputVector.x, Mathf.Abs(m_InputVector.z));
                //eulerY = Mathf.Rad2Deg * m_RotationSpeed * m_DeltaTime;
                //eulerY *= m_InputVector.x;
                //eulerY += m_Transform.eulerAngles.y;
                //rotationDifference = eulerY - m_Transform.eulerAngles.y;
                //if (rotationDifference < 0 || rotationDifference > 0) eulerY = m_Transform.eulerAngles.y;
                //m_LookRotation = Quaternion.Euler(m_Transform.eulerAngles.x, eulerY, m_Transform.eulerAngles.z);
                //m_LookRotation = Quaternion.Lerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime);
                //m_Rigidbody.MoveRotation(m_LookRotation);

                var targetDirection = m_LookDirection.normalized;
                m_LookRotation = Quaternion.LookRotation(targetDirection, m_Transform.up);
                rotationDifference = m_LookRotation.eulerAngles.y - m_Transform.eulerAngles.y;
                eulerY = m_Transform.eulerAngles.y;

                if(m_Grounded){
                    if (rotationDifference < 0 || rotationDifference > 0)
                        eulerY = m_LookRotation.eulerAngles.y;
                    var euler = new Vector3(m_Transform.eulerAngles.x, eulerY, m_Transform.eulerAngles.z);
                    m_Rigidbody.MoveRotation(Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(euler).normalized, m_RotationSpeed * m_DeltaTime));
                }

                //m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime));
            }
            else{
                rotationDifference = 0;
            }
            //m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime));
        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            if (m_SlopeAngle >= m_SlopeLimit) return;

            if(m_Aiming)
            {
                m_Speed = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z));
                m_Speed = Mathf.Clamp(m_Speed, 0, 1);
                if (m_Running) m_Speed += 1f;

            }
            else{
                m_Speed = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z)) * m_SpeedChangeMultiplier;
                //m_Speed = Mathf.Clamp(m_Speed, 0, 1);
                //if (m_Running)
                    //m_Speed += 0.5f;
            }


            if (m_UseRootMotion)
            {
                m_Velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
                m_Velocity.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, 10 * m_DeltaTime);  //m_Acceleration
            }
            else{
                if(m_Aiming){
                    m_Velocity = (m_Transform.TransformDirection(m_InputVector) * m_Speed);
                    m_Velocity.y = m_Rigidbody.velocity.y;
                    m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_Acceleration * m_DeltaTime);
                }
                else{
                    m_Velocity = m_MoveDirection + m_Transform.forward * m_Speed;
                    m_Rigidbody.velocity = m_Velocity;
                    m_Rigidbody.AddForce(m_MoveDirection * (m_Speed) * m_DeltaTime, ForceMode.VelocityChange);
                }
            } 


            m_Moving = m_InputVector.sqrMagnitude > 0.1f;
        }


        private void UpdateAnimator()
        {
            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z * m_Speed);
            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x * m_Speed);
            m_Animator.SetBool(HashID.Moving, m_Moving);
        }



        public void SetPosition(Vector3 position){
            m_Rigidbody.MovePosition(position);
        }


        public void SetRotation(Quaternion rotation){
            m_Rigidbody.MoveRotation(rotation.normalized);
        }


        public void UpdateLookDirection(Transform referenceTransform = null)
        {
            if (referenceTransform)
            {
                var forward = referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                m_LookDirection = m_InputVector.x * right + m_InputVector.z * forward;
            }
            else{
                m_LookDirection = m_InputVector.x * m_Transform.right + m_InputVector.z * m_Transform.forward;
            }
                
            
        }


		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            //throw new NotImplementedException(string.Format("<color=yellow>{0}</color> MoveCharacter not Implemented.", GetType()));

            m_InputVector.x = horizontalMovement;
            m_InputVector.y = m_Rigidbody.velocity.y;
            m_InputVector.z = forwardMovement;
            m_LookRotation = lookRotation;


            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime));

            m_Velocity = m_MoveDirection + m_Transform.forward * m_Speed;
            m_Rigidbody.velocity = m_Velocity;
            m_Rigidbody.AddForce(m_MoveDirection * (m_Speed) * m_DeltaTime, ForceMode.VelocityChange);



            m_Moving = m_InputVector.sqrMagnitude > 0.1f;


            ////if (Mathf.Abs(horizontalMovement) < 1 && Mathf.Abs(forwardMovement) < 1) return;

            //m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, lookRotation.normalized, m_RotationSpeed * m_DeltaTime));
            //if (m_UseRootMotion)
            //{

            //    m_Velocity = m_Transform.forward * forwardMovement;
            //    m_Velocity += m_Transform.right * horizontalMovement;
            //    m_Velocity.Normalize();
            //}
            //else
            //{
            //    m_Velocity = m_Transform.forward * forwardMovement * m_GroundSpeed.z;
            //    m_Velocity += m_Transform.right * horizontalMovement * m_GroundSpeed.x;
            //    m_Velocity.Normalize();
            //    m_Rigidbody.velocity = m_Velocity * m_RootMotionSpeedMultiplier;
            //}
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }


        private void OnAnimatorMove()
        {
            //if (m_UseRootMotion)
            //{
            //    //m_Rigidbody.velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
            //    m_Rigidbody.velocity = m_Velocity;
            //    //Debug.Log(m_Animator.deltaPosition);
            //}
        }



        private void OnAimActionStart(bool aim)
        {
            m_Aiming = aim;



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




        public T GetAction<T>() where T : CharacterAction
        {
            return GetComponent<T>();
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


        public void ActionStopped()
        {

        }

















		private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);

        protected void OnDrawGizmos()
        {
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
            if(Aiming && m_DrawDebugLine){
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_LookAtPoint, 0.1f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 1.35f), m_LookAtPoint);
            }
        }







    }

}
