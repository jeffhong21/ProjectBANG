namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class ThrowObject : CharacterAction
    {

        protected virtual void Start()
        {
            EventHandler.RegisterEvent(m_GameObject, "OnActivateThrowableObject", OnActivateThrowableObject);
        }


        protected virtual void OnDestroy()
        {
            EventHandler.UnregisterEvent(m_GameObject, "OnActivateThrowableObject", OnActivateThrowableObject);
        }



        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                m_DestinationStateName = "ThrowSingle1";
                return m_DestinationStateName;
            }
            return "";
        }



        public override bool CanStopAction()
        {
            int layerIndex = 0;
            if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0)
            {
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_Animator.IsInTransition(layerIndex))
            {
                //Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_AnimatorMonitor.GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                return true;
            }

            //if (m_Animator.GetCurrentAnimatorStateInfo(layerIndex). == 0)
            //{
            //    m_ExitingAction = true;
            //}

            return false;
        }



        private void OnActivateThrowableObject()
        {
            Debug.Log("OnActivateThrowableObject!");


            Debug.Break();
        }



    }

}

