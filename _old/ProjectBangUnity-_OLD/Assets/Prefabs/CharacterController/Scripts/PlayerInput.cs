namespace CharacterController
{
    using UnityEngine;
    using Bang;


    [RequireComponent(typeof(CharacterLocomotion))]
    public class PlayerInput : MonoBehaviour
    {
        private string m_HorizontalInputName = "Horizontal";
        private string m_VerticalInputName = "Vertical";
        private string m_LeftMouseInputName = "Fire1";
        private string m_MiddleMouseInputName = "Fire2";
        private string m_RightMouseInputName = "Fire3";
        private string m_ReloadInputName = "Reload";


        [SerializeField, ReadOnly]
        private float m_MoveAmount;
        [SerializeField, ReadOnly]
        private float m_Horizontal;
        [SerializeField, ReadOnly]
        private float m_Vertical;
        [SerializeField, ReadOnly]
        private Vector3 m_AimPosition;
        [SerializeField, ReadOnly]
        private Vector2 m_MousePosition;
        private Vector3 m_InputVector;
        private float m_RayLookDistance = 20f;
        [SerializeField]
        private LayerMask m_LayerMask;


        private Ray m_Ray;
        private RaycastHit m_RaycastHit;


        [SerializeField]
        private CameraController m_CameraController;
        private CharacterLocomotion m_Controller;
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







            //  For Debugging.
            DebugButtonPress();
        }


        private void DebugButtonPress()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                EventHandler.LogAllRegistered();
            }

            if (InputManager.ESC)
            {
                Debug.Break();
            }
        }


    }
}


