namespace CharacterController
{
    using UnityEngine;


    public class ShootableWeapon : ItemObject, IUseableItem, IReloadableItem
    {
        //
        // Fields
        //
        [Header("--  Shootable Weapon Settings --")]
        [SerializeField]
        protected Transform m_FirePoint;
        [SerializeField]
        protected float m_FireRate = 1;                 //  The number of shots per second
        [SerializeField]
        protected int m_FireCount = 1;
        [SerializeField]
        protected int m_CurrentAmmo = 30;
        [SerializeField]
        protected int m_MaxAmmo = 30;
        [SerializeField]
        protected bool m_AutoReload;
        [SerializeField]
        protected float m_RecoilAmount = 0.1f;
        [SerializeField, Range(0,1)]
        protected float m_Spread = 0.01f;
        [SerializeField]
        protected string m_ShootStateName = "Shoot";
        [SerializeField]
        protected string m_ReloadStateName = "Reload";
        [SerializeField]
        protected GameObject m_Smoke;
        [SerializeField]
        protected Transform m_SmokeLocation;
        [Header("-- Hitscan Settings --")]
        [SerializeField]
        protected float m_HitscanFireRange = float.MaxValue;
        [SerializeField]
        protected LayerMask m_HitscanImpactLayers = -1;
        [SerializeField]
        protected string m_HitscanDamageEvent;
        [SerializeField]
        protected float m_HitscanDamageAmount = 10;
        [SerializeField]
        protected float m_HitscanImpactForce = 5;
        [Header("-- Projectile Settings --")]
        [SerializeField]
        protected GameObject m_Projectile;

        [Header("Debug"), SerializeField]
        private bool m_DrawAimLine;

        [SerializeField]
        protected GameObject m_DefaultDecal;
        [SerializeField]
        protected GameObject m_DefaultDust;


        private float m_ReloadTime = 3f;
        private float m_NextUseTime = 0;
        private float m_ImpactForceMultiplier = 10;

        private Quaternion m_Rotation;


        //
        // Methods
        //
        protected override void Awake()
        {
            base.Awake();
            if (m_CurrentAmmo > m_MaxAmmo)
                m_CurrentAmmo = m_MaxAmmo;

        }

        public override void Initialize(Inventory inventory)
        {
            base.Initialize(inventory);

            m_Rotation = m_Transform.parent.localRotation;
        }




        public virtual bool TryUse()
        {
            if (InUse())
                return false;
            
            if (m_CurrentAmmo > 0)
            {
                //  Shoot weapon.
                Fire();
                //  Set cooldown variables.
                m_NextUseTime = Time.timeSinceLevelLoad + m_FireRate;
                //Debug.LogFormat("Shooting | {0}", Time.timeSinceLevelLoad);
                return true;
            }
            return false;
        }


        public virtual bool InUse()
        {
            return Time.timeSinceLevelLoad < m_NextUseTime;
        }





        public virtual bool TryStartReload()
        {
            if(IsReloading())
                return false;
            
            m_NextUseTime = Time.timeSinceLevelLoad + m_ReloadTime;

            //Debug.LogFormat("Reloading | {0}", Time.timeSinceLevelLoad);
            return true;
        }


        public virtual bool IsReloading()
        {
            return Time.timeSinceLevelLoad < m_NextUseTime;
        }






        protected override void ItemActivated()
        {

        }



        protected virtual void Fire()
        {
            //  Play gun shooting animation;

            if (m_Projectile){
                ProjectileFire();
            } else {
                HitscanFire();
            }
            //  Play Particle Shoot Effects.
            if(m_Smoke){
                var go = Instantiate(m_Smoke, m_SmokeLocation == null ? m_FirePoint.position : m_SmokeLocation.position, Quaternion.identity);
                var ps = go.GetComponentInChildren<ParticleSystem>();
                ps.Play();
                Destroy(ps.gameObject, ps.main.duration + 1);
            }


            //  Update current ammo.
            m_CurrentAmmo -= m_FireCount;
            //  Reload if auto reload is set.
            if (m_AutoReload && m_CurrentAmmo <= 0){
                TryStartReload();
            }
        }



        protected virtual void ProjectileFire()
        {
            //  Spawn Projectile from the PooManager.
            var go = Instantiate(m_Projectile, m_FirePoint.position, m_FirePoint.rotation);
            //Debug.Break();
            //  Get Projectile Component.
            var projectile = go.GetComponent<Projectile>();
            //  Initialize projectile.
            projectile.Initialize(m_FirePoint.forward, m_GameObject);
        }


        protected virtual void HitscanFire()
        {
            RaycastHit hit;

            if(Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hit, m_HitscanFireRange, m_HitscanImpactLayers)){

                //var damagableObject = hit.transform.GetComponent<Health>();
                var damagableObject = hit.transform.GetComponent<DamageReciever>();

                if(damagableObject)
                {
                    damagableObject.TakeDamage(m_HitscanDamageAmount, hit.point, hit.normal, m_Character);

                    //SpawnParticles(damagableObject.GetType() == typeof(CharacterHealth) ? m_DefaultDecal : m_DefaultDust, hit.point, hit.normal);
                    SpawnParticles(m_DefaultDecal, hit.point, hit.normal);
                }
                else{
                    SpawnParticles(m_DefaultDust, hit.point, hit.normal);
                    //SpawnHitEffects(hit.point, -hit.normal);
                }

            }


            var rigb = hit.transform.GetComponent<Rigidbody>();
            if (rigb && !hit.transform.gameObject.isStatic){
                //rigb.AddForce(transform.forward * m_HitscanImpactForce, ForceMode.Impulse);
                var hitDirection = rigb.transform.position - m_FirePoint.position;
                rigb.AddForceAtPosition(hitDirection.normalized * (m_HitscanImpactForce * m_ImpactForceMultiplier), hit.point, ForceMode.Acceleration);
            }
        }




        protected override void OnStartAim()
        {
            
        }


        protected override void OnAim(bool aim)
        {
            //Debug.Log("Weapon Starting to aim");
            if(aim){
                var targetDirection = m_Controller.LookPosition - m_Transform.position;
                //if(targetDirection == Vector3.zero)
                    //targetDirection.Set(m_Controller.transform.position.x, 1.35f, m_Controller.transform.position.x + 10);
                Debug.DrawRay(m_Transform.position, targetDirection, Color.blue, 1);
                //var targetRotation = Quaternion.LookRotation(targetDirection);
                //targetRotation *= Quaternion.Euler(0, 90, 90);
                //m_Transform.rotation = targetRotation;
                m_Transform.parent.localEulerAngles = new Vector3(0, 90, 90);
            }else{
                m_Transform.parent.localRotation = m_Rotation;
            }
        }



        private void SpawnHitEffects(Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var decal = Instantiate(m_DefaultDecal, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
            Destroy(decal, 5);
            var dust = Instantiate(m_DefaultDust, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
            var ps = dust.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void SpawnParticles(GameObject particleObject, Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var go = Instantiate(particleObject, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void OnDrawGizmos()
        {
            if(m_DrawAimLine && m_FirePoint != null){
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(m_FirePoint.position, (m_FirePoint.forward * 12));  //  + (Vector3.up * m_FirePoint.position.y) 
            }
        }

    }

}