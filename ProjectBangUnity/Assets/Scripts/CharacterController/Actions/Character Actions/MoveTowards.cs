namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class MoveTowards : CharacterAction
    {
        [SerializeField]
        protected Vector3 m_ActionStartLocation;

        private float m_StoppingDistanceSqr = 0.1f;

        public Vector3 ActionStartLocation{
            get { return m_ActionStartLocation; }
            set { m_ActionStartLocation = value; }
        }

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (m_Transform.position != m_ActionStartLocation)
                return true;
            return false;
        }


        protected override void ActionStarted()
        {
            //Debug.LogFormat("{0} Action has started {1}", GetType().Name, Time.time);
            //m_TargetDirection = m_ActionStartLocation - m_Transform.position;
        }


        public override void UpdateAction()
        {
            m_Transform.position = Vector3.Lerp(m_Transform.position, m_ActionStartLocation, Time.deltaTime * 2);
        }

        //public override bool UpdateMovement()
        //{
        //    m_Transform.position = Vector3.Lerp(m_Transform.position, m_ActionStartLocation, Time.deltaTime);
        //    return true;
        //}



        public override bool CanStopAction()
        {
            if ((m_ActionStartLocation - m_Transform.position).sqrMagnitude < m_StoppingDistanceSqr)
                return true;
            return false;
        }


        protected override void ActionStopped(){
            m_ActionStartLocation = Vector3.zero;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer){
            return "";
        }

        public override bool IsConcurrentAction(){
            return true;
        }



		protected void OnDrawGizmos(){
            if(m_IsActive){
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(m_ActionStartLocation, 0.15f);
            }
		}

	}

}

