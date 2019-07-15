namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class JumpBase : CharacterAction
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
                if (m_Controller.Moving && DetectEdge() && Time.time > m_NextJump)
                {
                    return true;
                }





            }
            return false;
        }



        protected bool DetectEdge()
        {
            if (!m_Controller.Grounded)
                return false;
            bool detectEdge = false;



            var m_DetectObjectHeight = 0.25f;
            Vector3 start = m_Transform.position + (m_Transform.forward * m_CapsuleCollider.radius);
            start.y = start.y + m_DetectObjectHeight;
            //start.y = start.y + 0.05f + (Mathf.Tan(m_SlopeAngle) * start.magnitude);

            Vector3 dir = -m_Transform.up;
            float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;


            float front = m_CapsuleCollider.radius * Mathf.Sign(m_Transform.InverseTransformDirection(m_Controller.Velocity).z);
            Vector3 raycastOrigin = m_Transform.TransformPoint(0, m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + 0.1f, front);
            Debug.DrawRay(raycastOrigin, -m_Transform.up * maxDetectEdgeDistance, detectEdge ? Color.green : Color.gray);

            //  Check if anything is in front of the character.
            if (Physics.Raycast(m_Transform.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward, 2, m_Layers.SolidLayers) == false)
            {
                //  Check if there is anything solid.
                if (Physics.Raycast(start, dir, maxDetectEdgeDistance, m_Layers.SolidLayers) == false)
                {
                    detectEdge = true;
                }
            }



            //if (Debug && hitObject == false) Debug.DrawRay(m_Transform.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward * 2, hitObject ? Color.red : Color.green);
            if (m_Debug) Debug.DrawRay(start, dir * maxDetectEdgeDistance, detectEdge ? Color.green : Color.gray);

            return detectEdge;
        }



        protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Jump);

            if (m_Controller.Moving)
            {
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
            if (layer == 0)
            {
                if (m_Controller.Moving)
                {
                    if (m_Animator.pivotWeight >= 0.5f)
                        m_DestinationStateName = "RunStart_LeftUp";
                    else if (m_Animator.pivotWeight < 0.5f)
                        m_DestinationStateName = "RunStart_RightUp";
                }
                else
                {
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

