namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    using static CharacterController.RigidbodyCharacterController;
    using DebugUI;

    [DisallowMultipleComponent]
    public class PlayerInput : MonoBehaviour
    {
        #region Input Names
        private static readonly string m_HorizontalInputName = "Horizontal";
        private static readonly string m_VerticalInputName = "Vertical";
        private static readonly string m_RotateCameraXInput = "Mouse X";
        private static readonly string m_RotateCameraYInput = "Mouse Y";
        private static readonly string m_MouseScrollInput = "Mouse ScrollWheel";
        #endregion


        private enum MouseButtonKeyCode { LMB = 0, RMB = 1, Middle = 2 }

        [SerializeField] private Vector2 m_lookSensitivity = new Vector2(3, 3);
        [SerializeField] private float m_moveAxisDeadZone = 0.1f;
        [SerializeField] private float lookDistance = 20;
        [SerializeField] private bool axisRaw = true;
        [SerializeField] private bool lockCursor = true;
        [SerializeField] private bool cursorVisible;
        [SerializeField] private MouseButtonKeyCode rotateCharacterKeyCode = MouseButtonKeyCode.LMB;

        [SerializeField]
        private Transform m_lookTarget;



        

        [Header("-- Debug Settings --")]
        [SerializeField, Tooltip("What key to select player.")]
        private KeyCode m_SelectPlayerKeyCode = KeyCode.P;
        [SerializeField, Tooltip("Locks camera's rotation.")]
        private KeyCode m_LockCameraRotation = KeyCode.L;
        [SerializeField, Tooltip("What key to debug.break.")]
        private KeyCode m_DebugBreakKeyCode = KeyCode.Return;




        private bool m_freeLook;
        private Ray m_lookRay;
        private float m_lookHeight = 1.4f;
        private Vector3 m_inputVector;
        private Vector3 m_cameraFwd = new Vector3(1, 0, 1);
        private Vector3 m_lookDirection;
        private Quaternion m_lookRotation;


        private Vector3 m_targetVelocitySmooth;

        //[SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform m_transform;





        public Vector2 LookSensitivity { get { return m_lookSensitivity; } set { m_lookSensitivity = value; } }

        public Transform LookTarget { get { return m_lookTarget; } set { m_lookTarget = value; } }


        private Vector3 InputVectorRaw{
            get {
                m_inputVector.Set(Input.GetAxisRaw(m_HorizontalInputName), 0f, Input.GetAxisRaw(m_VerticalInputName));
                return m_inputVector;
            }
        }

        private Vector3 InputVector{
            get {
                var inputX = Input.GetAxis(m_HorizontalInputName);
                var inputZ = Input.GetAxis(m_VerticalInputName);
                m_inputVector.Set(Mathf.Abs(inputX) < m_moveAxisDeadZone ? 0 : inputX, 0f, Mathf.Abs(inputZ) < m_moveAxisDeadZone ? 0 : inputZ);
                return m_inputVector;
            }
        }

        private Vector2 MouseInputVector{
            get { return new Vector2(Input.GetAxis(m_RotateCameraXInput) * m_lookSensitivity.x, Input.GetAxis(m_RotateCameraYInput) * m_lookSensitivity.y); }
        }

        private Vector2 MouseInputVectorRaw {
            get { return new Vector2(Input.GetAxisRaw(m_RotateCameraXInput) * m_lookSensitivity.x, Input.GetAxisRaw(m_RotateCameraYInput) * m_lookSensitivity.y); }
        }

        private float MouseZoomInput{ get { return Input.GetAxis(m_MouseScrollInput) ; } }

        private float MouseZoomInputRaw { get { return Input.GetAxisRaw(m_MouseScrollInput); } }








        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            m_transform = transform;

            m_lookTarget = new GameObject("Look Target").transform;
            m_lookTarget.parent = transform;

            FinalIKController finalIK = GetComponent<FinalIKController>();
            if(finalIK != null) {
                finalIK.LookTarget = m_lookTarget;
            }
        }

		private void OnEnable()
		{
            EventHandler.RegisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

        }


		private void OnDisable()
		{
            EventHandler.UnregisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

        }


		private void Start()
		{
            InitializeInput();


            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = cursorVisible;

            m_CameraController = CameraController.Instance;
            m_Camera = CameraController.Instance.Camera.transform;

            if (m_Camera == null)
                m_Camera = Camera.main.transform;
            m_lookRay = new Ray(transform.position + Vector3.up * m_lookHeight, transform.forward);

            //CharacterDebugUI.Instance.Initialize(m_Controller);
            //Debug.Log(Resources.Load<CharacterDebugUI>("CharacterDebugUI"));

            CameraController.Instance.SetCameraState("TOPDOWN");
        }


        private string m_inputDebug;
        [SerializeField]
        private List<KeyCode>[] m_inputDownKeys;
        [SerializeField]
        private List<KeyCode>[] m_inputUpKeys;

        private void InitializeInput()
        {
            int actionCount = m_Controller.CharActions.Length;
            m_inputDownKeys = new List<KeyCode>[actionCount];
            m_inputUpKeys = new List<KeyCode>[actionCount];
            for (int i = 0; i < actionCount; i++){
                m_inputDownKeys[i] = new List<KeyCode>();
                m_inputUpKeys[i] = new List<KeyCode>();
            }

            for (int i = 0; i < actionCount; i++)
            {
                var characterAction = m_Controller.CharActions[i];
                if (characterAction.StartType == ActionStartType.ButtonDown ||
                    characterAction.StartType == ActionStartType.DoublePress ||
                    characterAction.StopType == ActionStopType.ButtonUp ||
                    characterAction.StopType == ActionStopType.ButtonToggle)
                {
                    string[] inputNames = characterAction.InputNames;
                    if(inputNames != null && inputNames.Length > 0) {
                        for (int k = 0; k < inputNames.Length; k++)
                        {
                            if (string.IsNullOrWhiteSpace(inputNames[k]))
                                m_inputDebug += characterAction.GetType().Name + ": No Input specified. \n";
                            else
                                m_inputDebug += characterAction.GetType().Name + ": [<b>" + inputNames[k] + "</b>] (" + k + ")\n";

                            if (string.IsNullOrWhiteSpace(inputNames[k]))
                                continue;

                            KeyCode? asciiChar = (KeyCode)System.Enum.Parse(typeof(KeyCode), inputNames[k]);

                            Debug.AssertFormat(asciiChar != null, characterAction, "[{0}] Incorrect keycode: | {1} |", characterAction.GetType().Name, inputNames[k]);
                            if (asciiChar == null) {
                                Debug.LogError(characterAction.GetType().Name + " has incorrect inputname.");
                            }


                            var inputKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), inputNames[k]);
                            if (characterAction.StartType == ActionStartType.ButtonDown || characterAction.StartType == ActionStartType.DoublePress)
                                m_inputDownKeys[i].Add(inputKey);
                            if (characterAction.StopType == ActionStopType.ButtonUp || characterAction.StopType == ActionStopType.ButtonToggle)
                                m_inputUpKeys[i].Add(inputKey);

                        }
                    }

                }


            }

            Debug.Log(m_inputDebug);
        }



        private void Update()
		{

            m_cameraFwd.Set(1, 0, 1);
            m_lookDirection = m_CameraController == null ? m_transform.forward : Vector3.Scale(m_Camera.forward, m_cameraFwd).normalized;
            //Vector3 fwd = Vector3.ProjectOnPlane(m_Camera.forward, transform.up);
            //Vector3 right = Vector3.Cross(fwd, Vector3.up);
            //m_lookDirection = m_Camera.right * InputVector.x + m_lookDirection * InputVector.z;

            m_inputVector = InputVector;
            //m_inputVector = axisRaw ? InputVectorRaw : InputVector;
            m_lookRotation = Quaternion.FromToRotation(m_transform.forward, m_lookDirection);

            m_Controller.InputVector = m_inputVector;
            m_Controller.LookRotation = m_lookRotation;
            //m_Controller.Move(m_inputVector.x, m_inputVector.z, m_lookRotation);

            lookDistance = 8;
            //  Set the look target's position and rotation.
            m_lookRay.origin = transform.position + Vector3.up * m_lookHeight;
            m_lookRay.direction = m_lookDirection;
            m_lookTarget.position = Vector3.SmoothDamp(m_lookTarget.position, m_lookRay.GetPoint(lookDistance), ref m_targetVelocitySmooth, 0.1f);
            m_lookTarget.rotation = Quaternion.RotateTowards(m_lookTarget.rotation, m_transform.rotation, Time.fixedDeltaTime * 4);









            DebugButtonPress();
        }


        private void ExecuteInput()
        {
            var inputString = "<i>[null]</i>";
            if (Input.anyKeyDown) {
                inputString = Input.inputString;
                DebugUI.Log(this, inputString, "KeyPressed", RichTextColor.LightBlue);
            }

            //for (int i = 0; i < m_inputDownKeys.Length; i++) {
            //    int keyCodeCount = m_inputDownKeys[i].Count;
            //    if (keyCodeCount > 0) {

            //        for (int k = 0; i < keyCodeCount; k++) {
            //            KeyCode keycode = m_inputDownKeys[i][k];
            //            if (Input.GetKeyDown(keycode)) {
            //                Debug.LogFormat("Initiated {0} with Keycode {1}", m_Controller.CharActions[i].name, keycode);
            //            }
            //        }
            //    }
            //}
        }


        private void LateUpdate()
        {
            UpdateCameraInmputs();
        }



        private void UpdateCameraInmputs()
        {
            //  -----------
            //  Camera Input
            if (m_CameraController == null) return;

            m_CameraController.UpdateRotation(MouseInputVector.x, MouseInputVector.y);

            m_CameraController.UpdateZoom(Input.GetAxisRaw(m_MouseScrollInput));


            for (int number = 0; number < CinamachineCameraController.Controller.VirtualStatesCount; number++){
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(number.ToString())){
                    string stateName = CinamachineCameraController.Controller.GetVirtualCameraState(number).StateName;
                    CinamachineCameraController.Controller.SetCameraState(stateName);
                }
            }
        }





        private void DebugButtonPress()
        {

            if (Input.GetKeyDown(m_SelectPlayerKeyCode))
            {
                UnityEditor.Selection.activeGameObject = gameObject;
            }

            if (Input.GetKeyDown(m_DebugBreakKeyCode))
            {
                Debug.Break();
            }

            if (Input.GetKey(KeyCode.M))
            {
                CinamachineCameraController.Controller.PlayImpulse();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LevelManager.PauseGame();
            }


        }



        private void OnAimActionStart(bool aim)
        {
            if (aim)
                CameraController.Instance.SetCameraState("AIM");
            else
                CameraController.Instance.SetCameraState("DEFAULT");

        }


        [SerializeField]
        bool DebugMode;
        private void OnDrawGizmosSelected()
        {
            if(m_lookTarget != null && DebugMode) {
                //UnityEditor.Handles.color = Color.magenta;
                //UnityEditor.Handles.SphereHandleCap(0, m_lookTarget.position, m_lookTarget.rotation, 1, EventType.Layout);


                Gizmos.color = Color.magenta;
                //Gizmos.DrawRay(m_lookRay.origin, m_lookRay.direction * lookDistance);
                Gizmos.DrawLine(m_lookRay.origin, m_lookTarget.position);
                Gizmos.DrawSphere(m_lookTarget.position, 0.15f);

                //  Forward direction for look target.
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(m_lookTarget.position, m_lookTarget.forward);
            }
        }

        GUIStyle guiStyle = new GUIStyle();
        Rect rect = new Rect();
        private void OnGUI()
        {
            guiStyle.normal.textColor = Color.black;
            rect.width = Screen.width * 0.25f;
            rect.x = (Screen.width * 0.5f) - (rect.width * 0.5f);
            rect.y = 8;
            rect.height = 16 + rect.y;
            GUI.Label(rect, "MouseInput: " + MouseInputVector.ToString(), guiStyle);
            rect.y += rect.height = 16; 
            rect.height += rect.height;
            GUI.Label(rect, "MouseInputAngle: " + Mathf.Atan2(MouseInputVector.x, MouseInputVector.y).ToString(), guiStyle);
        }

    }
}


