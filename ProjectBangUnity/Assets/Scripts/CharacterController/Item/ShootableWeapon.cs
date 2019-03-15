namespace CharacterController
{
    using UnityEngine;


    public class ShootableWeapon : Item, IUseableItem, IReloadableItem
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
        [SerializeField]
        protected float m_DamageAmount = 10;
        [SerializeField]
        protected float m_ImpactForce = 5;
        [SerializeField]
        protected float m_FireRange = float.MaxValue;
        [SerializeField]
        protected LayerMask m_ImpactLayers = -1;
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
                m_Smoke = Instantiate(m_Smoke, m_Transform);
                m_Smoke.transform.localPosition = m_SmokeLocation.localPosition;
                m_Smoke.transform.localRotation = m_SmokeLocation.localRotation;
                m_Smoke.SetActive(false);
            }
            if (m_MuzzleFlash != null){
                m_MuzzleFlash = Instantiate(m_MuzzleFlash, m_Transform);
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
            }  else {
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
            //  Get Projectile Component.
            var projectile = go.GetComponent<Projectile>();
            //  Initialize projectile.
            projectile.Initialize(m_DamageAmount, m_FirePoint.forward, m_Character);

        }




        protected virtual void HitscanFire()
        {
            RaycastHit hit;
            Debug.DrawRay(m_FirePoint.position, m_FirePoint.forward * m_FireRange, Color.green, 1);

            if(Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hit, m_FireRange, m_ImpactLayers))
            {
                var damagableObject = hit.transform.GetComponentInParent<Health>();
                Vector3 hitDirection = hit.transform.position - m_FirePoint.position;
                Vector3 force = hitDirection.normalized * m_ImpactForce;
                var rigb = hit.transform.GetComponent<Rigidbody>();



                //if(damagableObject is CharacterHealth)
                //{
                //    RaycastHit hitGameObject;
                //    if (Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hitGameObject, m_FireRange, LayerMask.NameToLayer("Ragdoll"))){
                //        damagableObject.TakeDamage(m_DamageAmount, hitGameObject.point, force, m_Character, hitGameObject.collider.gameObject);
                //    }
                //    else{
                //        damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character, hit.collider.gameObject);
                //    }
                //    //Debug.Log(hitGameObject.collider);
                //}
                //else if(damagableObject is Health)
                //{
                //    damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character);
                //}
                //else{
                //    ObjectPoolManager.Instance.Spawn(m_DefaultDust, hit.point, Quaternion.LookRotation(hit.normal));
                //    //SpawnParticles(m_DefaultDust, hit.point, hit.normal);
                //    //SpawnHitEffects(hit.point, -hit.normal);
                //}


                //if (rigb && !hit.transform.gameObject.isStatic){
                //    //rigb.AddForce(hitDirection.normalized * m_ImpactForce, ForceMode.Impulse);
                //    rigb.AddForceAtPosition(hitDirection.normalized * m_ImpactForce, hit.point, ForceMode.Impulse);
                //}


                if (damagableObject is CharacterHealth)
                {
                    damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character, hit.collider.gameObject);
                }
                else if (damagableObject is Health)
                {
                    damagableObject.TakeDamage(m_DamageAmount, hit.point, force, m_Character);
                }
                else
                {
                    //ObjectPoolManager.Instance.Spawn(m_DefaultDust, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
                    ObjectPoolManager.Instance.Spawn(m_DefaultDust, hit.point, Quaternion.LookRotation(hit.normal));
                }


                if (rigb && !hit.transform.gameObject.isStatic)
                {
                    rigb.AddForceAtPosition(hitDirection.normalized * m_ImpactForce, hit.point, ForceMode.Impulse);
                }



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
            var decal = Instantiate(m_DefaultDecal, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            Destroy(decal, 5);
            var dust = Instantiate(m_DefaultDust, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            var ps = dust.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void SpawnParticles(GameObject particleObject, Vector3 collisionPoint, Vector3 collisionPointNormal)
        {
            var go = Instantiate(particleObject, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }


        private void OnDrawGizmos()
        {
            if(m_DrawAimLine && m_FirePoint != null){
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(m_FirePoint.position, (m_FirePoint.forward * 50));  //  + (Vector3.up * m_FirePoint.position.y) 
            }
        }













    }

}