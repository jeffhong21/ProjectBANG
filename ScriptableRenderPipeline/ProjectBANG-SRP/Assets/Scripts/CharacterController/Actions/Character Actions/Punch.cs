namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Punch : CharacterAction
    {

        private float m_StartTime;

        [Header("-----  Debug -----")]
        public string[] stateNames = new string[0];
        [SerializeField]
        private int currentAnimIndex = 0;




        //
        // Methods
        //
        public override bool CanStartAction()
        {

            return base.CanStartAction();
        }


        protected override void ActionStarted()
        {

        }

        protected override void ActionStopped()
        {
            //Debug.Log("Shooting action done");
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
                return stateNames[currentAnimIndex];
            return "";
        }

    }

}

