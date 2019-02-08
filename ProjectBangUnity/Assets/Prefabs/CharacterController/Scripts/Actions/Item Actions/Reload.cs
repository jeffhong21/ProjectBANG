namespace CharacterController
{
    using UnityEngine;


    public class Reload : CharacterAction
    {

        private Item m_EquipedItem;

        private bool m_IsReloading;

        //
        // Methods
        //
        private void Reset()
        {
            m_StateName = "Rifle";
            m_InputName = "Reload";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.Automatic;
        }



        public override bool CanStopAction()
        {
            bool canStopAction = false;


            if (m_StopType == ActionStopType.Automatic)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash == m_StateHash)
                {
                    //Debug.LogFormat("Current Hash: {0} | {1} Hash: {2}", m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash, GetType().Name, m_StateHash);
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


            return canStopAction;
        }


        public override float GetNormalizedTime()
        {
            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1;
            return m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).normalizedTime % 1; ;
        }




        protected override void ActionStarted()
        {
            m_IsReloading = true;
        }

        protected override void ActionStopped()
        {
            m_IsReloading = false;
        }

        public override string GetDestinationState(int layer)
        {
            return "Rifle.Reload";
        }
    }

}