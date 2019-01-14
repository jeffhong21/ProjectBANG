namespace CharacterController
{
    using UnityEngine;
    using Bang;


    [RequireComponent(typeof(CharacterLocomotion))]
    public class PlayerInput : MonoBehaviour
    {
        
        private CharacterLocomotion m_Controller;
        [SerializeField]
        private float m_Horizontal;
        [SerializeField]
        private float m_Vertical;
        private Quaternion m_LookRotation;

        [SerializeField]
        private CameraController m_CameraCtrl;
        private Camera m_Camera;



        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();

            if(m_CameraCtrl){
                m_CameraCtrl = Instantiate(m_CameraCtrl) as CameraController;
                m_CameraCtrl.SetTarget(transform);
                m_Camera = Camera.main;
            }
            else{
                Debug.LogError("No camera controller created.");
            }
        }


        private void Update()
        {

            m_Horizontal = Input.GetAxis("Horizontal");
            m_Vertical = Input.GetAxis("Vertical");




            if (InputManager.LMB)
            {
                m_Controller.ShootWeapon();
            }

            else if (InputManager.RMB)
            {

            }

            else if (InputManager.Space)
            {
                m_Controller.Jump();
            }

            else if (InputManager.R)
            {
                m_Controller.ReloadWeapon();
            }

            else if (InputManager.Q)
            {
                m_Controller.TryStartAction(m_Controller.CharActions[(int)CharacterActionType.Generic]);
            }

            else if (InputManager.E)
            {

            }

            m_Controller.IsCrouching = InputManager.ShiftDown;


            if (InputManager.ESC)
            {
                Debug.Break();
            }
        }



        private void FixedUpdate()
        {
            if (m_Camera)
            {
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
                float rayDistance;

                if (groundPlane.Raycast(ray, out rayDistance))
                {
                    Vector3 mousePosition = ray.GetPoint(rayDistance);
                    Vector3 lookRotation = mousePosition - transform.position;
                    if (lookRotation != Vector3.zero)
                    {
                        // Create a quaternion (rotation) based on looking down the vector from the player to the target.
                        m_LookRotation = Quaternion.LookRotation(lookRotation);
                    }
                }
            }

 

            m_Controller.Move(m_Horizontal, m_Vertical, m_LookRotation);
        }







    }
}


