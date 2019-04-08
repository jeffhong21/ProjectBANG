namespace CharacterController
{
    using UnityEngine;


    public class PlayerInput : MonoBehaviour
    {
        private string m_HorizontalInputName = "Horizontal";
        private string m_VerticalInputName = "Vertical";
        private string m_RotateCameraXInput = "Mouse X";
        private string m_RotateCameraYInput = "Mouse Y";
        private string m_MouseScrollInput = "Mouse ScrollWheel";

        private bool m_AxisRaw = false;


        [Header("-- Debug Settings --")]
        //[SerializeField, DisplayOnly]
        private float m_Horizontal, m_Forward;
        //[SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        [SerializeField, DisplayOnly]
        private float m_MouseHorizontal, m_MouseVertical;
        private Vector3 m_MouseInputVector;
        [SerializeField]
        private float m_LookDistance = 50f;
        [SerializeField]
        private LayerMask m_LayerMask;


        private Ray m_Ray;


        private Vector3 m_ScreenCenter = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private ItemActionManager m_ItemAction;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;

        public string HorizontalInputName{
            get { return m_HorizontalInputName; }
        }

        public string VerticalInputName{
            get { return m_VerticalInputName; }
        }





        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_ItemAction = GetComponent<ItemActionManager>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;
            m_LayerMask = ~(1 << gameObject.layer);
        }

		private void OnEnable()
		{
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}

		private void OnDisable()
		{
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}

		private void Start()
		{
            if (CameraController.Instance == null && m_CameraController)
            {
                m_CameraController = Instantiate(m_CameraController) as CameraController;
                m_CameraController.SetMainTarget(m_GameObject);
                Debug.Log("Instantiating a new Camera Controller through the player.");
            }
            else if(m_CameraController == null && CameraController.Instance != null){
                m_CameraController = CameraController.Instance;
                m_CameraController.SetMainTarget(m_GameObject);
            }
            else{
                Debug.LogError("Player has no Camera");
            }

            //m_Ray = new Ray(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward * 10);
		}



		public virtual float GetAxis(string name, bool useRaw = false){
            return useRaw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }



		private void FixedUpdate()
		{
            m_Horizontal = GetAxis(m_HorizontalInputName, m_AxisRaw);
            m_Forward = GetAxis(m_VerticalInputName, m_AxisRaw);

            m_MouseHorizontal = GetAxis(m_RotateCameraXInput, false);
            m_MouseVertical = GetAxis(m_RotateCameraYInput, false);


            if(m_CameraController != null){
                m_InputVector.Set(m_Horizontal, 0, m_Forward);
                m_Controller.InputVector = m_InputVector;
                m_Controller.TurnAmount = m_MouseHorizontal;
            } else {
                m_InputVector = m_Horizontal * Vector3.right + m_Forward * Vector3.forward;
                m_Controller.InputVector = m_InputVector;
            }


            //  Set look at point.
            m_Controller.LookAtPoint = m_CameraController.Camera.transform.position + m_CameraController.Camera.transform.forward * m_LookDistance;
            //Debug.DrawRay(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward * m_LookDistance, Color.blue);

            m_MouseInputVector.Set(m_MouseHorizontal, m_MouseVertical, m_CameraController.Camera.nearClipPlane);
            //var viewport = m_CameraController.Camera.ViewportToWorldPoint(m_MouseInputVector);
            var direction = m_Controller.LookAtPoint - m_Transform.position;
            //var direction = m_Transform.position - m_CameraController.Camera.transform.position;
            //var direction = m_Transform.position - viewport;
            direction.y = 0;
            direction.Normalize();
            //Debug.DrawRay(m_Transform.position +(Vector3.up * 1.35f), direction * m_LookDistance, Color.green);

            m_Controller.LookDirection = m_CameraController.Camera.transform.forward * m_LookDistance;

            if(m_Controller.IndependentLook())
            {
                m_Controller.LookRotation = m_Transform.rotation;
            }
            else{
                direction.y = m_Transform.position.y;
                if (direction != Vector3.zero)
                    m_Controller.LookRotation = Quaternion.LookRotation(direction, m_Transform.up);
                else
                    m_Controller.LookRotation = m_Transform.rotation;
            }



		}


		private void Update()
        {
            //  -- Character Actions
            //Aim(KeyCode.Mouse1);

            //  --

            CameraInput();

            DebugButtonPress();
        }

		private void LateUpdate()
		{
            //CameraInput();
            LockCameraRotation();
		}











        private void CameraInput()
        {
            if (m_CameraController == null) return;

            m_MouseHorizontal = Input.GetAxis(m_RotateCameraXInput);
            m_MouseVertical = Input.GetAxis(m_RotateCameraYInput);

            m_CameraController.RotateCamera(m_MouseHorizontal, m_MouseVertical);
            m_CameraController.ZoomCamera(Input.GetAxisRaw(m_MouseScrollInput));


        }



        private void LockCameraRotation()
        {
            if (Input.GetKeyDown(KeyCode.L)){
                if (CameraController.Instance != null){
                    CameraController.LockRotation = !CameraController.LockRotation;
                }
            }
        }







        private void OnAimActionStart(bool aim)
        {
            if (aim == true)
            {
                CameraState aimState = CameraController.Instance.GetCameraStateWithName("TPS_Aim");
                CameraController.Instance.ChangeCameraState("TPS_Aim");
            }
            else
            {
                CameraController.Instance.ChangeCameraState("TPS_Default");
            }
        }





        private void DebugButtonPress()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                //EventHandler.LogAllRegistered();
                UnityEditor.Selection.activeGameObject = gameObject;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPlaying;
                Debug.Break();
            }
        }





        protected void OnDrawGizmos()
        {
            //if (m_DrawDebugLine)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, m_Velocity.normalized );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.forward );
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.right );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            //}
            //if(Application.isPlaying){
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawSphere(m_LookDirection, 0.1f);
            //    Gizmos.DrawLine(transform.position + (transform.up * 1.35f), m_LookDirection);
            //}
            if (Application.isPlaying)
            {

            }
        }





        private void OnGUI()
        {
            //if (displayCursorPosition)
            //{
            //    var cam = Camera.main;
            //    if (cam == null)
            //        return;

            //    var p = cam.WorldToScreenPoint(cursorPosition);
            //    p.y = Screen.height - p.y;

            //    GUI.color = cursorPositionDisplayColor;

            //    var content = new GUIContent(string.Format("({0})", cursorPosition));
            //    var size = new GUIStyle(GUI.skin.label).CalcSize(content);
            //    GUI.Label(new Rect(p.x, p.y, size.x, size.y), content);
            //}

            //if (displayPlayerInputValues)
            //{
            //    GUILayout.BeginArea(new Rect(5f, Screen.height * 0.3f, Screen.width * 0.2f, Screen.height * 0.2f), GUI.skin.box);

            //    var playerInputContent = new GUIContent(string.Format("PlayerInput X: {0}\nPlayerInput Y: {1}", playerInput.x, playerInput.y));
            //    GUILayout.Label(playerInputContent);

            //    var dotProduct = new GUIContent(string.Format("fwdDotProduct: {0}\nrightDotProduct: {1}", fwdDotProduct, rightDotProduct));
            //    GUILayout.Label(dotProduct);

            //    var dotProductMsg = new GUIContent(dotProductInfo);
            //    GUILayout.Label(dotProductMsg);

            //    GUILayout.EndArea();
            //}
        }
	}
}


