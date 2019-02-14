namespace CharacterController
{
    using UnityEngine;


    public class Generic : CharacterAction
    {




        //
        // Methods
        //
        protected override void ActionStarted()
        {
            Debug.Log("New Generic Action");
        }

        protected override void ActionStopped()
        {

        }
    }

}