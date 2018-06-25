namespace Bang
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;


    public class PlayerController : MonoBehaviour
    {
        public Action<int> ammoChange;
        public Action<float> healthChange;


        public float moveSpeed = 6f;
        public float rollSpeed = 10f;
        public float rollDistance = 10f;
        public float idleTimeout = 5f;
        public bool canAttack;
        public bool isMoving;

        public FirearmBase weapon;
        public FirearmBase defaultWeapon;
        public PlayerCrosshairs crosshairs;
        public Transform weaponHolder;
        public float aimHeight = 0.75f;



        protected Camera playerCamera;
        protected PlayerInput1 input;
        protected ActorHealth health;
        protected InventoryHandler inventory;
        protected AnimationHandler animHandler;
        protected Renderer[] renderers;                 // References used to make sure Renderers are reset properly. 
        protected float idleTimer;                      // Used to count up to player considering a random idle.


        protected Vector3 cursorPosition;
        protected Vector3 playerInput;
        protected Vector3 playerVelocity;
        protected Vector3 previousPosition;
        [SerializeField] protected float fwdDotProduct;
        [SerializeField] protected float rightDotProduct;

        public Vector3 CursorPosition{
            get { return cursorPosition; }
        }


        private void Awake()
        {
            input = GetComponent<PlayerInput1>();
            animHandler = GetComponent<AnimationHandler>();
            inventory = GetComponent<InventoryHandler>();

            if (playerCamera == null) playerCamera = Camera.main;

            crosshairs = Instantiate(crosshairs, this.transform.position, crosshairs.transform.rotation, this.transform);



        }


		private void OnEnable()
		{
            renderers = GetComponentsInChildren<Renderer>();

            EquipWeapon(defaultWeapon, weaponHolder);
		}


		private void OnDisable()
		{
            //for (int i = 0; i < renderers.Length; ++i)
            //{
            //    renderers[i].enabled = true;
            //}
		}


		private void FixedUpdate()
		{
            CalculateMovement();

            UpdatePosition();
            UpdateOrientation();
		}


		public virtual void EquipWeapon(FirearmBase weapon, Transform location)
        {
            this.weapon = Instantiate(weapon, location.position, location.rotation, location) as FirearmBase;
        }


        public virtual void FireWeapon(Vector3 target)
        {
            weapon.Shoot(target);
        }


        public virtual void Reload()
        {
            //canShoot = false;
            //equippedFirearm.Reload();
            //canShoot = true;
            //HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
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
            animHandler.Idle(isMoving);
        }


        protected void UpdatePosition()
        {
            playerInput.Set(input.PlayerInput.x, 0, input.PlayerInput.y);
            transform.position += playerInput.normalized * moveSpeed * Time.deltaTime;

            animHandler.Locomotion(fwdDotProduct, rightDotProduct);
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




        public void EnableControl()
        {
            input.enabled = true;
        }

        public void DisableControl()
        {
            input.enabled = false;
        }

    }
}


