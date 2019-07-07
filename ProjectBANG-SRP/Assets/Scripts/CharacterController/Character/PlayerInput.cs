namespace CharacterController
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class PlayerInput : MonoBehaviour
    {
        private static readonly string m_HorizontalInputName = "Horizontal";
        private static readonly string m_VerticalInputName = "Vertical";
        private static readonly string m_RotateCameraXInput = "Mouse X";
        private static readonly string m_RotateCameraYInput = "Mouse Y";
        private static readonly string m_MouseScrollInput = "Mouse ScrollWheel";


        [SerializeField]
        private float m_MoveAxisDeadZone = 0.2f;
        [SerializeField]
        private float m_LookDistance = 50f;
        //[SerializeField]
        //private LayerMask m_LayerMask;
        [SerializeField]
        private bool m_AxisRaw = true;



        [Header("-- Debug Settings --")]
        [SerializeField, Tooltip("Shift + what key to select player.")]
        private KeyCode m_SelectPlayerKeyCode = KeyCode.Alpha0;
        [SerializeField, Tooltip("Locks camera's rotation.")]
        private KeyCode m_LockCameraRotation = KeyCode.L;
        [SerializeField, Tooltip("Shift + what key to debug.break.")]
        private KeyCode m_DebugBreakKeyCode = KeyCode.Delete;


        [SerializeField, DisplayOnly]
        private Vector3 m_InputVector, m_InputVectorRaw;
        //[SerializeField, DisplayOnly]
        private float m_MouseHorizontal, m_MouseVertical;
        private Vector3 m_MouseInputVector;



        private Vector3 m_CameraFwd;
        private Vector3 m_LookDirection;


        [SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;





        private Vector3 InputVectorRaw{
            get {
                m_InputVectorRaw.Set(Input.GetAxisRaw(m_HorizontalInputName), 0f, Input.GetAxisRaw(m_VerticalInputName));
                return m_InputVectorRaw;
            }
        }

        private Vector3 InputVector{
            get {
                m_InputVector.Set(Input.GetAxis(m_HorizontalInputName), 0f, Input.GetAxis(m_VerticalInputName));
                return m_InputVector;
            }
        }

        private Vector2 MouseInputVector{
            get { return new Vector2(Input.GetAxis(m_RotateCameraXInput), Input.GetAxis(m_RotateCameraYInput)); }
        }

        private Vector2 MouseInputVectorRaw{
            get { return new Vector2(Input.GetAxisRaw(m_RotateCameraXInput), Input.GetAxisRaw(m_RotateCameraYInput)); }
        }

        private float MouseZoomInput{
            get { return Input.GetAxis(m_MouseScrollInput) ; }
        }

        private float MouseZoomInputRaw
        {
            get { return Input.GetAxisRaw(m_MouseScrollInput); }
        }




        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;
            //m_LayerMask = ~(1 << gameObject.layer);
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
            m_CameraController = CameraController.Instance;
            m_Camera = CameraController.Instance.Camera.transform;

            if (m_Camera == null)
                m_Camera = Camera.main.transform;


            //CharacterDebugUI.Instance.Initialize(m_Controller);
            //Debug.Log(Resources.Load<CharacterDebugUI>("CharacterDebugUI"));

        }







        private void Update()
		{
            //  LOCK CAMERA
            //if (LockCameraRotation() == true) return;

            Vector3 lookDirection = m_CameraController == null ? m_Transform.forward : Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            //Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            Quaternion lookRotation = Quaternion.FromToRotation(m_Transform.forward, lookDirection);
            m_Controller.Move(InputVectorRaw.x, InputVectorRaw.z, lookRotation);
            m_Controller.LookDirection = lookDirection;

            //m_Controller.InputVector = InputVectorRaw;
            //m_LookDirection = m_CameraController == null ? m_Transform.forward : Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            //m_Controller.LookDirection = m_LookDirection;


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
            }

            //  Check if character is moving.
            //m_Controller.Moving = InputVector != Vector3.zero || InputVectorRaw != Vector3.zero;
        }


        //private bool LockCameraRotation()
        //{
        //    if (Input.GetKeyDown(m_LockCameraRotation)){
        //        Debug.Log(CameraController.LockRotation);
        //        if (CameraController.Instance != null){
        //            CameraController.LockRotation = !CameraController.LockRotation;
        //        }

        //        return true;
        //    }
        //    return false;
        //}



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
        }




	}
}


