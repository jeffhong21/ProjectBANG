namespace Bang
{
    using UnityEngine;


    public class ProjectileBase : MonoBehaviour, IProjectile
    {
        protected ActorController _owner;
        [SerializeField]
        protected float _damage = 1f;
        [SerializeField]
        protected float _lifeDuration = 2f;
        [SerializeField]
        protected Vector3 _spawnLocation;
        [SerializeField]
        protected float _maxRange = 50f;
        [SerializeField]
        protected float _velocity = 100f;



        public float damage{
            get{
                return _damage;
            }
            set{
                _damage = value;
            }
        }
            
        public Vector3 spawnLocation{
            get{
                return _spawnLocation;
            }
            //set{
            //    _spawnLocation = value;
            //}
        }

        public float velocity{
            get { 
                return _velocity; 
            }
            set{
                _velocity = value;
            }
        }


        private float spawnTime;


        public void Init(ActorController actorCtrl)
		{
            _owner = actorCtrl;
		}


		protected virtual void OnEnable()
        {
            spawnTime = Time.time;
            
            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, Layers.hitableObjects);
            if (initialCollisions.Length > 0)
            {
                OnHitObject(initialCollisions[0], transform.position);
            }
        }

        protected virtual void OnDisable()
        {
            
        }


        protected virtual void Update()
        {
            float moveDistance = _velocity * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);

            if(Time.time > spawnTime + _lifeDuration){
                PoolManager.instance.Return(PoolTypes.Projectile, this);
            }

            //Destroy(gameObject, _lifeDuration);
        }


        private void CheckCollisions(float moveDistance)
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, moveDistance, Layers.hitableObjects, QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider, hit.point);
            }
        }


        private void OnHitObject(Collider c, Vector3 hitPoint)
        {
            //Debug.Log(hit.collider.gameObject.name);

            IHasHealth damageableObject = c.GetComponent<IHasHealth>();

            if (damageableObject != null)
            {
                damageableObject.TakeDamage(_damage, hitPoint, transform.forward);
            }


            if (c.gameObject.layer == LayerMask.NameToLayer("Cover"))  // LayerMask.NameToLayer("WorldObjects")
            {
                ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ImpactHit, hitPoint, Quaternion.FromToRotation(Vector3.forward, -transform.forward));
            }


            PoolManager.instance.Return(PoolTypes.Projectile, this);

            //  Reset the owner of projectile.
            _owner = null;
        }


    }
}


