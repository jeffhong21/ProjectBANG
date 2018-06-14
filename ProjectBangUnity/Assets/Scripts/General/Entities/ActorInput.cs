namespace Bang
{
    using UnityEngine;
    using System;



    public abstract class ActorInput : MonoBehaviour
    {
        const string inputXStringId = "InputX";
        const string inputYStringId = "InputY";


        protected Animator animator;
        [SerializeField, Util.ReadOnly]
        protected bool isMoving;


        Vector3 velocity = Vector3.zero;
        Vector3 prevPos = Vector3.zero;
        protected float fwdDotProduct;
        protected float rightDotProduct;
        float directionMagnitude;



        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

        }

        protected virtual void FixedUpdate()
        {
            velocity = (transform.position - prevPos) / Time.deltaTime;
            prevPos = transform.position;
        }

        protected virtual void OnEnable(){}

        protected virtual void OnDisable(){}


        protected virtual void PlayAnimations()
        {
            //PlayIdleAnimation();
            //if(isMoving == false) PlayWalkAnimation();

            PlayLocomotionAnimation();
            animator.SetBool("IsMoving", isMoving);
        }

        private void PlayLocomotionAnimation()
        {
            //Vector3 direction = cursorPosition - this.transform.position;
            velocity.y = 0;
            directionMagnitude = velocity.magnitude;
            velocity = velocity.normalized;
            fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            rightDotProduct = Vector3.Dot(transform.right, velocity);


            animator.SetFloat("InputX", rightDotProduct);
            animator.SetFloat("InputY", fwdDotProduct);
        }


        private void PlayIdleAnimation()
        {

        }












        //  For finding Dot product.
        protected string dotProductInfo
        {
            get{
                return GetDotProduct();
            }
        }

        string GetDotProduct()
        {
            string info = "";
            //Vector3 direction = cursorPosition - this.transform.position;
            velocity.y = 0;
            directionMagnitude = velocity.magnitude;
            velocity = velocity.normalized;
            fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            rightDotProduct = Vector3.Dot(transform.right, velocity);

            if (fwdDotProduct > 0.85)
            {
                info = ("I am moving forward");
            }
            else if (fwdDotProduct < -0.85)
            {
                info = ("I am moving backward");
            }
            else if (fwdDotProduct > 0.4 && rightDotProduct < 0)
            {
                info = ("I am moving diagonally forward and left");
            }
            else if (fwdDotProduct > 0.4 && rightDotProduct > 0)
            {
                info = ("I am moving diagonally forward and right");
            }
            else if (fwdDotProduct < -0.4 && rightDotProduct < 0)
            {
                info = ("I am moving diagonally backward and left");
            }
            else if (fwdDotProduct < -0.4 && rightDotProduct > 0)
            {
                info = ("I am moving diagonally backward and right");
            }
            else if (rightDotProduct < 0)
            {
                info = ("I am moving left");
            }
            else if (rightDotProduct > 0)
            {
                info = ("I am moving right");
            }
            else
            {
                info = ("I am not moving");
            }

            return info;
        }
    }
}


