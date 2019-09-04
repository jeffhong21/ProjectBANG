//namespace CharacterController
//{
//    using UnityEngine;
//    using Bang;


//    [RequireComponent(typeof(CharacterLocomotion))]
//    public class PlayerInput1 : MonoBehaviour
//    {
//        public enum CameraType { ThirdPerson, TopDown };

//        private string m_HorizontalInputName = "Horizontal";
//        private string m_VerticalInputName = "Vertical";

//        [SerializeField, ReadOnly]
//        private float m_MoveAmount;

//        private float m_Horizontal;
//        private float m_Vertical;
//        private Quaternion m_LookRotation;

//        [SerializeField]
//        private CameraType m_CameraType;
//        [SerializeField]
//        private Bang.CameraController m_CameraCtrl;
//        [SerializeField]
//        private CameraController m_CameraController;
//        private Camera m_Camera;


//        private CharacterLocomotion m_Controller;
//        private Inventory m_Inventory;



//        public string HorizontalInputName{
//            get { return m_HorizontalInputName; }
//        }

//        public string VerticalInputName{
//            get { return m_VerticalInputName; }
//        }



//        private void Awake()
//        {
//            m_Controller = GetComponent<CharacterLocomotion>();
//            m_Inventory = GetComponent<Inventory>();


//            switch(m_CameraType)
//            {
//                case CameraType.ThirdPerson:
//                    if (m_CameraController)
//                    {
//                        m_CameraController = Instantiate(m_CameraController) as CameraController;
//                        m_CameraController.m_Target = gameObject;
//                        m_Camera = Camera.main;
//                    }
//                    break;
//                case CameraType.TopDown:
//                    if (m_CameraCtrl)
//                    {
//                        m_CameraCtrl = Instantiate(m_CameraCtrl) as Bang.CameraController;
//                        m_CameraCtrl.SetTarget(transform);
//                        m_Camera = Camera.main;
//                    }
//                    break;
//            }





//        }


//        public virtual float GetAxis(string name){
//            return Input.GetAxis(name);
//        }

//        public virtual float GetAxisRaw(string name){
//            return Input.GetAxisRaw(name);
//        }

//        public virtual Vector2 GetMousePosition(){
//            return Input.mousePosition;
//        }



//        private void Update()
//        {

//            //m_Horizontal = Input.GetAxisRaw("Horizontal");
//            //m_Vertical = Input.GetAxisRaw("Vertical");

//            m_Horizontal = Input.GetAxis(m_HorizontalInputName);
//            m_Vertical = Input.GetAxis(m_VerticalInputName);
//            m_MoveAmount = Mathf.Clamp01(Mathf.Abs(m_Horizontal) + Mathf.Abs(m_Vertical));

//            if (InputManager.LMB)
//            {
//                //m_Inventory.UseItem();
//                //m_Controller.ShootWeapon();
//            }

//            else if (InputManager.RMB)
//            {

//            }

//            else if (InputManager.Space)
//            {

//            }

//            else if (InputManager.R)
//            {
//                //m_Inventory.ReloadItem();
//                //m_Controller.ReloadWeapon();
//            }

//            else if (InputManager.Q)
//            {

//            }

//            else if (InputManager.E)
//            {
//                EventHandler.LogAllRegistered();
//            }

//            //m_Controller.IsCrouching = InputManager.ShiftDown;


//            if (InputManager.ESC)
//            {
//                //Time.timeScale = Time.timeScale == 0 ? 1 : 0;
//                Debug.Break();
//            }


//            //CalculateDirection();
//        }



//        private void FixedUpdate()
//        {
//            if (m_Camera)
//            {
//                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
//                Plane groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
//                float rayDistance;

//                if (groundPlane.Raycast(ray, out rayDistance))
//                {
//                    Vector3 mousePosition = ray.GetPoint(rayDistance);
//                    Vector3 lookRotation = mousePosition - transform.position;
//                    if (lookRotation != Vector3.zero)
//                    {
//                        // Create a quaternion (rotation) based on looking down the vector from the player to the target.
//                        m_LookRotation = Quaternion.LookRotation(lookRotation);
//                    }
//                    else{
//                        m_LookRotation = Quaternion.identity;
//                    }
//                }
//            }

 

//            //m_Controller.Move(m_Horizontal, m_Vertical, m_LookRotation);
//        }





//        //private void GetOrientation(float inputX, float inputY)
//        //{
//        //    Vector2 inputDir = new Vector2(inputX, inputY).normalized;

//        //    if(inputDir != Vector2.zero){
//        //        float targetRotation = Mathf.Atan2(inputX, inputY) * Mathf.Rad2Deg;

//        //    }

//        //}

//        //float angle;
//        //Quaternion targetRotation;
//        //float turnSpeed = 20;
//        //void CalculateDirection()
//        //{
//        //    angle = Mathf.Atan2(m_Horizontal, m_Vertical);
//        //    angle = Mathf.Rad2Deg * angle;
//        //    angle += m_ThirdPersonCam.cam.transform.eulerAngles.y;
//        //    targetRotation = Quaternion.Euler(0, angle, 0);
//        //    m_LookRotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

//        //    m_Controller.Move(m_Horizontal, m_Vertical, m_LookRotation, true);
//        //}



//    }
//}


