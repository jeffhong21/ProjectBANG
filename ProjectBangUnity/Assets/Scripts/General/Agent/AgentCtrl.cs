namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    using UtilityAI;


    /// <summary>
    /// Agent Controller has methods that the agent can perform.
    /// </summary>
    public class AgentCtrl : ActorCtrl, IAgentCtrl, IHasFirearm
    {
        readonly static float minimumAttackSpeed = 0.25f;
        readonly static int colliderBufferCount = 15;

        protected AgentInput _agentInput;

        [Header("----- Agent Stats -----")]
        [SerializeField, Range(0, 360), Tooltip("Agent's field of view.")]
        protected float _fieldOfView = 108f;

        [SerializeField, Tooltip("How far can the agent see.")]
        protected float _sightRange = 20;

        [SerializeField, Tooltip("How far can the agent scan for things.")]
        protected float _scanRadius;

        [SerializeField, Tooltip("Agents fire speed.")]
        protected float _attackSpeed = 1f;

        [SerializeField, Tooltip("Agents accuracy range.")]
        protected float _aimAccuracy = 3f;

        protected IHasHealth _attackAtarget;
        [SerializeField, Tooltip("Current cover target")]
        protected Collider _coverTarget;

        [SerializeField, Tooltip("Icons to indicate what the agent is doing.")]
        protected AgentStateIndicator indicator = new AgentStateIndicator();


        bool isAiming;

        IAIContext _context;
        Collider[] _colliders = new Collider[colliderBufferCount];
        float fireWeaponAttackTime;
        float fireWeaponCoolDown;



        public AgentInput agentInput
        {
            get { return _agentInput; }
        }

        public float scanRadius
        {
            get { return _scanRadius; }
            set { _scanRadius = value; }
        }

        public float attackSpeed
        {
            get { return _attackSpeed; }
            set{
                _attackSpeed = value < minimumAttackSpeed ? minimumAttackSpeed : value;
            }
        }

        public float aimAccuracy
        {
            get { return _aimAccuracy; }
        }


        public IHasHealth attackTarget
        {
            get { 
                return _attackAtarget; 
            }
            set { 
                _attackAtarget = value;
                //  Set context focus target?
                OnAttackTargetChanged(_attackAtarget);
            }
        }

        public Collider coverTarget
        {
            get { return _coverTarget; }
            set { _coverTarget = value; }
        }






        protected override void Awake()
        {
            base.Awake();
            _agentInput = GetComponent<AgentInput>();
            _context = GetComponent<AIContextProvider>().GetContext();
        }


        protected virtual void Start()
        {
            if (_equippedFirearm == null)
            {
                if (actorBody.RightHand != null)
                    EquipGun(gm.defaultWeapon, actorBody.RightHand);
                else if (actorBody.LeftHand != null)
                    EquipGun(gm.defaultWeapon, actorBody.LeftHand);
                else
                    Debug.Log("No weapon holder");
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }



        public override void FireWeapon(Vector3 target)
        {
            if(Time.time > fireWeaponCoolDown && CanSeeTarget(target))
            {
                fireWeaponCoolDown = Time.time + _attackSpeed;
                //target = UnityEngine.Random.insideUnitSphere * aimAccuracy;
                target.y = equippedFirearm == null ? YFocusOffset : equippedFirearm.projectileSpawn.position.y; ;

                StartCoroutine(AimWeapon(2f));
                equippedFirearm.Shoot(target);
            }
        }


        IEnumerator AimWeapon(float aimDelay)
        {
            //agentInput.SetLocomotion(false);
            agentInput.StopWalking();
            float timer = aimDelay;
            float rate = 1 / aimDelay;

            while(timer > 0)
            {
                timer -= Time.deltaTime * rate;
                yield return 0;
            }
            //agentInput.SetLocomotion(true);
            agentInput.ResumeWalking();
        }



        //  Calculates if npc can see target.
        public bool CanSeeTarget(Vector3 target)
        {
            //var targetPosition = new Vector3(target.position.x, (target.position.y + transform.position.y), target.position.z);
            target.y = YFocusOffset;
            var dirToPlayer = (target - transform.position).normalized;

            var angleBetweenNpcAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (Vector3.Distance(transform.position, target) < _sightRange &&
                angleBetweenNpcAndPlayer < _fieldOfView / 2f &&
                Physics.Linecast(transform.position, target, Layers.cover) == false)
            {
                return true;
            }
            return false;
        }






        /// <summary>
        /// Called when the attack target changes.
        /// </summary>
        /// <param name="newAttackTarget">The new attack target.</param>
        public virtual void OnAttackTargetChanged(IHasHealth newAttackTarget)
        {
            var target = newAttackTarget != null ? newAttackTarget.transform : null;
            agentInput.LookAt(target);
        }

        /// <summary>
        /// Called when the attack target dies.
        /// </summary>
        public virtual void OnAttackTargetDead()
        {
            //When our target dies, stop shooting
            this.attackTarget = null;
            //_playerShooting.shooting = false;
        }


		public override void Death()
		{
            base.Death();
            gameObject.GetComponent<TaskNetworkComponent>().enabled = false;
		}

	}
}


