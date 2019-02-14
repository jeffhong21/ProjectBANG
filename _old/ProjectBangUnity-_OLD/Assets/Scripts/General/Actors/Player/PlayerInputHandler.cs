namespace Bang
{
    using UnityEngine;
    using System;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private RigidbodyCharacterController rigidbodyCharCtrl;
        private PlayerController playerCtrl;
        private Camera playerCamera;


        [SerializeField]
        private float horizontal;
        [SerializeField]
        private float vertical;
        //[SerializeField]
        //private float moveAmount;
        //[SerializeField]
        //private Vector3 moveDirection;

        //[SerializeField]
        //private bool useRawInput = true;
        //[SerializeField]
        //private Vector3 playerInput;
        [SerializeField]
        private bool isAiming = true;

        Vector3 lookRotation;
        Vector3 cursorPosition;
        Ray ray;
        RaycastHit hit;
        Plane groundPlane;
        float rayDistance;

        //Vector3 direction = Vector3.zero;

        public Vector3 CursorPosition{
            get { return cursorPosition; }
        }


        private void Awake()
        {
            rigidbodyCharCtrl = GetComponent<RigidbodyCharacterController>();
            playerCtrl = GetComponent<PlayerController>();
            playerCamera = Camera.main;

            groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
        }





        private void Update()
        {
            //if (useRawInput){
            //    horizontal = Input.GetAxisRaw("Horizontal");
            //    vertical = Input.GetAxisRaw("Vertical");
            //    playerInput.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            //}
            //else{
            //    horizontal = Input.GetAxis("Horizontal");
            //    vertical = Input.GetAxis("Vertical");
            //    playerInput.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //}


            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");


            if (InputManager.Space)
            {
                isAiming = !isAiming;
                Debug.Log(isAiming);
                playerCtrl.AnimHandler.SetAim(isAiming);

            }

            else if (InputManager.LMB){
                playerCtrl.ShootWeapon(cursorPosition);
            }

            else if (InputManager.RMB){

            }

            else if (InputManager.R)
            {
                playerCtrl.Reload();
                Debug.LogFormat("<color=#800080ff>{0}</color>.  Current ammo is <color=#800080ff>{1}</color>", "Reloading playerCtrl.weapon", playerCtrl.weapon.CurrentAmmo);  // purple
            }

            else if (InputManager.Q)
            {

            }

            else if (InputManager.E){

            }

        }


        private void FixedUpdate()
        {
            //currentSpeed = playerCtrl.stats.walkSpeed;

            //UpdatePosition(currentSpeed);
            ////GetMovedirection();

            rigidbodyCharCtrl.Move(horizontal, vertical);
            UpdateOrientation();

        }



        private void UpdatePosition(float speed)
        {
            //Debug.DrawRay(transform.position, moveDirection, Color.red, 0.25f);
            //transform.position += playerInput.normalized * speed * Time.deltaTime;
        }



        private void UpdateOrientation()
        {
            ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            //if(Physics.Raycast(ray, out hit))
            //{
                
            //}

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                cursorPosition = ray.GetPoint(rayDistance);
                lookRotation = cursorPosition - transform.position;
                if (lookRotation != Vector3.zero)
                {
                    // Create a quaternion (rotation) based on looking down the vector from the player to the target.
                    Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * playerCtrl.stats.turnSpeed);
                }

                playerCtrl.AimPosition = cursorPosition;
            }
        }


    }
}


