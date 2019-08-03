namespace CharacterController
{
    using UnityEngine;
    using System;

    public class CharacterActionBehavior : StateMachineBehaviour
    {
        [Serializable]
        public class CharacterAnimationEvent
        {
            public string eventID;
            [Range(0,0.99f)]
            public float time;
        }

        public CharacterAnimationEvent[] animEvent = new CharacterAnimationEvent[0];



        protected AnimatorMonitor m_AnimatorMonitor;
        [SerializeField]
        protected AnimationEvent m_AnimationEvent = new AnimationEvent();


        public AnimatorMonitor AnimMonitor
        {
            get { return m_AnimatorMonitor; }
            set { m_AnimatorMonitor = value; }
        }


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.LogFormat("On State <color=magenta> {0} </color> | Length: {1} | NormalizedTime: {2}", "Enter", stateInfo.length, stateInfo.normalizedTime);

            //m_AnimatorMonitor.ExecuteEvent("OnAnimatorItemEquip");
        }


		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            base.OnStateUpdate(animator, stateInfo, layerIndex);
		}


		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            //Debug.LogFormat("{0}", stateInfo.);
            Debug.LogFormat("On State <color=red> {0} </color> | Length: {1} | NormalizedTime: {2}", "Exit", stateInfo.length, stateInfo.normalizedTime);
            //Debug.Break();
            //m_AnimatorMonitor.ExecuteEvent("OnAnimatorItemEquipComplete");
        }


        //public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        //{
        //    base.OnStateMachineEnter(animator, stateMachinePathHash);
        //    Debug.LogFormat("On StateMachine <color=cyan> {0} </color> | FullHashPath: {1}", "Enter", stateMachinePathHash);

        //}

        //public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        //{
        //    base.OnStateMachineExit(animator, stateMachinePathHash);
        //    Debug.LogFormat("On StateMachine <color=blue> {0} </color> | FullHashPath: {1}", "Exit", stateMachinePathHash);

        //}
    }

}