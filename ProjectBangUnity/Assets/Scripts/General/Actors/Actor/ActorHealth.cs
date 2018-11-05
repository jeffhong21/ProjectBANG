namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class ActorHealth : MonoBehaviour
    {
        //
        //  Fields
        //
        [SerializeField]
        protected bool invinsible;
        [SerializeField]
        protected float currentHealth;
        [SerializeField]
        protected float maxHealth = 4f;
        [SerializeField]
        protected float timeInvincibleAfterRespawn;
        [SerializeField]
        protected float sinkSpeed = 0.5f;
        [SerializeField]
        protected float delaySinkTime = 3f;

        private ActorController controller;
        private float startSinkingTime;
        private WaitForSeconds sinkDelay;
        [SerializeField]
        private bool isDead;


        public event Action<float, Vector3, Vector3, GameObject> OnHealthDamage;

        public event Action<float> OnHeal;

        public event Action<Vector3, Vector3, GameObject> OnDeath;



        //
        //  Properties
        //
        public Vector3 position{
            get{
                return this.transform.position;
            }
        }

        public bool Invinsible
        {
            get { return invinsible; }
        }


        public float MaxHealth
        {
            get { return maxHealth; }
        }


        public float CurrentHealth
        {
            get { return currentHealth; }
            //set { currentHealth = value; }
        }


        public bool IsDead
        {
            get
            {
                isDead = this.CurrentHealth <= 0f || (this.gameObject != null && this.gameObject.activeSelf == false);
                return isDead;
            }
            set { isDead = value; }
        }



        //
        //  Methods
        //
        private void Awake()
        {
            controller = GetComponent<ActorController>();
            sinkDelay = new WaitForSeconds(delaySinkTime);
        }


        private void OnEnable()
        {
            currentHealth = maxHealth;
        }



        public void TakeDamage(float damage, Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
            if (!invinsible)
            {
                if (currentHealth > 0){
                    currentHealth -= damage;
                    //OnHealthDamage(damage, hitDirection, hitDirection, attacker);
                    controller.TakeDamage(hitDirection, hitDirection, attacker);
                }

                if(currentHealth <= 0){
                    Die(hitLocation, hitDirection, attacker);
                }
            }
            ParticlePoolManager.instance.SpawnParticleSystem(ParticlesType.ActorHit, hitLocation, Quaternion.FromToRotation(Vector3.forward, hitDirection));
        }


        private void Die(Vector3 position, Vector3 force, GameObject attacker){
            isDead = true;
            //OnDeath(position, force, attacker);
            controller.Death(position, force, attacker);
            Debug.LogFormat("{0} was killed by {1}", gameObject.name, attacker.name);
            StartCoroutine(StartSinking());
        }


        private IEnumerator StartSinking()
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


