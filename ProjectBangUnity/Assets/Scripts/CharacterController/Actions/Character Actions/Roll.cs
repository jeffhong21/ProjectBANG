namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        public const int ACTION_ID = 15;
        protected readonly float m_MaxRollDistance = 4f;
        protected readonly float m_CheckHeight = 0.35f;

        [SerializeField]
        protected LayerMask m_StopRollLayer;

        protected Vector3 m_RollDirection;
        protected Vector3 m_StartDirection;

        protected float m_ActionIntData;

        public Vector3 RollDirection{
            get { return m_RollDirection; }
            set { m_RollDirection = value; }
        }

        
        //
        // Methods
        //

        public override bool CanStartAction()
        {
            var checkHeight = Vector3.up * m_CheckHeight;
            Debug.DrawRay(m_Transform.position + checkHeight, m_Transform.forward * m_MaxRollDistance, Color.blue, 1f);
            if (Physics.Raycast(m_Transform.position + checkHeight, m_Transform.forward, m_MaxRollDistance, m_StopRollLayer)){
                return false;
            }
            return true;
        }


        protected override void ActionStarted()
        {
            m_StartDirection = m_Transform.forward;
            if(m_RollDirection != Vector3.zero)
                m_Controller.SetRotation(Quaternion.LookRotation(m_RollDirection, m_Transform.up));
            
            m_Animator.SetInteger(HashID.ActionIntData, 0);


        }


        protected override void ActionStopped()
        {

            m_Controller.SetRotation(Quaternion.LookRotation(m_StartDirection, m_Transform.up));
            m_RollDirection = Vector3.zero;

            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


		public override bool UpdateMovement()
		{
            var velocity = m_Animator.deltaPosition / Time.deltaTime;
            velocity.y = 0;
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, velocity, 10 * Time.deltaTime);  //m_Acceleration
            m_Rigidbody.AddForce(-m_Transform.forward * 1, ForceMode.Acceleration);
            return false;
		}


		public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1){
                return false;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName)){
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration){
                    return true;
                }
                //Debug.LogFormat("Current state: {0} .  Normalized time.  {1} ",m_StateName, m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                return false;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName) == false){
                return true;
            }

            return false;


        }
    }

}

