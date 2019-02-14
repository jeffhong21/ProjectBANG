namespace CharacterController
{
    using UnityEngine;


    public class ItemAction : CharacterAction
    {



        public new void StartAction()
        {
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterItemActionActive", this, true);
            ActionStarted();

            for (int index = 0; index < m_Animator.layerCount; index++)
            {
                if (string.IsNullOrEmpty(GetDestinationState(index)) == false)
                {
                    m_Animator.CrossFade(GetDestinationState(index), m_TransitionDuration, index);
                }
            }

        }


        public new void StopAction()
        {
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterItemActionActive", this, false);
            ActionStopped();
            m_AnimatorMonitor.PlayDefaultState();
        }



        public override bool CanStopAction()
        {
            bool canStopAction = false;


            if (m_StopType == ActionStopType.Automatic)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash == m_StateHash)
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
                if (m_ActionStopToggle)
                {
                    canStopAction = false;
                }
                else
                {
                    canStopAction = true;
                }

            }

            return canStopAction;
        }


    }

}