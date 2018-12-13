namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    public static class HashID
    {
        public static readonly int InputX = Animator.StringToHash("InputX");
        public static readonly int InputY = Animator.StringToHash("InputY");
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int Aiming = Animator.StringToHash("Aiming");
        public static readonly int WeaponID = Animator.StringToHash("WeaponID");
        public static readonly int AnimationIndex = Animator.StringToHash("AnimationIndex");

        public static readonly int ShootWeapon = Animator.StringToHash("ShootWeapon");
        public static readonly int Damage = Animator.StringToHash("Damage");
        public static readonly int IsReloading = Animator.StringToHash("IsReloading");
        public static readonly int Die = Animator.StringToHash("Die");
    }

    //[RequireComponent(typeof(PlayerController))]
    public class AnimationHandler : MonoBehaviour
    {

        [SerializeField]
        private bool debugTransition;

        private ActorController actor;
        private Animator anim;
        private Transform aimPivot;

        private Vector3 velocity;
        private Vector3 previousPosition;
        private float fwdDotProduct;
        private float rightDotProduct;
        private AnimatorStateInfo currentAnimation;




        private float PercentComplete{
            get{
                currentAnimation = anim.GetCurrentAnimatorStateInfo(0);
                return currentAnimation.normalizedTime;
            }
        }

        public Animator Anim{
            get { return anim;}
        }





        private void Awake()
        {
            actor = GetComponent<ActorController>();
            anim = GetComponent<Animator>();

            currentAnimation = anim.GetCurrentAnimatorStateInfo(0);
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
            anim.SetFloat(HashID.InputX, _rightDotProduct);
            anim.SetFloat(HashID.InputY, _fwdDotProduct);

            //if(_fwdDotProduct == 0 && _rightDotProduct == 0)
                //anim.SetBool("IsMoving", true);
        }


        public float AnimationLength(string clipName)
        {
            float time = 0;
            RuntimeAnimatorController ac = anim.runtimeAnimatorController;

            for (int i = 0; i < ac.animationClips.Length; i++)
                if (ac.animationClips[i].name == clipName)
                    time = ac.animationClips[i].length;
            
            return time;
        }


        public void EquipWeapon(int index)
        {
            anim.SetInteger(HashID.WeaponID, index);
        }


		public void PlayIdle()
        {

        }


        public void WalkingState(bool state)
        {
            anim.SetBool(HashID.Moving, state);
        }


        public void PlayReload(bool isPlaying)
        {
            anim.SetBool(HashID.IsReloading, isPlaying);
        }


        public void PlayTakeDamage(Vector3 hitLocation)
        {
            float fwd = Vector3.Dot(transform.forward, hitLocation);
            float right = Vector3.Dot(transform.right, hitLocation);

            int index = 0;

            if(fwd >= 0.45 || fwd <= -0.45){
                if (fwd >= 0.45){
                    index = 0;
                } else {
                    index = 3;
                }
            }

            if (right <= -0.45){
                index = 1;
            }
            else if (right >= 0.45){
                index = 2;
            }
            else{
                index = 0;
            }

            anim.SetInteger(HashID.AnimationIndex, index);
            anim.SetTrigger(HashID.Damage);
            //Debug.Log(index);
        }


        public void SetAim(bool isAiming)
        {
            anim.SetBool(HashID.Aiming, isAiming);
        }


        public void PlayShootAnim()
        {
            anim.SetTrigger(HashID.ShootWeapon);
        }



        public void Death(Vector3 hitLocation)
        {
            float fwd = Vector3.Dot(transform.forward, hitLocation);
            float right = Vector3.Dot(transform.right, hitLocation);

            int index = 0;

            if (fwd >= 0.45 || fwd <= -0.45){
                if (fwd >= 0.45){
                    index = 0;
                } else {
                    index = 3;
                }
            }

            if (right <= -0.45){
                index = 1;
            }
            else if (right >= 0.45){
                index = 2;
            }
            else{
                index = 0;
            }

            anim.SetBool(HashID.Moving, false);
            anim.SetInteger(HashID.AnimationIndex, index);
            anim.SetTrigger(HashID.Die);
        }




        public void OnDrawGizmosSelected()
        {
            //if(actor != null){
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawLine(actor.AimOrigin, actor.AimPosition);
            //}

        }
    }
}