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
        [SerializeField, Range(0, 1)]
        protected float m_RecoilAmount = 0.1f;
        [SerializeField, Range(0,1)]
        protected float m_Spread = 0.01f;
        [SerializeField]
        protected GameObject m_Smoke;
        [SerializeField]
        protected GameObject m_MuzzleFlash;
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

        [Header("-- Decals --")]
        [SerializeField]
        protected GameObject m_DefaultDecal;
        [SerializeField]
        protected GameObject m_DefaultDust;


        [Header("Debug"), SerializeField]
        private bool m_DrawAimLine;

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



            if (m_Smoke != null){
                m_Smoke.transform.localPosition = m_FirePoint.localPosition;
                m_Smoke.transform.localRotation = m_FirePoint.localRotation;
                m_Smoke.SetActive(false);
            }
            if (m_MuzzleFlash != null){
                m_MuzzleFlash.transform.localPosition = m_FirePoint.localPosition;
                m_MuzzleFlash.transform.localRotation = m_FirePoint.localRotation;
                m_MuzzleFlash.SetActive(false);
            }
        }

        public override void Initialize(Inventory inventory)
        {
            base.Initialize(inventory);

            m_Rotation = m_Transform.parent.localRotation;
        }




        public virtual bool TryUse()
        {
            if (InUse()) return false;

            if (m_CurrentAmmo > 0)
            {
                Fire();
                //  Set cooldown variables.
                m_NextUseTime = Time.timeSinceLevelLoad + m_FireRate;
                return true;
            }
            return false;
        }


        public virtual bool InUse(){
            return Time.timeSinceLevelLoad < m_NextUseTime;
        }





        public virtual bool TryStartReload()
        {
            if (IsReloading()) return false;

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
            if (m_Smoke) m_Smoke.SetActive(true);
            if (m_MuzzleFlash) m_MuzzleFlash.SetActive(true);
        }

        protected override void ItemDeactivated()
        {
            if (m_Smoke) m_Smoke.SetActive(false);
            if (m_MuzzleFlash) m_MuzzleFlash.SetActive(false);
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
                m_Smoke.GetComponentInChildren<ParticleSystem>().Play();
            }
            if(m_MuzzleFlash){
                m_MuzzleFlash.GetComponentInChildren<ParticleSystem>().Play();
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

            Debug.DrawRay(m_FirePoint.position, m_FirePoint.forward * m_HitscanFireRange, Color.green, 1);
            if(Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hit, m_HitscanFireRange, m_HitscanImpactLayers))
            {
                var damagableObject = hit.transform.GetComponent<Health>();
                //var damagableObject = hit.transform.GetComponent<DamageReciever>();
                Vector3 hitDirection = hit.transform.position - m_FirePoint.position;
                Vector3 force = hitDirection.normalized * m_HitscanImpactForce;
                var rigb = hit.transform.GetComponent<Rigidbody>();


                if(damagableObject is CharacterHealth)
                {
                    RaycastHit hitGameObject;
                    if (Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hitGameObject, m_HitscanFireRange, LayerMask.NameToLayer("Ragdoll"))){
                        damagableObject.TakeDamage(m_HitscanDamageAmount, hitGameObject.point, force, m_Character, hitGameObject.collider.gameObject);
                    }
                    else{
                        damagableObject.TakeDamage(m_HitscanDamageAmount, hit.point, force, m_Character, hit.collider.gameObject);
                    }
                    //Debug.Log(hitGameObject.collider);
                }
                else if(damagableObject is Health)
                {
                    damagableObject.TakeDamage(m_HitscanDamageAmount, hit.point, force, m_Character);
                }
                else{
                    ObjectPoolManager.Instance.Spawn(m_DefaultDust, hit.point, Quaternion.FromToRotation(m_DefaultDust.transform.forward, hit.normal));
                    //SpawnParticles(m_DefaultDust, hit.point, hit.normal);
                    //SpawnHitEffects(hit.point, -hit.normal);
                }



                if (rigb && !hit.transform.gameObject.isStatic)
                {
                    //rigb.AddForce(hitDirection.normalized * m_HitscanImpactForce, ForceMode.Impulse);
                    rigb.AddForceAtPosition(hitDirection.normalized * m_HitscanImpactForce, hit.point, ForceMode.Impulse);
                    //rigb.AddExplosionForce(m_HitscanImpactForce, hitDirection, 50f);
                }

                //Debug.Log(hit.collider.gameObject.name);
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