namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using JH_Utils;



    public class CameraController : MonoBehaviour
    {
        private static CameraController m_Instance;
        private static bool m_LockRotation;

        public static CameraController Instance{
            get { return m_Instance; }
        }

        public static bool LockRotation{
            get { return m_LockRotation; }
            set { m_LockRotation = value; }
        }


        [SerializeField]
        private Image m_Crosshair;
        [SerializeField]
        private bool m_LockCursor;
        public GameObject m_Target;
        [SerializeField]
        private Transform m_Pivot;
        [SerializeField]
        private float m_MouseSensitivity = 2;
        [SerializeField]
        private CameraState m_CameraState;

        [SerializeField] private bool m_IsAiming;
        [SerializeField] private bool m_IsCrouching;
        [SerializeField] private bool m_LockCamera;


        [SerializeField, DisplayOnly]
        private Vector3 m_ScreenToViewport;  //  Not used.  Just for viewing.
        [SerializeField, DisplayOnly]
        private float m_Yaw;            //  Rotation on Y Axis.  (Horizontal rotation)
        [SerializeField, DisplayOnly]
        private float m_Pitch;          //  Rotation on X Axis.  (Vertical rotation)
        private float m_SmoothYaw;
        private float m_SmoothPitch;
        private float m_SmoothYawVelocity;
        private float m_SmoothPitchVelocity;

        private float m_TargetYawAngle;
        private Quaternion m_TargetRotation;
        private Vector3 m_TargetPosition;
        private Vector3 m_PivotPosition;
        private Vector3 m_CameraPosition;


        private CharacterLocomotion m_Controller;
        private Camera m_Camera;
        private GameObject m_GameObject;
        private Transform m_Transform;



        [SerializeField, HideInInspector]
        private bool m_CameraStateToggle;


        public CameraState CameraState{
            get { return m_CameraState; }
        }

        public Camera Camera{
            get { return m_Camera; }
        }


        private void Awake()
        {
            m_Instance = this;
            m_Camera = GetComponentInChildren<Camera>();
            m_GameObject = gameObject;
            m_Transform = transform;
        }


		public void Start()
		{
            if(m_Target != null){
                m_Controller = m_Target.GetComponent<CharacterLocomotion>();
            }

            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
		}


		private void OnEnable()
		{
            //EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, "OnTakeDamage", OnTakeDamage);
		}

		private void OnDisable()
		{
			
		}


		public void LateUpdate()
        {
            if (m_Target == null) return;
            m_ScreenToViewport = m_Camera.ScreenToViewportPoint(Input.mousePosition);

            HandlePosition();
            if(!m_LockRotation)
                HandleRotation();

            float speed = m_IsAiming ? m_CameraState.AimSpeed : m_CameraState.MoveSpeed;

            m_TargetPosition = Vector3.Lerp(m_Transform.position, m_Target.transform.position, speed * Time.deltaTime);
            m_Transform.position = m_TargetPosition;


            if(m_Crosshair){
                m_Crosshair.enabled = m_Controller.Aiming;
            }
        }



        private void HandlePosition()
        {
            float targetX = m_CameraState.DefaultPositionX;
            float targetZ = -m_CameraState.DefaultPositionZ;
            float targetY = m_CameraState.DefaultPositionY;

            if(m_Controller != null){
                if (m_IsCrouching)
                    targetY = m_CameraState.CrouchPositionY;

                if (m_Controller.Aiming)
                {
                    m_IsAiming = m_Controller.Aiming;
                    targetX = m_CameraState.AimPositionX;
                    targetZ = -m_CameraState.AimPositionZ;
                }
            }


            m_PivotPosition = m_Pivot.localPosition;
            m_PivotPosition.x = targetX;
            m_PivotPosition.y = targetY;

            m_CameraPosition = m_Camera.transform.localPosition;
            m_CameraPosition.z = targetZ;

            float time = Time.deltaTime * m_CameraState.AdaptSpeed;
            m_Pivot.localPosition = Vector3.Lerp(m_Pivot.localPosition, m_PivotPosition, time);
            m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, m_CameraPosition, time);
        }


        private void HandleRotation()
        {
            m_Yaw += Input.GetAxis("Mouse X") * m_CameraState.YawRotateSpeed;
            m_Pitch -= Input.GetAxis("Mouse Y") * m_CameraState.PitchRotateSpeed;
            //m_Yaw = Mathf.Clamp(m_Yaw, m_CameraState.MinYaw, m_CameraState.MaxYaw);
            m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);


            if(m_CameraState.TurnSmooth > 0)
            {
                m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
                m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
            }
            else{
                m_SmoothYaw = m_Yaw;
                m_SmoothPitch = m_Pitch;
            }

            //eulerYAngle = Mathf.SmoothDampAngle(m_Transform.eulerAngles.y, m_Target.transform.eulerAngles.y, ref rotationVel, m_CameraState.TurnSmooth);
            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, eulerYAngle, 0), m_CameraState.YawRotateSpeed * Time.deltaTime);





            if (m_Controller != null)
            {
                //  If character is aiming.
                if (m_Controller.Aiming)
                {
                    m_TargetYawAngle = m_Transform.eulerAngles.y;
                    m_Controller.LookRotation = Quaternion.Euler(0, m_TargetYawAngle, 0);
                    //m_Controller.LookRotation = Quaternion.Euler(m_SmoothPitch, m_TargetYawAngle, 0);
                }
                //  If character is moving and not aiming.
                else if (Mathf.Clamp01(Mathf.Abs(m_Controller.InputVector.x) + Mathf.Abs(m_Controller.InputVector.z)) > 0)
                {
                    m_TargetYawAngle = Mathf.Atan2(m_Controller.RelativeInputVector.x, Mathf.Abs(m_Controller.RelativeInputVector.z));
                    m_TargetYawAngle = Mathf.Rad2Deg * m_TargetYawAngle;
                    m_TargetYawAngle += m_Transform.eulerAngles.y;
                    m_Controller.LookRotation = Quaternion.Euler(0, m_TargetYawAngle, 0);
                }
                else{
                    //m_TargetYawAngle = m_Transform.eulerAngles.y;
                    //m_Controller.LookRotation = Quaternion.Euler(0, m_TargetYawAngle, 0);
                }
            }

            //  Update the camera rig holder.
            m_TargetRotation = Quaternion.Euler(0, m_SmoothYaw, 0);
            m_Transform.rotation = m_TargetRotation;
            //  Update the camera pivot.
            m_Pivot.localRotation = Quaternion.Euler(m_SmoothPitch - m_CameraState.PivotRotationOffset, 0, 0);

        }





    }
}


