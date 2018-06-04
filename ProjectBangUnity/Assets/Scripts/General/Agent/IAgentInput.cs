namespace Bang
{
    using UnityEngine;


    public interface IAgentInput
    {


        void MoveTo(Vector3 destination);

        void StopWalking();

        void LookAt(Transform lookAtTarget);


    }
}