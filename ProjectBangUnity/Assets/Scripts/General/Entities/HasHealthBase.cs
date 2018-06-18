namespace Bang
{
    using UnityEngine;


    public abstract class HasHealthBase : EntityBase, IHasHealth
    {
        [Header("----- Health -----")]
        [SerializeField, Tooltip("Total max health.")]
        private float _maxHealth = 4f;
        [SerializeField, Tooltip("Current health")]
        private float _currentHealth;
        [SerializeField, ReadOnly, Tooltip("isDead")]
        private bool _isDead;


        /// <summary>
        /// Max health for entity.
        /// </summary>
        /// <value>The max health.</value>
        public float maxHealth{
            get { return _maxHealth; }
        }

        /// <summary>
        /// Current health for Actor.
        /// </summary>
        /// <value>The current health.</value>
        public float currentHealth{
            get { return _currentHealth; }
            set { _currentHealth = value; }
        }

        /// <summary>
        /// Is the Actor dead.
        /// </summary>
        /// <value><c>true</c> if is dead; otherwise, <c>false</c>.</value>
        public bool isDead{
            get {
                _isDead = this.currentHealth <= 0f || (this.gameObject != null && this.gameObject.activeSelf == false);
                return _isDead;
            }  
            set { _isDead = value; }
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            _currentHealth = _maxHealth;
        }

        protected override void OnDisable()
        {
            base.OnDisable();


        }



        public abstract void TakeDamage(float damage);

        public abstract void TakeDamage(float damage, Vector3 hitLocation);

        public abstract void TakeDamage(float damage, Vector3 hitLocation, Vector3 hitDirection);

        public abstract void Death();
    }
}


