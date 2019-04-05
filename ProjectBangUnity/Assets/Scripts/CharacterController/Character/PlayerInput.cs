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

        private bool m_UseAxisRaw = false;

        private KeyCode m_RunInput = KeyCode.LeftShift;
        private KeyCode m_CrouchInput = KeyCode.C;
        private KeyCode m_JumpInput = KeyCode.Space;
        private KeyCode m_SwitchItemBack = KeyCode.Q;
        private KeyCode m_SwitchItemFwd = KeyCode.E;
        private KeyCode m_ReloadInput = KeyCode.R;
        private KeyCode m_InteractInput = KeyCode.F;
        //private KeyCode m_ThrowInput = KeyCode.G;
        private KeyCode m_CoverInput = KeyCode.V;
        private KeyCode m_UseItemInput = KeyCode.Mouse0;
        private KeyCode m_AimInput = KeyCode.Mouse1;

        private float m_InputDelay = 0.1f;
        private float m_ActionInputDelay = 0.5f;
        private float m_ActionInputTimer;

        private KeyCode m_FirstButtonPressed;
        private float m_TimeOfFirstButtoonPressed;
        private float m_DoubleTapInputTime = 0.25f;
        //[SerializeField, DisplayOnly]
        private bool m_IsCrouching, m_IsRunning, m_IsAiming, m_InCover;
        [Header("-- Debug Settings --")]
        //[SerializeField, DisplayOnly]
        private float m_Horizontal, m_Forward;
        //[SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        [SerializeField, DisplayOnly]
        private float m_MouseHorizontal, m_MouseVertical;
        private Vector3 m_MouseInputVector;

        private float m_RayLookDistance = 20f;
        [SerializeField]
        private LayerMask m_LayerMask;


        private Ray m_Ray;


        private Vector3 m_ScreenCenter = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private ItemActionManager m_ItemAction;
        //private LayerManager m_LayerManager;
        //private Inventory m_Inventory;
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
            //m_LayerManager = GetComponent<LayerManager>();
            //m_Inventory = GetComponent<Inventory>();
            m_ItemAction = GetComponent<ItemActionManager>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;
            m_LayerMask = ~(1 << gameObject.layer);


        }


		private void Start()
		{
            if (CameraController.Instance == null && m_CameraController)
            {
                m_CameraController = Instantiate(m_CameraController) as CameraController;
                m_CameraController.SetMainTarget(m_GameObject);
                m_Camera = m_CameraController.Camera.transform;
            }
            else if(m_CameraController == null && CameraController.Instance != null){
                m_CameraController = CameraController.Instance;
                m_CameraController.SetMainTarget(m_GameObject);
                m_Camera = m_CameraController.Camera.transform;
            }
            else{
                Debug.LogError("Player has no Camera");
            }

            m_Ray = new Ray(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward * 10);
		}


		public virtual float GetAxis(string name, bool useRaw = false){
            return useRaw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }





        [SerializeField]
        float angleY;
		private void FixedUpdate()
		{
            m_Horizontal = GetAxis(m_HorizontalInputName, false);
            m_Forward = GetAxis(m_VerticalInputName, false);

            m_MouseHorizontal = Input.GetAxis(m_RotateCameraXInput);
            m_MouseVertical = Input.GetAxis(m_RotateCameraYInput);


            if(m_CameraController != null){
                m_InputVector = m_Horizontal * m_Camera.right + m_Forward * m_Camera.forward;
                //m_InputVector.Set(m_Horizontal, 0, m_Forward);
                m_Controller.InputVector = m_InputVector;
            } else {
                m_InputVector = m_Horizontal * Vector3.right + m_Forward * Vector3.forward;
                m_Controller.InputVector = m_InputVector;
            }



            //m_Ray.origin = m_CameraController.Camera.transform.position;
            //m_Ray.direction = m_CameraController.Camera.transform.forward;
            //m_Controller.LookDirection = m_Ray.GetPoint(m_RayLookDistance) - m_CameraController.Camera.transform.position;
            //m_Controller.LookAtPoint = m_Ray.GetPoint(m_RayLookDistance);

            m_MouseInputVector.Set(m_MouseHorizontal, m_MouseVertical, m_CameraController.Camera.nearClipPlane);
            var viewport = m_CameraController.Camera.ViewportToWorldPoint(m_MouseInputVector);

            //var direction = m_Transform.position - m_CameraController.Camera.transform.position;
            var direction = m_Transform.position - viewport;
            direction.y = 0;
            direction.Normalize();
            //direction = direction * Mathf.Abs(m_CameraController.Camera.transform.localPosition.z);
            //direction = direction * 5;


            //m_Controller.LookDirection = (m_Transform.position + direction * Mathf.Abs(m_CameraController.Camera.transform.localPosition.z)) - m_CameraController.Camera.transform.position;
            //m_Controller.LookDirection = m_Controller.LookDirection + (Vector3.up * 1.5f);

            m_Controller.LookAtPoint = m_CameraController.Camera.transform.position + m_CameraController.Camera.transform.forward * 8;
            //m_Controller.LookAtPoint = m_Transform.position + (Vector3.up * 1.5f) + m_Controller.LookDirection;

            var lookDirection = m_Controller.LookAtPoint - m_Transform.position + (m_CameraController.Camera.transform.right * .5f);
            //lookDirection = lookDirection - m_Transform.right * 0.5f;
            lookDirection.y = 0;
            //lookDirection.Normalize();
            lookDirection = lookDirection.normalized * 5f;
            m_Controller.LookDirection = lookDirection ;

            //Debug.DrawRay(m_CameraController.Camera.transform.position, direction, Color.green);
            Debug.DrawRay(m_Transform.position  + (m_CameraController.Camera.transform.right * .5f), lookDirection, Color.yellow);
            Debug.DrawRay(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward * 8, Color.blue);


            if(m_Controller.IndependentLook()){

            }
            else{
                direction.y = m_Transform.position.y;
                if (direction != Vector3.zero)
                    m_Controller.LookRotation = Quaternion.LookRotation(direction, Vector3.up);
                else
                    m_Controller.LookRotation = m_Transform.rotation;


            }

            //m_Controller.LookDirection = m_CameraController.Camera.transform.forward * 10;
		}


		private void Update()
        {
            //  Set input vectors.
            //SetInputVector(m_UseAxisRaw);
            SetTargetLookAt();
            //  Aim
            Aim(m_AimInput);
            //  Use current item
            UseItem(m_UseItemInput);
            //  Reload
            Reload(m_ReloadInput);


            CameraInput();

            DebugButtonPress();
        }

		private void LateUpdate()
		{
            //CameraInput();
            LockCameraRotation();
		}





		private void SetInputVector(bool useAxisRaw)
        {
            m_Horizontal = GetAxis(m_HorizontalInputName, useAxisRaw);
            m_Forward = GetAxis(m_VerticalInputName, useAxisRaw);

            m_InputVector.Set(m_Horizontal, 0, m_Forward);
            m_Controller.InputVector = m_InputVector;

            m_MouseHorizontal = Input.GetAxis(m_RotateCameraXInput);
            m_MouseVertical = Input.GetAxis(m_RotateCameraYInput);
            var difference = m_CameraController.Camera.transform.position.y - m_Transform.position.y;
            m_MouseInputVector.Set(m_MouseHorizontal, m_MouseVertical, difference);
            //var viewPortToScreen = m_CameraController.Camera.ViewportToScreenPoint(m_MouseInputVector);
            //m_Controller.RelativeInputVector = m_CameraController.Camera.ScreenToWorldPoint(m_MouseInputVector);

            m_Controller.RelativeInputVector = m_MouseInputVector;
            //  Set Look Rotation
            //m_Controller.LookRotation = Quaternion.Euler(m_Transform.eulerAngles.x, m_CameraController.transform.eulerAngles.y, m_Transform.eulerAngles.z);

        }



        private void SetTargetLookAt()
        {
            ////  Set Look Direction
            //var direction = m_CameraController.Camera.transform.forward - m_CameraController.Camera.transform.position;
            ////var lookDirection = Vector3.
            //m_Controller.LookDirection = direction * 5;

            //m_Ray.origin = m_CameraController.Camera.transform.position;
            //m_Ray.direction = m_CameraController.Camera.transform.forward;
            //m_Controller.LookDirection = m_Ray.GetPoint(m_RayLookDistance) - m_CameraController.Camera.transform.position;
            //m_Controller.LookAtPoint = m_Ray.GetPoint(m_RayLookDistance);
        }



        #region Camera methods

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

        #endregion



        #region Input actions

        private void Aim(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode)){
                var action = m_Controller.GetAction<Aim>();
                if (action.IsActive == false) {
                    if(m_Controller.TryStartAction(action)){
                        CameraState aimState = CameraController.Instance.GetCameraStateWithName("TPS_Aim");
                        CameraController.Instance.ChangeCameraState("TPS_Aim");
                    }

                } else {
                    m_Controller.TryStopAction(action);
                    CameraController.Instance.ChangeCameraState("TPS_Default");
                }
            }
        }


        public void UseItem(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode)){
                RaycastHit hit;
                if(Physics.Raycast(m_Ray, out hit, m_RayLookDistance)){
                    m_Controller.LookAtPoint = m_Ray.GetPoint(hit.distance);
                }
                m_ItemAction.UseItem();
            }
        }


        public void Reload(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode)){
                m_ItemAction.Reload();
            }
        }




        //public void DropItem(int itemID)
        //{

        //}

        #endregion




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
                //Gizmos.color = Color.green;
                //Gizmos.DrawSphere(m_Ray.GetPoint(m_RayLookDistance), 0.1f);
                //Gizmos.DrawRay(m_Ray.origin, m_Ray.direction * m_RayLookDistance);
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


