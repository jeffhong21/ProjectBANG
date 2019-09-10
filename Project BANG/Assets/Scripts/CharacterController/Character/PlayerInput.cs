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

        [SerializeField]
        private CharacterActionInputKeys[] m_inputKeys;

        [System.Serializable]
        private struct CharacterActionInputKeys
        {
            public CharacterAction characterAction;
            public KeyCode[] inputKeys;

            public CharacterActionInputKeys(CharacterAction characterAction, KeyCode[] inputKeys)
            {
                this.characterAction = characterAction;
                this.inputKeys = inputKeys;
            }
        }


        private bool m_freeLook;
        private Ray m_lookRay;
        private float m_lookHeight = 1.4f;
        private Vector3 m_inputVector;
        private Vector3 cameraFwd = new Vector3(1, 0, 1);
        private Vector3 lookDirection;
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





        private void FixedUpdate()
		{
            //m_inputVector = axisRaw ? InputVectorRaw : InputVector;
            m_inputVector = InputVector;
            cameraFwd.Set(1, 0, 1);

            lookDirection = m_CameraController == null ? m_transform.forward : Vector3.Scale(m_Camera.forward, cameraFwd).normalized;


            switch (m_Controller.Movement)
            {
                case (MovementTypes.Adventure):

                    Vector3 fwd = Vector3.ProjectOnPlane(m_Camera.forward, transform.up);
                    Vector3 right = Vector3.Cross(fwd, Vector3.up);
                    //Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, fwd);
                    //m_inputVector = referentialShift * m_inputVector;
                    //m_inputVector = right * m_inputVector.x + fwd * m_inputVector.z;
                    m_inputVector = m_Camera.right * InputVector.x + lookDirection * InputVector.z;

                    //m_inputVector = Vector3.Project(m_inputVector, fwd);
                    //if(lookDirection != Vector3.zero)
                    //{
                    //    var fwd = Vector3.ProjectOnPlane(m_Camera.forward, transform.up);
                    //}
                    //Debug.DrawRay(transform.position + Vector3.up * 0.5f, m_inputVector, Color.black);

                    //m_inputVector = InputVector;
                    break;

                case (MovementTypes.Combat):
                    m_inputVector = InputVector;
                    //turnAmount = Mathf.Atan2(lookDirection.x, lookDirection.z);
                    //m_lookRotation = Quaternion.AngleAxis(turnAmount, transform.up);

                    break;
                default:
                    Debug.Log("<b><i>• PlayerInput</i></b> movement type is default");
                    m_inputVector = axisRaw ? InputVectorRaw : InputVector;

                    break;
            }

            m_lookRotation = Quaternion.FromToRotation(m_transform.forward, lookDirection);
            DebugUI.Log(this, "playerInput", m_inputVector, RichTextColor.Red);

            m_Controller.InputVector = m_inputVector;
            m_Controller.LookRotation = m_lookRotation;





            lookDistance = 8;
            //  Set the look target's position and rotation.
            m_lookRay.origin = transform.position + Vector3.up * m_lookHeight;
            m_lookRay.direction = lookDirection;
            m_lookTarget.position = Vector3.SmoothDamp(m_lookTarget.position, m_lookRay.GetPoint(lookDistance), ref m_targetVelocitySmooth, 0.1f);
            m_lookTarget.rotation = Quaternion.RotateTowards(m_lookTarget.rotation, m_transform.rotation, Time.fixedDeltaTime * 4);









            DebugButtonPress();
        }



        private void LateUpdate()
        {
            //  -----------
            //  Camera Input
            if (m_CameraController != null){
                m_CameraController.RotateCamera(MouseInputVector.x, MouseInputVector.y);
                //m_CameraController.ZoomCamera(Input.GetAxisRaw(m_MouseScrollInput));

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


    }
}


