namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class MeleeUnarmed : CharacterAction
    {




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


        public override bool Move()
        {



            return true;
        }


        public override bool UpdateAnimator()
        {



            return base.UpdateAnimator();
        }


        public override bool CanStopAction()
        {



            return false;
        }



    }

}

