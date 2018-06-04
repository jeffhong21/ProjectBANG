namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;



    /// <summary>
    /// Controller contains methods for performing actions such as Move or Shoot.
    /// </summary>
    public class ActorCtrl : HasHealthBase, IHasFirearm
    {
        protected GameManagerController gm;

        [Header("----- Weapons -----")]
        [SerializeField, Tooltip("Currently equipped firearm")]
        protected FirearmBase _equippedFirearm;


        [Header("----- Actor Misc Settings -----")]
        [SerializeField, Range(0, 1), Tooltip("YAxis height for aiming and sight")]
        protected float _yFocusOffset = 0.75f;
        [SerializeField, Tooltip("The speed at which the enemy sinks through the floor when dead.")]
        protected float sinkSpeed = 2.5f;           




        private bool isSinking;   // Whether the enemy has started sinking through the floor.
        private float startSinkingTime;

        protected Animator animator;
        protected ActorBody actorBody;
        protected CapsuleCollider capsuleCollider;


        public Vector3 spawnPoint
        {
            get;
            private set;
        }


        public float YFocusOffset
        {
            get { return _yFocusOffset; }
        }


        public bool canShoot
        {
            get;
            set;
        }

        public float reloadSpeed
        {
            get;
            set;
        }


        public FirearmBase equippedFirearm
        {
            get { return _equippedFirearm; }
            set { _equippedFirearm = value; }
        }




        protected virtual void Awake()
        {
            gm = GameManagerController.instance;
            if (gm == null){
                gm = FindObjectOfType<GameManagerController>();
                if (gm == null)
                    Debug.LogWarning("Still can't find GameManagerInstance.");
            }


            actorBody = GetComponent<ActorBody>();
            animator = GetComponent<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>();

        }



        protected override void OnEnable()
        {
            base.OnEnable();
            isSinking = false;
            spawnPoint = transform.position;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }






        public virtual void EquipGun(FirearmBase fireArm, Transform location)
        {
            equippedFirearm = Instantiate(fireArm, location.position, location.rotation) as FirearmBase;
            equippedFirearm.transform.parent = location;
        }


        public virtual void FireWeapon(Vector3 target)
        {
            target.y = equippedFirearm == null ? _yFocusOffset : equippedFirearm.projectileSpawn.position.y; ;
            equippedFirearm.Shoot(target);
        }


        public virtual void Reload()
        {
            canShoot = false;
            equippedFirearm.Reload();
            canShoot = true;
        }




        public override void TakeDamage(float damage)
        {
            currentHealth -= damage;;
            if (currentHealth <= 0){
                Death();
            }
        }


        public override void TakeDamage(float damage, Vector3 hitLocation)
        {
            throw new NotImplementedException();
        }


        public override void TakeDamage(float damage, Vector3 hitLocation, Vector3 hitDirection)
        {
            TakeDamage(damage);
            ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ActorHit, hitLocation,Quaternion.FromToRotation(Vector3.forward, hitDirection));
        }


		public override void Death()
		{
            isDead = true;
            animator.SetBool("isDead", isDead);

            StartCoroutine(StartSinking());
		}



        IEnumerator StartSinking(float delaySinkTime = 2f)
        {
            yield return new WaitForSeconds(delaySinkTime);

            startSinkingTime = Time.time;
            isSinking = true;
            capsuleCollider.isTrigger = true;       // Turn the collider into a trigger so shots can pass through it.

            while(isSinking)
            {
                transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
                //transform.position += -Vector3.up * sinkSpeed * Time.deltaTime;

                if (Time.time > startSinkingTime + 2f)
                {
                    //ActorPoolManager.instance.Return(this);
                    gameObject.SetActive(false);
                    yield break;
                }


                //Debug.LogFormat("Direction:  {3}\nVector3 Up:  {0}\nSinkSpeed:  {1}\nDeltaTime:  {2}", -Vector3.up, sinkSpeed, Time.deltaTime, (-Vector3.up * sinkSpeed * Time.deltaTime));
                yield return null;
            }

        }




	}
}


