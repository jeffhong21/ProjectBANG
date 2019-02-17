namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using JH_Utils;



    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
    {
        public event Action<bool> OnAim = delegate {};


        public enum MovementType {Adventure, TopDown };

        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
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
        protected float m_SlopeLimit = 120;
        [SerializeField]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;
        [SerializeField, DisplayOnly]
        private CharacterAction[] m_ActiveActions;
        private CharacterAction m_CurrentAction;


        [Header("-- Debug --")]
        private bool m_Moving, m_Aim, m_Grounded = true;
        private float m_Speed;
        private float m_MoveAmount;
        Vector3 m_Velocity, m_CurrentVelocity, m_TargetVelocity;
        Vector3 m_InputVector;
        Quaternion m_LookRotation;


        PlayerInput m_Input;
        AnimatorMonitor m_AnimationMonitor;
        LayerManager m_Layers;
        Animator m_Animator;
        CapsuleCollider m_CapsuleCollider;
        Rigidbody m_Rigidbody;
        Camera m_Camera;
        GameObject m_GameObject;
        Transform m_Transform;
        Transform m_LookAtPoint;
        float m_DeltaTime;

        bool m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true;



        RaycastHit m_HitInfo;
        float m_SlopeAngle;
        float m_FwdDotProduct, m_RightDotProduct;





        [SerializeField] bool m_DrawDebugLine;
        [SerializeField] bool m_ShowComponents;
        [SerializeField] bool m_DebugActiveActions;
        [SerializeField, HideInInspector] CharacterAction m_SelectedAction;




        #region Properties

        public bool Moving
        {
            get{
                m_Moving = Mathf.Abs(InputVector.x) < 1 && Mathf.Abs(InputVector.z) < 1;
                return m_Moving;
            }
            set { m_Moving = value; }
        }

        public float RotationSpeed
        {
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public float AimRotationSpeed
        {
            get { return m_AimRotationSpeed; }
            set { m_AimRotationSpeed = value; }
        }

        public bool Aim
        {
            get { return m_Aim; }
            set { m_Aim = value; }
        }

        public bool Aiming{
            get {
                if(m_Aim){
                    if(Grounded){
                        return true;
                    }
                }
                return false;
            }
        }

        public Vector3 InputVector
        {
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 RelativeInputVector
        {
            get { return m_InputVector.normalized; }
        }

        public Quaternion LookRotation
        {
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public bool Grounded
        {
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public Vector3 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }


        public Vector3 LookPosition{
            get { return m_LookAtPoint.position; }
            set { m_LookAtPoint.position = value; }
        }

        public CharacterAction[] CharActions
        {
            get { return m_Actions; }
            //set { m_Actions = value; }
        }


        #endregion



        protected void Awake()
        {
            m_Input = GetComponent<PlayerInput>();
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;


            m_LookAtPoint = new GameObject("LookAtPoint").transform; //.parent = gameObject.transform;
            m_LookAtPoint.transform.parent = m_GameObject.transform;
            m_LookAtPoint.position = (m_Transform.forward * 10) + (Vector3.up * 1.35f);


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


		protected void Start()
        {
            m_ActiveActions = new CharacterAction[m_Actions.Length];

            if (m_Input) m_Camera = CameraController.Instance.Camera;
        }




        private void Update()
		{
            if(m_Input){
                UpdateSpeed();
                //DebugActiveActions();
            }




            if (m_Actions != null)
            {
                m_UpdateRotation = true;
                m_UpdateMovement = true;
                m_UpdateAnimator = true;

                for (int i = 0; i < m_Actions.Length; i++)
                {
                    //  STARTING
                    //  If active actions list slot is null, that means that action can attempt to be started.
                    //  If it contains an item, that means that action is currently active.
                    if (m_ActiveActions[i] == null)
                    {
                        //  Can start if there is no active action.
                        if (m_ActiveAction == null)
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                m_Actions[i].UpdateAnimator();

                                //  Cache the active action.
                                m_ActiveAction = m_Actions[i];
                                m_ActiveActions[i] = m_Actions[i];
                                //  Move on to the next Action.
                                continue;
                            }
                        }
                        //  Can start if action is concurrent
                        else if (m_Actions[i].IsConcurrentAction())
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                m_Actions[i].UpdateAnimator();
                                //  Cache the active action.
                                m_ActiveActions[i] = m_Actions[i];
                                //  Move on to the next Action.
                                continue;
                            }
                        }
                        //  What to do if there is an active action.
                        else if (m_ActiveAction != null)
                        {

                        }
                    }
                    //  STOPPING
                    //  If the active actions slot is equal to the current action, than that action can be stopped.
                    else if (m_ActiveActions[i] == m_Actions[i])
                    {
                        if (m_Actions[i].CanStopAction())
                        {
                            //  Start the Action and update the animator.
                            m_Actions[i].StopAction();
                            m_Actions[i].UpdateAnimator();
                            //  Cache the active action.
                            if (m_Actions[i] == m_ActiveAction) m_ActiveAction = null;
                            m_ActiveActions[i] = null;
                            //  Move on to the next Action.
                            continue;
                        }
                    }
                    //  UPDATING
                    //  Can't stop or start action, so will update.
                    m_Actions[i].UpdateAction();
                    if (m_ActiveAction == m_Actions[i])
                    {
                        m_UpdateRotation = m_ActiveAction.UpdateRotation();
                        m_UpdateMovement = m_ActiveAction.UpdateMovement();
                        m_UpdateAnimator = m_ActiveAction.UpdateAnimator();
                    }
                }


            }


		}



        private void FixedUpdate()
        {
            if (m_UpdateRotation) UpdateRotation();
            if (m_UpdateMovement) UpdateMovement();
            if (m_UpdateAnimator) UpdateAnimator();
            //MoveCharacter(RelativeInputVector.x, RelativeInputVector.z, m_LookRotation);
        }



        //  Update the rotation forces.
        private void UpdateRotation()
        {
            //var angle = Mathf.Atan2(InputVector.x, Mathf.Abs(InputVector.z));
            //angle = Mathf.Rad2Deg * m_RotationSpeed;
            //angle += m_Transform.eulerAngles.y;
            //LookRotation = Quaternion.Euler(0, angle, 0);

            //m_LookRotation *= Quaternion.AngleAxis(m_RotationSpeed * m_InputVector.x * m_DeltaTime, Vector3.up);
            //m_Rigidbody.MoveRotation(m_LookRotation.normalized);

            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime));
        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            if (m_UseRootMotion)
            {
                m_Velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
            }
            else
            {
                m_CurrentVelocity = m_Rigidbody.velocity;
                m_MoveAmount = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.y));
                if (m_MoveAmount > 0)
                {
                    m_TargetVelocity = m_Transform.forward * m_InputVector.z * m_GroundSpeed.z;
                    m_TargetVelocity += m_Transform.right * m_InputVector.x * m_GroundSpeed.x;
                    m_TargetVelocity = m_TargetVelocity.normalized * 2;
                    m_TargetVelocity.y = 0;
                }
                else
                {
                    m_TargetVelocity = Vector3.zero;
                }
                m_Velocity = Vector3.Lerp(m_CurrentVelocity, m_TargetVelocity, m_DeltaTime * 3);
                m_Rigidbody.velocity = m_Velocity;

                //m_Velocity = m_Transform.forward * m_InputVector.z * m_GroundSpeed.z;
                //m_Velocity += m_Transform.right * m_InputVector.x * m_GroundSpeed.x;
                //m_Velocity.Normalize();
                //m_Rigidbody.velocity = m_Velocity;
            }

        }


        private void UpdateAnimator()
        {
            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z * m_Speed);
            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x * m_Speed);
            m_Animator.SetFloat(HashID.Speed, m_Speed);
        }


		private void MoveCharacter()
		{

		}


		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            //if (Mathf.Abs(horizontalMovement) < 1 && Mathf.Abs(forwardMovement) < 1) return;

            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, lookRotation.normalized, m_RotationSpeed * m_DeltaTime));
            if (m_UseRootMotion)
            {
                //m_MoveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovement) + Mathf.Abs(forwardMovement));
                m_Velocity = m_Transform.forward * forwardMovement;
                m_Velocity += m_Transform.right * horizontalMovement;
                m_Velocity.Normalize();
            }
            else
            {
                m_Velocity = m_Transform.forward * forwardMovement * m_GroundSpeed.z;
                m_Velocity += m_Transform.right * horizontalMovement * m_GroundSpeed.x;
                m_Velocity.Normalize();
                m_Rigidbody.velocity = m_Velocity * m_RootMotionSpeedMultiplier;
            }



            //  Play Animation
            UpdateAnimator();
            //if(Aiming){
            //    m_AnimationMonitor.SetHorizontalInputValue(horizontalMovement);
            //}
        }




        private void UpdateSpeed()
        {
            m_MoveAmount = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z));
            m_Speed = m_MoveAmount;
            if (Input.GetKey(KeyCode.LeftShift) && m_Speed > 0.5f)
            {
                m_Speed = 2;
            }
        }





        public void SetPosition(Vector3 position)
        {
            m_Rigidbody.MovePosition(position);
        }

        public void SetRotation(Quaternion rotation)
        {
            m_Rigidbody.MoveRotation(rotation);
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }


        private void OnAnimatorMove()
        {
            if (m_UseRootMotion)
            {
                //m_Rigidbody.velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
                m_Rigidbody.velocity = m_Velocity;
                //Debug.Log(m_Animator.deltaPosition);
            }

        }


        private void OnAimActionStart(bool aim)
        {
            m_Aim = aim;
            OnAim(aim);
            //Debug.LogFormat("Aiming is {0}", aim);
        }


        private void OnActionActive(CharacterAction action, bool activated)
        {
            if(m_DebugActiveActions){
                if (action is Aim) return;
                Debug.Log(action + " activated: " + activated);
            }
        }




        public T GetAction<T>() where T : CharacterAction
        {
            return GetComponent<T>();
        }


        public bool TryStartAction(CharacterAction ability)
        {
            if (ability.CanStartAction())
            {
                ability.StartAction();
                return true;
            }
            return false;
        }



        public void TryStopAllActions()
        {

        }

        public void TryStopAction()
        {

        }

        public void ActionStopped()
        {

        }









        private void UpdateActions_Orignial()
        {
            if (m_Actions != null)
            {
                for (int i = 0; i < m_Actions.Length; i++)
                {
                    //  Cache current action.
                    if (m_Actions[i] == null) continue;
                    //  Cache the current action.
                    m_CurrentAction = m_Actions[i];


                    //  Update the current action.
                    m_CurrentAction.UpdateAction();

                    //  If active action is active, try to stop it.
                    if (m_ActiveAction)
                    {
                        //Debug.LogFormat("Active action is {0}", m_ActiveAction.GetType().Name);
                        //  Check if we can stop the current action.
                        if (m_ActiveAction.CanStopAction())
                        {
                            //  Start the Action and update the animator.
                            m_ActiveAction.UpdateAnimator();
                            m_ActiveAction.StopAction();

                            //  Reset the active action.
                            m_ActiveAction = null;
                            m_ActiveActions[i] = null;
                        }
                        //  If there is an active action and the current action is concurrent.
                        else if (m_CurrentAction.IsConcurrentAction())
                        {
                            //  Check if we can start the action.
                            if (m_CurrentAction.CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_CurrentAction.UpdateAnimator();
                                m_CurrentAction.StartAction();
                            }
                        }
                    }
                    //  If there is no active action, try to start current action.
                    else
                    {
                        //  Check if we can start the action.
                        if (m_CurrentAction.CanStartAction())
                        {
                            //  Start the Action and update the animator.
                            m_CurrentAction.UpdateAnimator();
                            m_CurrentAction.StartAction();
                            //  Cache the active action.
                            //  TODO:  Currently can't get out of toggle Actions if it isn't cached as Active action.
                            //if( !m_CurrentAction.IsConcurrentAction())          
                            //m_ActiveAction = m_CurrentAction;
                            m_ActiveAction = m_CurrentAction;
                            m_ActiveActions[i] = m_Actions[i];
                            Debug.Log("Setting ActiveActions " + m_ActiveActions[i]);
                        }
                    }


                    //  Reset the current action for this loop.
                    m_CurrentAction = null;
                }
            }
            if (m_UpdateRotation == false)
            {
                m_LookRotation = Quaternion.Euler(0, 0, 0);
            }
            //if(m_UpdateRotation) UpdateRotation();
            MoveCharacter(RelativeInputVector.x, RelativeInputVector.z, m_LookRotation);
        }








		#region Debug 



        //private void DebugActiveActions()
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        var actions = "Active Actions:\n";
        //        foreach (var item in m_ActiveActions)
        //        {
        //            actions += string.Format("{0}\n", item == null ? "null" : item.GetType().Name);
        //        }
        //        Debug.Log(actions);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        int index = 0;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        int index = 1;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        int index = 2;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        int index = 3;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        int index = 4;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha6))
        //    {
        //        int index = 5;
        //        Debug.LogFormat("{0} | Is active: {1}", m_Actions[index].GetType().Name, m_ActiveActions[index] == null ? "null" : m_ActiveActions[index].GetType().Name);
        //    }
        //}



		private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);

        protected void OnDrawGizmos()
        {
            if (m_DrawDebugLine)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, m_Velocity.normalized );
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.forward );
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.right );
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            }

            if(Aiming && m_DrawDebugLine){
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(LookPosition, 0.1f);
                Gizmos.DrawLine(transform.position + (transform.up * 1.35f), LookPosition);
            }
        }

        #endregion







        //public void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        //{
        //    //m_Animator.SetBool("Crouching", m_IsCrouching);
        //    m_InputVector.Set(horizontalMovement, 0, forwardMovement);

        //    if(horizontalMovement == 0 && forwardMovement == 0)
        //    {
        //        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_DeltaTime * m_RotationSpeed);
        //        if (lookRotation != Quaternion.identity)
        //            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, lookRotation, m_DeltaTime * m_RotationSpeed));
        //        m_Velocity = Vector3.zero;
        //        m_Rigidbody.velocity = Vector3.zero;

        //        //  Play Animation
        //        m_AnimationMonitor.SetHorizontalInputValue(0);
        //        m_AnimationMonitor.SetForwardInputValue(0);

        //        //m_Animator.SetFloat("HorizontalInput", 0);
        //        //m_Animator.SetFloat("ForwardInput", 0);
        //        //m_Animator.SetBool("Moving", false);

        //    }
        //    else
        //    {
        //        m_Velocity.Set(m_GroundSpeed.x * m_InputVector.x, m_Rigidbody.velocity.y, m_GroundSpeed.z * m_InputVector.z);

        //        if(lookRotation != Quaternion.identity)
        //            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, lookRotation, m_DeltaTime * m_RotationSpeed));
        //        m_Rigidbody.MovePosition( transform.position + (m_Velocity.normalized * 2 * m_DeltaTime));


        //        m_FwdDotProduct = Vector3.Dot(transform.forward, m_Velocity);
        //        m_RightDotProduct = Vector3.Dot(transform.right, m_Velocity);

        //        //  Play Animation
        //        m_AnimationMonitor.SetHorizontalInputValue(m_RightDotProduct);
        //        m_AnimationMonitor.SetForwardInputValue(m_FwdDotProduct);

        //        //m_Animator.SetBool("Moving", true);
        //        //m_Animator.SetFloat("HorizontalInput", m_RightDotProduct);
        //        //m_Animator.SetFloat("ForwardInput", m_FwdDotProduct);
        //    }

        //    m_AngleVector3 = Vector3.Angle(transform.forward, transform.rotation * transform.forward);
        //}
    }

}
