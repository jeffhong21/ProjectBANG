namespace Bang
{
    using UnityEngine;


    public class Projectile : MonoBehaviour, IPooled
    {
        protected ActorController owner;

        private float damage;
        private float force;
        private float range;
        [SerializeField]
        private float velocity = 100f;
        [SerializeField]
        private float lifeDuration = 2f;
        [SerializeField]
        private float size = 0.1f;


        //  cached variables.
        private Vector3 stopPosition;
        private float distanceRemaining;
        private Ray ray;
        private RaycastHit hit;
        private float spawnTime;
        private float moveDistance;


		private void Awake()
		{
            ray = new Ray(transform.position, transform.forward);

		}


		public void Initialize(ActorController owner, float damage, float force, float range)
        {
            this.owner = owner;
            this.damage = damage;
            this.force = force;
            this.range = range;
        }


        private void OnEnable()
        {
            spawnTime = Time.time;
            
            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, Layers.hitableObjects);
            if (initialCollisions.Length > 0){
                OnHitObject(initialCollisions[0], transform.position);
            }

            stopPosition = transform.forward * range - transform.position;
            distanceRemaining = (stopPosition - transform.position).magnitude;

        }

        private void OnDisable()
        {
            owner = null;
        }


        private void Update()
        {
            moveDistance = velocity * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);

            if(Time.time > spawnTime + lifeDuration){
                PoolManager.instance.Return(PoolTypes.Projectile, this);
            }

        }


        private void CheckCollisions(float dist)
        {
            //Debug.DrawRay(transform.position, transform.forward, Color.red);

            ray.origin = transform.position;
            ray.direction = transform.forward;

            if (Physics.SphereCast(ray, size, out hit, dist, Layers.hitableObjects, QueryTriggerInteraction.Collide)){
                OnHitObject(hit.collider, hit.point);
            }
        }


        private void OnHitObject(Collider c, Vector3 hitPoint)
        {
            //Debug.Log(hit.collider.gameObject.name);
            IHasHealth damageableObject = c.GetComponent<IHasHealth>();

            //  Hit a damagable object.
            if (damageableObject != null){

                //Debug.Log(damageableObject);
                //Debug.Log(owner.gameObject);
                //Debug.Log(damage);
                //Debug.Log(hitPoint);
                //Debug.Log(transform.forward * force);

                //Debug.LogFormat(" DamageableObject: {4} \n ProjectileOwner: {0}\n Damage: {1}\n HitPoint: {2} \n Force: {3}",
                                //owner.gameObject, damage, hitPoint, transform.forward * force, damageableObject);
                
                damageableObject.TakeDamage(damage, hitPoint, transform.forward * force, owner.gameObject);
            }
            else{
                ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ImpactHit, hitPoint, Quaternion.FromToRotation(Vector3.forward, -transform.forward));
            }


            //  Return projectile to pool.
            PoolManager.instance.Return(PoolTypes.Projectile, this);
            //  Reset the owner of projectile.
            owner = null;
        }



    }
}


