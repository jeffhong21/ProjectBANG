namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Fall : CharacterAction
    {
        [SerializeField]
        protected float m_MinFallHeight = 1f;

        private Vector3 startFallPosition;
        private Vector3 endFallPosition;
        private float m_Heightfall;

        private RaycastHit groundCheck;
        //
        // Methods
        //


        public override bool CanStartAction()
        {
            //if(Mathf.Abs(m_Rigidbody.velocity.y) > m_MinFallHeight ){
            //    return true;
            //}

            return false;
        }


		public override bool CanStopAction()
		{
            if (m_Rigidbody.velocity.y == 0){
                return true;
            }

            return false;
		}


		protected override void ActionStarted()
        {
            m_AnimatorMonitor.SetActionID(2);
            startFallPosition = m_Transform.position;
            m_Heightfall = 0;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if(layer == 0){
                return "Fall";
            }
            return "";
        }



        protected override void ActionStopped()
        {
            //endFallPosition = m_Transform.position;
            //m_Heightfall = Vector3.Distance(startFallPosition, endFallPosition);

            m_Heightfall = Mathf.Abs(m_Rigidbody.velocity.y);
            m_AnimatorMonitor.SetActionID(0);
        }




        protected virtual void Reset()
		{
            m_StartType = ActionStartType.Automatic;
            m_StopType = ActionStopType.Automatic;
		}
	}

}

