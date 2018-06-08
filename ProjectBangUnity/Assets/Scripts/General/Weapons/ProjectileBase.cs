namespace Bang
{
    using UnityEngine;


    public class ProjectileBase : MonoBehaviour, IProjectile
    {
        [SerializeField, Tooltip("TEMP")]
        protected float _damage = 1f;
        [SerializeField, Tooltip("TEMP")]
        protected float _lifeDuration = 2f;
        [SerializeField, Tooltip("TEMP")]
        protected Vector3 _spawnLocation;
        [SerializeField, Tooltip("TEMP")]
        protected float _maxRange = 50f;
        [SerializeField, Tooltip("TEMP")]
        protected float _velocity = 100f;




        public float Damage { get {return _damage; } }

        public float LifeDuration { get { return _lifeDuration; } }

        public Vector3 SpawnLocation { get { return _spawnLocation; } }

        public float MaxRange { get { return _maxRange; } }

        public float Velocity { get { return _velocity; } }


        private float currentTime;


        protected virtual void OnEnable()
        {
            currentTime = Time.time;
            
            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, Layers.hitableObjects);
            if (initialCollisions.Length > 0)
            {
                OnHitObject(initialCollisions[0], transform.position);
            }
        }


        protected virtual void Update()
        {
            float moveDistance = _velocity * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);

            if(Time.time > currentTime + LifeDuration){
                PoolManager.instance.Return(PoolTypes.Projectile, this);
            }

            //Destroy(gameObject, _lifeDuration);
        }


        void CheckCollisions(float moveDistance)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, moveDistance, Layers.hitableObjects, QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider, hit.point);
            }
        }


        void OnHitObject(Collider c, Vector3 hitPoint)
        {
            //Debug.Log(hit.collider.gameObject.name);

            IHasHealth damageableObject = c.GetComponent<IHasHealth>();

            if (damageableObject != null)
            {
                damageableObject.TakeDamage(_damage, hitPoint, transform.forward);
            }


            if (c.gameObject.layer == LayerMask.NameToLayer("WorldObjects"))
            {
                //PoolManager.instance.Spawn(PoolTypes.VFX, hitPoint, Quaternion.FromToRotation(Vector3.forward, -transform.forward));
                ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ImpactHit, hitPoint, Quaternion.FromToRotation(Vector3.forward, -transform.forward));
            }

            //Destroy(gameObject);
            PoolManager.instance.Return(PoolTypes.Projectile, this);

        }


    }
}


