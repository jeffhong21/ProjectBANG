namespace CharacterController
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected float m_DamageAmount = 1f;
        [SerializeField]
        protected float m_ImpactForce = 5f;
        [SerializeField]
        protected string m_DamageEvent = "OnTakeDamage";
        [SerializeField]
        protected GameObject m_Explosion;
        [SerializeField]
        protected GameObject m_DefaultDecal;
        [SerializeField]
        protected GameObject m_DefaultDust;
        [Header("Projectile")]
        [SerializeField]
        protected float m_InitialSpeed;
        [SerializeField]
        protected float m_Speed = 5;
        [SerializeField]
        protected float m_Lifespan = 5;
        [SerializeField]
        protected bool m_DestroyOnCollision = true;
        protected Transform m_Transform;
        protected Collider m_Collider;

        [SerializeField]
        private LayerMask m_LayerMask;
        private bool m_Initialized;
        private GameObject m_Originator;
        private Rigidbody m_Rigidbody;
        private float m_CurrentLifespan;
        private Ray m_Ray;
        private RaycastHit m_Hit;
        private float m_Size = 0.1f;                    //  If size is too small, sometimes physics doesn't register it.
        private Vector3 m_VelocityDamp = Vector3.zero;

        private Vector3 m_StartPosition;
        private Vector3 m_PreviousPosition;


        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_Transform = transform;
            m_Collider = GetComponent<Collider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            //m_LayerMask = ~(1 << 0 | 1 << 1 | 1 << 4 | 1 << 5);
            //m_LayerMask = ~(1 << 9 | 1 << 10 | 1 << 11 );
            m_Rigidbody.useGravity = false;
            WaitForInitialization();
        }


        public void Initialize(Vector3 direction, GameObject originator)
        {
            m_Originator = originator;
            m_Ray = new Ray(m_Transform.position, m_Transform.forward);

            if (direction != Vector3.zero)
                m_Transform.rotation = Quaternion.LookRotation(direction);
            else
                m_Transform.rotation = Quaternion.LookRotation(m_Transform.forward);
            
            m_Rigidbody.velocity = m_Transform.forward * m_Speed;
            //m_Rigidbody.AddForce(m_Transform.forward * m_Speed, ForceMode.Impulse);

            m_StartPosition = m_Transform.position;
            m_PreviousPosition = m_Transform.position;
            gameObject.SetActive(true);
            m_Initialized = true;

            //Debug.Break();
        }


        public void Initialize(Vector3 direction, Vector3 torque, GameObject originator)
        {
            m_Originator = originator;
            m_Ray = new Ray(m_Transform.position, m_Transform.forward);

            if (direction != Vector3.zero)
                m_Transform.rotation = Quaternion.LookRotation(direction);
            else
                m_Transform.rotation = Quaternion.LookRotation(m_Transform.forward);
            m_Rigidbody.velocity = m_Transform.forward * m_Speed;
            m_Rigidbody.AddTorque(torque);

            gameObject.SetActive(true);
            m_Initialized = true;
        }


        public void WaitForInitialization()
        {
            m_Originator = null;
            gameObject.SetActive(false);
            m_Initialized = false;
            m_Rigidbody.velocity = Vector3.zero;
            //Debug.Log("Waiting for initialization");
        }


        protected virtual void Update()
        {
            if(!m_Initialized)
                return;

            RaycastHit hitInfo;
            if (m_Rigidbody.velocity != Vector3.zero)
                m_Transform.forward = Vector3.SmoothDamp(m_Transform.forward, m_Rigidbody.velocity, ref m_VelocityDamp, Time.deltaTime);

            m_Ray.origin = m_Transform.position;
            m_Ray.direction = m_Transform.forward;
            if (Physics.SphereCast(m_Ray, m_Size, out hitInfo, m_LayerMask)){
                Collide(m_Originator, hitInfo.transform, hitInfo.point, hitInfo.normal, false);
            }

            //if(Physics.Linecast(m_PreviousPosition, m_Transform.forward * 0.5f, out hitInfo, m_LayerMask)){
            //    Collide(m_Originator, hitInfo.transform, hitInfo.point, hitInfo.normal, false);
            //}
            else{
                m_CurrentLifespan += Time.deltaTime;
                if (m_CurrentLifespan > m_Lifespan)
                {
                    Destroy(gameObject);
                    //WaitForInitialization();
                }
            }

        }



        protected virtual void Collide(GameObject originator, Transform collisionTransform, Vector3 collisionPoint, Vector3 collisionPointNormal, bool destroy)
        {
            var damagableObject = collisionTransform.GetComponent<Health>();

            if(damagableObject != null)
            {
                //Debug.LogFormat("Hit {0}", collisionTransform.gameObject.name);
                if (damagableObject.GetType() ==  typeof(Health))
                {
                    damagableObject.TakeDamage(m_DamageAmount, collisionPoint, collisionPointNormal, originator);
                    //  Play DefaultDust particle system.
                    if (m_DefaultDust){
                        SpawnParticles(m_DefaultDust, collisionPoint, collisionPointNormal);
                    }
                    //Debug.LogFormat("{0} is of type {1} | Health",damagableObject, damagableObject.GetType());
                }
                else if (damagableObject.GetType() == typeof(CharacterHealth))
                {
                    //EventHandler.ExecuteEvent(damagableObject.gameObject, "OnTakeDamage", m_DamageAmount, collisionPoint, collisionPointNormal, originator);
                    damagableObject.TakeDamage(m_DamageAmount, collisionPoint, collisionPointNormal, originator);
                    //  Play DefaultDecal particle system.
                    if (m_DefaultDecal){
                        SpawnParticles(m_DefaultDecal, collisionPoint, collisionPointNormal);
                    }
                    //Debug.LogFormat("{0} is of type {1} | CharacterHealth", damagableObject, damagableObject.GetType());
                }


                var rigb = damagableObject.GetComponent<Rigidbody>();
                if(rigb && !damagableObject.gameObject.isStatic){
                    rigb.AddForce(transform.forward * m_ImpactForce, ForceMode.VelocityChange);
                }

                //Debug.LogFormat("Hit {0}", damagableObject.gameObject.name);
            }
            //  Did not hit an object that can be damaged.
            else{
                //  Play DefaultDust particle system.
                if (m_DefaultDust){
                    SpawnParticles(m_DefaultDust, collisionPoint, collisionPointNormal);
                }
            }

            //Debug.LogFormat("Hit {0}.", collisionTransform.gameObject.name);
            Destroy(gameObject);
            //if(destroy)
            //    Destroy(gameObject);
            //else
                //WaitForInitialization();
        }




        private void SpawnParticles(GameObject particleObject, Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var go = Instantiate(particleObject, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }



		//private void OnDrawGizmos()
		//{
  //          Gizmos.color = Color.green;
  //          Gizmos.DrawRay(m_Transform.position, (m_Transform.forward * (m_Size)));
  //          //Gizmos.color = Color.green;
  //          //Gizmos.DrawSphere(m_Transform.position, m_Size);
		//}
	}

}