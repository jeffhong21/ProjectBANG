namespace Bang
{
    using UnityEngine;
    using System;


    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField]
        private bool useRawInput = true;
        [SerializeField, Range(0.1f, 1)]
        private float sensitivity = 1f;

        public float aimHeight = 0.75f;
        [SerializeField]
        private Vector3 playerInput;

        private Vector3 cursorPosition;
        [SerializeField]
        private float delta;

        private float currentSpeed;

        private PlayerController playerCtrl;




        private void Awake()
        {
            playerCtrl = GetComponent<PlayerController>();
        }


        private void Update()
        {
            delta = Time.deltaTime;


            if (useRawInput){
                playerInput.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            }
            else{
                playerInput.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            }


            if (InputManager.Space){
                playerCtrl.EnterCover();
            }
            else if (InputManager.LMB){
                playerCtrl.ShootWeapon(cursorPosition);
                //Debug.Log("Shooting");
            }
            else if (InputManager.RMB){

            }
            else if (InputManager.R)
            {
                Debug.LogFormat("<color=#800080ff>{0}</color>.  Current ammo is <color=#800080ff>{1}</color>", "Reloading playerCtrl.weapon", playerCtrl.weapon.currentAmmo);  // purple
            }

            //  Check if player is moving.
            //isMoving = Math.Abs(playerInput.x) >= 0.1f || Math.Abs(playerInput.y) >= 0.1f ? true : false;

        }


        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            currentSpeed = playerCtrl.stats.walkSpeed;

            UpdatePosition(currentSpeed);
            UpdateOrientation();
        }


        private void UpdatePosition(float speed)
        {
            this.transform.position += playerInput.normalized * speed * delta;
        }


        private void UpdateOrientation()
        {
            Ray ray = playerCtrl.playerCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                cursorPosition = ray.GetPoint(rayDistance);


                transform.LookAt(cursorPosition);

                if (playerCtrl.weapon != null)
                {
                    Vector3 aimDirection = cursorPosition;
                    aimDirection.y = playerCtrl.weapon.transform.position.y;
                    playerCtrl.weapon.transform.LookAt(aimDirection);
                }

                cursorPosition.y = aimHeight;
                playerCtrl.crosshairs.transform.position = cursorPosition;
            }
        }


    }
}

