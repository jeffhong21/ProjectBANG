namespace CharacterController
{
    using UnityEngine;
    using static CharacterController.RigidbodyCharacterController;

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


        

        
        [SerializeField] private float m_MoveAxisDeadZone = 0.2f;
        [SerializeField] private float lookDistance = 20;
        [SerializeField] private bool m_AxisRaw = true;
        [SerializeField] private bool m_LockCursor = true;
        [SerializeField] private bool m_CursorVisible;

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
        private KeyCode m_SelectPlayerKeyCode = KeyCode.Alpha0;
        [SerializeField, Tooltip("Locks camera's rotation.")]
        private KeyCode m_LockCameraRotation = KeyCode.L;
        [SerializeField, Tooltip("What key to debug.break.")]
        private KeyCode m_DebugBreakKeyCode = KeyCode.Delete;


        private float inputHorizontal, inputVertical;
        private Vector3 inputVector;
        private Vector3 cameraFwd = new Vector3(1, 0, 1);
        private float mouseHorizontal, mouseVertical;
        private Vector3 mouseInputVector;
        private Vector3 lookDirection;
        private Quaternion lookRotation;
        private float rotationVelocitySmooth;

        //[SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform mTransform;




        public Transform LookTarget { get { return lookTarget; } set { lookTarget = value; } }


        private Vector3 InputVectorRaw{
            get {
                inputVector.Set(Input.GetAxisRaw(m_HorizontalInputName), 0f, Input.GetAxisRaw(m_VerticalInputName));
                return inputVector;
            }
        }

        private Vector3 InputVector{
            get {
                inputVector.Set(Input.GetAxis(m_HorizontalInputName), 0f, Input.GetAxis(m_VerticalInputName));
                return inputVector;
            }
        }

        private Vector2 MouseInputVector{
            get { return new Vector2(Input.GetAxis(m_RotateCameraXInput), Input.GetAxis(m_RotateCameraYInput)); }
        }

        private Vector2 MouseInputVectorRaw {
            get { return new Vector2(Input.GetAxisRaw(m_RotateCameraXInput), Input.GetAxisRaw(m_RotateCameraYInput)); }
        }

        private float MouseZoomInput{ get { return Input.GetAxis(m_MouseScrollInput) ; } }

        private float MouseZoomInputRaw { get { return Input.GetAxisRaw(m_MouseScrollInput); } }




        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            mTransform = transform;

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
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = m_CursorVisible;

            m_CameraController = CameraController.Instance;
            m_Camera = CameraController.Instance.Camera.transform;

            if (m_Camera == null)
                m_Camera = Camera.main.transform;
            lookRay = new Ray(transform.position + Vector3.up * lookHeight, transform.forward);

            //CharacterDebugUI.Instance.Initialize(m_Controller);
            //Debug.Log(Resources.Load<CharacterDebugUI>("CharacterDebugUI"));

        }





        private void Update()
		{
            inputVector = m_AxisRaw ? InputVectorRaw : InputVector;
            cameraFwd.Set(1, 0, 1);

            lookDirection = m_CameraController == null ? mTransform.forward : Vector3.Scale(m_Camera.forward, cameraFwd).normalized;

            switch (m_Controller.Movement) {
                case (MovementTypes.Adventure):

                    inputVector = m_Camera.right * InputVector.x + lookDirection * InputVector.z;
                    //inputVector = Vector3.ProjectOnPlane(inputVector, transform.forward);

                    float turnAmount = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                    //float turnAmount = Mathf.SmoothDampAngle(m_Controller.transform.rotation.y, Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Rad2Deg, ref rotationVelocitySmooth, 0.1f);
                    lookRotation = Quaternion.AngleAxis(turnAmount, transform.up);
                    break;

                case (MovementTypes.Combat):
                    inputVector = InputVector;
                    //turnAmount = Mathf.Atan2(lookDirection.x, lookDirection.z);
                    //lookRotation = Quaternion.AngleAxis(turnAmount, transform.up);
                    lookRotation = Quaternion.FromToRotation(mTransform.forward, lookDirection);
                    break;
                default:
                    Debug.Log("<b><i>• PlayerInput</i></b> movement type is default");
                    inputVector = m_AxisRaw ? InputVectorRaw : InputVector;
                    lookRotation = Quaternion.FromToRotation(mTransform.forward, lookDirection);
                    break;
            }



            m_Controller.InputVector = inputVector;
            m_Controller.LookRotation = lookRotation;


            lookDistance = 8;
            //  Set the look target's position and rotation.
            lookRay.origin = transform.position + Vector3.up * lookHeight;
            lookRay.direction = lookDirection;
            lookTarget.position = lookRay.GetPoint(lookDistance);
            //if (Physics.Raycast(lookRay, lookDistance, checkLayer)) {
            //    lookTarget.position = Vector3.Lerp(lookTarget.position, hit.point, Time.deltaTime * 8);
            //} else {
            //    lookTarget.position = lookRay.GetPoint(lookDistance);

            //}


            float eulerY = Vector3.Angle(transform.forward, lookRay.direction);
            Quaternion targetRotation = Quaternion.AngleAxis(eulerY, transform.up);
            lookTarget.rotation = targetRotation;



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
                Gizmos.DrawRay(lookRay.origin, lookRay.direction * lookDistance);
                Gizmos.DrawSphere(lookTarget.position, 0.15f);
            }
        }


    }
}


