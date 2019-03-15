//namespace CharacterController
//{
//    using UnityEngine;

//    public enum ActionStartType { Automatic, Manual, ButtonDown, DoublePress };
//    public enum ActionStopType { Automatic, Manual, ButtonUp, ButtonToggle };

//    public abstract class GenericAction : CharacterAction
//    {
//        [Header("-- Generic Action Properties --")]
//        [SerializeField]
//        protected KeyCode m_Keycode;
//        [SerializeField]
//        protected ActionStartType m_StartType;
//        [SerializeField]
//        protected ActionStopType m_StopType = ActionStopType.Manual;


//        public ActionStartType StartType
//        {
//            get { return m_StartType; }
//            set { m_StartType = value; }
//        }

//        public ActionStopType StopType
//        {
//            get { return m_StopType; }
//            set { m_StopType = value; }
//        }
//        //
//        // Methods
//        //
//        //  Checks if action can be started.
//        public override bool CanStartAction()
//        {
//            bool canStartAction = false;
//            switch (m_StartType)
//            {
//                case ActionStartType.Automatic:
//                    canStartAction = true;
//                    break;
//                case ActionStartType.Manual:
//                    if (!m_CanManualStart)
//                    {
//                        m_CanManualStart = true;
//                        canStartAction = true;
//                    }
//                    break;
//                case ActionStartType.ButtonDown:
//                    if (Input.GetKeyDown(m_Keycode))
//                    {
//                        canStartAction = true;
//                        if (m_StopType == ActionStopType.ButtonToggle)
//                            m_ActionStopToggle = true;
//                    }

//                    break;
//                case ActionStartType.DoublePress:
//                    canStartAction = true;
//                    break;
//            }
//            return canStartAction;
//        }



//        public override bool CanStopAction()
//        {
//            bool canStopAction = false;
//            switch (m_StopType)
//            {
//                case ActionStopType.Automatic:
//                    if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).shortNameHash == m_StateHash)
//                    {
//                        if (GetNormalizedTime() >= 1 - m_TransitionDuration)
//                        {
//                            canStopAction = true;
//                        }
//                        else
//                        {
//                            canStopAction = false;
//                        }
//                    }
//                    else
//                    {
//                        canStopAction = false;
//                    }

//                    break;
//                case ActionStopType.Manual:
//                    canStopAction = true;
//                    break;
//                case ActionStopType.ButtonUp:
//                    if (Input.GetKeyUp(m_Keycode))
//                    {
//                        canStopAction = true;
//                    }

//                    break;
//                case ActionStopType.ButtonToggle:
//                    if (m_ActionStopToggle)
//                    {
//                        if (Input.GetKeyDown(m_Keycode))
//                        {
//                            canStopAction = true;
//                            m_ActionStopToggle = false;
//                        }
//                    }

//                    break;
//            }

//            m_CanManualStart = false;
//            return canStopAction;
//        }




//        protected override void ActionStarted()
//        {

//        }

//        protected override void ActionStopped()
//        {

//        }
//    }

//}