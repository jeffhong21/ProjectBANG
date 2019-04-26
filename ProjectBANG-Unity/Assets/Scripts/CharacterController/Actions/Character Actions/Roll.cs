namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        //public const int ACTION_ID = 15;
        protected readonly float m_MaxRollDistance = 4f;
        protected readonly float m_CheckHeight = 0.35f;


        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                
            }

            return false;
        }


        protected override void ActionStarted()
        {

        }


        protected override void ActionStopped()
        {

        }



    }

}

