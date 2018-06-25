namespace Bang
{
    using UnityEngine;


    public class MovementHandler : MonoBehaviour
    {
        private PlayerController playerCtrl;
        [HideInInspector]
        public AnimationHandler animHandler;

        [SerializeField]
        protected bool useRawInput;
        [SerializeField, Range(0.1f, 1)]
        protected float _sensitivity = 1f;
        [SerializeField]
        protected float _moveSpeed = 6f;
        [SerializeField]
        protected float _angularSpeed = 270f;
        [SerializeField]
        protected float _rollSpeed = 8f;
        [SerializeField]
        protected float _rollDistance = 5f;


        float inputX, inputY;
        bool canMove = true;
        bool isMoving;
        bool isRolling;
        Vector3 moveInput;
        Vector3 dashStartPosition;
        Vector3 playerVelocity;



        public float moveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }

        public float angularSpeed
        {
            get { return _angularSpeed; }
        }

        public float rollSpeed
        {
            get { return _rollSpeed; }
        }

        public float rollDistance
        {
            get { return _rollDistance; }
        }


        private void Awake()
        {
            playerCtrl = GetComponent<PlayerController>();
            animHandler = GetComponent<AnimationHandler>();
        }


        private void FixedUpdate()
        {
            //  Move player.
            MovePlayer();
        }


        protected virtual void Update()
        {
            //  Get player input values.
            if (useRawInput)
            {
                inputX = Input.GetAxisRaw("Horizontal");
                inputY = Input.GetAxisRaw("Vertical");
            }
            else
            {
                inputX = Input.GetAxis("Horizontal") * _sensitivity;
                inputY = Input.GetAxis("Vertical") * _sensitivity;
            }
            //  Check if player is moving.
            isMoving = Mathf.Abs(inputX) >= 0.1f || Mathf.Abs(inputY) >= 0.1f ? true : false;
            //animHandler.Locomotion(isMoving);


            if (InputManager.Space)
            {
                isRolling = true;
                animHandler.Roll(isRolling);
                dashStartPosition = transform.position;
            }

        }

        protected void MovePlayer()
        {
            if (!canMove) return;

            #region Old Dashing
            if (isRolling)
            {
                Debug.DrawRay(dashStartPosition, transform.forward * _rollDistance, Color.green);
                transform.Translate(Vector3.forward * Time.deltaTime * _rollSpeed);
                if ((transform.position - dashStartPosition).sqrMagnitude >= (_rollDistance * _rollDistance)){
                    isRolling = false;
                    animHandler.Roll(isRolling);
                }
            }
            else
            {
                moveInput = new Vector3(inputX, 0, inputY);
                playerVelocity = moveInput.normalized * moveSpeed * Time.deltaTime;
                this.transform.position += playerVelocity;
            }
            #endregion

            //Vector3 moveInput = new Vector3(inputX, 0, inputY);
            //playerVelocity = moveInput.normalized * moveSpeed * Time.deltaTime;
            //this.transform.position += playerVelocity;
        }




    }
}


