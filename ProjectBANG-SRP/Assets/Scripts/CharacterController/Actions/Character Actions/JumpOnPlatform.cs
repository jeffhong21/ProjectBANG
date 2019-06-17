namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;


    public class JumpOnPlatform : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;

        //[Header("-- Climb Object Settings --")]
        [SerializeField]
        protected float m_MoveToClimbDistance = 2f;
        [SerializeField, Tooltip("The highest level the character can climb.")]
        protected float m_MaxHeight = 2f;
        [SerializeField]
        protected float m_MinHeight = 0.4f;
        [SerializeField]
        protected LayerMask m_CheckLayers;
        [SerializeField]
        protected AnimatorStateMatchTarget[] m_MatchTargetStates = new AnimatorStateMatchTarget[0];



        private float m_DistanceToWall;
        private Vector3 m_Velocity, m_VerticalVelocity;
        private float m_PlatformHeight, m_HeightDifference;
        private Vector3 m_StartPosition, m_EndPosition;

        private RaycastHit m_WallCheckHit, m_GrabPointHit;

        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);


        [SerializeField, DisplayOnly]
        private bool isMatchingTarget;
        [SerializeField, DisplayOnly]
        private float normalizeTime;


        private Vector3 heightCheckStart;



        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;



            if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_WallCheckHit, m_MoveToClimbDistance, m_CheckLayers))
            {
                //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                heightCheckStart = m_WallCheckHit.point;
                //  Add a little buffer to max height.
                heightCheckStart.y += (m_MaxHeight + 0.1f) - m_CheckHeight;
                heightCheckStart += m_Transform.forward * m_CapsuleCollider.radius;

                if(m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight), Color.cyan, 1f);


                if (Physics.Raycast(heightCheckStart, Vector3.down, out m_GrabPointHit, (m_MaxHeight - m_MinHeight), m_CheckLayers))
                {
                    if (m_GrabPointHit.distance < m_MaxHeight)
                    {
                        return true;
                    }
                }
            }
            return false;
        }





        protected override void ActionStarted()
        {
            m_DistanceToWall = m_WallCheckHit.distance;
            m_StartPosition = m_Transform.position;
            m_EndPosition = m_GrabPointHit.point;
            //  Get the objet to vault over height.
            m_PlatformHeight = (m_MaxHeight + 0.1f) - m_GrabPointHit.distance;

            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;




            float fwdSpeed = Vector3.Dot(m_Rigidbody.velocity, m_Transform.forward);
            m_Velocity = m_Transform.forward * fwdSpeed;
            m_VerticalVelocity = Vector3.up * (Mathf.Sqrt((m_PlatformHeight) * -2 * Physics.gravity.y));
            Debug.DrawRay(m_Transform.position, m_VerticalVelocity, Color.yellow, 2);


            m_Velocity = Quaternion.Inverse(m_Transform.rotation) * m_Velocity + m_VerticalVelocity;
            //m_Velocity = Quaternion.Inverse(m_Transform.rotation) * m_Velocity + m_VerticalVelocity;
            Debug.DrawRay(m_Transform.position, m_Velocity, Color.cyan, 2);



            //m_Rigidbody.velocity = verticalVelocity;
            m_Rigidbody.AddForce(m_Velocity, ForceMode.VelocityChange);

            Debug.LogFormat("{0} height: {1}, distance: {2}", m_WallCheckHit.transform.name, m_PlatformHeight, m_DistanceToWall);
            Debug.LogFormat("Velocity: {0} | VerticalVel;ocity {1} | FwdSpeed : {2}", m_Velocity, m_VerticalVelocity, fwdSpeed);

        }



        public override string GetDestinationState(int layer)
        {
            if (layer != 0) return "";


            if (m_Controller.Moving || m_DistanceToWall > 1f){
                return m_DestinationStateName = "RunningClimb1M";
            }
            else{
                return m_DestinationStateName = "Climb1M";
            }
        }



        public override bool UpdateRotation()
        {

            Quaternion targetRotation = Quaternion.Inverse(m_Transform.rotation) * Quaternion.FromToRotation(-m_Transform.forward, m_WallCheckHit.normal);
            float wallAngleDifference = Vector3.Angle(m_Transform.forward, -m_WallCheckHit.normal);
            //if(angle != 0){
            //    //Quaternion rotation = Quaternion.AngleAxis(angle, m_Transform.up) * m_Transform.rotation;
            //    //m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle * m_DeltaTime * 20, m_Transform.up) * m_Rigidbody.rotation);
            //    m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle, m_Transform.up) * m_Rigidbody.rotation);
            //}

            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, targetRotation, m_DeltaTime * 10));


            //Quaternion rotation = Quaternion.FromToRotation(m_Transform.forward, -m_WallCheckHit.normal) * m_Rigidbody.rotation;
            //m_Rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_Rigidbody.rotation, m_DeltaTime * 10));

            return false;
        }


        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt((m_PlatformHeight) * -2 * Physics.gravity.y));

            float objectHeight = m_PlatformHeight < 1.5f ? m_GrabPointHit.point.y : m_GrabPointHit.point.y - m_CapsuleCollider.height;
            //float distance = objectHeight - m_Rigidbody.position.y;

            float distance = m_GrabPointHit.point.y - m_Rigidbody.position.y;
            float percent = (m_PlatformHeight - distance) / m_PlatformHeight;


            m_Rigidbody.MovePosition(Vector3.Lerp(m_Rigidbody.position, new Vector3(m_Rigidbody.position.x, m_GrabPointHit.point.y, m_StartPosition.z), percent * 0.25f));




            return false;
        }



        public override bool Move()
        {

            //m_Velocity = m_Animator.deltaPosition / m_DeltaTime;
            //m_Rigidbody.velocity = m_Velocity;
            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
            return false;
        }








        public override bool CanStopAction()
        {
            int layerIndex = 0;
            if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0){
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_Animator.IsInTransition(layerIndex)){
                if (m_Animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime >= 0.96f){
                    return true;
                }

            }

            //if (m_ExitingAction && m_Animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == Animator.StringToHash(m_DestinationStateName))
            //{
            //    if(m_Animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 0.96f && m_Animator.IsInTransition(layerIndex))
            //    {
            //        //Debug.LogFormat("{0} is exiting via {1}.",  this.GetType(), "normalized time greater than 0.9");
            //        return true;
            //    }
            //}



            return false;
        }


        protected override void ActionStopped()
        {
            if (m_Animator.isMatchingTarget)
                m_Animator.InterruptMatchTarget();

            m_Rigidbody.isKinematic = false;

            m_CapsuleCollider.center = m_ColliderCenter;
            m_CapsuleCollider.height = m_ColliderHeight;

            m_EndPosition = Vector3.zero;
            m_Velocity = Vector3.zero;
            m_VerticalVelocity = Vector3.zero;
            //m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }




        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {

                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.1f);
                }
            }
        }




        float velocity;
        float angle = 45f;
        int resolution = 5;

        float gravity;
        float radianAngle;
        Vector3[] arcPoints = new Vector3[0];

        protected Vector3[] CalculateArcArray()
        {
            resolution = 5;
            Vector3[] arcArray = new Vector3[resolution];
            angle = 45f;
            velocity = m_DistanceToWall - m_CapsuleCollider.radius * 2;
            gravity = Mathf.Abs(Physics.gravity.y);
            radianAngle = Mathf.Deg2Rad * angle;

            //Debug.Log(gravity);



            float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

            for (int index = 0; index < resolution; index++)
            {
                float t = (float)index / (float)resolution;
                arcArray[index] = CalculateArcPoint(t, maxDistance);
            }

            return arcArray;
        }


        //  Calculate height and distance of each vertex.
        Vector3 CalculateArcPoint(float t, float maxDistance)
        {
            float x = t * maxDistance;
            float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
            return new Vector3(x, y);
        }








    }
}

