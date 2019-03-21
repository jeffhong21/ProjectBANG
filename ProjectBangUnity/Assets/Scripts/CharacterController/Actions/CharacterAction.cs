﻿namespace CharacterController
{
    using UnityEngine;
    using System;

    public enum ActionStartType { Automatic, Manual, ButtonDown, DoublePress };
    public enum ActionStopType { Automatic, Manual, ButtonUp, ButtonToggle };

    [Serializable]
    public abstract class CharacterAction : MonoBehaviour
    {
        [SerializeField]
        protected bool m_IsActive;
        [SerializeField, HideInInspector]
        protected string m_StateName;
        [SerializeField, HideInInspector]
        protected int m_ActionID;
        //[SerializeField]
        protected float m_TransitionDuration = 0.2f;
        //[SerializeField]
        protected float m_SpeedMultiplier = 1;
        [SerializeField]
        protected KeyCode m_Keycode;
        [SerializeField]
        protected ActionStartType m_StartType;
        [SerializeField]
        protected ActionStopType m_StopType = ActionStopType.Manual;

        [Space(12)]


        protected CharacterLocomotion m_Controller;
        protected Rigidbody m_Rigidbody;
        protected CapsuleCollider m_CapsuleCollider;
        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected LayerManager m_Layers;
        protected Inventory m_Inventory;
        protected GameObject m_GameObject;
        protected Transform m_Transform;

        protected AnimatorTransitionInfo m_TransitionInfo;


        //[SerializeField]
        protected bool m_ActionStopToggle;        //  Used for double clicks.


        //
        // Properties
        //
        public bool IsActive { 
            get { return m_IsActive;}
            set { m_IsActive = value; }
        }

        public int ActionID{
            get { return m_ActionID; }
            set { m_ActionID = value; }
        }


        public float SpeedMultiplier{
            get { return m_SpeedMultiplier; }
            set { m_SpeedMultiplier = value; }
        }


        public ActionStartType StartType{
            get { return m_StartType; }
            //set { m_StartType = value; }
        }

        public ActionStopType StopType{
            get { return m_StopType; }
            //set { m_StopType = value; }
        }




        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Layers = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();
            m_GameObject = gameObject;
            m_Transform = transform;
            //EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);

            if (string.IsNullOrWhiteSpace(m_StateName)){
                m_StateName = GetType().Name;
            }

        }


        protected void OnEnable()
		{

		}

        protected void OnDisable()
        {

        }


		private void OnValidate()
		{
            if (string.IsNullOrEmpty(m_StateName)) m_StateName = GetType().Name;
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
            switch (m_StartType)
            {
                case ActionStartType.Automatic:
                    if (m_IsActive == false)
                        return true;
                    break;
                case ActionStartType.Manual:
                    if (m_IsActive == false)
                        return true;
                    break;
                case ActionStartType.ButtonDown:
                    if (Input.GetKeyDown(m_Keycode))
                    {
                        if (m_IsActive == false)
                            return true;
                        if (m_StopType == ActionStopType.ButtonToggle)
                            m_ActionStopToggle = true;
                    }
                    break;
            }
            return false;
        }

        public virtual bool CanStopAction()
        {
            if (m_IsActive == false) return false;

            switch (m_StopType)
            {
                case ActionStopType.Automatic:
                    for (int index = 0; index < m_Animator.layerCount; index++){
                        if (m_Animator.GetCurrentAnimatorStateInfo(index).IsName(m_StateName)){
                            Debug.LogFormat("Stopping Action State {0}", m_StateName);
                            return false;
                        }
                    }
                    Debug.LogFormat("Trying to stopping Action State {0}", m_StateName);
                    m_IsActive = false;
                    return true;
                case ActionStopType.Manual:
                    m_IsActive = false;
                    return true;


                case ActionStopType.ButtonUp:
                    if (Input.GetKeyUp(m_Keycode)){
                        m_IsActive = false;
                        return true;
                    }
                    break;
                case ActionStopType.ButtonToggle:
                    if (m_ActionStopToggle){
                        if (Input.GetKeyDown(m_Keycode)){
                            m_ActionStopToggle = false;
                            m_IsActive = false;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }



        public void StartAction()
        {
            m_IsActive = true;
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, m_IsActive);

            ActionStarted();

            m_AnimatorMonitor.SetActionID(m_ActionID);
            //m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);

            for (int index = 0; index < m_Animator.layerCount; index++){
                if (string.IsNullOrEmpty(GetDestinationState(index)) == false){
                    m_Animator.CrossFade(GetDestinationState(index), m_TransitionDuration, index);
                }
            }

        }


        public void StopAction()
        {
            m_AnimatorMonitor.SetActionID(0);
            //m_Animator.ResetTrigger(HashID.ActionChange);

            ActionStopped();

            m_IsActive = false;
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, m_IsActive);
        }


        //  The action has started.  Best as an initializer.
        protected virtual void ActionStarted()
        {

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
            if (Physics.Raycast(m_Transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2, m_Layers.SolidLayer)){
                return true;
            }
            return false;
        }








        //  Returns the state the given layer should be on.
        public virtual string GetDestinationState(int layer)
        {
            return m_StateName;
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