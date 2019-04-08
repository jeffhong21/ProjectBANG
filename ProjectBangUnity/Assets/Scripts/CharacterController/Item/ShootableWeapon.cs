namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

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
        [SerializeField, Range(0, 30)]
        protected float m_RotationRecoilAmount = 4f;
        [SerializeField, Range(0,1)]
        protected float m_Spread = 0.01f;
        [SerializeField]
        protected GameObject m_Smoke;
        [SerializeField]
        protected GameObject m_MuzzleFlash;
        [SerializeField]
        protected Transform m_SmokeLocation;
        [SerializeField]
        protected AudioClip[] m_FireSounds = new AudioClip[0];
        [SerializeField]
        protected float m_FireSoundDelay = 0.1f;
        [SerializeField]
        protected float m_DamageAmount = 10;
        [SerializeField]
        protected float m_ImpactForce = 5;
        [SerializeField]
        protected float m_FireRange = float.MaxValue;
        [SerializeField]
        protected LayerMask m_ImpactLayers = -1;
        [SerializeField]
        protected GameObject m_Tracer;
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
        private float m_NextUseTime;
        //  Recoil
        private float m_RecoilAngle;
        private Vector3 m_RecoilSmoothDampVelocity;
        private float m_RecoilRotSmoothDampVelocity;
        private Vector3 m_RecoilTargetPosition;
        private Quaternion m_RecoilTargetRotation;

        //  Audio
        private WaitForSeconds m_fireSoundSecondsDelay;
        private Quaternion m_Rotation;



        public int CurrentAmmo{
            get { return m_CurrentAmmo; }
            set { m_CurrentAmmo = value; }
        }


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

            m_fireSoundSecondsDelay = new WaitForSeconds(m_FireSoundDelay);
        }

        public override void Initialize(Inventory inventory)
        {
            base.Initialize(inventory);

            m_Rotation = m_Transform.parent.localRotation;
        }


		private void LateUpdate()
		{
            if(m_Transform.localPosition != Vector3.zero){
                m_RecoilTargetPosition = Vector3.SmoothDamp(m_Transform.localPosition, Vector3.zero, ref m_RecoilSmoothDampVelocity, 0.12f);
                m_Transform.localPosition = Vector3.Lerp(m_RecoilTargetPosition, Vector3.zero, Time.deltaTime * 2f);
            }

            if(m_RecoilAngle != 0){
                m_RecoilAngle = Mathf.SmoothDamp(m_RecoilAngle, 0, ref m_RecoilRotSmoothDampVelocity, 0.12f);
                m_RecoilTargetRotation = Quaternion.Lerp(m_Transform.localRotation, Quaternion.Euler(m_RecoilAngle, 0, 0), Time.deltaTime * 2f);
                m_Transform.localRotation = m_RecoilTargetRotation;
            }


            //Debug.Log(m_RecoilAngle);
		}





		public virtual bool TryUse()
        {
            if (InUse()) return false;

            if (m_CurrentAmmo > 0){
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
                if (m_Tracer){
                    var go = Instantiate(m_Tracer, m_FirePoint.position, Quaternion.LookRotation(m_FirePoint.forward));
                    var ps = go.GetComponentInChildren<ParticleSystem>();
                    ps.Play();
                    Destroy(ps.gameObject, 5);
                }
            }
            ////  Recoil
            m_Transform.localPosition -= Vector3.forward * m_RecoilAmount;
            m_RecoilAngle += Mathf.Clamp( m_RotationRecoilAmount, 0, 30);
            m_Transform.localEulerAngles += Vector3.left * m_RecoilAngle;

            //  Play Particle Shoot Effects.
            if(m_Smoke){
                m_Smoke.GetComponentInChildren<ParticleSystem>().Play();
            }
            if(m_MuzzleFlash){
                m_MuzzleFlash.GetComponentInChildren<ParticleSystem>().Play();
            }

            //  Play Sound Effects
            if(m_FireSounds.Length > 0)
                StartCoroutine(PlaySoundEffect());


            //  Update current ammo.
            //m_CurrentAmmo -= m_FireCount;
            //  Reload if auto reload is set.
            if (m_AutoReload && m_CurrentAmmo <= 0){
                TryStartReload();
                Debug.LogFormat("{0} is auto reloading.", m_GameObject.name);
            }
        }



        private IEnumerator PlaySoundEffect()
        {
            yield return m_fireSoundSecondsDelay;

            var audioSource = GetComponent<AudioSource>();
            var clipIndex = Random.Range(0, m_FireSounds.Length);
            audioSource.clip = m_FireSounds[clipIndex];
            audioSource.Play();

            yield return null;
        }



        protected virtual void ProjectileFire()
        {
            RaycastHit hit;
            if (Physics.Raycast(m_FirePoint.position, m_FirePoint.forward, out hit, m_FireRange))
            {
                //  Spawn Projectile from the PooManager.
                var go = Instantiate(m_Projectile, m_FirePoint.position, m_FirePoint.rotation);
                //  Get Projectile Component.
                var projectile = go.GetComponent<Projectile>();
                //  Initialize projectile.
                projectile.Initialize(m_DamageAmount, m_FirePoint.forward, hit.point,m_Character);
            }

        }




        protected virtual void HitscanFire()
        {
            RaycastHit hit;
            var targetDirection = m_Controller.LookAtPoint - m_FirePoint.position;
            //Debug.DrawRay(m_FirePoint.position, targetDirection, Color.blue, 1);
            if(Physics.Raycast(m_FirePoint.position, targetDirection, out hit, m_FireRange, m_ImpactLayers))
            {
                var damagableObject = hit.transform.GetComponentInParent<Health>();
                Vector3 hitDirection = hit.transform.position - m_FirePoint.position;
                Vector3 force = hitDirection.normalized * m_ImpactForce;
                var rigb = hit.transform.GetComponent<Rigidbody>();



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
            ////Debug.Log("Weapon Starting to aim");
            //if(aim){
            //    //var targetDirection = m_Controller.LookPosition - m_Transform.position;
            //    var targetDirection = m_Character.transform.forward - m_Transform.position;
            //    //if(targetDirection == Vector3.zero)
            //        //targetDirection.Set(m_Controller.transform.position.x, 1.35f, m_Controller.transform.position.x + 10);
            //    Debug.DrawRay(m_Transform.position, targetDirection, Color.blue, 1);
            //    //var m_RecoilTargetRotation = Quaternion.LookRotation(targetDirection);
            //    //m_RecoilTargetRotation *= Quaternion.Euler(0, 90, 90);
            //    //m_Transform.rotation = m_RecoilTargetRotation;
            //    m_Transform.parent.localEulerAngles = new Vector3(0, 90, 90);
            //}else{
            //    m_Transform.parent.localRotation = m_Rotation;
            //}
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
            if(Application.isPlaying){
                if (m_DrawAimLine && m_FirePoint != null)
                {
                    Gizmos.color = Color.yellow;
                    //Gizmos.DrawRay(m_FirePoint.position, (m_FirePoint.forward * 50));  //  + (Vector3.up * m_FirePoint.position.y) 
                    Gizmos.DrawRay(m_FirePoint.position, m_Controller.LookDirection * 50);  //  + (Vector3.up * m_FirePoint.position.y) 

                }
            }

        }













    }

}