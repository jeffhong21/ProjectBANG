namespace CharacterController
{
    using UnityEngine;
    using System;


    public class QuickMovement : CharacterAction
    {
        public static readonly int LateralDirectionHashID = Animator.StringToHash("LateralDirection");

        [SerializeField]
        float m_LateralDirection;
        float m_LateralVelocity;
        [SerializeField]
        float m_AngleRootToMove;
        private float m_StartTime;
        //
        // Methods
        //
        public override bool CanStartAction()
        {



            //if(m_Controller.Moving)
            //{
            //    var inputDir = m_Controller.InputVector.x;
            //    Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(m_Transform.forward));
            //    Vector3 moveDirection = referentialShift * m_Controller.RelativeInputVector;
            //    Vector3 axisSign = Vector3.Cross(moveDirection, m_Transform.forward);
            //    m_AngleRootToMove = Vector3.Angle(m_Transform.forward, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
            //    m_LateralDirection = (m_AngleRootToMove / 180f) * 1.5f;
            //    m_LateralDirection = Mathf.SmoothDamp(m_LateralDirection, m_AngleRootToMove, ref m_LateralVelocity, 0.1f);
            //    m_LateralDirection = (float)Math.Round(m_LateralDirection, 2);

            //    //m_LateralDirection = Mathf.SmoothDamp(m_LateralDirection, m_Controller.RelativeInputVector.x, ref m_LateralVelocity, 0.5f);
            //    if( (inputDir > 0 && m_LateralDirection < 0) || (inputDir < 0 && m_LateralDirection > 0))
            //    {
            //        Debug.LogFormat("InputX: {0} | Lateral Dir.x {1}", inputDir, m_LateralDirection);
            //        m_LateralDirection = 0;
            //        return true;
            //    }
            //}
            //else{
            //    m_LateralDirection = 0;
            //}


            return false;
        }



        //protected override void ActionStarted()
        //{
        //    m_Animator.SetInteger(HashID.ActionID, 8);
        //    m_Animator.SetInteger(HashID.ActionIntData, (int)m_Controller.InputVector.x);

        //    m_StartTime = Time.time;
        //}


        //public override bool CanStopAction()
        //{
        //    //return true;
        //    return m_StartTime + 0.25f < Time.time;;
        //}

        //protected override void ActionStopped()
        //{
        //    m_Animator.SetInteger(HashID.ActionID, 0);
        //    m_Animator.SetInteger(HashID.ActionIntData, 0);
        //    //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        //}




	}

}

