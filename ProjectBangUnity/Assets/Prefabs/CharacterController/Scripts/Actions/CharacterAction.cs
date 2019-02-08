namespace CharacterController
{
    using UnityEngine;
    using System;


    public enum ActionStartType { Automatic, Manual, ButtonDown, DoublePress };
    public enum ActionStopType { Automatic, Manual, ButtonUp, ButtonToggle };

    [Serializable]
    public abstract class CharacterAction : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected string m_InputName;
        [SerializeField]
        protected ActionStartType m_StartType;
        [SerializeField]
        protected ActionStopType m_StopType = ActionStopType.Manual;
        //[SerializeField]
        protected float m_TransitionDuration = 0.2f;
        //[SerializeField]
        protected float m_SpeedMultiplier = 1;
        [SerializeField, Header("State Info")]
        protected string m_StateName;
        [SerializeField]
        protected int m_ActionID;
        [SerializeField, Bang.ReadOnly]
        protected int m_StateHash;
        [Space(12)]
        protected bool m_EquipItem;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        protected AnimationMonitor m_AnimatorMonitor;
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;
        protected Rigidbody m_Rigidbody;
        protected LayerManager m_LayerManager;

        protected bool m_Input;
        //[SerializeField]
        protected bool m_ActionStopToggle;        //  Used for double clicks.


        //
        // Properties
        //
        public bool IsActive { 
            get { return enabled;}
        }

        public int ActionID{
            get { return m_ActionID; }
            set { m_ActionID = value; }
        }

        public string InputName
        {
            get { return m_InputName; }
            set { m_InputName = value; }
        }

        public ActionStartType StartType
        {
            get { return m_StartType; }
            set { m_StartType = value; }
        }

        public ActionStopType StopType
        {
            get { return m_StopType; }
            set { m_StopType = value; }
        }

        public float SpeedMultiplier
        {
            get { return m_SpeedMultiplier; }
            set { m_SpeedMultiplier = value; }
        }

        public bool EquipItem { 
            set { 
                m_EquipItem = value;
            }
        }





        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            m_AnimatorMonitor = GetComponent<AnimationMonitor>();
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_LayerManager = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();
            //EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);

            if (string.IsNullOrWhiteSpace(m_StateName))
            {
                m_StateName = GetType().Name;
            }
            m_StateHash = Animator.StringToHash(m_StateName);
        }



        protected void MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed, Action onComplete)
        {
            
        }

		// Executed on every action to allow the action to update.
		// The action may need to update if it needs to do something when inactive or show a GUI icon when the ability can be started.
		public virtual void UpdateAction()
        {
            
        }


        public void StartAction()
        {
            m_AnimatorMonitor.SetActionID(m_ActionID);
            m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, true);

            ActionStarted();

            for (int index = 0; index < m_Animator.layerCount; index++)
            {
                if(string.IsNullOrEmpty(GetDestinationState(index)) == false ){
                    m_Animator.CrossFade(GetDestinationState(index), m_TransitionDuration, index);
                }
            }

        }


        public void StopAction()
        {
            m_AnimatorMonitor.SetActionID(0);
            m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, false);

            ActionStopped();
            //m_AnimatorMonitor.PlayDefaultState();
        }


        //  The action has started.  Best as an initializer.
        protected virtual void ActionStarted()
        {

        }
        //  The action has stopped.  Best for cleaning up after action is stopped.
        protected virtual void ActionStopped()
        {

        }


        //  Checks if action can be started.
        public virtual bool CanStartAction()
        {
            bool canStartAction = false;


            if (m_StartType == ActionStartType.Automatic){
                canStartAction = true;
            }
            else if (m_StartType == ActionStartType.Manual){
                canStartAction = true;
            }
            else if (m_StartType == ActionStartType.ButtonDown)
            {
                if (Input.GetButtonDown(m_InputName))
                {
                    canStartAction = true;
                    if (m_StopType == ActionStopType.ButtonToggle)
                        m_ActionStopToggle = true;
                }
            }
            else if (m_StartType == ActionStartType.DoublePress){
                canStartAction = true;
            }
            return canStartAction;
        }



        public virtual bool CanStopAction()
        {
            bool canStopAction = false;

            if (m_StopType == ActionStopType.Automatic)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).shortNameHash == m_StateHash)
                {
                    if(GetNormalizedTime() >= 1 - m_TransitionDuration){
                        canStopAction = true;
                    }
                    else{
                        canStopAction = false;
                    }
                }
                else{
                    canStopAction = false;
                }
            }

            else if (m_StopType == ActionStopType.Manual)
            {
                canStopAction = true;
            }

            else if (m_StopType == ActionStopType.ButtonUp)
            {
                if (Input.GetButtonUp(m_InputName))
                {
                    canStopAction = true;
                }
            }
            else if (m_StopType == ActionStopType.ButtonToggle)
            {
                if (m_ActionStopToggle){
                    if (Input.GetButtonDown(m_InputName)){
                        canStopAction = true;
                        m_ActionStopToggle = false;
                    }
                }

            }

            return canStopAction;
        }


        public virtual bool UpdateMovement()
        {
            return true;
        }

        //  Should the CharacterLocomotion continue execution of its UpdateRotation method?
        public virtual bool UpdateRotation()
        {
            return true;
        }


        // Updates the animator.  If true is returned, controller can continue with its animation.  
        // If false is returned, controller stops the current animation
        public virtual bool UpdateAnimator()
        {
            return true;
        }

        //  Can this ability run at the same time as another ability?
        public virtual bool IsConcurrentAction()
        {
            return false;
        }

        public virtual bool CanUseIK(int layer)
        {
            return true;
        }

        //  Called when another actioin is attempting to start and the current actioin is active.
        public virtual bool ShouldBlockActionStart(CharacterAction startingAction)
        {
            return false;
        }


        public virtual bool CheckGround()
        {
            Vector3 origin = m_Transform.position;
            origin.y += 0.6f;
            Vector3 direction = -Vector3.up;
            float distance = 0.7f;
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, distance, m_LayerManager.m_SolidLayer))
            {
                Vector3 targetPosition = hit.point; // targetPosition
                m_Transform.position = targetPosition;
                return true;
            }
            return false;
        }








        //  Returns the state the given layer should be on.
        public virtual string GetDestinationState(int layer)
        {
            return GetType().Name;
        }


        public virtual float GetTransitionDuration()
        {
            return m_TransitionDuration;
        }


        public virtual float GetNormalizedTime()
        {
            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1;
            return m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1;;
        }









    }
}