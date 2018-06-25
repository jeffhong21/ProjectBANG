namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    [RequireComponent(typeof(PlayerController))]
    public class AnimationHandler : MonoBehaviour
    {
        readonly int HashInputX = Animator.StringToHash("InputX");
        readonly int HashInputY = Animator.StringToHash("InputY");
        //readonly int HashRolling = Animator.StringToHash("Rolling");
        readonly int HashDeath = Animator.StringToHash("isDead");

        private Animator animator;

        [SerializeField]
        private bool debugTransition;


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


		public void Idle(bool isMoving)
        {
            animator.SetBool("IsMoving", isMoving);
        }


        public void Walk()
        {
            
        }

        public void Locomotion(float fwdDotProduct, float rightDotProduct)
        {
            animator.SetFloat(HashInputX, rightDotProduct);
            animator.SetFloat(HashInputY, fwdDotProduct);
            //animator.SetBool("IsMoving", isMoving);
        }

        public void Run()
        {

        }


        public void Roll(bool isRolling)
        {
            animator.SetBool("Dash", isRolling);
        }


        public void Death()
        {
            animator.SetBool(HashDeath, true);
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