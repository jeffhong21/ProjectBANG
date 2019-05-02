namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class GenericAction : CharacterAction
    {

        private float m_StartTime;

        [Header("-----  Debug -----")]
        public string[] stateNames = new string[0];
        [SerializeField]
        private int currentAnimIndex = 2;


        //
        // Methods
        //



        protected override void ActionStarted()
        {
            //currentAnimIndex = 0;
            m_StartTime = Time.time;
            //Debug.LogFormat("Playing:  {0}.", stateNames[currentAnimIndex]);
            foreach (var clip in m_Animator.runtimeAnimatorController.animationClips)
            {
                Debug.LogFormat("ClipName: {0} | Length: {1}", clip.name, clip.length);
            }
        }



        protected override void ActionStopped()
        {
            currentAnimIndex++;
            if (currentAnimIndex > stateNames.Length - 1) currentAnimIndex = 0;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currentAnimIndex]))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration)
                    return true;
                return false;
            }

            return m_StartTime + 3f < Time.time;
        }



		public override bool UpdateMovement()
		{
            return false;
		}

        public override bool UpdateRotation()
        {
            return false;
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
                return stateNames[currentAnimIndex];
            return "";
        }





    }

}

