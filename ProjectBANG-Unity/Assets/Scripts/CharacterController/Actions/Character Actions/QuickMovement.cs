namespace CharacterController
{
    using UnityEngine;
    using System;


    public class QuickMovement : CharacterAction
    {
        public static readonly int LateralDirectionHashID = Animator.StringToHash("LateralDirection");


        [SerializeField]
        protected float m_LateralDirection, m_LateralVelocity;
        protected float m_LateralDirectionDamping = 0.8f;
        protected float m_ForwardDirection;
        [SerializeField]
        protected float m_AngleRootToMove;

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            //if (m_Controller.InputVector.x != 0)
            //{
            //    m_LateralDirection = Mathf.SmoothDamp(m_LateralDirection, m_Controller.InputVector.x, ref m_LateralVelocity, m_LateralDirectionDamping);
            //    m_LateralDirection = (float)Math.Round(m_LateralDirection, 2);

            //    if ((m_LateralDirection > 0 && m_Controller.InputVector.x < 0) || (m_LateralDirection < 0 && m_Controller.InputVector.x > 0))
            //    {
            //        //Debug.DrawRay(m_Transform.position + Vector3.up * 0.35f, m_Transform.right * m_LateralDirection, Color.magenta, 1);
            //        //Debug.DrawRay(m_Transform.position + Vector3.up * 0.5f, m_Transform.right * m_Controller.InputVector.x, Color.cyan, 1);
            //        Debug.LogFormat("InputVectorX: {0} | m_LateralDirection: {1}", m_Controller.InputVector.x, m_LateralDirection);
            //        return true;
            //    }
            //}
            //else{
            //    m_LateralDirection = 0;
            //}

            return false;
        }


        //private void TEMP()
        //{
        //    Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(m_Transform.forward));
        //    Vector3 moveDirection = referentialShift * m_RelativeInputVector;
        //    Vector3 axisSign = Vector3.Cross(moveDirection, m_Transform.forward);
        //    m_AngleRootToMove = Vector3.Angle(m_Transform.forward, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

        //    if (m_InputVector.x != 0)
        //    {
        //        m_LateralDirection = Mathf.SmoothDamp(m_LateralDirection, m_InputVector.x, ref m_LateralVelocity, 0.25f);
        //        m_LateralDirection = (float)Math.Round(m_LateralDirection, 2);
        //    }

        //    //m_LateralDirection = Mathf.Clamp(Mathf.Round(m_LateralDirection), -1, 1);
        //    //if (m_InputVector.x > 0) m_LateralDirection = 1;
        //    //else if (m_InputVector.x < 0) m_LateralDirection = -1;
        //    //else m_LateralDirection = 0;

        //    if ((m_LateralDirection > 0 && m_InputVector.x < 0) || (m_LateralDirection < 0 && m_InputVector.x > 0))
        //    {
        //        Debug.DrawRay(m_Transform.position + Vector3.up * 0.35f, m_Transform.right * m_LateralDirection, Color.magenta, 1);
        //        Debug.DrawRay(m_Transform.position + Vector3.up * 0.5f, m_Transform.right * m_InputVector.x, Color.cyan, 1);
        //        //Debug.Break();
        //    }

        //    //m_LateralDirection = Mathf.Clamp(Mathf.Round(m_InputVector.x), -1, 1);
        //    m_ForwardDirection = Mathf.Clamp(Mathf.Round(m_InputVector.z), -1, 1);
        //}

        //protected override void ActionStarted()
        //{
        //    //m_Animator.SetInteger(HashID.ActionID)
        //}


        public override bool CanStopAction()
        {
            return true;
        }

        public override bool IsConcurrentAction (){
            return true;
        }


		public override bool UpdateAnimator()
		{
            //if (m_Controller.InputVector.x != 0)
            //{
            //    m_LateralDirection = Mathf.SmoothDamp(m_LateralDirection, m_Controller.InputVector.x, ref m_LateralVelocity, m_LateralDirectionDamping);
            //    m_LateralDirection = (float)Math.Round(m_LateralDirection, 2);

            //    if ((m_LateralDirection > 0 && m_Controller.InputVector.x < 0) || (m_LateralDirection < 0 && m_Controller.InputVector.x > 0))
            //    {
            //        //Debug.DrawRay(m_Transform.position + Vector3.up * 0.35f, m_Transform.right * m_LateralDirection, Color.magenta, 1);
            //        //Debug.DrawRay(m_Transform.position + Vector3.up * 0.5f, m_Transform.right * m_Controller.InputVector.x, Color.cyan, 1);
            //        Debug.LogFormat("InputVectorX: {0} | m_LateralDirection: {1}", m_Controller.InputVector.x, m_LateralDirection);
            //        return true;
            //    }
            //}
            //else{
            //    m_LateralDirection = 0;
            //}

            //m_Animator.SetInteger(LateralDirectionHashID, 0);
            return true;
		}


	}

}

