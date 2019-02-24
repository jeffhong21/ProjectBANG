namespace CharacterController
{
    using UnityEngine;
    using JH_Utils;


    [RequireComponent(typeof(CharacterLocomotion))]
    public class PlayerInput : MonoBehaviour
    {
        private string m_HorizontalInputName = "Horizontal";
        private string m_VerticalInputName = "Vertical";
        private string m_LeftMouseInputName = "Fire1";
        private string m_MiddleMouseInputName = "Fire2";
        private string m_RightMouseInputName = "Fire3";
        private string m_ReloadInputName = "Reload";

        [SerializeField]
        private KeyCode m_RunInput = KeyCode.LeftShift;
        [SerializeField]
        private LayerMask m_LayerMask;

        [Header("-- Debug Settings --")]
        [SerializeField, DisplayOnly]
        private float m_Horizontal;
        [SerializeField, DisplayOnly]
        private float m_Vertical;
        [SerializeField, DisplayOnly]
        private Vector3 m_AimPosition;
        [SerializeField, DisplayOnly]
        private Vector2 m_MousePosition;
        private Vector3 m_InputVector;
        private float m_RayLookDistance = 20f;


        [SerializeField]
        private bool m_DisplayKeyMapping;

        private Ray m_Ray;
        private RaycastHit m_RaycastHit;


        [SerializeField]
        private CameraController m_CameraController;
        private CharacterLocomotion m_Controller;
        private ItemActionManager m_ItemAction;
        private LayerManager m_LayerManager;
        private Inventory m_Inventory;



        public string HorizontalInputName{
            get { return m_HorizontalInputName; }
        }

        public string VerticalInputName{
            get { return m_VerticalInputName; }
        }





        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_LayerManager = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();
            m_ItemAction = GetComponent<ItemActionManager>();



            m_LayerMask = ~(1 << gameObject.layer);
        }


		private void Start()
		{
            if(CameraController.Instance == null && m_CameraController){
                m_CameraController = Instantiate(m_CameraController) as CameraController;
                m_CameraController.SetMainTarget(transform);
            }
		}


		public virtual float GetAxis(string name){
            return Input.GetAxis(name);
        }


        public virtual float GetAxisRaw(string name){
            return Input.GetAxisRaw(name);
        }


        public virtual Vector2 GetMousePosition(){
            m_MousePosition = Input.mousePosition;
            return Input.mousePosition;
        }



        private void Update()
        {
            SetInputVector(true);



            SetRunInput(m_RunInput);

            UseItem(KeyCode.Mouse0);

            Reload(KeyCode.R);
           
            SwitchItem(KeyCode.Q, true);

            SwitchItem(KeyCode.E, false);

            //else if (Input.GetKeyDown(KeyCode.F))
            //{
                
            //}
            //else if (Input.GetKeyDown(KeyCode.C))
            //{
                
            //}
            //else if (Input.GetKeyDown(KeyCode.V))
            //{

            //}
            //else if (Input.GetKeyDown(KeyCode.B))
            //{

            //}






            //  For Debugging.
            DebugButtonPress();
        }

		private void LateUpdate()
		{
            SetCameraPosition();

            LockCameraRotation();
		}


		private void SetInputVector(bool useGetAxis)
        {
            m_Horizontal = GetAxis(m_HorizontalInputName);
            m_Vertical = GetAxis(m_VerticalInputName);

            m_InputVector.Set(m_Horizontal, 0, m_Vertical);
            m_Controller.InputVector = m_InputVector;
        }


        private void SetRunInput(KeyCode input)
        {
            if (Input.GetKeyDown(input)){
                m_Controller.Running = true;
            }
            else if (Input.GetKeyUp(input)){
                m_Controller.Running = false;
            }
        }


        private void SetCameraPosition()
        {
            //  Find where the camera is looking.
            if (m_Controller.Aiming){
                m_Ray = new Ray(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward);
                m_Controller.LookPosition = m_Ray.GetPoint(m_RayLookDistance);

                //Debug.DrawRay(m_Ray.origin, m_Ray.direction * 20, Color.red);
                if (Physics.Raycast(m_Ray.origin, m_Ray.direction, out m_RaycastHit, 50, m_LayerMask)){
                    m_Controller.LookPosition = m_RaycastHit.point;
                }
                else{
                    m_Controller.LookPosition = m_Controller.LookPosition;
                }
            }
            else{
                m_Controller.LookPosition = m_Controller.transform.position + (m_Controller.transform.forward * 10) + (m_Controller.transform.up * 1.35f);
            }
        }



        public void UseItem(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode))
            {
                m_ItemAction.UseItem();
            }

        }


        public void Reload(KeyCode keycode)
        {
            if (Input.GetKeyDown(keycode))
            {
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

        //public void ToggleItem()
        //{

        //}


        //public void DropItem(int itemID)
        //{

        //}




        private void LockCameraRotation()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if(CameraController.Instance != null){
                    CameraController.LockRotation = !CameraController.LockRotation;
                    //if (CameraController.LockRotation)
                        //Debug.LogFormat(" -- Locking Camera Rotation -- ");
                }
            }
        }


        private void DebugButtonPress()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                EventHandler.LogAllRegistered();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPlaying;
                Debug.Break();
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


