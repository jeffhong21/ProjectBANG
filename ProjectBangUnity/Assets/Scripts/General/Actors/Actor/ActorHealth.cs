namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class ActorHealth : MonoBehaviour, IHasHealth
    {
        //
        //  Fields
        //
        public bool _invinsible;
        [SerializeField]
        protected float _maxHealth = 4f;
        [SerializeField]
        protected float _currentHealth;
        [SerializeField]
        protected float _timeInvincibleAfterRespawn;
        [SerializeField, ReadOnly]
        protected bool _isDead;
        [SerializeField]
        protected float sinkSpeed = 0.5f;
        [SerializeField]
        protected float delaySinkTime = 3f;

        private IActorController controller;
        private AnimationHandler AnimHandler;
        private float startSinkingTime;
        private WaitForSeconds sinkDelay;



        //
        //  Properties
        //
        public Vector3 position{
            get{
                return this.transform.position;
            }
        }


        public float maxHealth
        {
            get { return _maxHealth; }
        }


        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = value; }
        }


        public float timeInvincibleAfterRespawn
        {
            get { return _timeInvincibleAfterRespawn; }
        }


        public bool isDead
        {
            get
            {
                _isDead = this.currentHealth <= 0f || (this.gameObject != null && this.gameObject.activeSelf == false);
                return _isDead;
            }
            set { _isDead = value; }
        }



        //
        //  Methods
        //
        protected virtual void Awake()
        {
            controller = GetComponent<IActorController>() as ActorController;
            sinkDelay = new WaitForSeconds(delaySinkTime);
        }


        protected virtual void OnEnable()
        {
            _currentHealth = _maxHealth;
        }


        protected virtual void OnDisable()
        {
            
        }



        public void TakeDamage(float damage)
        {
            if(!_invinsible)
                currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Death();
            }
        }


        public void TakeDamage(float damage, Vector3 hitLocation, Vector3 hitDirection)
        {
            TakeDamage(damage);
            ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ActorHit, hitLocation, Quaternion.FromToRotation(Vector3.forward, hitDirection));
        }



        public void Death()
        {
            isDead = true;
            controller.Death();
            StartCoroutine(StartSinking());
        }


        IEnumerator StartSinking()
        {
            yield return sinkDelay;

            startSinkingTime = Time.time;

            while (true)
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


