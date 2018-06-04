namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    using UtilityAI;

    public class AgentCtrl : ActorCtrl, IAgentCtrl,IHasFirearm
    {
        [Header("----- Agent Stas -----")]
        [SerializeField, Tooltip("Agents accuracy range.")]
        protected float _fireSpeed = 1f;
        [SerializeField, Tooltip("Agents accuracy range.")]
        protected float _aimAccuracy = 3f;

        AgentInput _agentInput;
        IHasHealth _attackAtarget;
        IAIContext _context;

        float fireWeaponAttackTime;
        float fireWeaponCoolDown;

        public AgentInput agentInput
        {
            get { return _agentInput; }
        }


        public float aimAccuracy{
            get { return _aimAccuracy; }
            set { _aimAccuracy = value; }
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
            if(Time.time > fireWeaponCoolDown)
            {
                fireWeaponCoolDown = Time.time + _fireSpeed;

                //target = UnityEngine.Random.insideUnitSphere * aimAccuracy;
                target.y = equippedFirearm == null ? YFocusOffset : equippedFirearm.projectileSpawn.position.y; ;



                //Debug.LogFormat("Taget Vector:  {0} | AimTarget Vector: {1}", target, aimTarget);
                equippedFirearm.Shoot(target);
            }

        }



        public virtual void MoveTo(Vector3 destination)
        {
            agentInput.MoveTo(destination);
        }


        public virtual void StopMoving()
        {
            agentInput.StopWalking();
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


