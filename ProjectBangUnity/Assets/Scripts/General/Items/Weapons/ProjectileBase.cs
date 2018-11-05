namespace Bang
{
    using UnityEngine;


    public class ProjectileBase : MonoBehaviour, IPooled
    {
        protected ActorController owner;
        [SerializeField]
        protected float damage = 1f;
        [SerializeField]
        protected float force = 300f;
        [SerializeField]
        protected float maxRange = 50f;
        [SerializeField]
        protected float velocity = 100f;
        [SerializeField]
        protected float lifeDuration = 2f;

        private float spawnTime;





        public void Init(ActorController actorCtrl)
		{
            owner = actorCtrl;
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
            float moveDistance = velocity * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);

            if(Time.time > spawnTime + lifeDuration){
                PoolManager.instance.Return(PoolTypes.Projectile, this);
            }
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
            ActorHealth damageableObject = c.GetComponent<ActorHealth>();

            //  Hit a damagable object.
            if (damageableObject != null){
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


