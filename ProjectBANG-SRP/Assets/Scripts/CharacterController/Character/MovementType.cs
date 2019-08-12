namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public abstract class MovementType
    {





        public float GetMoveDirectionInDegrees(float characterHorizontalMovement, float characterFwdMovement)
        {
            throw new System.NotImplementedException();
        }

        public Quaternion GetTargetRotation()
        {
            throw new System.NotImplementedException();
        }




        public abstract Vector2 GetInputVector( Vector2 inputVector );


        public abstract float GetRotationAngle( float charHorizontalMovement, float charFwdMovement, float cameraHorizontalMovement, float cameraVerticalMovement );
    }

}

