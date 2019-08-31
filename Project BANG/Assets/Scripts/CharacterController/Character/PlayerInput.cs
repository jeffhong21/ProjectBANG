namespace CharacterController
{
    using UnityEngine;

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

        [SerializeField] private Vector2 lookSensitivity = new Vector2(3, 3);
        [SerializeField] private float moveAxisDeadZone = 0.1f;
        [SerializeField] private float lookDistance = 20;
        [SerializeField] private bool axisRaw = true;
        [SerializeField] private bool lockCursor = true;
        [SerializeField] private bool cursorVisible;
        [SerializeField] private MouseButtonKeyCode rotateCharacterKeyCode = MouseButtonKeyCode.LMB;

        [SerializeField]
        private Transform lookTarget;

        private LayerMask checkLayer = ~0;
        private Ray lookRay;
        private RaycastHit hit;
        [SerializeField]
        private RaycastHit[] hitBuffer;
        private float lookHeight = 1.4f;
        

        [Header("-- Debug Settings --")]
        [SerializeField, Tooltip("What key to select player.")]
        private KeyCode m_SelectPlayerKeyCode = KeyCode.P;
        [SerializeField, Tooltip("Locks camera's rotation.")]
        private KeyCode m_LockCameraRotation = KeyCode.L;
        [SerializeField, Tooltip("What key to debug.break.")]
        private KeyCode m_DebugBreakKeyCode = KeyCode.Return;



        private Vector3 inputVector;
        private Vector3 cameraFwd = new Vector3(1, 0, 1);
        private float mouseHorizontal, mouseVertical;
        private Vector3 mouseInputVector;
        private Vector3 lookDirection;
        private Quaternion lookRotation;
        private float rotationVelocitySmooth;

        private Vector3 m_targetVelocitySmooth;

        //[SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform m_transform;





        public Vector2 LookSensitivity { get { return lookSensitivity; } set { lookSensitivity = value; } }

        public Transform LookTarget { get { return lookTarget; } set { lookTarget = value; } }


        private Vector3 InputVectorRaw{
            get {
                inputVector.Set(Input.GetAxisRaw(m_HorizontalInputName), 0f, Input.GetAxisRaw(m_VerticalInputName));
                return inputVector;
            }
        }

        private Vector3 InputVector{
            get {
                var inputX = Input.GetAxis(m_HorizontalInputName);
                var inputZ = Input.GetAxis(m_VerticalInputName);
                inputVector.Set(Mathf.Abs(inputX) < moveAxisDeadZone ? 0 : inputX, 0f, Mathf.Abs(inputZ) < moveAxisDeadZone ? 0 : inputZ);
                return inputVector;
            }
        }

        private Vector2 MouseInputVector{
            get { return new Vector2(Input.GetAxis(m_RotateCameraXInput) * lookSensitivity.x, Input.GetAxis(m_RotateCameraYInput) * lookSensitivity.y); }
        }

        private Vector2 MouseInputVectorRaw {
            get { return new Vector2(Input.GetAxisRaw(m_RotateCameraXInput) * lookSensitivity.x, Input.GetAxisRaw(m_RotateCameraYInput) * lookSensitivity.y); }
        }

        private float MouseZoomInput{ get { return Input.GetAxis(m_MouseScrollInput) ; } }

        private float MouseZoomInputRaw { get { return Input.GetAxisRaw(m_MouseScrollInput); } }








        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            m_transform = transform;

            lookTarget = new GameObject("Look Target").transform;
            lookTarget.parent = transform;

            FinalIKController finalIK = GetComponent<FinalIKController>();
            if(finalIK != null) {
                finalIK.LookTarget = lookTarget;
            }
        }

		private void OnEnable()
		{
            EventHandler.RegisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

            CharacterDebug.ActiveCharacter = m_Controller;
        }


		private void OnDisable()
		{
            EventHandler.UnregisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

            CharacterDebug.ActiveCharacter = null;
        }


		private void Start()
		{
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = cursorVisible;

            m_CameraController = CameraController.Instance;
            m_Camera = CameraController.Instance.Camera.transform;

            if (m_Camera == null)
                m_Camera = Camera.main.transform;
            lookRay = new Ray(transform.position + Vector3.up * lookHeight, transform.forward);

            //CharacterDebugUI.Instance.Initialize(m_Controller);
            //Debug.Log(Resources.Load<CharacterDebugUI>("CharacterDebugUI"));

        }





        private void FixedUpdate()
		{
            //inputVector = axisRaw ? InputVectorRaw : InputVector;
            inputVector = InputVector;
            cameraFwd.Set(1, 0, 1);

            lookDirection = m_CameraController == null ? m_transform.forward : Vector3.Scale(m_Camera.forward, cameraFwd).normalized;

            switch (m_Controller.Movement) {
                case (MovementTypes.Adventure):

                    Vector3 fwd = Vector3.ProjectOnPlane(m_Camera.forward, transform.up);
                    Vector3 right = Vector3.Cross(fwd, Vector3.up);
                    Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, fwd);
                    //inputVector = referentialShift * inputVector;
                    inputVector = m_Camera.right * inputVector.x + fwd * inputVector.z;
                    

                    //inputVector = m_Camera.right * InputVector.x + lookDirection * InputVector.z;
                    
                    //inputVector = Vector3.Project(inputVector, fwd);
                    //if(lookDirection != Vector3.zero)
                    //{
                    //    var fwd = Vector3.ProjectOnPlane(m_Camera.forward, transform.up);
                    //}
                    //Debug.DrawRay(transform.position + Vector3.up * 0.5f, inputVector, Color.black);


                    break;

                case (MovementTypes.Combat):
                    inputVector = InputVector;
                    //turnAmount = Mathf.Atan2(lookDirection.x, lookDirection.z);
                    //lookRotation = Quaternion.AngleAxis(turnAmount, transform.up);
                    lookRotation = Quaternion.FromToRotation(m_transform.forward, lookDirection);
                    break;
                default:
                    Debug.Log("<b><i>• PlayerInput</i></b> movement type is default");
                    inputVector = axisRaw ? InputVectorRaw : InputVector;
                    lookRotation = Quaternion.FromToRotation(m_transform.forward, lookDirection);
                    break;
            }



            lookDistance = 8;
            //  Set the look target's position and rotation.
            lookRay.origin = transform.position + Vector3.up * lookHeight;
            lookRay.direction = lookDirection;
            lookTarget.position = Vector3.SmoothDamp(lookTarget.position, lookRay.GetPoint(lookDistance), ref m_targetVelocitySmooth, 0.1f);
            lookTarget.rotation = Quaternion.RotateTowards(lookTarget.rotation, m_transform.rotation, Time.fixedDeltaTime * 4);
            //lookTarget.position = lookRay.GetPoint(lookDistance);
            //if (Physics.Raycast(lookRay, lookDistance, checkLayer)) {
            //    lookTarget.position = Vector3.Lerp(lookTarget.position, hit.point, Time.deltaTime * 8);
            //} else {
            //    lookTarget.position = lookRay.GetPoint(lookDistance);
            //}



            //  Get the characters view angle.
            var axisSign = Vector3.Cross(lookDirection, m_transform.forward).y;
            float viewAngle = Vector3.Angle(transform.forward, lookRay.direction) * (axisSign >= 0 ? -1f : 1f);
            DebugUI.Log(this, "ViewAngle", viewAngle);




            m_Controller.InputVector = inputVector;
            m_Controller.LookRotation = lookRotation;
            m_Controller.ViewAngle = viewAngle;


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
            if(lookTarget != null && DebugMode) {
                //UnityEditor.Handles.color = Color.magenta;
                //UnityEditor.Handles.SphereHandleCap(0, lookTarget.position, lookTarget.rotation, 1, EventType.Layout);


                Gizmos.color = Color.magenta;
                //Gizmos.DrawRay(lookRay.origin, lookRay.direction * lookDistance);
                Gizmos.DrawLine(lookRay.origin, lookTarget.position);
                Gizmos.DrawSphere(lookTarget.position, 0.15f);

                //  Forward direction for look target.
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(lookTarget.position, lookTarget.forward);
            }
        }


    }
}


