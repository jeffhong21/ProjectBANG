namespace Bang
{
    using UnityEngine;

    public interface IActorMoveCtrl
    {

        float moveSpeed
        {
            get;
            set;
        }

        float acceleration
        {
            get;
            set;
        }

        float angularSpeed
        {
            get;
            set;
        }

        float angularAcceleration
        {
            get;
            set;
        }


        void MoveTo(Vector3 destination);

    }
}