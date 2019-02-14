namespace Bang
{
    using UnityEngine;
    using System.Collections;


    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyCharacterController : MonoBehaviour
    {
        [SerializeField]
        protected bool m_DrawDebugLine;
        [SerializeField]
        protected float m_RotationSpeed = 15f;
        [SerializeField, Tooltip("Should the character stay aligned to the ground rotation?")]
        protected bool m_AlignToGround;
        [SerializeField]
        protected float m_AlignToGroundDepthOffset = 0.5f;
        [SerializeField]
        protected Vector3 m_GroundSpeed = new Vector3(1, 0, 1);
        [SerializeField]
        protected float m_SkinWidth = 0.08f;
        [SerializeField]
        protected float m_SlopeLimit = 120;



        Animator m_Animator;
        CapsuleCollider m_CapsuleCollider;
        Rigidbody m_Rigidbody;

        LayerMask m_GroundLayer;

        float m_Delta;
        [SerializeField]
        Vector3 m_Velocity;
        Vector3 m_InputVector;
        bool m_Grounded;
        Quaternion m_LookRotation;
        RaycastHit m_HitInfo;
        float m_SlopeAngle;
        float m_FwdDotProduct;
        float m_RightDotProduct;
        Vector3 m_PreviousPosition;




        public bool Moving{
            get;
            set;
        }

        public float RotationSpeed{
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }


        public Vector3 InputVector{
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Quaternion LookRotation{
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public bool Grounded{
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public Vector3 Velocity{
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }






		protected void Awake()
		{
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
		}


        protected void Start()
        {
            m_AlignToGround = true;
            m_GroundLayer = Layers.ground;
            m_Delta = Time.fixedDeltaTime;
        }


        public void Move(float horizontalMovement, float forwardMovement)
        {
            //m_Velocity = (transform.position - m_PreviousPosition) / m_Delta;

            m_Velocity.x = m_GroundSpeed.x * horizontalMovement;
            m_Velocity.y = m_Rigidbody.velocity.y;
            m_Velocity.z = m_GroundSpeed.z * forwardMovement;

            //m_PreviousPosition = transform.position;


            //  Calculate slope angle.
            if (!m_Grounded)
            {
                m_SlopeAngle = 90;
            }
            else
            {
                m_SlopeAngle = Vector3.Angle(m_HitInfo.normal, transform.forward);
            }

            //  Check Ground
            if (Physics.Raycast(transform.position + Vector3.up * m_AlignToGroundDepthOffset, -Vector3.up, out m_HitInfo, m_AlignToGroundDepthOffset + m_SkinWidth, m_GroundLayer))
            {
                if (Vector3.Distance(transform.position + Vector3.up, m_HitInfo.point) < m_AlignToGroundDepthOffset)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * m_AlignToGroundDepthOffset, 2 * m_Delta);
                }
                m_Grounded = true;
            }
            else
            {
                m_Grounded = false;
            }

            //  Apply Gravity
            if (!m_Grounded)
            {
                transform.position += Physics.gravity * m_Delta;
            }


            //  Rotate
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_Delta * m_RotationSpeed);

            //  Move
            if (m_SlopeAngle >= m_SlopeLimit)
            {
                return;
                //Debug.Log("");
            }
            else
            {
                transform.position += m_Velocity.normalized * 4 * m_Delta;
                //m_Rigidbody.MovePosition(transform.position + m_Velocity.normalized * 4 * m_Delta);
            }


            m_FwdDotProduct = Vector3.Dot(transform.forward, m_Velocity);
            m_RightDotProduct = Vector3.Dot(transform.right, m_Velocity);

            m_Animator.SetFloat(HashID.InputX, m_RightDotProduct);
            m_Animator.SetFloat(HashID.InputY, m_FwdDotProduct);
        }


        public void Move (float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            //m_Velocity = (transform.position - m_PreviousPosition) / m_Delta;
            m_InputVector.x = horizontalMovement;
            m_InputVector.z = forwardMovement;

            m_Velocity.x = m_GroundSpeed.x * m_InputVector.x;
            m_Velocity.y = m_Rigidbody.velocity.y;
            m_Velocity.z = m_GroundSpeed.z * m_InputVector.z;

            //m_PreviousPosition = transform.position;


            //  Calculate slope angle.
            if(!m_Grounded){
                m_SlopeAngle = 90;
            }else{
                m_SlopeAngle = Vector3.Angle(m_HitInfo.normal, transform.forward);
            }

            //  Check Ground
            if(Physics.Raycast(transform.position + Vector3.up * m_AlignToGroundDepthOffset, -Vector3.up, out m_HitInfo, m_AlignToGroundDepthOffset + m_SkinWidth, m_GroundLayer)){
                if(Vector3.Distance(transform.position + Vector3.up, m_HitInfo.point) < m_AlignToGroundDepthOffset){
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * m_AlignToGroundDepthOffset, 2 * m_Delta);
                }
                m_Grounded = true;
            } else {
                m_Grounded = false;
            }

            //  Apply Gravity
            if(!m_Grounded){
                transform.position += Physics.gravity * m_Delta;
            }


            //  Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_Delta * m_RotationSpeed);

            //  Move
            if (m_SlopeAngle >= m_SlopeLimit){
                return;
                //Debug.Log("");
            }else{
                transform.position += m_Velocity.normalized * 4 * m_Delta;
                //m_Rigidbody.MovePosition(transform.position + m_Velocity.normalized * 4 * m_Delta);
            }


            m_FwdDotProduct = Vector3.Dot(transform.forward, m_Velocity);
            m_RightDotProduct = Vector3.Dot(transform.right, m_Velocity);

            m_Animator.SetFloat(HashID.InputX, m_RightDotProduct);
            m_Animator.SetFloat(HashID.InputY, m_FwdDotProduct);
        }




        public void SetPosition(Vector3 position){
            //transform.position = position;
            m_Rigidbody.MovePosition(position);
        }

        public void SetRotation(Quaternion rotation){
            transform.rotation = rotation;
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }








        #region Debug 


        Vector3 heightOffset = new Vector3(0, 0.25f, 0);
        protected void OnDrawGizmos()
        {
            if (m_DrawDebugLine)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + heightOffset, m_Velocity.normalized + heightOffset);
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + heightOffset, transform.forward + heightOffset);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position + heightOffset, transform.right + heightOffset);
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            }
        }

        #endregion


	}

}