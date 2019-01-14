namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class TakeDamage : CharacterAction
    {




        protected virtual int GetDamageTypeIndex(float amount, Vector3 hitLocation, Vector3 force, GameObject attacker)
        {
            int index = 0;

            float fwd = Vector3.Dot(m_Transform.forward, hitLocation);
            float right = Vector3.Dot(m_Transform.right, hitLocation);

            if (fwd >= 0.45 || fwd <= -0.45)
            {
                if (fwd >= 0.45)
                    index = 0;
                else
                    index = 3;
            }

            if (right <= -0.45)
                index = 1;
            
            else if (right >= 0.45)
                index = 2;
            
            else
                index = 0;
            

            return index;
        }

        //
        // Methods
        //
        protected override void ActionStarted()
        {

        }

        protected override void ActionStopped()
        {

        }
    }

}

