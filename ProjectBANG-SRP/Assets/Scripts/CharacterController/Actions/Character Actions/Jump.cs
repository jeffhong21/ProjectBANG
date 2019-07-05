namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        [SerializeField]
        protected float m_Force = 1;
        [SerializeField]
        protected float m_RecurrenceDelay = 0.1f;



        private float m_AirborneHeight = 0.6f;
        private float m_NextJump;

		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                //if(m_Controller.Moving && m_Controller.DetectEdge() && Time.time > m_NextJump){
                //    return true;
                //}
                return false;
            }
            return false;
		}

		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Jump);

            if(m_Controller.Moving){
                Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt(m_Force * -2 * Physics.gravity.y));
                Vector3 fwdVelocity = m_Transform.forward * m_Force;

                m_Rigidbody.velocity += fwdVelocity + verticalVelocity;
            }
        }


        public override bool CanStopAction()
        {
            int layerIndex = 0;
            var currentState = m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentState.shortNameHash == Animator.StringToHash(m_DestinationStateName))
            {
                if (m_Animator.IsInTransition(layerIndex) || currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
            //if(Time.time > m_ActionStartTime + 0.1f){
            //    return m_Controller.Grounded;
            //}
            //return false;

            return Time.time > m_ActionStartTime + 0.9f;
        }


        protected override void ActionStopped()
        {
            m_NextJump = Time.time + m_RecurrenceDelay;

        }


		public override bool CheckGround()
		{
            //RaycastHit hit = m_Controller.GetRaycastHit();
            //m_Controller.Grounded = hit.distance < m_AirborneHeight;

            m_Controller.Grounded = false;

            return false;
		}




		public override bool UpdateAnimator()
		{
            return base.UpdateAnimator();
		}


		//  Returns the state the given layer should be on.
		public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                if(m_Controller.Moving){
                    if (m_Animator.pivotWeight >= 0.5f)
                        m_DestinationStateName = "RunStart_LeftUp";
                    else if (m_Animator.pivotWeight < 0.5f)
                        m_DestinationStateName = "RunStart_RightUp";
                }
                else{
                    m_DestinationStateName = "IdleStart";
                }

                return m_DestinationStateName;
            }
            return "";
        }









        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {

            }
        }



        protected override void DrawOnGUI()
        {

        }
    }

}

