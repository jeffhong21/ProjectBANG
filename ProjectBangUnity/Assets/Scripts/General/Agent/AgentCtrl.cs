namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using System;
    using System.Collections;

    using UtilityAI;

    using Random = UnityEngine.Random;

    /// <summary>
    /// Agent Controller has methods that the agent can perform.
    /// </summary>
    public class AgentCtrl : ActorCtrl //, IAgentCtrl //, IHasFirearm
    {
        readonly static float minimumAttackSpeed = 0.25f;
        //readonly static int colliderBufferCount = 15;

        protected AgentInput _agentInput;

        [Header("----- Agent Stats -----")]
        [SerializeField, Range(0, 360)]
        protected float _fieldOfView = 135f;

        [SerializeField]
        protected float _sightRange = 20;

        [SerializeField]
        protected float _scanRadius = 15;

        [SerializeField]
        protected float _attackSpeed = 1f;

        [SerializeField]
        protected float _aimSpeed = 1.25f;

        [SerializeField, Range(0,3)]
        protected float _aimAccuracy = 3f;




        bool _canAttack;
        bool _isAiming;
        bool _hasTargets;
        bool _moveToCover;


        //Collider[] colliders = new Collider[colliderBufferCount];
        float fireWeaponAttackTime, fireWeaponCoolDown;
        AgentContext _context;
        IEnumerator fireWeaponCoroutine;


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

        public float aimSpeed
        {
            get { return _aimSpeed; }
        }

        public float aimAccuracy
        {
            get { return _aimAccuracy; }
        }

        public AgentContext context
        {
            get { return _context; }
        }


        public bool isAiming
        {
            get { return _isAiming; }
        }






        protected override void Awake()
        {
            base.Awake();
            _agentInput = GetComponent<AgentInput>();
            _context = GetComponent<AIContextProvider>().GetContext() as AgentContext;

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
            agentInput.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            agentInput.enabled = false;

            //CoroutineHelper.Stop(fireWeaponCoroutine);
            if (fireWeaponCoroutine != null)
                StopCoroutine(fireWeaponCoroutine);
            
        }



        public void MoveTo(Vector3 destination)
        {
            context.destination = destination;
            agentInput.MoveTo(destination);
        }


        public void StopMoving()
        {
            context.destination = this.position;
            agentInput.StopMoving();
        }


        public void FireWeapon(Transform target)
        {
            if (Time.time > fireWeaponCoolDown && CanSeeTarget(target.position))
            {
                fireWeaponCoolDown = Time.time + _attackSpeed;

                fireWeaponCoroutine = FireWeapon(target, aimSpeed);

                StartCoroutine(fireWeaponCoroutine);
                //CoroutineHelper.Start(fireWeaponCoroutine);
            }
        }


        private IEnumerator FireWeapon(Transform target, float aimDelay)
        {
            _isAiming = true;

            agentInput.StopMoving();

            float timer = aimDelay;
            float rate = 1 / aimDelay;

            while (timer > 0)
            {
                timer -= Time.deltaTime * rate;
                yield return 0;
            }

            if (CanSeeTarget(target.position))
            {
                var targetAimAdjusted = VectorSpread(target.position, aimAccuracy);
                equippedFirearm.Shoot(targetAimAdjusted);
            }

            agentInput.ResumeWalking();
            _isAiming = false;
            yield return null;
        }


        int missCount;
        public virtual void OnTargetMiss()
        {
            missCount++;
            if(missCount > Random.Range(2, 5)){
                missCount = 0;
                _moveToCover = true;
            }
        }


        private bool RaycastToTarget(Vector3 origin, Vector3 target, float targetHitlocation = 0.5f, bool debug = true)
        {
            bool hitTarget = false;
            //Vector3 origin = transform.position;
            origin.y = actorBody.Head.position.y;
            target.y = targetHitlocation;
            Vector3 direction = target - origin;

            RaycastHit hit;
            if(Physics.Raycast(origin, direction, out hit, scanRadius))  //  Maybe need a layers to ignore.
            {
                if(hit.transform.GetComponent<IHasHealth>() != null)
                {
                    hitTarget = true;
                    if (debug) Debug.DrawRay(origin, direction * scanRadius, Color.green);
                }
            }
            else{
                if (debug) Debug.DrawRay(origin, direction * scanRadius, Color.red);
            }

            return hitTarget;
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
            context.attackTarget = null;
            context.lastTargetPosition = default(Vector3);
            //_playerShooting.shooting = false;
        }


		public override void Death()
		{
            base.Death();
            this.enabled = false;
            gameObject.GetComponent<TaskNetworkComponent>().enabled = false;
		}






        //Takes a vector and returns a new vector that is slightly variated in direction
        //0 = 100% accurate and the larger the number, the less accurate
        public static Vector3 VectorSpread(Vector3 target, float accuracy)
        {
            float myIntx = Random.Range(-accuracy, accuracy);
            //float myInty = (float)Random.Range(-accuracy, accuracy) / 1000;
            float myIntz = Random.Range(-accuracy, accuracy);
            Vector3 newVector = new Vector3(target.x + myIntx, target.y, target.z + myIntz);

            return newVector;
        }


        //  Calculates if npc can see target.
        public bool CanSeeTarget(Vector3 target)
        {
            //var targetPosition = new Vector3(target.position.x, (target.position.y + transform.position.y), target.position.z);
            //target.y = YFocusOffset;
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

	}
}


