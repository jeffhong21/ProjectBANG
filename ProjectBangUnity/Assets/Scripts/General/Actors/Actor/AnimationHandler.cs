namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    //[RequireComponent(typeof(PlayerController))]
    public class AnimationHandler : MonoBehaviour
    {
        readonly int HashInputX = Animator.StringToHash("InputX");
        readonly int HashInputY = Animator.StringToHash("InputY");
        readonly int HashWalking = Animator.StringToHash("IsWalking");
        readonly int HashInCover = Animator.StringToHash("IsInCover");
        readonly int HashIsDead = Animator.StringToHash("IsDead");


        private Animator animator;
        [SerializeField]
        private bool debugTransition;



        private Vector3 velocity;
        private Vector3 previousPosition;
        private float fwdDotProduct;
        private float rightDotProduct;


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            Locomotion(fwdDotProduct, rightDotProduct);
        }


        private void CalculateMovement()
        {
            velocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
            velocity.y = 0;
            velocity = velocity.normalized;

            previousPosition = transform.position;

            fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            rightDotProduct = Vector3.Dot(transform.right, velocity);
        }


        private void Locomotion(float _fwdDotProduct, float _rightDotProduct)
        {
            animator.SetFloat(HashInputX, _rightDotProduct);
            animator.SetFloat(HashInputY, _fwdDotProduct);
            //animator.SetBool("IsMoving", isMoving);
        }


		public void PlayIdle()
        {

        }


        public void WalkingState(bool state)
        {
            animator.SetBool(HashWalking, state);
        }


        public void EnterCover()
        {
            animator.SetBool(HashInCover, true);
            animator.SetBool(HashWalking, false);
        }


        public void CoverState()
        {
            animator.SetBool(HashInCover, true);
            animator.SetBool(HashWalking, false);
        }


        public void ExitCover()
        {
            animator.SetBool(HashInCover, false);
            animator.SetBool(HashWalking, true);
        }


        public void Death()
        {
            animator.SetBool(HashWalking, false);
            animator.SetBool(HashIsDead, true);
        }


        public void TimeoutIdle()
        {
            
        }


        public void Taunt()
        {
            
        }


        public void StandOff()
        {
            
        }


    }
}