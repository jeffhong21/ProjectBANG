namespace CharacterController
{
    using UnityEngine;


    public class Equip : CharacterAction
    {


        protected bool m_IsSwitching;

        protected int m_ActionIntData = 3;
        //
        // Methods
        //
        private void Reset()
        {
            m_StateName = "Rifle";
            m_InputName = "";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.Automatic;
        }


        public override bool CanStartAction()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                return true;
            }
            return false;
        }


        public override bool CanStopAction()
        {
            
            if (m_StopType == ActionStopType.Automatic)
            {
                //Debug.LogFormat("Current Hash: {0} | {1} Hash: {2}", m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash, GetType().Name, Animator.StringToHash("Unequip"));
                //if (m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash == Animator.StringToHash("Unequip")){
                if (m_Animator.GetCurrentAnimatorStateInfo(3).shortNameHash == Animator.StringToHash("Unequip"))
                {
                    //Debug.LogFormat("Current Hash: {0} | {1} Hash: {2}", m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash, GetType().Name, m_StateHash);
                    if (GetNormalizedTime() >= 1 - m_TransitionDuration)
                        return true;
                }

            }


            return false;
        }

        public override float GetNormalizedTime()
        {
            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.BaseLayerIndex).normalizedTime % 1;
            return m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).normalizedTime % 1; ;
        }


        protected override void ActionStarted()
        {
            m_IsSwitching = true;
            m_Inventory.ToggleEquippedItem();

            m_AnimatorMonitor.SetActionID(101);
            m_AnimatorMonitor.SetIntDataValue(m_ActionIntData);
            m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 3)
                return "Unequip";
            return string.Empty;
            //if(m_ActionIntData == 3)
            //    return "Rifle.Unequip";
            //return "Rifle.Equip";
        }


        protected override void ActionStopped()
        {
            //Debug.LogFormat("{0} is done equipping.", GetType().Name);
            m_IsSwitching = false;

            m_AnimatorMonitor.SetActionID(0);
            m_AnimatorMonitor.SetIntDataValue(0);
            m_AnimatorMonitor.SetActionTrigger(HashID.ActionChange);
        }
    }

}
