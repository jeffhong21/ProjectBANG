namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class PickupItem : CharacterAction
    {

        public GameObject item;

        //
        // Methods
        //
        protected override void ActionStarted()
        {

        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                m_StateName = "Pickup_Item.Pickup_Item_Ground";
                return m_StateName;
            }
            return "";
        }



        protected override void ActionStopped()
        {
            m_Animator.SetInteger(HashID.ActionID, 0);
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }




        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                return false;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration)
                {
                    return true;
                }
                //Debug.LogFormat("Current state: {0} .  Normalized time.  {1} ",m_StateName, m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                return false;
            }


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName) == false)
            {
                return true;
            }

            return false;

        }
    }

}

