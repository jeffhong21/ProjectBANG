namespace CharacterController
{
    using UnityEngine;
    using System;

    public class ActionStateBehavior : StateBehavior
    {
        [SerializeField]
        protected bool matchTarget;
        [SerializeField]
        protected Vector3 matchTargetOffset = new Vector3(0.1f, 0, 0);
        [SerializeField, Range(0f, 1f)]
        public float startMatchTarget = 0.1f;
        [SerializeField, Range(0f, 1f)]
        public float endMatchTarget = 0.2f;
        [SerializeField]
        protected AvatarTarget targetBodyPart = AvatarTarget.Root;
        [SerializeField]
        protected Vector3 positionXYZWeight = Vector3.one;
        [SerializeField, Range(0f, 1f)]
        protected float rotationWeight = 1;

        protected MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1);




        protected Vector3 matchPosition;
        protected Quaternion matchRotation;
        protected bool cachedApplyRootMotion;





        public Vector3 GetMatchPosition(Animator animator, AvatarTarget target )
        {
            switch (target) {
                case AvatarTarget.LeftHand:
                    return animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
                case AvatarTarget.RightHand:
                    return animator.GetBoneTransform(HumanBodyBones.RightHand).position;
                case AvatarTarget.LeftFoot:
                    return animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                case AvatarTarget.RightFoot:
                    return animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                case AvatarTarget.Body:
                    return animator.bodyPosition;
                case AvatarTarget.Root:
                    return animator.rootPosition;
                default:
                    return animator.transform.position;
            }
        }

        public Quaternion GetMatchRotation( Animator animator, AvatarTarget target )
        {
            switch (target) {
                case AvatarTarget.LeftHand:
                    return animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation;
                case AvatarTarget.RightHand:
                    return animator.GetBoneTransform(HumanBodyBones.RightHand).rotation;
                case AvatarTarget.LeftFoot:
                    return animator.GetBoneTransform(HumanBodyBones.LeftFoot).rotation;
                case AvatarTarget.RightFoot:
                    return animator.GetBoneTransform(HumanBodyBones.RightFoot).rotation;
                case AvatarTarget.Body:
                    return animator.bodyRotation;
                case AvatarTarget.Root:
                    return animator.rootRotation;
                default:
                    return animator.transform.rotation;
            }
        }




        protected override void OnInitialize()
        {
            weightMask = new MatchTargetWeightMask(positionXYZWeight.normalized, rotationWeight);
        }


        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);



            cachedApplyRootMotion = animator.applyRootMotion;
        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (matchTarget && !animator.isMatchingTarget) {
                animator.applyRootMotion = true;

                matchPosition = GetMatchPosition(animator, targetBodyPart);
                matchRotation = GetMatchRotation(animator, targetBodyPart);

                animator.MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startMatchTarget, endMatchTarget);

            }



        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);



            animator.applyRootMotion = cachedApplyRootMotion;
        }



    }

}