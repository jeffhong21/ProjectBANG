namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class GenericAction_2 : CharacterAction
    {

        private float m_StartTime;

        public string[] stateNames = new string[0];
        [SerializeField]
        private int index;

        protected override void ActionStarted()
        {

            m_StartTime = Time.time;
            //Debug.LogFormat("Playing:  {0}.", stateNames[currentAnimIndex]);
            //foreach (var clip in m_Animator.runtimeAnimatorController.animationClips)
            //{
            //    Debug.LogFormat("ClipName: {0} | Length: {1}", clip.name, clip.length);
            //}
        }



        protected override void ActionStopped()
        {

            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(stateNames[index]))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration)
                {
                    //Debug.LogFormat("{0} has stopped by comparing nameHASH", m_StateName);
                    return true;
                }
                return false;
            }

            return m_StartTime + 2f < Time.time;
        }





        public override bool Move()
        {
            m_Animator.ApplyBuiltinRootMotion();
            var velocity = m_Animator.deltaPosition / m_DeltaTime;
            m_Rigidbody.velocity = velocity;

            return false;
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
                return stateNames[index];
            return "";
        }





    }

}

