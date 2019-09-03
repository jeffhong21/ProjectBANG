namespace CharacterController
{
    using UnityEngine;
    using System;

    public class ActionStateBehavior : StateBehavior
    {
        [SerializeField]
        protected AnimatorMatchTarget m_matchTarget;









        protected override void OnInitialize()
        {
            m_matchTarget = new AnimatorMatchTarget(m_animator);
        }




        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);


        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            //if (matchTarget) {
            //    if (stateInfo.normalizedTime >= startMatchTime && stateInfo.normalizedTime <= endMatchTime && animatorMonitor.HasMatchTarget) {
            //        animatorMonitor.MatchTarget(targetBodyPart, weightMask, startMatchTime, endMatchTime);
            //    }
            //}
            //if(!animator.isMatchingTarget && m_matchTarget.matchTarget) {

            //}

        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            //if (animatorMonitor.HasMatchTarget) {
            //    animatorMonitor.ResetMatchTarget();
            //}
        }












    }

}