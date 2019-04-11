namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


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
        private bool IsMoving;
        [SerializeField, DisplayOnly]
        private string m_ActiveCameraState;
        //[SerializeField]
        private CameraState m_CameraState;
        [SerializeField]
        private CameraState[] m_CameraStates;
        [SerializeField]
        private GameObject m_Character;
        [SerializeField]
        private Transform m_Anchor;
        [SerializeField]
        private bool m_AutoAnchor;
        [SerializeField]
        private HumanBodyBones m_AutoAnchorBone = HumanBodyBones.Head;




        [Serializable]
        public class CursorOptions
        {
            public bool lockCursor;
            public bool cursorVisible;
        }
        [SerializeField]
        private CursorOptions m_CursorOptions = new CursorOptions();



        private Dictionary<string, CameraState> m_CameraStateLookup;


        private bool m_IsRotating;
        private bool m_IsAiming;
		private bool m_IsCrouching;
        private bool m_FreeRotation;




        //[SerializeField, DisplayOnly]
        private Vector3 m_ScreenToViewport;  //  Not used.  Just for viewing.
        [SerializeField, DisplayOnly]
        private Vector3 m_MouseInput;
        //[SerializeField, DisplayOnly]
        private float m_Yaw;            //  Rotation on Y Axis.  (Horizontal rotation)
        //[SerializeField, DisplayOnly]
        private float m_Pitch;          //  Rotation on X Axis.  (Vertical rotation)
        //[SerializeField, DisplayOnly]
        private float m_ZoomAmount;
        private float m_SmoothYaw;
        private float m_SmoothPitch;
        private float m_SmoothYawVelocity;
        private float m_SmoothPitchVelocity;
        private float m_SmoothRotationVelocity;
        private Vector3 m_CameraVelocitySmooth;
        [SerializeField]
        private float m_TargetYawAngle, m_TargetPitchAngle;

        private Vector3 m_CurrentPosition, m_PreviousPosition;


        private Vector3 m_TargetPosition, m_YawPivotPosition, m_PitchPivotPosition;
        private Quaternion m_TargetRotation, m_YawPivotRotation, m_PitchPivotRotation;
        private Vector3 m_LookDirection;
        private Vector3 m_CameraPosition;
        private float m_DistanceFromTarget;



        private Transform m_PitchPivot;
        private Camera m_Camera;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;


        [SerializeField, HideInInspector]
        private bool m_CameraStateToggle;


        //
        //  Properties
        // 
        public CameraState ActiveState{
            get { return m_CameraState; }
            set { m_CameraState = value; }
        }

        public CameraState[] CameraStates{
            get { return m_CameraStates; }
        }

        public GameObject Character{
            get { return m_Character; }
            set { m_Character = value; }
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
            m_PitchPivot = m_Camera.transform.parent;
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;



            m_CameraStateLookup = new Dictionary<string, CameraState>();
            for (int i = 0; i < m_CameraStates.Length; i++){
                //  Add the camera state.
                if (!m_CameraStateLookup.ContainsKey(m_CameraStates[i].name)){
                    m_CameraStateLookup.Add(m_CameraStates[i].name, m_CameraStates[i]);
                    if (i == 0) m_CameraState = m_CameraStates[i];
                }
            }

            if (m_CameraStates.Length == 0){
                m_CameraState = ScriptableObject.CreateInstance<CameraState>();
                m_CameraStateLookup.Add(m_CameraState.name, m_CameraState);
            }


        }


		private void OnEnable()
		{

            Cursor.lockState = m_CursorOptions.lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = m_CursorOptions.cursorVisible;
		}

		private void OnDisable()
		{
			
		}


        private void InitializeState()
        {
            m_CurrentPosition = m_Character.transform.position;
            m_PreviousPosition = m_CurrentPosition;

            //////m_Camera.transform.LookAt(m_Character.transform, Vector3.up);
            //var direction = m_Anchor.position - m_Camera.transform.position;
            ////direction.Normalize();
            //m_Camera.transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);

            //m_Camera.transform.localEulerAngles = Vector3.zero;
            //var direction = m_Anchor.position - m_Camera.transform.position;
            //var angle = Vector3.Angle(direction, m_Camera.transform.forward);
            //m_Camera.transform.rotation = Quaternion.Euler(angle, 0, 0);
        }



        public void SetMainTarget(GameObject target)
        {
            m_Character = target;
            var animator = m_Character.GetComponent<Animator>();

            if(m_Character){
                if (m_Anchor == null) m_Anchor = m_Character.transform;
                if (m_AutoAnchor && animator!= null) 
                    m_Anchor = animator.GetBoneTransform(m_AutoAnchorBone);


                InitializeState();
            }
        }



        public CameraState GetCameraStateWithName(string name){
            if (m_CameraStateLookup.ContainsKey(name))
                return m_CameraStateLookup[name];
            return null;
        }

        public bool ChangeCameraState(string name){
            if (m_CameraStateLookup.ContainsKey(name)){
                m_CameraState = m_CameraStateLookup[name];
                InitializeState();
                //Debug.LogFormat("CameraState: {0}", m_CameraState.name);
                return true;
            }                 
            //Debug.LogWarningFormat("** Camera State {0} does not exist", name);
            return false;
        }

        public bool ChangeCameraState(CameraState state)
        {
            if (m_CameraStateLookup.ContainsKey(state.name)){
                m_CameraState = m_CameraStateLookup[state.name];
                InitializeState();
                Debug.LogFormat("CameraState: {0}", m_CameraState.name);
                return true;
            }
            Debug.LogWarningFormat("** Camera State {0} does not exist", name);
            return false;
        }


        private float ClampAngle(float angle, float min, float max){
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }


		private void FixedUpdate()
		{
            m_PitchPivotPosition = m_PitchPivot.localPosition;
            //m_PitchPivotPosition.z = -m_CameraState.ViewDistance;
            m_PitchPivotPosition.y = m_CameraState.VerticalOffset;

            m_CameraPosition = m_Camera.transform.localPosition;
            if (m_CameraState.ApplyCameraOffset)
                m_CameraPosition = m_CameraState.CameraOffset;
            m_CameraPosition.z = m_CameraPosition.z + (-m_CameraState.ViewDistance);
                

            m_PitchPivot.localPosition = Vector3.Lerp(m_PitchPivot.localPosition, m_PitchPivotPosition, 12 * m_DeltaTime);
            m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, m_CameraPosition, 12 * m_DeltaTime);

            //m_TargetPosition = Vector3.Lerp(m_Transform.position, m_Anchor.position, time);
            m_TargetPosition = Vector3.SmoothDamp(m_Transform.position, m_Anchor.position, ref m_CameraVelocitySmooth, 0.12f);
            m_TargetPosition.y = m_Transform.position.y;
            m_Transform.position = m_TargetPosition;


		}


		private void Update()
        {
            m_ActiveCameraState = ActiveState.name;
            if (m_Character == null)
                return;

            //UpdateCameraMovement();
            HandleRotationMovement();

            if(m_Camera.fieldOfView != m_CameraState.FieldOfView)
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_CameraState.FieldOfView, m_DeltaTime * m_CameraState.FieldOfViewSpeed);
        }


        [SerializeField]
        float angle;
        private void HandleRotationMovement()
        {
            //  Main controller
            if (m_CameraState.ApplyRotation){
                angle = Quaternion.Angle(m_Transform.rotation, m_Character.transform.rotation);
                if (Quaternion.Angle(m_Transform.rotation, m_Character.transform.rotation) != 0){
                    m_TargetRotation = Quaternion.FromToRotation(m_Transform.forward, m_Character.transform.forward);
                    m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation * m_Transform.rotation, m_CameraState.RotationSpeed * m_DeltaTime);
                }
            }

            if (m_LockRotation) return;

            //  Yaw
            m_Yaw = m_MouseInput.x;
            m_Pitch = m_MouseInput.y;
            if(m_CameraState.TurnSmooth > 0){
                m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
                m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
            } else {
                m_SmoothYaw = m_Yaw;
                m_SmoothPitch = m_Pitch;
            }

            m_TargetPitchAngle -= m_SmoothPitch * m_CameraState.TurnSpeed;
            m_TargetYawAngle += m_SmoothYaw * m_CameraState.TurnSpeed;

            if (m_CameraState.ApplyYawLimit)
                m_TargetYawAngle = Mathf.Clamp(m_TargetYawAngle, m_CameraState.MinYaw, m_CameraState.MaxYaw);
            if (m_CameraState.ApplyPitchLimit)
                m_TargetPitchAngle = Mathf.Clamp(m_TargetPitchAngle, m_CameraState.MinPitch, m_CameraState.MaxPitch);
            
            if (m_CameraState.ApplyTurn)
            {
                m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(0, m_TargetYawAngle, 0), m_CameraState.TurnSpeed * m_DeltaTime);
                m_PitchPivot.localRotation = Quaternion.Lerp(m_PitchPivot.localRotation, Quaternion.Euler(m_TargetPitchAngle, 0, 0), m_CameraState.TurnSpeed * m_DeltaTime); ;
            }


            ////  Camera Look Towards Anchor.
            //if (m_CameraState.ApplyCameraOffset){
            //    var lookDirection = m_PitchPivot.transform.position - m_Camera.transform.position;
            //    m_Camera.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            //}

        }





        //private void UpdateCameraMovement()
        //{
        //    if (m_LockRotation) return;

        //    //  Yaw
        //    m_Yaw = m_MouseInput.x; // * m_CameraState.TurnSpeed;

        //    m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
        //    m_TargetYawAngle += m_SmoothYaw * m_CameraState.TurnSpeed;
        //    if (m_CameraState.ApplyYawLimit)
        //        m_TargetYawAngle = Mathf.Clamp(m_TargetYawAngle, m_CameraState.MinYaw, m_CameraState.MaxYaw);
            
        //    if (m_CameraState.ApplyTurn)
        //    {
        //        //m_TargetRotation = Quaternion.Euler(0, m_TargetYawAngle, 0);//* m_YawPivotRotation;
        //        //m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation, m_CameraState.TurnSpeed * m_DeltaTime);
        //        m_YawPivotRotation = Quaternion.Euler(0, m_TargetYawAngle, 0);//* m_YawPivotRotation;
        //        m_YawPivot.localRotation = Quaternion.Slerp(m_YawPivot.localRotation, m_YawPivotRotation, m_CameraState.TurnSpeed * m_DeltaTime);
        //    }


        //    //  Pitch
        //    m_Pitch = m_MouseInput.y; // * m_CameraState.TurnSpeed;

        //    m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
        //    m_TargetPitchAngle -= m_SmoothPitch * m_CameraState.TurnSpeed;

        //    if (m_CameraState.ApplyPitchLimit)
        //        m_TargetPitchAngle = Mathf.Clamp(m_TargetPitchAngle, m_CameraState.MinPitch, m_CameraState.MaxPitch);

        //    if (m_CameraState.ApplyTurn)
        //    {
        //        m_PitchPivotRotation = Quaternion.Euler(m_TargetPitchAngle, 0, 0);
        //        m_PitchPivot.localRotation = Quaternion.Slerp(m_PitchPivot.localRotation, Quaternion.Euler(m_TargetPitchAngle, 0, 0), m_CameraState.TurnSpeed * m_DeltaTime); ;
        //    }

        //    //m_Camera.transform.LookAt(m_Character.transform);
        //    var lookDirection = m_Character.transform.position + (Vector3.up * m_CameraState.VerticalOffset) - m_Camera.transform.position;
        //    m_Camera.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        //    //  Main controller
        //    if (m_CameraState.ApplyRotation){
        //        if (Quaternion.Angle(m_Transform.rotation, m_Character.transform.rotation) != 0){
        //            m_TargetRotation = Quaternion.FromToRotation(m_Transform.forward, m_Character.transform.forward);
        //            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, m_TargetRotation * m_Transform.rotation, m_CameraState.RotationSpeed * m_DeltaTime);
        //        }
        //    }
        //}


        //private void HandlePosition()
        //{
        //    if (m_CameraState.ApplyCameraOffset)
        //        m_YawPivot.localPosition = m_CameraState.CameraOffset;
        //    m_YawPivotPosition = m_YawPivot.localPosition;
        //    //m_YawPivotPosition.y = m_CameraState.VerticalOffset;

        //    m_CameraPosition = m_Camera.transform.localPosition;
        //    m_CameraPosition.z = -m_CameraState.ViewDistance;


        //    m_YawPivot.localPosition = Vector3.Lerp(m_YawPivot.localPosition, m_YawPivotPosition, 4 * m_DeltaTime);
        //    m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, m_CameraPosition, 4 * m_DeltaTime);

        //    //m_TargetPosition = Vector3.Lerp(m_Transform.position, m_Anchor.position, time);
        //    m_TargetPosition = Vector3.SmoothDamp(m_Transform.position, m_Anchor.position, ref m_CameraVelocitySmooth, 0.12f);
        //    m_TargetPosition.y = m_Transform.position.y;
        //    m_Transform.position = m_TargetPosition;

        //    //m_Transform.LookAt(m_Anchor, Vector3.up);
        //}






















        public void RotateCamera(float mouseX, float mouseY)
        {
            m_MouseInput.x = mouseX;
            m_MouseInput.y = mouseY;
        }

        public void ZoomCamera(float zoomInput)
        {
            //var direction = m_Character.transform.position - m_PitchPivot.position;
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









        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent();
        Vector2 size;
        private void OnGUI()
        {

            if(LockRotation){
                GUI.color = Color.red;
                content.text = string.Format("LOCK ROTATION IS: {0}", LockRotation);
                size = new GUIStyle(GUI.skin.label).CalcSize(content);
                GUILayout.BeginArea(new Rect(Screen.width - size.x - 25, 15, size.x * 2, size.y * 2), GUI.skin.box);
                GUILayout.Label(content);
                //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
                GUILayout.EndArea();
            }

        }





    }
}


