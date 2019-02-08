namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    [CreateAssetMenu(menuName = "CharacterController/Camera State")]
    public class CameraState : ScriptableObject
    {
        public float TurnSmooth = 0.12f;
        public float MoveSpeed = 9;
        public float AimSpeed = 24;
        public float AdaptSpeed = 8;
        [Header("Rotation Settings")]
        public float YawRotateSpeed = 4;
        public float PitchRotateSpeed = 2;
        public float MinYaw = -90;
        public float MaxYaw = 90;
        public float MinPitch = -35;
        public float MaxPitch = 35;
        public float PivotRotationOffset;
        [Header("Position Settings")]
        public float DefaultPositionX = 0;
        public float DefaultPositionZ = 3;
        public float AimPositionX = 0.5f;
        public float AimPositionZ = 1;
        public float DefaultPositionY = 1.5f;
        public float CrouchPositionY = 1;

        public Vector3 LookDirection = Vector3.forward;


    }
}
