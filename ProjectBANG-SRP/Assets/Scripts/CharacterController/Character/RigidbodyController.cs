namespace CharacterController
{
    using UnityEngine;
    using System;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class RigidbodyController : MonoBehaviour
    {
        //  Physics variables
        [SerializeField, HideInInspector]
        protected float m_Mass = 100;
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector, Range(0, 90)]
        protected float m_SlopeLimit = 45f;
        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.25f;
        [SerializeField, HideInInspector]
        protected float m_GroundStickiness = 6f;            
        [SerializeField, HideInInspector]
        protected float m_ExternalForceDamping = 0.1f;     
        [SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
        protected float m_StepOffset = 0.15f;
        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;
        [SerializeField, HideInInspector]
        protected float m_GravityModifier = 2f;

        protected CapsuleCollider m_CapsuleCollider;
        protected Collider[] m_LinkedColliders = new Collider[0];
        protected PhysicMaterial m_GroundIdleFrictionMaterial;
        protected PhysicMaterial m_GroundedMovingFrictionMaterial;
        protected PhysicMaterial m_StepFrictionMaterial;
        protected PhysicMaterial m_SlopeFrictionMaterial;
        protected PhysicMaterial m_AirFrictionMaterial;


        [SerializeField, HideInInspector]
        protected float m_DetectObjectHeight = 0.4f;


        [SerializeField, DisplayOnly]
        protected bool m_Grounded = true;
        protected Vector3 m_Gravity;
        protected RaycastHit m_GroundHit, m_StepHit;
        protected float m_GroundDistance;
        protected float m_AirbornThreshold = 0.3f;
        protected Vector3 m_GroundNormal;
        protected float m_GroundCheckHeight;
        protected float m_SlopeAngle;
        protected float m_Stickiness;


        protected LayerManager m_Layers;
        protected Rigidbody m_Rigidbody;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected float m_DeltaTime;



        //  Debug parameters.
        [SerializeField, HideInInspector]
        protected bool m_Debug;
        [SerializeField, HideInInspector]
        protected bool m_DrawDebugLine;






        public RaycastHit GroundHit{
            get { return m_GroundHit; }
        }

        public float GroundDistance{
            get { return m_GroundDistance; }
        }






        protected virtual void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            if(m_CapsuleCollider == null)
                m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_Layers = GetComponent<LayerManager>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;

            if (m_Layers == null)
                m_Layers = m_GameObject.AddComponent<LayerManager>();

            m_Gravity = Physics.gravity;

            // slides the character through walls and edges
            m_GroundedMovingFrictionMaterial = new PhysicMaterial
            {
                name = "GroundedMovingFrictionMaterial",
                staticFriction = .25f,
                dynamicFriction = .25f,
                frictionCombine = PhysicMaterialCombine.Multiply
            };

            // prevents the collider from slipping on ramps
            m_GroundIdleFrictionMaterial = new PhysicMaterial
            {
                name = "GroundIdleFrictionMaterial",
                staticFriction = 1f,
                dynamicFriction = 1f,
                frictionCombine = PhysicMaterialCombine.Maximum
            };

            // air physics 
            m_AirFrictionMaterial = new PhysicMaterial
            {
                name = "AirFrictionMaterial",
                staticFriction = 0f,
                dynamicFriction = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum
            };
        }


        protected virtual void CheckGround() { }

        protected virtual void CheckMovement() { }

        protected virtual void UpdateRotation() { }

        protected virtual void UpdateMovement() { }

        protected virtual void Move() { }

        protected virtual void SetPhysicsMaterial() { }

        


        public bool DetectObject(Vector3 direction, out RaycastHit raycastHit, float maxDistance, LayerMask layerMask, int rayCount = 1, float maxAngle = 0)
        {
            if (rayCount < 1) rayCount = 1;

            if (Physics.Raycast(m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight), direction, out raycastHit, maxDistance, layerMask))
            {
                return true;
            }

            return false;
        }



        private void OnCollisionEnter(Collision collision)
        {
            
        }




        //protected void ScaleCapsule(float scaleFactor)
        //{
        //    if (m_CapsuleCollider.height != m_ColliderHeight * scaleFactor)
        //    {
        //        m_CapsuleCollider.height = Mathf.MoveTowards(m_CapsuleCollider.height, m_ColliderHeight * scaleFactor, Time.deltaTime * 4);
        //        m_CapsuleCollider.center = Vector3.MoveTowards(m_CapsuleCollider.center, m_ColliderCenter * scaleFactor, Time.deltaTime * 2);
        //    }
        //}




        protected Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);
        protected Color _Magenta = new Color(0.8f, 0, 0.8f, 0.8f);

        protected void OnDrawGizmos()
        {

            #region Slope Check
            float slopeCheckHeight = 0.5f;
            Quaternion rotation = Quaternion.AngleAxis(m_SlopeLimit, -transform.right);
            //  Hypotenuse
            Gizmos.color = _Magenta;
            float slopeCheckHypotenuse = slopeCheckHeight / Mathf.Cos(m_SlopeLimit);
            Vector3 slopeAngleVector = slopeCheckHypotenuse * transform.forward;
            Gizmos.DrawRay(transform.position + (transform.forward) * 0.3f, rotation * slopeAngleVector - (slopeAngleVector * 0.3f));

            //Check distance
            Gizmos.color = Color.magenta;
            float slopeCheckDistance = Mathf.Tan(m_SlopeLimit) * slopeCheckHeight;
            Vector3 slopeCheckVector = slopeCheckDistance * transform.forward;
            Gizmos.DrawRay(transform.position + transform.up * slopeCheckHeight, slopeCheckVector );//- (transform.forward) * 0.3f);
            #endregion

            if (m_Debug && Application.isPlaying)
            {
                DrawGizmos();
            }


        }

        protected virtual void DrawGizmos()
        {

        }
    }

}
