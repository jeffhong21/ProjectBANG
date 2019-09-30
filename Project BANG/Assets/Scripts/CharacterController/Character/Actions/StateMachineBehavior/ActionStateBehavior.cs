namespace CharacterController
{
    using UnityEngine;
    using System;

    public class ActionStateBehavior : StateBehavior
    {
        [SerializeField]
        protected string m_characterAction;
        [SerializeField]
        protected AnimatorMatchTarget m_matchTarget;








        protected override void OnInitialize()
        {
            m_matchTarget = new AnimatorMatchTarget(m_animator);
        }




        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            //m_animatorMonitor.SetActiveStateBehavior(this);
        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            //if (m_matchTarget.matchTarget) m_matchTarget.MatchTarget();
            //if (!m_animator.isMatchingTarget && )
            //{

            //}
        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            //if (animatorMonitor.HasMatchTarget) {
            //    animatorMonitor.ResetMatchTarget();
            //}

            //if (m_matchTarget.matchTarget && m_animator.isMatchingTarget){
            //    m_matchTarget.Reset(true, false);
            //}
            //m_animatorMonitor.SetActiveStateBehavior(null);
        }



        public bool MatchTarget(Vector3 matchPosition, Quaternion matchRotation)
        {
            m_matchTarget.matchTarget = true;
            return m_matchTarget.SetMatchTarget(matchPosition, matchRotation);
        }








    }

}