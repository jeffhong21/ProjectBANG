namespace CharacterController
{
    using UnityEngine;


    public class Aim : CharacterAction
    {
        protected const int m_ItemStateID = 1;

        [SerializeField]
        protected int m_ItemID;
        [SerializeField]
        protected string m_ItemName;



        //
        // Methods
        //
        //public override bool CanStartAction()
        //{
        //    return m_Controller.Aiming;
        //}

        //public override bool CanStopAction()
        //{
        //    return m_Controller.Aiming;
        //}



        protected override void ActionStarted()
        {

            //m_AnimatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            m_Animator.SetBool(HashID.Aiming, true);
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", true);
        }


        protected override void ActionStopped()
        {

            //m_AnimatorMonitor.SetItemID(GetItemID(), 0);
            m_Animator.SetBool(HashID.Aiming, false);
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", false);
        }



        public override string GetDestinationState(int layer)
        {
            return "";
        }



        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}
