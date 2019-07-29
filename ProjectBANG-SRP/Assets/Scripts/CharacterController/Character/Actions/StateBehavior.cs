﻿namespace CharacterController
{
    using UnityEngine;
    using System;

    public abstract class StateBehavior : StateMachineBehaviour
    {





        protected AnimatorMonitor animatorMonitor;





        public void Initialize( AnimatorMonitor am )
        {
            animatorMonitor = am;

            OnInitialize();
        }


        protected abstract void OnInitialize();


        public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.LogFormat("On State <color=magenta> {0} </color> | Length: {1} | NormalizedTime: {2}", "Enter", stateInfo.length, stateInfo.normalizedTime);


        }


        public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);


        }


        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            Debug.LogFormat("On State <color=red> {0} </color> | Length: {1} | NormalizedTime: {2}", "Exit", stateInfo.length, stateInfo.normalizedTime);

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