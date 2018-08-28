namespace Bang
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;


    public class PlayerController : ActorController
    {
        public Action<int> ammoChange;
        public Action<float> healthChange;

        //
        //  Fields
        //
        public PlayerCrosshairs crosshairs;


        protected PlayerInputHandler input;
        protected Vector3 cursorPosition;
        protected Vector3 playerInput;
        protected Vector3 playerVelocity;
        protected Vector3 previousPosition;
        [SerializeField, ReadOnly]
        protected float fwdDotProduct;
        [SerializeField, ReadOnly]
        protected float rightDotProduct;


        //
        //  Properties
        //
        public Vector3 CursorPosition
        {
            get { return cursorPosition; }
        }


        //
        //  Methods
        //
        protected override void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
            animHandler = GetComponent<AnimationHandler>();
            inventory = GetComponent<InventoryHandler>();

            if (playerCamera == null)
            {
                gm.PlayerCamera.target = this.transform;
                playerCamera = gm.PlayerCamera.GetComponent<Camera>();
            }

            crosshairs = Instantiate(crosshairs, this.transform.position, crosshairs.transform.rotation, this.transform);
        }


        protected override void OnEnable()
		{
            base.OnEnable();
		}


        protected override void OnDisable()
		{
            base.OnDisable();
		}


        protected virtual void FixedUpdate()
		{
            //CalculateMovement();

            UpdatePosition();
            UpdateOrientation();
		}


        public override void EquipWeapon(FirearmBase weapon, Transform location)
        {
            this.weapon = Instantiate(weapon, location.position, location.rotation, location) as FirearmBase;
        }


        public override void FireWeapon(Vector3 target)
        {
            weapon.Shoot(target);
        }


        public override void Reload()
        {
            // TODO    
        }


        protected void CalculateMovement()
        {
            playerVelocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            playerVelocity.y = 0;
            playerVelocity = playerVelocity.normalized;
            fwdDotProduct = Vector3.Dot(transform.forward, playerVelocity);
            rightDotProduct = Vector3.Dot(transform.right, playerVelocity);

            isMoving = Math.Abs(playerInput.x) >= 0.1f || Math.Abs(playerInput.y) >= 0.1f;
        }


        protected void UpdatePosition()
        {
            playerInput.Set(input.PlayerInput.x, 0, input.PlayerInput.y);
            transform.position += playerInput.normalized * moveSpeed * Time.deltaTime;

            //animHandler.Idle(isMoving);
            //animHandler.Locomotion(fwdDotProduct, rightDotProduct);
        }


        protected void UpdateOrientation()
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                cursorPosition = ray.GetPoint(rayDistance);


                transform.LookAt(cursorPosition);

                if (weapon != null)
                {
                    Vector3 aimDirection = cursorPosition;
                    aimDirection.y = weapon.transform.position.y;
                    weapon.transform.LookAt(aimDirection);
                }

                cursorPosition.y = aimHeight;
                crosshairs.transform.position = cursorPosition;
            }
        }



        public override void Death()
        {
            animHandler.Death();
            DisableControls();
        }


        public override void EnableControls()
        {
            input.enabled = true;
        }

        public override void DisableControls()
        {
            input.enabled = false;
        }

    }
}


