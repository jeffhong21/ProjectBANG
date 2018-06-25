namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class ActorHealth : MonoBehaviour
    {
        public bool _invinsible;
        [SerializeField]
        private float _maxHealth = 4f;
        [SerializeField]
        private float _currentHealth;
        [SerializeField]
        private float _timeInvincibleAfterRespawn;
        [SerializeField, ReadOnly]
        private bool _isDead;

        protected float sinkSpeed = 0.5f;
        private float startSinkingTime;



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





        protected virtual void OnEnable()
        {
            _currentHealth = _maxHealth;
        }


        protected virtual void OnDisable()
        {
            
        }



        public void TakeDamage(float damage)
        {
            currentHealth -= damage; ;
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
            StartCoroutine(StartSinking());
        }


        IEnumerator StartSinking(float delaySinkTime = 3f)
        {
            yield return new WaitForSeconds(delaySinkTime);

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


