namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    public enum ActionStartType { Automatic, Manual, ButtonDown, DoublePress };
    public enum ActionStopType { Automatic, Manual, ButtonUp, ButtonToggle };

    [Serializable]
    public abstract class CharacterAction : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        protected bool m_IsActive;
        [SerializeField, HideInInspector]
        protected string m_StateName;
        //[SerializeField, HideInInspector]
        protected string m_DestinationStateName;
        [SerializeField, HideInInspector]
        protected int m_ActionID;
        [SerializeField]
        protected float m_TransitionDuration = 0.2f;
        //[SerializeField]
        protected float m_SpeedMultiplier = 1;
        [SerializeField]
        protected string[] m_InputNames = new string[0];
        [SerializeField]
        protected ActionStartType m_StartType = ActionStartType.Manual;
        [SerializeField]
        protected ActionStopType m_StopType = ActionStopType.Manual;


        [Space(12)]


        //  InputNames to KeyCodes
        protected KeyCode[] m_KeyCodes = new KeyCode[0];
        protected int m_InputIndex = -1;
        //  Check double press variables.
        private KeyCode m_FirstButtonPressed;
        private float m_TimeOfFirstButtoonPressed;
        private float m_DoublePressInputTime = 0.25f;

        protected float m_DeltaTime;
        protected CharacterLocomotion m_Controller;
        protected Rigidbody m_Rigidbody;
        protected CapsuleCollider m_CapsuleCollider;
        protected CharacterIK m_CharacterIK;
        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected LayerManager m_Layers;
        protected Inventory m_Inventory;
        protected GameObject m_GameObject;
        protected Transform m_Transform;

        protected AnimatorStateInfo m_StateInfo;
        protected AnimatorTransitionInfo m_TransitionInfo;


        //[SerializeField]
        protected bool m_ActionStopToggle;        //  Used for double clicks.


        //
        // Properties
        //
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        public int ActionID
        {
            get { return m_ActionID; }
            set { m_ActionID = value; }
        }


        public float SpeedMultiplier
        {
            get { return m_SpeedMultiplier; }
            set { m_SpeedMultiplier = value; }
        }


        public ActionStartType StartType
        {
            get { return m_StartType; }
            //set { m_StartType = value; }
        }

        public ActionStopType StopType
        {
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
            m_CharacterIK = GetComponent<CharacterIK>();
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Layers = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;
            //EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);


            Initialize();
        }

        private void Initialize()
        {
            //  Setup state name.
            if (string.IsNullOrWhiteSpace(m_StateName))
            {
                m_StateName = GetType().Name;
            }

            //  Translate input name to keycode.
            if (m_StartType != ActionStartType.Automatic || m_StartType != ActionStartType.Manual ||
               m_StopType != ActionStopType.Automatic || m_StopType != ActionStopType.Manual)
            {
                m_KeyCodes = new KeyCode[m_InputNames.Length];
                for (int i = 0; i < m_InputNames.Length; i++)
                {
                    m_KeyCodes[i] = (KeyCode)Enum.Parse(typeof(KeyCode), m_InputNames[i]);
                }
            }

        }


        protected void OnEnable()
        {

        }

        protected void OnDisable()
        {

        }


		//private void Update()
		//{
  //          if(m_IsActive){
  //              if (m_Animator.IsInTransition(0)){
  //                  m_Animator.ResetTrigger(HashID.ActionChange);
  //              }
  //          }

		//}


		protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(m_StateName)) m_StateName = GetType().Name;
        }


        protected void MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed, Action onComplete)
        {
            StartCoroutine(MoveToTarget(targetPosition, targetRotation, minMoveSpeed));
            if (onComplete != null)
                onComplete();
        }

        private IEnumerator MoveToTarget(Vector3 targetPosition, Quaternion targetRotation, float minMoveSpeed)
        {
            var startTime = Time.time;

            var direction = targetPosition - m_Transform.position;
            var distanceRemainingSqr = direction.sqrMagnitude;
            while (distanceRemainingSqr >= 0.1f || startTime > startTime + 5)
            {
                var velocityDelta = targetPosition - m_Transform.position;
                m_Transform.position += velocityDelta.normalized * minMoveSpeed * m_DeltaTime;

                distanceRemainingSqr = velocityDelta.sqrMagnitude;
                startTime += m_DeltaTime;
                yield return null;
            }
        }


        private bool CheckDoubleTap(KeyCode key)
        {
            if (Input.GetKeyDown(key) && m_FirstButtonPressed == key)
            {
                m_FirstButtonPressed = KeyCode.F12;
                if (Time.time - m_TimeOfFirstButtoonPressed < m_DoublePressInputTime)
                {
                    return true;
                }
            }
            if (Input.GetKeyDown(key) && m_FirstButtonPressed != key)
            {
                m_FirstButtonPressed = key;
                m_TimeOfFirstButtoonPressed = Time.time;
                return false;
            }

            return false;
        }




        //  Checks if action can be started.
        public virtual bool CanStartAction()
        {
            if (this.enabled == false) return false;

            if (m_IsActive == false)
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

                        for (int i = 0; i < m_KeyCodes.Length; i++)
                        {
                            if (Input.GetKeyDown(m_KeyCodes[i]))
                            {
                                if (m_StopType == ActionStopType.ButtonToggle)
                                    m_ActionStopToggle = true;
                                m_InputIndex = i;

                                return true;
                            }
                        }
                        break;
                    case ActionStartType.DoublePress:

                        for (int i = 0; i < m_KeyCodes.Length; i++)
                        {
                            if (CheckDoubleTap(m_KeyCodes[i]))
                            {
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }

		public virtual bool CanStopAction()
        {
            if (enabled == false || m_IsActive == false) return false;


            switch (m_StopType)
            {
                case ActionStopType.Automatic:

                    for (int index = 0; index < m_Animator.layerCount; index++)
                    {
                        if (m_Animator.GetCurrentAnimatorStateInfo(index).IsName(m_StateName))
                        {
                            Debug.LogFormat("Stopping Action State {0}", m_StateName);
                            return false;
                        }
                    }
                    //Debug.LogFormat("Trying to stopping Action State {0}", m_StateName);
                    m_IsActive = false;
                    return true;
                case ActionStopType.Manual:

                    if (m_IsActive)
                    {
                        m_IsActive = false;
                        return true;
                    }
                    return false;

                case ActionStopType.ButtonUp:

                    if (Input.GetKeyUp(m_KeyCodes[m_InputIndex]))
                    {
                        m_InputIndex = -1;
                        m_IsActive = false;
                        return true;
                    }
                    break;
                case ActionStopType.ButtonToggle:

                    if (m_ActionStopToggle)
                    {
                        if (Input.GetKeyDown(m_KeyCodes[m_InputIndex]))
                        {
                            m_InputIndex = -1;
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

            m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            m_Animator.SetTrigger(HashID.ActionChange);

            ActionStarted();

            //m_AnimatorMonitor.SetActionID(m_ActionID);
            //m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);
            string destinationState = m_StateName;
            for (int index = 0; index < m_Animator.layerCount; index++)
            {
                if (string.IsNullOrEmpty(GetDestinationState(index)) == false){
                    destinationState = string.Format("{0}.{1}", m_StateName, GetDestinationState(index));
                } else{
                    destinationState = m_StateName;
                }
                if (m_TransitionDuration > 0)
                    m_Animator.CrossFade(GetDestinationState(index), m_TransitionDuration, index);
                else
                    m_Animator.Play(GetDestinationState(index), index);
            }

        }


        public void StopAction()
        {
            m_Animator.SetInteger(HashID.ActionID, 0);
            m_Animator.SetInteger(HashID.ActionIntData, 0);
            m_Animator.ResetTrigger(HashID.ActionChange);
            //m_AnimatorMonitor.SetActionID(0);

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

        // Executed on every action to allow the action to update.
        // The action may need to update if it needs to do something when inactive or show a GUI icon when the ability can be started.
        public virtual void UpdateAction()
        {

        }

        //  Moves the character according to the input
        public virtual bool Move()
        {
            return true;
        }


        //  Ensure the current movement direction is valid.
        public virtual bool CheckMovement()
        {
            return true;
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
            return true;
        }





        public virtual float GetColliderHeightAdjustment()
        {

            return m_CapsuleCollider.height;
        }


        //  Returns the state the given layer should be on.
        public virtual string GetDestinationState(int layer)
        {
            return m_StateName;
        }


        public virtual float GetTransitionDuration()
        {
            int layerIndex = 0;
            float transitionDuration = m_Animator.GetAnimatorTransitionInfo(layerIndex).duration;
            return transitionDuration;
        }


        public virtual float GetNormalizedTime()
        {
            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1;
            return m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1; ;
        }








        #region Debug




        //Color debugTextColor = new Color(0, 0.6f, 1f, 1);
        private GUIStyle textStyle = new GUIStyle();
        private GUIStyle style = new GUIStyle();

        private Vector2 size;

        private Rect location = new Rect();
        protected GUIContent content = new GUIContent();


        private void OnGUI()
        {
            if (Application.isPlaying && m_IsActive)
            {
                GUI.color = Color.black;
                textStyle.fontStyle = FontStyle.Bold;
                size = new GUIStyle(GUI.skin.label).CalcSize(content);
                location.Set(Screen.width - size.x - 10, 15, size.x, size.y * 2);
                GUILayout.BeginArea(location, GUI.skin.box);

                GUILayout.Label(string.Format("Transition Duration: {0}", GetTransitionDuration()));
                DrawOnGUI();

                //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));

                GUILayout.EndArea();
            }
        }


        protected virtual void DrawOnGUI()
        {
            content.text = string.Format("-- {0} --", GetType());
        }


        #endregion

    }
}