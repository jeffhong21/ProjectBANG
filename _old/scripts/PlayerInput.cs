//namespace Bang
//{
//    using UnityEngine;
//    using System;



//    [RequireComponent(typeof(PlayerCtrl))]
//    public class PlayerInput : ActorInput
//    {
        
//        [Header("----- Movement -----")]
//        [SerializeField]
//        protected float _moveSpeed = 6f;
//        [SerializeField, Range(0.1f, 1)]
//        protected float _sensitivity = 1f;
//        [SerializeField]
//        protected float _angularSpeed = 270f;
//        [SerializeField]
//        protected float _dashSpeed = 10f;
//        [SerializeField]
//        protected float _dashDistance = 10f;


//        private Vector3 dashStartPosition, cursorPosition, playerVelocity;

//        [Header("----- References -----")]
//        [SerializeField]
//        protected PlayerCrosshairs _crosshairs;
//        //[SerializeField]
//        protected Camera _playerCamera;

//        PlayerCtrl playerCtrl;
//        Vector2 playerInput;
//        bool canMove = true;



//        [Header("----- Debug -----")]
//        [SerializeField]
//        private bool useRawInput;
//        [SerializeField]
//        private bool drawAimLine;
//        [SerializeField]
//        private Color aimLineColor = Color.red;
//        [SerializeField]
//        private bool displayCursorPosition;
//        [SerializeField]
//        private Color cursorPositionDisplayColor = Color.black;
//        [SerializeField]
//        private bool displayPlayerInputValues;
//        Vector3 startAimLoc, endAimLoc;



//        public float moveSpeed{
//            get { return _moveSpeed; }
//            set { _moveSpeed = value; }
//        }

//        public float angularSpeed{
//            get { return _angularSpeed; }
//            set { _angularSpeed = value; }
//        }

//        public float dashSpeed{
//            get { return _dashSpeed; }
//            set { _dashSpeed = value; }
//        }

//        public float dashDistance{
//            get { return _dashDistance; }
//            set { _dashDistance = value; }
//        }

//        PlayerCrosshairs crosshairs{
//            get { return _crosshairs; }
//            set { _crosshairs = value; }
//        }



//        protected override void Awake()
//        {
//            base.Awake();
//            playerCtrl = GetComponent<PlayerCtrl>();
//            //animator = GetComponent<Animator>();

//            if(_playerCamera == null) _playerCamera = Camera.main;

//            if (_crosshairs != null)
//                crosshairs = Instantiate(_crosshairs, this.transform.position, _crosshairs.transform.rotation, this.transform);
//            else
//                Debug.LogFormat("<color={0}>No crosshairs instantiated. </color>", "#ff0000ff"); //  red.
            
//        }


//        protected virtual void Start()
//        {
//            //  Update HUD
//            HUDState.UpdatePlayer("PlayerName");
//        }


//        protected override void OnEnable()
//        {
//            base.OnEnable();
//            canMove = true;
//        }


//        protected override void OnDisable()
//        {
//            base.OnDisable();
//        }


//        protected override void FixedUpdate()
//		{
//            base.FixedUpdate();

//            //  Move player.
//            MovePlayer();
//		}


//		protected virtual void Update()
//        {
//            //if(Input.GetKeyDown(KeyCode.Q)){
//            //    displayPlayerInputValues = !displayPlayerInputValues;
//            //}

//            //  Draw aim line.
//            DrawAimLine(drawAimLine);
//            //  Get player input values.
//            GetPlayerInput(useRawInput);
//            //  Aim the player to the mouse location.
//            RotatePlayer();


//            if (InputManager.Space)
//            {
//                isDashing = true;
//                animator.SetBool("Dash", isDashing);
//                dashStartPosition = transform.position;
//            }
//            else if (InputManager.LMB)
//            {
//                playerCtrl.FireWeapon(cursorPosition);
//            }
//            else if (InputManager.RMB)
//            {
                
//            }
//            else if (InputManager.Q)
//            {
                
//            }
//            else if (InputManager.E)
//            {

//            }
//            else if (InputManager.R)
//            {
//                Debug.LogFormat("<color=#800080ff>{0}</color>.  Current ammo is <color=#800080ff>{1}</color>", "Reloading weapon", playerCtrl.equippedFirearm.currentAmmo);  // purple
//                playerCtrl.Reload();
//            }


