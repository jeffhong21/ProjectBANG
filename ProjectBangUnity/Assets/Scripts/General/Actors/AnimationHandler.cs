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
        readonly int HashRolling = Animator.StringToHash("IsRolling");
        readonly int HashDeath = Animator.StringToHash("IsDead");
        readonly int HashHasPistol = Animator.StringToHash("HasPistol");
        readonly int HashHasRifle = Animator.StringToHash("HasRifle");


        public enum EquippedWeapon { Pistol, Rifle}
        public EquippedWeapon equippedWeapon;


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


            switch(equippedWeapon)
            {
                case(EquippedWeapon.Pistol):
                    HasPistol();
                    break;
                case (EquippedWeapon.Rifle):
                    HasRifle();
                    break;
            }
        }


        private void CalculateMovement()
        {
            velocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            velocity.y = 0;
            velocity = velocity.normalized;
            fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            rightDotProduct = Vector3.Dot(transform.right, velocity);
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


        public void HasPistol()
        {
            animator.SetBool(HashHasPistol, true);
            animator.SetBool(HashHasRifle, false);
        }

        public void HasRifle()
        {
            animator.SetBool(HashHasPistol, false);
            animator.SetBool(HashHasRifle, true);
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