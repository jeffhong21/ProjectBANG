namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class QuickTurn : CharacterAction
    {


        public override int ActionID
        {
            get { return m_ActionID = ActionTypeID.QuickTurn; }
            set { m_ActionID = value; }
        }


        float quickTurnThreshopld = 135;
        [SerializeField]
        float turnDirection;
        [SerializeField]
        Vector3 previousInput = Vector3.zero;
        [SerializeField]
        Vector3 currentInput = Vector3.zero;

        float threshold = 0.75f;
        bool initiatedQuickTurn;

        //  We hand give the previousInput a snapshot every X seconds and compare if a change has occured.
        float handoffTime = 0.5f;
        float handOffTimer;




        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                previousInput = currentInput;
                currentInput = m_Transform.InverseTransformPoint(m_Controller.InputVector);


                if ((previousInput.z > threshold && currentInput.z < -threshold) ||
                    (previousInput.z < -threshold && currentInput.z > threshold) ||
                    (previousInput.x > threshold && currentInput.x < -threshold) ||
                    (previousInput.x < -threshold && currentInput.x > threshold)
                  )
                {
                    Debug.Log("Pre:  " + previousInput + " | Current: " + currentInput);
                    return true;
                }


                //  Get the stop move direction.
                var axisSign = Vector3.Cross(m_Controller.MoveDirection, m_Transform.forward);
                turnDirection = Vector3.Angle(m_Transform.forward, m_Controller.MoveDirection) * (axisSign.y >= 0 ? -1f : 1f);

                if (Mathf.Abs(turnDirection) > quickTurnThreshopld)
                {
                    return true;
                }
            }

            return false;
        }




        protected override void ActionStarted()
        {

            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_Animator.SetInteger(HashID.ActionID, m_ActionID);

            m_Animator.SetFloat(HashID.ActionFloatData, turnDirection);


        }



        public override bool CanStopAction()
        {

            if (Time.time > m_ActionStartTime + .75f)
                return true;
            return false;
        }


    }

}

