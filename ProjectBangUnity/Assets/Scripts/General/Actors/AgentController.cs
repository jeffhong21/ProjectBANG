namespace Bang
{
    using UnityEngine;
    using System.Collections;

    public class AgentController : ActorController
    {
        readonly static float minimumAttackSpeed = 0.25f;

        //
        //  Fields
        //
        [Header("Agent Parameters")]
        [Range(0, 360)]
        public float fieldOfView = 135f;
        public float sightRange = 20;
        public float scanRadius = 15;
        public float attackSpeed = 1f;
        public float aimSpeed = 1.25f;
        [Range(0, 3)]
        public float aimAccuracy = 3f;


        protected AgentContext context;
        protected NavMeshAgentBridge navMeshAgent;
        protected IEnumerator attackRoutine;
        private float attackCooldown;




        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();

            context = GetComponent<AIContextProvider>().GetContext() as AgentContext;
            navMeshAgent = GetComponent<NavMeshAgentBridge>();
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            if(attackRoutine != null){
                StopCoroutine(attackRoutine);
            }
        }


        public void MoveTo(Vector3 destination)
        {
            context.destination = destination;
            navMeshAgent.SetDestination(destination);
        }


        public void StopMoving()
        {
            context.destination = this.transform.position;
            navMeshAgent.StopMoving();
        }


        /// <summary>
        /// Fires the weapon.
        /// </summary>
        /// <param name="target">Target.</param>
        public override void FireWeapon(Vector3 target)
        {
            if(Time.time > attackCooldown)
            {
                attackCooldown = Time.time + attackSpeed;
                attackRoutine = AimWeapon(target, aimSpeed);
                StartCoroutine(attackRoutine);
            }
        }


        /// <summary>
        /// Aims the weapon.
        /// </summary>
        /// <returns>The weapon.</returns>
        /// <param name="target">Target.</param>
        /// <param name="aimDelay">Aim delay.</param>
        private IEnumerator AimWeapon(Vector3 target, float aimDelay)
        {
            Vector3 lookAtPoint = transform.position;
            lookAtPoint.y = 1f;
            bool canSeeTarget = false;
            float timer = aimDelay;
            float rate = 1 / aimDelay;

            while(timer > 0)
            {
                if (CanSeeTarget(lookAtPoint, target))
                {
                    timer -= Time.deltaTime * rate;
                    canSeeTarget = true;
                    yield return 0;
                }
                else
                {
                    canSeeTarget = false;
                    yield break;
                }
            }

            if (canSeeTarget)
            {
                float myIntx = Random.Range(-aimAccuracy, aimAccuracy);
                float myIntz = Random.Range(-aimAccuracy, aimAccuracy);
                Vector3 newVector = new Vector3(target.x + myIntx, target.y, target.z + myIntz);

                weapon.Shoot(target);
            }

            yield return null;

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

            if (Vector3.Distance(lookAtPoint, target) < sightRange &&
                angleBetweenNpcAndPlayer < fieldOfView / 2f &&
                Physics.Linecast(lookAtPoint, target, Layers.cover) == false)
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
            navMeshAgent.LookAt(target);
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







        public override void DisableControls()
        {
            GetComponent<UtilityAI.TaskNetworkComponent>().enabled = false;
            //throw new System.NotImplementedException();
        }

        public override void EnableControls()
        {
            GetComponent<UtilityAI.TaskNetworkComponent>().enabled = true;
            //throw new System.NotImplementedException();
        }











    }
}


