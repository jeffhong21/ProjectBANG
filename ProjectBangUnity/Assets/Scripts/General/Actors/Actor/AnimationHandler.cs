namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    //[RequireComponent(typeof(PlayerController))]
    public class AnimationHandler : MonoBehaviour
    {
        public enum MainHandEnum{ Right, Left}

        private class HashID
        {
            public readonly int InputX = Animator.StringToHash("InputX");
            public readonly int InputY = Animator.StringToHash("InputY");
            public readonly int IsWalking = Animator.StringToHash("IsWalking");
            public readonly int Reload = Animator.StringToHash("Reload");
            public readonly int EnterCover = Animator.StringToHash("EnterCover");
            public readonly int InCover = Animator.StringToHash("IsInCover");
            public readonly int ExitCover = Animator.StringToHash("ExitCover");
            public readonly int IsDead = Animator.StringToHash("IsDead");
        }

        [Serializable]
        public class BaseState
        {
            public float lookAtWeight;
            public float bodyWeight;
        }

        [Serializable]
        public class HandState
        {
            public Transform hand;
            public Transform target;
            public float weight;
            public Vector3 offset;
            public bool disableIK;
            [HideInInspector]
            public float savedWeight;

            public HandState(Transform hand){
                this.hand = hand;
            }
        }



        [SerializeField]
        private bool debugTransition;
        [SerializeField]
        private MainHandEnum mainHandEnum;

        public HandState leftHandState;
        public HandState rightHandState;

        private ActorController actor;
        private Animator anim;
        private HashID hashID;
        private Transform aimPivot;

        private Vector3 velocity;
        private Vector3 previousPosition;
        private float fwdDotProduct;
        private float rightDotProduct;
        private AnimatorStateInfo currentAnimation;

        private Transform shoulder;


        public Transform MainHand{
            get{
                if(mainHandEnum == MainHandEnum.Left){
                    return leftHandState.hand;
                }
                return rightHandState.hand;
            }
        }

        public Transform Offhand{
            get{
                if (mainHandEnum == MainHandEnum.Left){
                    return rightHandState.hand;
                }
                return leftHandState.hand;
            }
        }


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

            leftHandState = new HandState(anim.GetBoneTransform(HumanBodyBones.RightHand));
            rightHandState = new HandState(anim.GetBoneTransform(HumanBodyBones.LeftHand));

            hashID = new HashID();
            currentAnimation = anim.GetCurrentAnimatorStateInfo(0);

            InitializeAimPivot();
        }


        private void InitializeAimPivot()
        {
            shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder).transform;

            aimPivot = new GameObject().transform;
            aimPivot.name = "Aim Pivot";
            aimPivot.transform.parent = this.gameObject.transform;
            aimPivot.position = shoulder.position;
        }



        private void FixedUpdate()
        {
            CalculateMovement();
            Locomotion(fwdDotProduct, rightDotProduct);

            HandleShoulder();


        }


		private void OnAnimatorIK()
		{
            //  Update offhand
            UpdateIK(AvatarIKGoal.LeftHand, leftHandState.target, leftHandState.weight);

            //  Update mainhand
            //UpdateIK(AvatarIKGoal.RightHand, rightHandState.target, rightHandState.weight);
		}


		private void UpdateIK(AvatarIKGoal goal, Transform target, float w)
        {
            if(target != null){
                anim.SetIKPositionWeight(goal, w);
                anim.SetIKRotationWeight(goal, w);
                anim.SetIKPosition(goal, target.position);
                anim.SetIKRotation(goal, target.rotation);
            }
        }

        //  Used for Reload Animation Event.  
        private void StoreIKSettings()
        {
            leftHandState.savedWeight = leftHandState.weight;
        }

        //  Used for Reload Animation Event.  (Reload Clip has a event in the animation.)
        private void RestoreIKSettings()
        {
            leftHandState.weight = leftHandState.savedWeight;
            Debug.Log("Reloading Animation is done.");
        }


        private void HandleShoulder()
        {
            aimPivot.position = shoulder.position;

            Vector3 targetDir = actor.AimPosition - aimPivot.position;
            if (targetDir == Vector3.zero)
                targetDir = aimPivot.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, tr, Time.deltaTime * 15);
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
            anim.SetFloat(hashID.InputX, _rightDotProduct);
            anim.SetFloat(hashID.InputY, _fwdDotProduct);
            //anim.SetBool("IsMoving", isMoving);
        }


        public void EquidWeapon(Transform mainHand, Transform offHand)
        {
            rightHandState.target = mainHand;
            rightHandState.weight = 0f;
            leftHandState.target = offHand;
            leftHandState.weight = 1f;
        }


		public void PlayIdle()
        {

        }


        public void WalkingState(bool state)
        {
            anim.SetBool(hashID.IsWalking, state);
        }


        public void PlayReload()
        {
            StoreIKSettings();

            leftHandState.weight = 0f;
            anim.SetTrigger(hashID.Reload);
        }


        public void EnterCover()
        {
            anim.SetBool(hashID.InCover, true);
            anim.SetTrigger(hashID.EnterCover);
            anim.SetBool(hashID.IsWalking, false);
        }


        public void CoverState()
        {
            anim.SetBool(hashID.InCover, true);
            anim.SetBool(hashID.IsWalking, false);
        }


        public void ExitCover()
        {
            anim.SetBool(hashID.InCover, false);
            anim.SetTrigger(hashID.EnterCover);
            anim.SetBool(hashID.IsWalking, true);
        }


        public void Death()
        {
            anim.SetBool(hashID.IsWalking, false);
            anim.SetBool(hashID.IsDead, true);
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



        public void OnDrawGizmosSelected()
        {
            if(actor != null){
                Gizmos.color = Color.green;
                Gizmos.DrawLine(actor.AimOrigin, actor.AimPosition);
            }

        }
    }
}