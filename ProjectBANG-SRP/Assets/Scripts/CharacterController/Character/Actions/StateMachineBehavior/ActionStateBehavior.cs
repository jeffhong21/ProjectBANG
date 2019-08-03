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
        public float startMatchTime = 0.1f;
        [SerializeField, Range(0f, 1f)]
        public float endMatchTime = 0.2f;
        [SerializeField]
        protected AvatarTarget targetBodyPart = AvatarTarget.Root;
        [SerializeField]
        protected Vector3 positionXYZWeight = Vector3.one;
        [SerializeField, Range(0f, 1f)]
        protected float rotationWeight = 1;

        protected MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1);







        protected override void OnInitialize()
        {
            weightMask = new MatchTargetWeightMask(positionXYZWeight.normalized, rotationWeight);
        }




        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (matchTarget) {
                if (stateInfo.normalizedTime >= startMatchTime && stateInfo.normalizedTime <= endMatchTime && animatorMonitor.HasMatchTarget) {
                    animatorMonitor.MatchTarget(targetBodyPart, weightMask, startMatchTime, endMatchTime);
                }
            }


        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (animatorMonitor.HasMatchTarget) {
                animatorMonitor.ResetMatchTarget();
            }
        }












    }

}