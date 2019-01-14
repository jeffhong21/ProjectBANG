namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public enum CharacterActionType
    {
        Generic
    }


    [RequireComponent(typeof(Rigidbody))]
    public class CharacterLocomotion : MonoBehaviour
    {

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
        [SerializeField]
        protected CharacterAction[] m_Actions = 
        { 
            new Generic()
        };


        [Header("States")]
        [SerializeField]
        protected bool m_IsCrouching;
        public bool IsCrouching
        {
            get { return m_IsCrouching; }
            set { m_IsCrouching = value; }
        }


        [Header("Debug")]
        [SerializeField]
        private bool m_DrawDebugLine;

        Animator m_Animator;
        AnimationMonitor m_AnimationMonitor;
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




        public bool Moving
        {
            get;
            set;
        }

        public float RotationSpeed
        {
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }


        public Vector3 InputVector
        {
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Quaternion LookRotation
        {
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public bool Grounded
        {
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public Vector3 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public CharacterAction[] CharActions
        {
            get { return m_Actions; }
        }




        protected void Awake()
        {
            m_AnimationMonitor = GetComponent<AnimationMonitor>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();



        }


        protected void Start()
        {
            m_AlignToGround = true;
            m_GroundLayer = Bang.Layers.ground;
            m_Delta = Time.fixedDeltaTime;

            var generic = new Generic();
            m_Actions[0] = generic;


            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }



        public void ShootWeapon()
        {
            m_Animator.CrossFade("Upper Layer.Shoot Weapon.Rifle Shoot", 0.2f);
        }

        public void ReloadWeapon()
        {
            m_Animator.CrossFade("Upper Layer.Reload Weapon.Rifle Reload", 0.2f);
        }

        public void Jump()
        {
            m_Animator.CrossFade("Base Layer.Jump.Jump Up", 0.2f);
        }

        public void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            m_Animator.SetBool("Crouching", m_IsCrouching);

            m_InputVector.Set(horizontalMovement, 0, forwardMovement);


            if(horizontalMovement == 0 && forwardMovement == 0)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_Delta * m_RotationSpeed);
                m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, lookRotation, m_Delta * m_RotationSpeed));
                m_Velocity = Vector3.zero;
                m_Rigidbody.velocity = Vector3.zero;

                //  Play Animation
                m_AnimationMonitor.SetHorizontalInputValue(0);
                m_AnimationMonitor.SetForwardInputValue(0);

                //m_Animator.SetFloat("HorizontalInput", 0);
                //m_Animator.SetFloat("ForwardInput", 0);
                //m_Animator.SetBool("Moving", false);

            }
            else
            {
                m_Velocity.Set(m_GroundSpeed.x * m_InputVector.x, m_Rigidbody.velocity.y, m_GroundSpeed.z * m_InputVector.z);


                m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, lookRotation, m_Delta * m_RotationSpeed));
                m_Rigidbody.MovePosition( transform.position + (m_Velocity.normalized * 2 * m_Delta));


                m_FwdDotProduct = Vector3.Dot(transform.forward, m_Velocity);
                m_RightDotProduct = Vector3.Dot(transform.right, m_Velocity);

                //  Play Animation
                m_AnimationMonitor.SetHorizontalInputValue(m_RightDotProduct);
                m_AnimationMonitor.SetForwardInputValue(m_FwdDotProduct);

                //m_Animator.SetBool("Moving", true);
                //m_Animator.SetFloat("HorizontalInput", m_RightDotProduct);
                //m_Animator.SetFloat("ForwardInput", m_FwdDotProduct);
            }






            m_AngleQuaternion = Quaternion.Angle(transform.rotation,lookRotation);
            m_AngleVector3 = Vector3.Angle(transform.forward, transform.rotation * transform.forward);
            m_AngleDot = Vector3.Dot(transform.rotation * transform.forward, transform.forward);
        }


        [SerializeField]
        private float m_AngleQuaternion;
        [SerializeField]
        private float m_AngleVector3;
        [SerializeField]
        private float m_AngleDot;





        public void SetPosition(Vector3 position)
        {
            //transform.position = position;
            m_Rigidbody.MovePosition(position);
        }

        public void SetRotation(Quaternion rotation)
        {
            m_Rigidbody.MoveRotation(rotation);
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }




        public bool TryStartAction(CharacterAction ability)
        {
            if (ability.CanStartAction())
            {
                ability.StartAction();
                return true;
            }
            return false;
        }



        public void TryStopAllActions()
        {

        }

        public void TryStopAction()
        {

        }

        public void ActionStopped()
        {

        }



        #region Debug 


        private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);

        protected void OnDrawGizmos()
        {
            if (m_DrawDebugLine)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, m_Velocity.normalized );
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.forward );
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.right );
                //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            }
        }

        #endregion


    }

}
