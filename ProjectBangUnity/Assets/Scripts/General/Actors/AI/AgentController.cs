namespace Bang
{
    using UnityEngine;
    using System.Collections;

    public class AgentController : ActorController
    {
        protected readonly static float minimumAttackSpeed = 0.25f;
        protected readonly static Collider[] colliders = new Collider[50];

        private AgentContext context;
        private NavMeshAgentBridge navMeshAgent;

        public LayerMask targetLayerMask;               //  Specifies the layers that the targets are in
        public LayerMask ignoreLayerMask;               //  Specifies any layers that the sight check should ignore
        public AgentStats stats;

        private float currentSpeed;
        [SerializeField]
        private float lastShotFired;
        [SerializeField]
        private float shootingCooldown;
        [SerializeField]
        private float lastDamageTaken;
        [SerializeField]
        private float damageTakenCooldown;

        private float checkRateTimer;           //  Timer for when to check scanning.
        private int missCount;              //  Cache how many shots agent has missed.

        [Header("Agent States")]
        [SerializeField]
        private bool canShoot;
        [SerializeField]
        private bool isUnderFire;
        [SerializeField]
        private bool isReloading;
        [SerializeField]
        private bool isMovingToCover;
        [SerializeField]
        private bool isInCover;


        public bool CanShoot{
            get { return canShoot; }
        }

        public bool IsUnderFire{
            get { return isUnderFire; }
        }

        public bool IsReloading{
            get { return isReloading;}
            set{
                canShoot = !value;
                isReloading = value;
            }
        }

        public bool IsInCover{
            get { return isInCover; }
        }


        public bool IsMovingToCover{
            get { return isMovingToCover; }
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



		private void Update()
		{
            delta = Time.deltaTime;

            checkRateTimer += delta;
            if(checkRateTimer > stats.checkRate)
            {
                checkRateTimer = 0;
                //  Handle scanning or perception stuff.
            }


            if(shootingCooldown > 0){
                shootingCooldown -= delta;
                if(shootingCooldown <= 0){
                    canShoot = true;
                }
            }

            if(damageTakenCooldown > 0){
                damageTakenCooldown -= delta;
                if(damageTakenCooldown <= 0){
                    isUnderFire = false;
                }
            }
		}



        private void OnTargetMiss()
        {
            missCount++;
            if(missCount > Random.Range(1, 5)){
                missCount = 0;
                //  Handle something.  Set a bool ito be true.
            }
        }



        public void MoveTo(Vector3 destination)
        {
            context.destination = destination;
            navMeshAgent.SetDestination(destination);

            if(isInCover){
                ExitCover();
            }

            AnimHandler.WalkingState(true);
        }


        public void StopMoving()
        {
            context.destination = transform.position;
            navMeshAgent.StopMoving();

            AnimHandler.WalkingState(false);
        }


        public void EnterCover(CoverObject cover)
        {
            Vector3 origin = transform.position + Vector3.up;
            Vector3 directionToCover = -(transform.position - cover.transform.position);
            RaycastHit hit;
            float distance = 1f;

            if(Physics.Raycast(origin, directionToCover, out hit, distance, Layers.cover))
            {
                //  We hit a box collider
                if(hit.transform.GetComponent<BoxCollider>())
                {
                    Quaternion rot = Quaternion.FromToRotation(-transform.forward, hit.normal) * transform.rotation;
                    transform.rotation = rot;
                }
            }


            isInCover = true;
            canShoot = false;
            AnimHandler.CoverState();
            Debug.Log("Agent is taking cover");
        }


        public void ExitCover()
        {
            isInCover = false;
            canShoot = true;
            AnimHandler.ExitCover();
            Debug.Log("Agent is leaving cover");
        }



        public void ShootWeapon(Vector3 target)
        {
            if(canShoot)
            {
                float myIntx = Random.Range(-stats.aimAccuracy, stats.aimAccuracy);
                float myIntz = Random.Range(-stats.aimAccuracy, stats.aimAccuracy);
                Vector3 newVector = new Vector3(target.x + myIntx, target.y, target.z + myIntz);

                //bool raycastToTarget = true;//RaycastToTarget(target);

                weapon.Shoot(target);
                shootingCooldown = stats.shootSpeed;
                lastShotFired = Time.timeSinceLevelLoad;
                canShoot = false;

                bool hitTarget = false;
                if (hitTarget)
                {
                    //  OnTargetHit.
                }
                else
                {
                    //  OnTargetMiss
                }
            }

        }


        public void Reload()
        {
            IsReloading = true;
            //  Get the amount of ammo needed to reload.
            int ammoToReload = weapon.maxAmmo - weapon.currentAmmo;
            //  Subtract that ammo amount from inventory.

            //  Add it to the weapon current ammo.
            weapon.currentAmmo += ammoToReload;

        }


        public void TakeDamage(float cooldownTime)
        {
            lastDamageTaken = Time.timeSinceLevelLoad;
            damageTakenCooldown = cooldownTime;
            isUnderFire = true;
        }






        public void ScanForEntities()
        {
            Collider[] hits = colliders;
            Physics.OverlapSphereNonAlloc(this.position, stats.scanRadius, hits, Layers.entites, QueryTriggerInteraction.Collide);
            var timestamp = Time.timeSinceLevelLoad;

            for (int i = 0; i < hits.Length; i ++)
            {
                Collider hit = hits[i];

                if (hit == null){
                    continue;
                }

                // ignore hits with self
                if (hit.gameObject == this.gameObject){
                    continue;
                }

                //var entity = hit.GetEntity();
                //if (entity == null){
                //    // hit is somehow not an entity
                //    continue;
                //}

                if (hit.CompareTag(Tags.Actor))
                {
                   context.hostiles.Add(transform.GetComponent<ActorHealth>());
                }
            }
        }



        /// <summary>
        /// Calculates if npc can see target.
        /// </summary>
        /// <returns><c>true</c>, if see target was caned, <c>false</c> otherwise.</returns>
        /// <param name="lookAtPoint">Look at point.</param>
        /// <param name="target">Target.</param>
        public bool CanSeeTarget(Vector3 lookAtPoint, Vector3 target)
        {
            var dirToPlayer = (target - lookAtPoint).normalized;
            var angleBetweenNpcAndPlayer = Vector3.Angle(lookAtPoint + Vector3.forward, dirToPlayer);

            if (Vector3.Distance(lookAtPoint, target) < stats.sightRange &&
                angleBetweenNpcAndPlayer < stats.fieldOfView / 2f &&
                Physics.Linecast(lookAtPoint, target, Layers.cover) == false)
            {
                return true;
            }
            return false;
        }


        private bool RaycastToTarget(Transform target)
        {
            Vector3 origin = transform.position;
            origin.y += 1.4f;
            Vector3 dir = target.position;
            dir.y += 1.2f;
            dir = dir - origin;

            RaycastHit hit;
            Debug.DrawRay(origin, dir * stats.scanRadius);

            if (Physics.Raycast(origin, dir, out hit, stats.scanRadius, ignoreLayerMask))
            {
                ActorHealth other = hit.transform.GetComponentInParent<ActorHealth>();
                if (other.transform == context.attackTarget)
                    return true;
            }
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



        public override void Death()
        {
            AnimHandler.Death();
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


