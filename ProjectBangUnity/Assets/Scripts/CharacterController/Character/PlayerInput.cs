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


        [SerializeField, DisplayOnly]
        private float m_MoveAmount;
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
        private LayerMask m_LayerMask;
        [Header("-- Debug Settings --")]
        [SerializeField]
        private bool m_DisplayKeyMapping;

        private Ray m_Ray;
        private RaycastHit m_RaycastHit;


        [SerializeField]
        private CameraController m_CameraController;
        private CharacterLocomotion m_Controller;
        private ItemAction m_ItemAction;
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
            m_ItemAction = GetComponent<ItemAction>();

            if (m_CameraController)
            {
                m_CameraController = Instantiate(m_CameraController) as CameraController;
                m_CameraController.m_Target = gameObject;
            }

            m_LayerMask = ~(1 << gameObject.layer);
        }


        public virtual float GetAxis(string name){
            return Input.GetAxis(name);
        }


        public virtual float GetAxisRaw(string name){
            return Input.GetAxisRaw(name);
        }


        public virtual Vector2 GetMousePosition(){
            return Input.mousePosition;
        }



        private void Update()
        {
            m_Horizontal = GetAxis(m_HorizontalInputName);
            m_Vertical = GetAxis(m_VerticalInputName);

            m_MoveAmount = Mathf.Clamp01(Mathf.Abs(m_Horizontal) + Mathf.Abs(m_Vertical));

            m_InputVector.Set(m_Horizontal, 0, m_Vertical);
            m_Controller.InputVector = m_InputVector;

            //  Find where the camera is looking.
            if(m_Controller.Aiming)
            {
                m_Ray = new Ray(m_CameraController.Camera.transform.position, m_CameraController.Camera.transform.forward);
                m_Controller.LookPosition = m_Ray.GetPoint(m_RayLookDistance);

                //Debug.DrawRay(m_Ray.origin, m_Ray.direction * 20, Color.red);
                if(Physics.Raycast(m_Ray.origin, m_Ray.direction, out m_RaycastHit, 50, m_LayerMask)){
                    m_Controller.LookPosition = m_RaycastHit.point;
                }
                else{
                    m_Controller.LookPosition = m_Controller.LookPosition;
                }
            }
            else{
                m_Controller.LookPosition = m_Controller.transform.position + (m_Controller.transform.forward * 10) + (m_Controller.transform.up * 1.35f);
            }

            m_MousePosition = Input.mousePosition;


            if(Input.GetButtonDown("mouse0")){

            }
            else if(Input.GetButtonDown("mouse2")){

            }
            else if(Input.GetButtonDown("space")){
                
            }
            else if(Input.GetButtonDown("leftshift")){
                
            }



            LockCameraRotation();

            //  For Debugging.
            DebugButtonPress();
        }


        private void LockCameraRotation()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if(CameraController.Instance != null){
                    CameraController.LockRotation = !CameraController.LockRotation;
                    if (CameraController.LockRotation)
                        Debug.LogFormat(" -- Locking Camera Rotation -- ");
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


