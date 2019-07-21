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
        [SerializeField] private float m_LookDistance = 50f;
        [SerializeField] private bool m_AxisRaw = true;
        [SerializeField] private bool m_LockCursor = true;
        [SerializeField] private bool m_CursorVisible;



        [Header("-- Debug Settings --")]
        [SerializeField, Tooltip("What key to select player.")]
        private KeyCode m_SelectPlayerKeyCode = KeyCode.Alpha0;
        [SerializeField, Tooltip("Locks camera's rotation.")]
        private KeyCode m_LockCameraRotation = KeyCode.L;
        [SerializeField, Tooltip("What key to debug.break.")]
        private KeyCode m_DebugBreakKeyCode = KeyCode.Delete;


        private float inputHorizontal, inputVertical;
        private Vector3 inputVector;
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


            //CharacterDebugUI.Instance.Initialize(m_Controller);
            //Debug.Log(Resources.Load<CharacterDebugUI>("CharacterDebugUI"));

        }


        //private float GetDeltaYRotation( float horizontalMovement, float forwardMovement, float cameraHorizontalMovement, float cameraVerticalMovement )
        //{
        //    float movementAngle = Mathf.Atan2(horizontalMovement, forwardMovement) * Mathf.Rad2Deg;
        //    float targetAngle = Mathf.Atan2(cameraHorizontalMovement, cameraVerticalMovement) * Mathf.Rad2Deg;

        //    //float deltaY = Mathf.SmoothDampAngle(fwdAngle, targetAngle, ref rotationVelocitySmooth, 0.1f) * Time.deltaTime;
        //    return targetAngle - movementAngle;
        //}



        float targetAngle;
        float rotationAngle;
        float rotationVelocity;
        private void Update()
		{
            inputVector = InputVectorRaw;

            lookDirection = m_CameraController == null ? mTransform.forward : Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1).normalized);
            lookRotation = Quaternion.FromToRotation(mTransform.forward, lookDirection);

            //inputVector = m_Camera.right * InputVectorRaw.x + lookDirection * InputVectorRaw.z;

            m_Controller.InputVector = inputVector;
            m_Controller.LookRotation = lookRotation;




            //lookDirection = m_CameraController == null ? mTransform.forward : Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            //lookRotation = Quaternion.FromToRotation(mTransform.forward, lookDirection);
            //m_Controller.InputVector = InputVectorRaw;
            //m_Controller.LookRotation = lookRotation;
            //m_Controller.LookDirection = lookDirection;

            //m_Controller.Move(InputVectorRaw.x, InputVectorRaw.z, lookRotation);
            Debug.DrawRay(mTransform.position + Vector3.up * 1.5f, lookDirection, Color.black);
            DebugButtonPress();
        }



        private void LateUpdate()
        {
            //  -----------
            //  Camera Input
            if (m_CameraController != null)
            {
                m_CameraController.RotateCamera(MouseInputVector.x, MouseInputVector.y);
                //m_CameraController.ZoomCamera(Input.GetAxisRaw(m_MouseScrollInput));

                if (Input.GetKey(KeyCode.Tab))
                {

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

            if (Input.GetKey(KeyCode.LeftShift))
            {

            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LevelManager.PauseGame();
            }


        }



        private void OnAimActionStart(bool aim)
        {
            //if (aim == true)
            //{
            //    CameraState aimState = CameraController.Instance.GetCameraStateWithName("TPS_Aim");
            //    CameraController.Instance.ChangeCameraState("TPS_Aim");
            //}
            //else
            //{
            //    CameraController.Instance.ChangeCameraState("TPS_Default");
            //}
  

            if (aim)
                CameraController.Instance.SetCameraState("AIM");
            else
                CameraController.Instance.SetCameraState("DEFAULT");

        }




	}
}


