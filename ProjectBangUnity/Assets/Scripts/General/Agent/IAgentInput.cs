namespace Bang
{
    using UnityEngine;


    public interface IAgentInput
    {


        void MoveTo(Vector3 destination);

        void StopMoving();

        void LookAt(Transform lookAtTarget);


    }
}