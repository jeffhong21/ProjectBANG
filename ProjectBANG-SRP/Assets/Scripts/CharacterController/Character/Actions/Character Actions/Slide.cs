namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Slide : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;
        //public const int ACTION_ID = 15;
        [SerializeField]
        protected float m_MaxDistance = 3f;
        protected float _maxDistance;
        [SerializeField]
        protected LayerMask m_StopLayer;

        protected Vector3 m_EndPosition;
        protected float m_TotalDistance;
        protected float m_DistanceRemaining;
        protected float m_RollRecurrenceDelay = 0.2f;
        protected float m_NextAllowed;

        protected RaycastHit m_CheckDistanceHit;

        public float MaxDistance
        {
            get { return m_MaxDistance; }
            set { m_MaxDistance = value; }
        }



        //
        // Methods
        //
        public void SetMaxDistance(float maxDist)
        {
            _maxDistance = maxDist;
        }


        public override bool CanStartAction()
        {
            if (base.CanStartAction() && m_Controller.Moving && Time.time > m_NextAllowed)
            {
                //var checkDistance = m_MaxDistance + 2 * m_CapsuleCollider.radius;

                //if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_CheckDistanceHit, checkDistance, m_Layers.SolidLayers | m_StopLayer))
                //{
                //    return true;
                //}

            }
            return false;
        }

        protected override void ActionStarted()
        {
            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            m_EndPosition = m_Transform.position + m_Transform.forward * _maxDistance;
            m_TotalDistance = Vector3.Distance(m_Transform.position, m_EndPosition);
            m_DistanceRemaining = m_TotalDistance;

            Vector3 velocity = Vector3.Scale(transform.forward, _maxDistance * new Vector3((Mathf.Log(1f / (m_DeltaTime * m_Rigidbody.drag + 1)) / -m_DeltaTime), 0, (Mathf.Log(1f / (m_DeltaTime * m_Rigidbody.drag + 1)) / -m_DeltaTime)));
            m_Rigidbody.velocity = Vector3.ClampMagnitude(velocity, _maxDistance);
        }



        public override bool UpdateMovement()
        {
            m_CapsuleCollider.height = m_ColliderHeight * m_Animator.GetFloat(HashID.ColliderHeight);
            m_CapsuleCollider.center = (m_CapsuleCollider.height / 2) * Vector3.up;


            m_DistanceRemaining = Vector3.Distance(m_Rigidbody.position, m_EndPosition);



            //m_Rigidbody.drag = Mathf.Lerp(m_Rigidbody.drag, m_Rigidbody.mass, (m_TotalDistance - m_DistanceRemaining) / m_TotalDistance);


            return base.UpdateMovement();
        }



        public override bool CanStopAction()
        {
            int layerIndex = 0;
            if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0)
            {
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_Animator.IsInTransition(layerIndex))
            {
                //Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_AnimatorMonitor.GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                return true;
            }
            else
            {
                return m_DistanceRemaining < m_CapsuleCollider.radius * 2;
            }
        }


        protected override void ActionStopped()
        {
            m_CapsuleCollider.height = m_ColliderHeight;
            m_CapsuleCollider.center = m_ColliderCenter;

            _maxDistance = m_MaxDistance;

            m_EndPosition = Vector3.zero;
            m_TotalDistance = 0;
            m_DistanceRemaining = 0;

            m_NextAllowed = Time.time + m_RollRecurrenceDelay;
        }





        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                return m_StateName;
            }
            return "";
        }






    }

}

