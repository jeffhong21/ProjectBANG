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
        private float m_Horizontal;
        //[SerializeField, DisplayOnly]
        private float m_Vertical;
        //[SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        private float m_RayLookDistance = 20f;
        [SerializeField]
        private LayerMask m_LayerMask;


        private Ray m_Ray;


        private Vector3 m_ScreenCenter = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private CameraController m_CameraController;
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
            }
            else if(m_CameraController == null && CameraController.Instance != null){
                m_CameraController = CameraController.Instance;
                m_CameraController.SetMainTarget(m_GameObject);
            }
            else{
                Debug.LogError("Player has no Camera");
            }

            m_Ray = new Ray(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward * 10);
		}


		public virtual float GetAxis(string name, bool useRaw = false){
            return useRaw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }



        private bool CheckDoubleTap(KeyCode key)
        {
            if(Input.GetKeyDown(key) && m_FirstButtonPressed == key){
                m_FirstButtonPressed = KeyCode.F12;
                if(Time.time - m_TimeOfFirstButtoonPressed < m_DoubleTapInputTime){
                    return true;
                }
            }
            if(Input.GetKeyDown(key) && m_FirstButtonPressed != key){
                m_FirstButtonPressed = key;
                m_TimeOfFirstButtoonPressed = Time.time;
                return false;
            }

            return false;
        }




		private void FixedUpdate()
		{
            CameraInput();
            //SetCameraPosition();
		}


		private void Update()
        {
            CameraInput();

            if(m_Controller)
            {
                m_ActionInputTimer += m_DeltaTime;

                //  Set input vectors.
                SetInputVector(m_UseAxisRaw);
                SetTargetLookAt();

                Aim(m_AimInput);
                //  Crouch
                Crouch(m_CrouchInput);
                ////  Cover
                //EnterCover(KeyCode.W);
                //  Roll
                ForwardAction(KeyCode.W);
                //  Dodge
                Dodge();
                // Run.
                Run(m_RunInput);


                //  Use current item
                UseItem(m_UseItemInput);
                //  Reload
                Reload(m_ReloadInput);
                //  Switch item back
                SwitchItem(m_SwitchItemBack, true);
                //  Switch item forward
                SwitchItem(m_SwitchItemFwd, false);
                //  Interact
                Interact(m_InteractInput);
            }

            //  For Debugging.
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
            m_Vertical = GetAxis(m_VerticalInputName, useAxisRaw);

            m_InputVector.Set(m_Horizontal, 0, m_Vertical);
            m_Controller.InputVector = m_InputVector;
        }


        private void SetTargetLookAt()
        {
            m_Ray.origin = m_CameraController.Camera.transform.position;
            m_Ray.direction = m_CameraController.Camera.transform.forward;
            m_Controller.LookAtPoint = m_Ray.GetPoint(m_RayLookDistance);
            //m_Controller.LookDirection = m_CameraController.Camera.transform.forward * 10 - m_CameraController.Camera.transform.position;
        }






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


        private void ForwardAction(KeyCode keycode)
        {
            if (m_ActionInputTimer < m_ActionInputDelay) return;

            if (CheckDoubleTap(keycode))
            {
                var coverAction = m_Controller.GetAction<Cover>();
                if (!coverAction.IsActive){
                    //  If can't enter cover, than roll forward.
                    if(m_Controller.TryStartAction(coverAction) == false){
                        var rollAction = m_Controller.GetAction<Roll>();
                        if (!rollAction.IsActive){
                            m_Controller.TryStartAction(rollAction);
                        }
                    }
                }
                else{
                    //  Stop Cover Action if it is active.
                    m_Controller.TryStopAction(coverAction);
                }

                m_ActionInputTimer = 0;
            }
        }





        private void Dodge()
        {
            if (m_ActionInputTimer < m_ActionInputDelay) return;

            bool executeAction = false;
            int actionIntData = 0;

            if (CheckDoubleTap(KeyCode.S))
            {
                actionIntData = 0;
                executeAction = true;
            }
            else if (CheckDoubleTap(KeyCode.A))
            {
                actionIntData = 1;
                executeAction = true;
            }
            else if (CheckDoubleTap(KeyCode.D))
            {
                actionIntData = 2;
                executeAction = true;
            }

            if(executeAction){
                var action = m_Controller.GetAction<Dodge>();
                if (action != null){
                    action.SetDodgeDirection(actionIntData);
                    m_Controller.TryStartAction(action);

                    m_ActionInputTimer = 0;
                }
            }
        }


        private void Crouch(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode)){
                var action = m_Controller.GetAction<Crouch>();
                if (!action.IsActive)
                {
                    m_IsCrouching = m_Controller.TryStartAction(action);
                }
                else{
                    m_Controller.TryStopAction(action);
                    m_IsCrouching = false;
                }
            }
        }


        private void Run(KeyCode keycode)
        {
            if (Input.GetKey(keycode)){
                m_Controller.SpeedChangeMultiplier = 1.5f;
            } else {
                m_Controller.SpeedChangeMultiplier = 1f;
            }
            //m_Controller.Running = Input.GetKey(keycode);
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


        public void SwitchItem(KeyCode keycode, bool next)
        {
            if (Input.GetKeyDown(keycode))
            {
                m_ItemAction.SwitchItem(next);
            }
        }


        public void EquipItem(KeyCode keycode, int index)
        {
            if (Input.GetKeyDown(keycode))
            {
                m_ItemAction.EquipItem(index);
            }

        }

        public void Interact(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode))
            {
               // m_Controller.PlayInteractAnimation();

                //var action = m_Controller.GetAction<MoveTowards>();
                //if(action != null){
                //    if (!action.IsActive){
                //        action.ActionStartLocation = m_Transform.forward;
                //        m_Controller.TryStartAction(action);
                //    }
                //}

            }
        }


        //public void DropItem(int itemID)
        //{

        //}

        #endregion



        # region Camera methods

        private void CameraInput()
        {
            if (m_CameraController == null) return;

            m_CameraController.RotateCamera(Input.GetAxis(m_RotateCameraXInput), Input.GetAxis(m_RotateCameraYInput));
            m_CameraController.ZoomCamera(Input.GetAxisRaw(m_MouseScrollInput));

            m_Controller.UpdateLookDirection(m_CameraController != null ? m_CameraController.transform : null);
            if (m_Controller.Aiming) 
                RotateWithAnotherTransform(m_CameraController.transform);
        }


        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            //Quaternion rot = Quaternion.LookRotation()
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            var targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), m_Controller.AimRotationSpeed * Time.fixedDeltaTime);
            m_Controller.SetRotation(Quaternion.Euler(newRotation));
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
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_Ray.GetPoint(m_RayLookDistance), 0.1f);
                Gizmos.DrawRay(m_Ray.origin, m_Ray.direction * m_RayLookDistance);
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


