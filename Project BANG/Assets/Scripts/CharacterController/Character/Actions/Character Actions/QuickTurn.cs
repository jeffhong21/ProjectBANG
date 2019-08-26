namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class QuickTurn : CharacterAction
    {
         public override int ActionID { get { return m_ActionID = ActionTypeID.QuickTurn; } set { m_ActionID = value; } }


        [SerializeField]
        protected int maxInputCount = 4;

        [Range(-1, 1)]
        protected float threshold = -0.75f;     //  The threshold required to start checking for inputs
        protected bool startInputChecks;
        protected int inputCounts;
        //  Remaing rotation when action is in motion.
        protected float rotationRemaining;
        //  Cach the input direction
        protected Vector3 inputDirection;
        //  What direction turning towards.
        protected float turnDirection;
        //  The starting angle for lerping.
        protected float startAngle = 180f;

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            if (!m_Controller.Moving) return false;


            if (startInputChecks)
            {
                inputCounts++;
                if (inputCounts >= maxInputCount)
                {
                    inputCounts = 0;
                    startInputChecks = false;
                    return true;
                }
                return false;
            }


            float dot = Vector3.Dot(m_Controller.InputVector.normalized, m_Transform.forward);
            if (!startInputChecks && dot <= threshold)
            {
                startInputChecks = true;
                inputDirection = m_Controller.InputVector.normalized;
                rotationRemaining = Vector3.Angle(m_Transform.forward, inputDirection);
                Debug.LogFormat("dot: {0} | rotationRemaining: {1}", dot, rotationRemaining);
            }
            return false;
        }



        protected override void ActionStarted()
        {
            //  Get the start angle.
            startAngle = rotationRemaining;
            //  Get the turn direction.
            turnDirection = Vector3.Cross(inputDirection, m_Transform.forward).y >= 0 ? -1f : 1f;


            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_Animator.SetInteger(HashID.ActionID, m_ActionID);
        }


        public override bool CanStopAction()
        {
            rotationRemaining = Vector3.Angle(m_Transform.forward, inputDirection);
            //return rotationRemaining <= 10;
            if (Time.time > m_ActionStartTime + 1 || rotationRemaining <= 10)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            base.ActionStopped();

            Debug.LogFormat("inputDir: {0}", inputDirection);

            inputDirection = default;
            rotationRemaining = 0;
            turnDirection = 0;
        }




        public override bool UpdateMovement()
        {



            return false;
        }


        public override bool UpdateRotation()
        {
            float rotationAngle = Vector3.Angle(m_Transform.forward, inputDirection);
            float t = startAngle / (startAngle - rotationAngle); //  180 / (180 - X)
            float percentage = t * t * t;


            Quaternion currentRotation = Quaternion.AngleAxis(turnDirection * rotationAngle, m_Transform.up);
            Quaternion targetRotation = Quaternion.AngleAxis(startAngle, m_Transform.up);

            m_Rigidbody.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, percentage));


            

            //m_Rigidbody.MoveRotation(m_Transform.rotation * currentRotation);


            return false;
        }





        protected float Interpolate(float current, float total)
        {
            float t = total / (total - current); //  180 / (180 - X)
            float percentage = t * t * t;

            return percentage;
        }


        //protected float SpikeInterpolate(float current, float total)
        //{
        //    var t = Interpolate(current, total);
        //    if(t <=)
        //}
    }

}

