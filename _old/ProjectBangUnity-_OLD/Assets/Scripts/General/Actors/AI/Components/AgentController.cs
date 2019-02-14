namespace Bang
{
    using UnityEngine;
    using System.Collections;

    public class AgentController : ActorController
    {
        protected readonly static Collider[] colliders = new Collider[50];

        private AgentContext context;
        private NavMeshAgentBridge navMeshAgent;

        public AgentStats stats;


        private float currentSpeed;
        [SerializeField]
        private float lastShotFired;
        [SerializeField]
        private float lastDamageTaken;
        [SerializeField]
        private float damageTakenCooldown;

        private float checkRateTimer;           //  Timer for when to check scanning.



        [Header("Agent States")]
        [SerializeField]
        private bool isUnderFire;
        [SerializeField]
        private bool isSearching;


        public bool IsUnderFire{
            get { return isUnderFire; }
        }

        public bool IsSearching{
            get { return isSearching; }
            set { isSearching = value; }
        }

        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();

            context = GetComponent<AIContextProvider>().GetContext() as AgentContext;
            navMeshAgent = GetComponent<NavMeshAgentBridge>();

            if (context == null){ 
                Debug.Log("***** No context *****");
            }
        }


        private void Start()
        {
            currentSpeed = stats.walkSpeed;
            lastShotFired = Time.timeSinceLevelLoad;
        }


        protected override void ExecuteUpdate(float deltaTime)
        {
            checkRateTimer += deltaTime;
            if (checkRateTimer > stats.checkRate)
            {
                checkRateTimer = 0;
                //  Handle scanning or perception stuff.
            }

            weapon.transform.LookAt(AimPosition);


            //  Set damage taken cooldown timer.
            HandleDamageTaken(deltaTime);

            //  Set aiming position.
            HandleAimingPosition(context.attackTarget, stats.sightRange);
        }

        protected override void ExecuteFixedUpdate(float deltaTime)
        {
            
        }


        private void HandleAimingPosition(ActorHealth target, float distance)
        {
            if(target != null){
                AimPosition = target.position;
            }
            else{
                AimPosition = transform.position + transform.forward * distance;
            }
        }



        private void HandleDamageTaken(float time)
        {
            if(damageTakenCooldown > 0){
                damageTakenCooldown -= time;
                if (damageTakenCooldown <= 0){
                    isUnderFire = false;
                }
            }
        }




        protected override void OnEquipWeapon(ShootableWeapon weapon)
        {

        }

        protected override void OnShootWeapon()
        {
            lastShotFired = Time.timeSinceLevelLoad;
        }


        protected override void OnReload()
        {

        }


        protected override void OnTakeDamage(Vector3 hitDirection)
        {
            lastDamageTaken = Time.timeSinceLevelLoad;
            //damageTakenCooldown += cooldownTime;
            isUnderFire = true;

        }


        public void MoveTo(Vector3 destination)
        {
            context.destination = destination;
            navMeshAgent.SetDestination(destination);


            AnimHandler.WalkingState(true);
        }


        public void StopMoving()
        {
            context.destination = transform.position;
            navMeshAgent.StopMoving();

            AnimHandler.WalkingState(false);
        }



        public bool CanSeeTarget(Vector3 lookAtPoint, Vector3 target, bool debug = false)
        {
            target.y = 1.5f;
            Vector3 direction = target - lookAtPoint;
            RaycastHit hit;

            if (Physics.Raycast(lookAtPoint, direction, out hit, stats.scanRadius)){
                if (hit.transform.GetComponent<ActorHealth>()){
                    if (debug) Debug.DrawRay(lookAtPoint, direction, Color.green, 0.25f);
                    return true;
                }
            }

            if (debug) Debug.DrawRay(lookAtPoint, direction, Color.red, 0.25f);
            return false;
        }








        /// <summary>
        /// Called when the attack target changes.
        /// </summary>
        /// <param name="newAttackTarget">The new attack target.</param>
        public virtual void OnAttackTargetChanged(ActorHealth newAttackTarget)
        {
            var target = newAttackTarget != null ? newAttackTarget.transform : null;
            navMeshAgent.LookAtTarget(target);
        }


        /// <summary>
        /// Called when the attack target dies.
        /// </summary>
        public virtual void OnAttackTargetDead()
        {
            //When our target dies, stop shooting
            context.attackTarget = null;
            //context.lastTargetPosition = default(Vector3);
            //_playerShooting.shooting = false;
        }



        protected override void OnDeath()
        {
            navMeshAgent.enabled = false;
            GetComponent<AtlasAI.UtilityAIComponent>().enabled = false;
        }


        public override void DisableControls()
        {
            navMeshAgent.enabled = false;
            GetComponent<AtlasAI.UtilityAIComponent>().Pause();
        }

        public override void EnableControls()
        {
            navMeshAgent.enabled = true;
            GetComponent<AtlasAI.UtilityAIComponent>().Resume();
        }

















        public void TestHandleCover()
        {
            Collider col = FindClosestCover();

            if (col == null) return;

            Vector3 dirToTarget = context.attackTarget.transform.position - col.transform.position;
            dirToTarget.Normalize();

            Vector3 targetPosition = col.transform.position + (dirToTarget * -1);
            //  Move to cover.
        }


        public Collider FindClosestCover()
        {
            Collider[] coverColliders = Physics.OverlapSphere(transform.position, stats.scanRadius, Layers.cover);
            Debug.Log(string.Format("* FindClosestCover() - Collider count: {0}", coverColliders.Length));
            float mDist = float.MaxValue;
            Collider closest = null;

            for (int i = 0; i < coverColliders.Length; i++)
            {
                float tDist = Vector3.Distance(coverColliders[i].transform.position, transform.position);

                if (tDist < mDist)
                {
                    mDist = tDist;
                    closest = coverColliders[i];
                }
            }
            return closest;
        }

        private void OnCoverREach()
        {
            
        }




    }
}


