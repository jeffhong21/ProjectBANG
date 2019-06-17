namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    public enum CameraViewType { ThirdPerson, Cinamachine };


    public class CameraController : MonoBehaviour
    {
        protected static CameraController _instance;
        protected static bool _lockRotation;

        public static CameraController Instance{
            get { return _instance; }
        }

        public static bool LockRotation{
            get { return _lockRotation; }
            set { _lockRotation = value; }
        }




        protected bool IsMoving;

        [SerializeField, DisplayOnly]
        protected string m_ActiveCameraState;
        //[SerializeField]
        protected CameraState m_CameraState;
        [SerializeField]
        protected CameraState[] m_CameraStates;
        [SerializeField]
        protected GameObject m_Character;
        [SerializeField]
        protected Transform m_Anchor;
        [SerializeField]
        protected bool m_AutoAnchor;
        [SerializeField]
        protected HumanBodyBones m_AutoAnchorBone = HumanBodyBones.Head;


        [Header("--  Debug Options --")]
        [SerializeField]
        protected bool m_Debug;


        [Serializable]
        public class CursorOptions
        {
            public bool lockCursor;
            public bool cursorVisible;
        }
        [SerializeField]
        protected CursorOptions m_CursorOptions = new CursorOptions();



        protected Dictionary<string, CameraState> m_CameraStateLookup;







        protected Vector3 m_MouseInput;
        //[SerializeField, DisplayOnly]
        protected float m_Yaw;            //  Rotation on Y Axis.  (Horizontal rotation)
        //[SerializeField, DisplayOnly]
        protected float m_Pitch;          //  Rotation on X Axis.  (Vertical rotation)
        //[SerializeField, DisplayOnly]
        protected float m_ZoomDistance;
        protected float m_SmoothYaw;
        protected float m_SmoothPitch;
        protected float m_SmoothYawVelocity;
        protected float m_SmoothPitchVelocity;
        protected float m_SmoothRotationVelocity;
        protected Vector3 m_CameraVelocitySmooth;
        //[SerializeField]
        protected float m_TargetYawAngle, m_TargetPitchAngle;

        protected Vector3 m_CurrentPosition, m_PreviousPosition;


        protected Vector3 m_TargetPosition, m_YawPivotPosition, m_PitchPivotPosition;
        protected Quaternion m_TargetRotation, m_YawPivotRotation, m_PitchPivotRotation;
        protected Vector3 m_LookDirection;
        //[SerializeField, DisplayOnly]
        protected Vector3 m_CameraPosition;
        protected float m_DistanceFromTarget;

        protected Vector3 m_ZoomVelocity;

        protected Transform m_PitchPivot;
        [SerializeField, DisplayOnly]
        protected Camera m_Camera;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected float m_DeltaTime;


        [SerializeField, HideInInspector] //  Used in editor script.
        protected bool m_CameraStateToggle;


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





        //
        //  Methods
        //
        protected virtual void Awake()
        {
            _instance = this;
            m_Camera = GetComponent<Camera>();
            if(m_Camera == null)
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
                Debug.Log("No Camera States.  Creating a default camera state.");
            }


        }


        protected virtual void OnEnable()
		{
            Cursor.lockState = m_CursorOptions.lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = m_CursorOptions.cursorVisible;
		}

        protected virtual void OnDisable()
		{
			
		}


        protected virtual void InitializeState()
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



        public virtual void SetMainTarget(GameObject target)
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
            Debug.LogFormat("** Camera State {0} does not exist", name);
            return false;
        }





		protected virtual void LateUpdate()
		{
            if (m_Camera.transform.root == m_GameObject)
                return;

            if (Math.Abs(Time.timeScale) < float.Epsilon)
                return;



            m_ActiveCameraState = ActiveState.name;
            if (m_Character == null)
                return;

            if (Math.Abs(m_Camera.fieldOfView - m_CameraState.FieldOfView) > float.Epsilon)
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_CameraState.FieldOfView, m_DeltaTime * m_CameraState.FieldOfViewSpeed);
            UpdatePosition();
            UpdateRotation();
		}





        //protected float m_ZoomStepVelocity;
        //protected Vector3 m_CameraZoom;
        protected virtual void UpdatePosition()
        {


            //if (m_CameraState.ApplyStepZoom)
            //{
            //    m_CameraZoom.z = Mathf.SmoothStep(m_CameraZoom.z, m_ZoomDistance, Time.deltaTime);
            //    //m_CameraZoom.z = Mathf.SmoothDamp(m_Camera.transform.localPosition.z, m_ZoomDistance, ref m_ZoomVelocity, m_CameraState.StepZoomSmooth);
            //    m_Camera.transform.localPosition = m_CameraZoom;
            //}

            if(m_PitchPivot != null)
                m_PitchPivotPosition = m_PitchPivot.localPosition;
            //m_PitchPivotPosition.z = -m_CameraState.ViewDistance;
            //m_PitchPivotPosition.y = m_CameraState.VerticalOffset;

            m_CameraPosition = m_Camera.transform.localPosition;
            var viewDistance = (-1 * m_CameraState.ViewDistance);
            if (m_CameraState.ApplyCameraOffset)
                m_CameraPosition = m_CameraPosition + m_CameraState.CameraOffset + (Vector3.forward * viewDistance);
            else
                m_CameraPosition.z = viewDistance;

            //m_PitchPivot.localPosition = Vector3.Lerp(m_PitchPivot.localPosition, m_PitchPivotPosition, 12 * m_DeltaTime);
            m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, m_CameraPosition, 12 * m_DeltaTime);

            //m_TargetPosition = Vector3.Lerp(m_Transform.position, m_Anchor.position, 12 * m_DeltaTime);
            m_TargetPosition = Vector3.SmoothDamp(m_Transform.position, m_Anchor.position, ref m_CameraVelocitySmooth, 0.18f);
            m_TargetPosition.y = m_Anchor.position.y + m_CameraState.VerticalOffset;
            //m_Transform.position = m_TargetPosition;
            m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetPosition, m_CameraState.MoveSpeed * m_DeltaTime);


        }


        //float angle;
        protected virtual void UpdateRotation()
        {
            //  Main controller
            if (m_CameraState.ApplyRotation){
                //angle = Quaternion.Angle(m_Transform.rotation, m_Character.transform.rotation);
                if (Quaternion.Angle(m_Transform.rotation, m_Character.transform.rotation) != float.Epsilon){
                    m_TargetRotation = Quaternion.FromToRotation(m_Transform.forward, m_Character.transform.forward);
                    //m_TargetRotation = Quaternion.FromToRotation(Vector3.up, m_Character.transform.forward);
                    m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation * m_Transform.rotation, m_CameraState.RotationSpeed * m_DeltaTime);
                }
            }

            if (_lockRotation) return;


            if (m_CameraState.ApplyTurn)
            {
                m_Yaw = m_MouseInput.x;
                m_Pitch = m_MouseInput.y;
                if (m_CameraState.TurnSmooth > 0)
                {
                    m_SmoothYaw = Mathf.SmoothDamp(m_SmoothYaw, m_Yaw, ref m_SmoothYawVelocity, m_CameraState.TurnSmooth);
                    m_SmoothPitch = Mathf.SmoothDamp(m_SmoothPitch, m_Pitch, ref m_SmoothPitchVelocity, m_CameraState.TurnSmooth);
                }
                else
                {
                    m_SmoothYaw = m_Yaw;
                    m_SmoothPitch = m_Pitch;
                }

                m_TargetPitchAngle -= m_SmoothPitch * m_CameraState.TurnSpeed;
                m_TargetYawAngle += m_SmoothYaw * m_CameraState.TurnSpeed;

                if (m_CameraState.ApplyYawLimit)
                    m_TargetYawAngle = Mathf.Clamp(m_TargetYawAngle, m_CameraState.MinYaw, m_CameraState.MaxYaw);
                if (m_CameraState.ApplyPitchLimit)
                    m_TargetPitchAngle = Mathf.Clamp(m_TargetPitchAngle, m_CameraState.MinPitch, m_CameraState.MaxPitch);

                m_Transform.rotation = Quaternion.Euler(0, m_TargetYawAngle, 0);
                if (m_PitchPivot != null)
                    m_PitchPivot.localRotation = Quaternion.Euler(m_TargetPitchAngle, 0, 0);
                //m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(0, m_TargetYawAngle, 0), m_CameraState.TurnSpeed * m_DeltaTime);
                //m_PitchPivot.localRotation = Quaternion.Lerp(m_PitchPivot.localRotation, Quaternion.Euler(m_TargetPitchAngle, 0, 0), m_CameraState.TurnSpeed * m_DeltaTime); ;
            }


            ////  Camera Look Towards Anchor.
            //if (m_CameraState.ApplyCameraOffset){
            //    var lookDirection = m_PitchPivot.transform.position - m_Camera.transform.position;
            //    m_Camera.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            //}

        }



        public virtual void RotateCamera(float mouseX, float mouseY)
        {
            m_MouseInput.x = mouseX;
            m_MouseInput.y = mouseY;
        }


        public virtual void ZoomCamera(float zoomInput)
        {
            var direction = m_Character.transform.position - m_Camera.transform.position;
            m_ZoomDistance = direction.magnitude + m_CameraState.StepZoomSensitivity * Mathf.Sign(zoomInput);

            if (m_ZoomDistance > m_CameraState.MaxStepZoom)
            {
                m_ZoomDistance = m_CameraState.MaxStepZoom;
            }
            if (m_ZoomDistance < m_CameraState.MinStepZooom)
            {
                m_ZoomDistance = m_CameraState.MinStepZooom;
            }


            //m_Camera.transform.localPosition += new Vector3(0, 0, m_ZoomDistance);
            //m_CameraPosition.z = Mathf.SmoothDamp(m_CameraPosition.z, m_CameraPosition.z + m_ZoomDistance, ref cameraZoomVelocity, m_CameraState.StepZoomSmooth);
        }



        //RaycastHit[] hits;
        //protected void OcclusionDetection()
        //{
        //    float distance = Vector3.Distance(m_Camera.transform.position, m_Character.transform.position);
        //    hits = Physics.RaycastAll(m_Camera.transform.position, m_Camera.transform.forward, distance);
        //    for (int i = 0; i < hits.Length; i++)
        //    {
        //        RaycastHit hit = hits[i];
        //        Renderer rend = hit.transform.GetComponent<Renderer>();
        //        if(rend){
        //            // Change the material of all hit colliders
        //            // to use a transparent shader.
        //            rend.material.shader = Shader.Find("Transparent/Diffuse");
        //            Color tempColor = rend.material.color;
        //            tempColor.a = 0.3F;
        //            rend.material.color = tempColor;
        //        }
        //    }
        //}



        //bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
        //{
        //    bool value = false;
        //    //if (m_Debug) Debug.DrawRay(from, _to.LowerLeft);
        //    if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer))
        //    {
        //        value = true;
        //        //cullingDistance = hitInfo.distance;
        //    }

        //    if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer))
        //    {
        //        value = true;
        //        //if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        //    }

        //    if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer))
        //    {
        //        value = true;
        //        //if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        //    }

        //    if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer))
        //    {
        //        value = true;
        //        //if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        //    }

        //    return value;
        //}








        //protected float ClampAngle(float clampAngle, float min, float max)
        //{
        //    do
        //    {
        //        if (clampAngle < -360)
        //            clampAngle += 360;
        //        if (clampAngle > 360)
        //            clampAngle -= 360;
        //    } while (clampAngle < -360 || clampAngle > 360);

        //    return Mathf.Clamp(clampAngle, min, max);
        //}


        //public struct ClipPlanePoints
        //{
        //    public Vector3 UpperLeft;
        //    public Vector3 UpperRight;
        //    public Vector3 LowerLeft;
        //    public Vector3 LowerRight;
        //}



        //public ClipPlanePoints NearClipPlanePoints(Camera camera, Vector3 pos, float clipPlaneMargin)
        //{
        //    var clipPlanePoints = new ClipPlanePoints();

        //    var transform = camera.transform;
        //    var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
        //    var aspect = camera.aspect;
        //    var distance = camera.nearClipPlane;
        //    var height = distance * Mathf.Tan(halfFOV);
        //    var width = height * aspect;
        //    height *= 1 + clipPlaneMargin;
        //    width *= 1 + clipPlaneMargin;
        //    clipPlanePoints.LowerRight = pos + transform.right * width;
        //    clipPlanePoints.LowerRight -= transform.up * height;
        //    clipPlanePoints.LowerRight += transform.forward * distance;

        //    clipPlanePoints.LowerLeft = pos - transform.right * width;
        //    clipPlanePoints.LowerLeft -= transform.up * height;
        //    clipPlanePoints.LowerLeft += transform.forward * distance;

        //    clipPlanePoints.UpperRight = pos + transform.right * width;
        //    clipPlanePoints.UpperRight += transform.up * height;
        //    clipPlanePoints.UpperRight += transform.forward * distance;

        //    clipPlanePoints.UpperLeft = pos - transform.right * width;
        //    clipPlanePoints.UpperLeft += transform.up * height;
        //    clipPlanePoints.UpperLeft += transform.forward * distance;

        //    return clipPlanePoints;
        //}








        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent();
        Vector2 size;
        protected void OnGUI()
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


