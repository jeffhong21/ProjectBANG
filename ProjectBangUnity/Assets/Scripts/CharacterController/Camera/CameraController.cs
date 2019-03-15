namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;




    public class CameraController : MonoBehaviour
    {
        public enum ViewTypes { ThirdPerson, Topdown };

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
        public Transform m_Target;
        [SerializeField, Tooltip("Horizontal pivot point.")]
        private Transform m_YawPivot;
        [SerializeField, Tooltip("Vertical pivot point.")]
        private Transform m_PitchPivot;
        [SerializeField]
        private float m_MouseSensitivity = 2;
        [SerializeField]
        private CameraState m_CameraState;

        private CameraState[] m_CameraStates;
        private Dictionary<string, CameraState> m_CameraStateLookup;
        private bool m_IsRotating;





        private bool m_IsAiming;
		private bool m_IsCrouching;



        private bool m_FreeRotation;
        [SerializeField, DisplayOnly]
        private Vector3 m_ScreenToViewport;  //  Not used.  Just for viewing.
        private Vector2 m_MouseInput;
        [SerializeField, DisplayOnly]
        private float m_Yaw;            //  Rotation on Y Axis.  (Horizontal rotation)
        [SerializeField, DisplayOnly]
        private float m_Pitch;          //  Rotation on X Axis.  (Vertical rotation)
        [SerializeField, DisplayOnly]
        private float m_ZoomAmount;
        private float m_SmoothYaw;
        private float m_SmoothPitch;
        private float m_SmoothYawVelocity;
        private float m_SmoothPitchVelocity;
        private float m_SmoothRotationVelocity;

        private float m_TargetYawAngle;


        private Vector3 m_TargetPosition, m_YawPivotPosition, m_PitchPivotPosition;
        private Quaternion m_TargetRotation, m_YawPivotRotation, m_PitchPivotRotation;
        private Vector3 m_CameraPosition;
        private float m_DistanceFromTarget;



        private CharacterLocomotion m_Controller;
        private Camera m_Camera;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;


        [SerializeField, HideInInspector]
        private bool m_CameraStateToggle;


        //
        //  Properties
        // 
        public CameraState CameraState{
            get { return m_CameraState; }
        }

        public Camera Camera{
            get { return m_Camera; }
        }

        public Vector3 ScreenToViewPort{
            get{
                Vector3 screenToViewPort = m_Camera.ScreenToViewportPoint(Input.mousePosition);
                screenToViewPort.x -= 0.5f;
                screenToViewPort.y -= 0.5f;
                m_ScreenToViewport = screenToViewPort;
                return screenToViewPort;
            }
        }

        public bool FreeRotation{
            get { return m_FreeRotation; }
            set { m_FreeRotation = value; }
        }



        //
        //  Methods
        //
        private void Awake()
        {
            m_Instance = this;
            m_Camera = GetComponentInChildren<Camera>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;

            Initialize();
        }


		private void OnEnable()
		{
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
		}

		private void OnDisable()
		{
			
		}


        public void Initialize()
        {
            //m_CameraStateLookup = new Dictionary<string, CameraState>();
            //for (int i = 0; i < m_CameraStates.Length; i++){
            //    //  Add the camera state.
            //    if(!m_CameraStateLookup.ContainsKey(m_CameraStates[i].StateName)){
            //        m_CameraStateLookup.Add(m_CameraStates[i].StateName, m_CameraStates[i]);
            //        if(i == 0){
            //            m_CameraState = m_CameraStates[i];
            //        }
            //    }
            //}
            //if(m_CameraState == null){
            //    m_CameraState = ScriptableObject.CreateInstance<CameraState>();
            //}
        }


        public void SetMainTarget(Transform target)
        {
            m_Target = target;
            m_Controller = m_Target.gameObject.GetComponent<CharacterLocomotion>();
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
        }


        public void ChangeCameraState(string state){
            //if (m_CameraStateLookup.ContainsKey(state)){
            //    m_CameraState = m_CameraStateLookup[state];
            //    Debug.LogFormat("CameraState: {0}", m_CameraState.StateName);
            //} else {
            //    Debug.LogWarningFormat("** Camera State {0} does not exist", state);
            //}
        }

        public void SetDefaultCameraState(){
            //m_CameraState = m_CameraStates[0];
            //Debug.LogFormat("CameraState: {0}", m_CameraState.StateName);
        }




		private void Update()
		{
            
		}


		private void LateUpdate()
        {
            if (m_Target == null) return;

            HandleRotation();

            HandlePosition();

            if(m_Crosshair && m_Controller != null) m_Crosshair.enabled = m_Controller.Aiming;
        }



        private void HandleRotation()
        {
            if (m_FreeRotation)
            {
                m_Yaw += m_MouseInput.x * m_CameraState.YawRotateSpeed;
                if (!m_Controller.Aiming)
                    m_Yaw = Mathf.Clamp(m_Yaw, m_CameraState.MinYaw, m_CameraState.MaxYaw);
                m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
                m_YawPivotRotation = Quaternion.Slerp(m_YawPivot.localRotation, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);
                m_YawPivot.localRotation = m_YawPivotRotation;

                m_Pitch -= m_MouseInput.y * m_CameraState.PitchRotateSpeed;
                m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);
                m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
                m_PitchPivotRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);
                m_PitchPivot.localRotation = m_PitchPivotRotation;
            }
            else
            {
                m_MouseInput.x = 0;
                m_MouseInput.y = 0;
                m_Yaw = m_MouseInput.x * m_CameraState.YawRotateSpeed;
                m_Pitch = m_MouseInput.y * m_CameraState.PitchRotateSpeed;

                if (!m_Controller.Aiming)
                    m_Yaw = Mathf.Clamp(m_Yaw, m_CameraState.MinYaw, m_CameraState.MaxYaw);

                m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);


                m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
                m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);


                m_YawPivotRotation = Quaternion.Slerp(m_YawPivot.localRotation, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);
                m_PitchPivotRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);
                //  Update the vertical axis fo the camera pivot.
                m_YawPivot.localRotation = m_YawPivotRotation;
                //  Update the horizontal axis fo the camera pivot.
                m_PitchPivot.localRotation = m_PitchPivotRotation;


                var eulerY = Mathf.SmoothDampAngle(m_Transform.eulerAngles.y, m_Target.eulerAngles.y, ref m_SmoothRotationVelocity, m_CameraState.TurnSmooth);
                m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, eulerY, 0), m_CameraState.AdaptSpeed * m_DeltaTime);

               // m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, m_Target.eulerAngles.y, 0), m_CameraState.AdaptSpeed * m_DeltaTime);
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


            m_YawPivotPosition = m_YawPivot.localPosition;
            m_YawPivotPosition.x = targetX;
            m_YawPivotPosition.y = targetY;
            //m_YawPivotPosition.z = m_ZoomAmount;


            m_CameraPosition = m_Camera.transform.localPosition;
            m_CameraPosition.z = targetZ;

            float time = m_DeltaTime * m_CameraState.AdaptSpeed;
            m_YawPivot.localPosition = Vector3.Lerp(m_YawPivot.localPosition, m_YawPivotPosition, time);
            m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, m_CameraPosition, time);



            float speed = m_IsAiming ? m_CameraState.AimSpeed : m_CameraState.MoveSpeed;
            m_TargetPosition = Vector3.Lerp(m_Transform.position, m_Target.position, speed * m_DeltaTime);
            m_Transform.position = m_TargetPosition;
        }








        public void RotateCamera(float mouseX, float mouseY)
        {

            m_MouseInput.x = mouseX;
            m_MouseInput.y = mouseY;

            //if (m_LockRotation) return; 
            //if (m_Target == null) return;

            //m_IsRotating = (mouseX > 0 && mouseY > 0);

            //m_Yaw += mouseX * m_CameraState.YawRotateSpeed;
            //m_Pitch -= mouseY * m_CameraState.PitchRotateSpeed;
            //if (!m_Controller.Aiming) m_Yaw = Mathf.Clamp(m_Yaw, m_CameraState.MinYaw, m_CameraState.MaxYaw);
            //m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);

            //if (m_CameraState.TurnSmooth > 0){
            //    m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
            //    m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
            //}
            //else{
            //    m_SmoothYaw = m_Yaw;
            //    m_SmoothPitch = m_Pitch;
            //}


            //var eulerY = Mathf.SmoothDampAngle(m_Transform.eulerAngles.y, m_SmoothYaw, ref m_SmoothRotationVelocity, m_CameraState.TurnSmooth);
            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, eulerY, 0), m_CameraState.TurnSmooth);

            ////  Update the vertical axis fo the camera pivot.
            //m_YawPivot.localRotation = Quaternion.Slerp(m_YawPivot.localRotation, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);
            ////  Update the horizontal axis fo the camera pivot.
            //m_PitchPivot.localRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);

            //m_YawPivotRotation = Quaternion.Slerp(m_YawPivot.localRotation, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);
            //m_PitchPivotRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);
        }



        public void ZoomCamera(float zoomInput)
        {
            //var direction = m_Target.position - m_PitchPivot.position;
            //float distance = direction.magnitude;
            //var newDistance = distance + m_CameraState.ZoomStep * zoomInput;

            //m_ZoomAmount = Mathf.Lerp(distance, newDistance, m_CameraState.ZoomSmooth * m_DeltaTime);
            //if(m_ZoomAmount > m_CameraState.MaxZoom){
            //    m_ZoomAmount = m_CameraState.MaxZoom;
            //}
            //if (m_ZoomAmount > m_CameraState.MinZooom)
            //{
            //    m_ZoomAmount = m_CameraState.MinZooom;
            //}

            if(zoomInput != 0) Debug.LogFormat("Zoom Input {0}", zoomInput);
        }






        public void OrbitCamera(float mouseX, float mouseY)
        {
            //if (m_LockRotation) return; 
            //float pivotStart = 0.25f;
            //if(ScreenToViewPort.x < -pivotStart || ScreenToViewPort.x >  pivotStart )
            //{
            //    m_Yaw += mouseX * m_CameraState.YawRotateSpeed;
            //    m_Yaw = Mathf.Clamp(m_Yaw, m_CameraState.MinYaw, m_CameraState.MaxYaw);

            //    var yawSmoothDampSpeed = m_CameraState.TurnSmooth * Mathf.Abs(ScreenToViewPort.x);
            //    if (yawSmoothDampSpeed > 0)
            //        m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, yawSmoothDampSpeed);
            //    else
            //        m_SmoothYaw = m_Yaw;

            //    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);
            //}
            //else{
            //    var directionToTarget = m_Target.position - m_Transform.position;
            //    var rotatioinToTarget = m_Transform.rotation;
            //    if (directionToTarget != Vector3.zero)
            //        rotatioinToTarget = Quaternion.LookRotation(directionToTarget, m_Transform.up);

            //    var yawSmoothDampSpeed = m_CameraState.TurnSmooth * Mathf.Abs(ScreenToViewPort.x);
            //    if (yawSmoothDampSpeed > 0)
            //        m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, yawSmoothDampSpeed);
            //    else
            //        m_SmoothYaw = m_Yaw;

            //    m_Transform.rotation = Quaternion.Slerp(rotatioinToTarget, Quaternion.Euler(0, m_SmoothYaw, 0), m_CameraState.TurnSmooth);

            //}

            //if (ScreenToViewPort.y < -pivotStart || ScreenToViewPort.y > pivotStart)
            //{
            //    m_Pitch -= mouseY * m_CameraState.PitchRotateSpeed;
            //    m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);

            //    var pitchSmoothDampSpeed = m_CameraState.TurnSmooth * Mathf.Abs(ScreenToViewPort.y);
            //    if (pitchSmoothDampSpeed > 0)
            //        m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, pitchSmoothDampSpeed);
            //    else
            //        m_SmoothPitch = m_Pitch;

            //    //  Update the X-axis of the camera pivot.
            //    m_PitchPivot.localRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);
            //}



            ////var eulerY = Mathf.Atan2(mouseX, Mathf.Abs(mouseY));
            //eulerY = Mathf.Rad2Deg * m_CameraState.AdaptSpeed * m_DeltaTime;
            //eulerY *= mouseX;
            //eulerY += m_Transform.eulerAngles.y;

            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, m_Target.rotation, m_CameraState.AdaptSpeed * m_DeltaTime);



            //m_Pitch -= mouseY * m_CameraState.PitchRotateSpeed;
            //m_Pitch = Mathf.Clamp(m_Pitch, m_CameraState.MinPitch, m_CameraState.MaxPitch);

            //if(m_CameraState.TurnSmooth > 0){
            //    m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
            //    m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
            //} else {
            //    m_SmoothYaw = m_Yaw;
            //    m_SmoothPitch = m_Pitch;
            //}

            //var eulerYaw = Quaternion.Euler(0, m_SmoothYaw, 0);
            ////var targetEuler = Quaternion.Euler(0, m_Target.eulerAngles.y, 0);
            ////m_TargetRotation = eulerYaw * targetEuler;
            //m_TargetRotation = eulerYaw;

            //var eulerY = Mathf.SmoothDampAngle(m_Transform.eulerAngles.y, m_TargetRotation.eulerAngles.y, ref m_SmoothRotationVelocity, m_CameraState.TurnSmooth);
            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.Euler(0, eulerY, 0), m_CameraState.TurnSmooth);

            ////  Update the X-axis of the camera pivot.
            //m_PitchPivot.localRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_SmoothPitch, 0, 0), m_CameraState.TurnSmooth);
        }











    }
}


