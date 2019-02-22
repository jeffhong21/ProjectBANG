namespace CharacterController
{
    using UnityEngine;
    using System;
    using JH_Utils;


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
        [SerializeField, Header("-- State Info --")]
        protected string m_StateName;
        [SerializeField]
        protected int m_ActionID;
        [SerializeField, DisplayOnly]
        protected int m_StateHash;
        [Space(12)]
        protected bool m_EquipItem;

        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected LayerManager m_Layers;
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;
        protected Rigidbody m_Rigidbody;
        protected LayerManager m_LayerManager;

        protected AnimatorTransitionInfo m_TransitionInfo;

        protected bool m_Input;
        [SerializeField]
        protected bool m_IsActive;
        protected bool m_CanManualStart;
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
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Layers = GetComponent<LayerManager>();
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


        protected void OnEnable()
		{
            //this.hideFlags = HideFlags.HideInInspector;
		}

        protected void OnDisable()
        {

        }


		protected void MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed, Action onComplete)
        {
            
        }

		// Executed on every action to allow the action to update.
		// The action may need to update if it needs to do something when inactive or show a GUI icon when the ability can be started.
		public virtual void UpdateAction()
        {
            
        }



        //  Checks if action can be started.
        public virtual bool CanStartAction()
        {
            bool canStartAction = false;
            switch (m_StartType)
            {
                case ActionStartType.Automatic:
                    canStartAction = true;
                    break;
                case ActionStartType.Manual:
                    if(!m_CanManualStart){
                        m_CanManualStart = true;
                        canStartAction = true;
                    }
                    break;
                case ActionStartType.ButtonDown:
                    if (Input.GetButtonDown(m_InputName))
                    {
                        canStartAction = true;
                        if (m_StopType == ActionStopType.ButtonToggle)
                            m_ActionStopToggle = true;
                    }

                    break;
                case ActionStartType.DoublePress:
                    canStartAction = true;
                    break;
            }
            return canStartAction;
        }


        public virtual void StartAction()
        {
            m_AnimatorMonitor.SetActionID(m_ActionID);
            m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);

            ActionStarted();
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, true);

            for (int index = 0; index < m_Animator.layerCount; index++)
            {
                if (string.IsNullOrEmpty(GetDestinationState(index)) == false)
                {
                    m_Animator.CrossFade(GetDestinationState(index), m_TransitionDuration, index);
                }
            }
        }


        //  The action has started.  Best as an initializer.
        protected virtual void ActionStarted()
        {

        }


        public virtual bool CanStopAction()
        {
            bool canStopAction = false;
            switch (m_StopType)
            {
                case ActionStopType.Automatic:
                    if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).shortNameHash == m_StateHash)
                    {
                        if (GetNormalizedTime() >= 1 - m_TransitionDuration)
                        {
                            canStopAction = true;
                        }
                        else
                        {
                            canStopAction = false;
                        }
                    }
                    else
                    {
                        canStopAction = false;
                    }

                    break;
                case ActionStopType.Manual:
                    canStopAction = true;
                    break;
                case ActionStopType.ButtonUp:
                    if (Input.GetButtonUp(m_InputName))
                    {
                        canStopAction = true;
                    }

                    break;
                case ActionStopType.ButtonToggle:
                    if (m_ActionStopToggle)
                    {
                        if (Input.GetButtonDown(m_InputName))
                        {
                            canStopAction = true;
                            m_ActionStopToggle = false;
                        }
                    }

                    break;
            }

            m_CanManualStart = false;
            return canStopAction;
        }


        public virtual void StopAction()
        {
            m_AnimatorMonitor.SetActionID(0);
            m_Animator.ResetTrigger(HashID.ActionChange);

            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, false);

            ActionStopped();
        }


        //  The action has stopped.  Best for cleaning up after action is stopped.
        protected virtual void ActionStopped()
        {

        }


        //  When an action is about to be stopped, notify which action is starting.
        public virtual void ActionWillStart(CharacterAction nextAction)
        {
            
        }

        //  Should the action override the item's high priority state?
        public virtual bool OverrideItemState(int layer)
        {
            return false;
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
            RaycastHit hit;
            if (Physics.Raycast(m_Transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1, m_Layers.SolidLayer)){
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