//            ////  Play walk animation.
//            PlayAnimations();
//        }


//        protected void DrawAimLine(bool isDrawLine)
//        {
//            if (isDrawLine && playerCtrl.equippedFirearm != null)
//            {
//                startAimLoc = playerCtrl.equippedFirearm.projectileSpawn.position;
//                endAimLoc = cursorPosition;
//                endAimLoc.y = playerCtrl.YFocusOffset;
//                Debug.DrawLine(startAimLoc, endAimLoc, aimLineColor);
//            }
//        }


//        protected void GetPlayerInput(bool useRaw = true)
//        {
//            if(useRaw){
//                playerInput.x = Input.GetAxisRaw("Horizontal");
//                playerInput.y = Input.GetAxisRaw("Vertical");
//            }
//            else{
//                playerInput.x = Input.GetAxis("Horizontal") * _sensitivity;
//                playerInput.y = Input.GetAxis("Vertical") * _sensitivity;
//            }

//            //  Check if player is moving.
//            isMoving = Math.Abs(playerInput.x) >= 0.1f || Math.Abs(playerInput.y) >= 0.1f ? true : false;
//        }


//        protected void MovePlayer()
//        {
//            if (!canMove) return;

//            if (isDashing)
//            {
//                Debug.DrawRay(dashStartPosition, transform.forward * _dashDistance, Color.green);
//                transform.Translate(Vector3.up * 2 * animator.GetFloat("HeightCurve"));
//                transform.Translate(Vector3.forward * Time.deltaTime * _dashSpeed);
//                if ((transform.position - dashStartPosition).magnitude >= _dashDistance)
//                {
//                    isDashing = false;
//                    animator.SetBool("Dash", isDashing);
//                }
//            }
//            else
//            {
//                Vector3 moveInput = new Vector3(playerInput.x, 0, playerInput.y);
//                playerVelocity = moveInput.normalized * moveSpeed * Time.deltaTime;
//                this.transform.position += playerVelocity;
//            }
//        }


//        protected void RotatePlayer()
//        {
//            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
//            Plane groundPlane = new Plane(Vector3.up, Vector3.zero * 1);
//            float rayDistance;

//            if (groundPlane.Raycast(ray, out rayDistance))
//            {
//                cursorPosition = ray.GetPoint(rayDistance);
//                //Debug.DrawLine(transform.position, cursorPosition, aimLineColor);
//                // Aim the player to the mouse position.
//                playerCtrl.LookAt(cursorPosition);

//                if(playerCtrl.equippedFirearm != null){
//                    Vector3 aimDirection = cursorPosition;
//                    aimDirection.y = playerCtrl.equippedFirearm.transform.position.y;
//                    playerCtrl.equippedFirearm.transform.LookAt(aimDirection);
//                }

//                cursorPosition.y += playerCtrl.YFocusOffset;
//                crosshairs.transform.position = cursorPosition;
//                //crosshairs.transform.LookAt(GameManager.instance.playerCamera.transform);
//            }

//        }









//        protected void OnGUI()
//        {
//            if (displayCursorPosition)
//            {
//                var cam = Camera.main;
//                if (cam == null)
//                    return;

//                var p = cam.WorldToScreenPoint(cursorPosition);
//                p.y = Screen.height - p.y;

//                GUI.color = cursorPositionDisplayColor;

//                var content = new GUIContent(string.Format("({0})", cursorPosition));
//                var size = new GUIStyle(GUI.skin.label).CalcSize(content);
//                GUI.Label(new Rect(p.x, p.y, size.x, size.y), content);
//            }

//            if (displayPlayerInputValues)
//            {
//                GUILayout.BeginArea(new Rect(5f, Screen.height * 0.3f, Screen.width * 0.2f, Screen.height * 0.2f), GUI.skin.box);

//                var playerInputContent = new GUIContent(string.Format("PlayerInput X: {0}\nPlayerInput Y: {1}", playerInput.x, playerInput.y));
//                GUILayout.Label(playerInputContent);

//                var dotProduct = new GUIContent(string.Format("fwdDotProduct: {0}\nrightDotProduct: {1}", fwdDotProduct, rightDotProduct));
//                GUILayout.Label(dotProduct);

//                var dotProductMsg = new GUIContent(dotProductInfo);
//                GUILayout.Label(dotProductMsg);

//                GUILayout.EndArea();
//            }



//        }


//    }
//}


