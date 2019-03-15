using UnityEngine;

namespace CharacterController
{
    [CreateAssetMenu(menuName = "Character Controller/Camera State")]
    public class CameraState : ScriptableObject
    {
        public string StateName = "CameraState";
        [Header("--  Move Settings  --")]
        public float MoveSpeed = 9;
        public float AimSpeed = 24;
        [Tooltip("The lerp speed when handling camera position.")]
        public float AdaptSpeed = 8;


        [Header("--  Position Settings  --")]
        public float DefaultPositionX = 0;
        public float DefaultPositionZ = 3;
        public float DefaultPositionY = 1.5f;


        [Header("--  Rotation Settings  --")]
        public float TurnSmooth = 0.12f;
        [Tooltip("Left and right rotation speed.")]
        public float YawRotateSpeed = 4; 
        [Tooltip("Up and down rotation speed.")]
        public float PitchRotateSpeed = 2;
        [Tooltip("Minimum and maximum left and right rotation. ")]
        public float MinYaw = -90;
        [Tooltip("Minimum and maximum left and right rotation. ")]
        public float MaxYaw = 90;
        [Tooltip("Minimum and maximum up and down rotation. ")]
        public float MinPitch = -35;
        [Tooltip("Minimum and maximum up and down rotation. ")]
        public float MaxPitch = 35;
        public float PivotRotationOffset;


        [Header("--  Zoom Settings  --")]
        public bool AllowZoom;
        public float ZoomSmooth = 0.12f;
        public float ZoomStep = 1;
        public float MaxZoom = 10;
        public float MinZooom = -1;


        [Space]
        [Header("--  Misc Settings  --")]
        public float AimPositionX = 0.5f;
        public float AimPositionZ = 1;

        public float CrouchPositionY = 1;
        [HideInInspector]
        public Vector3 LookDirection = Vector3.forward;


    }
}
