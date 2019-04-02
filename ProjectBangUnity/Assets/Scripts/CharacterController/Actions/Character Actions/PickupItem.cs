namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class PickupItem : CharacterAction
    {

        public GameObject item;

        protected float m_StartTime;

        //
        // Methods
        //
        protected override void ActionStarted()
        {
            m_StartTime = Time.time;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                m_StateName = "PickupItem.PickupItem_Ground";
                return m_StateName;
            }
            return "";
        }



        protected override void ActionStopped()
        {
            m_Animator.SetInteger(HashID.ActionID, 0);
        }




        public override bool CanStopAction()
        {
            return m_StartTime + 1.5f < Time.time;
        }
    }

}

