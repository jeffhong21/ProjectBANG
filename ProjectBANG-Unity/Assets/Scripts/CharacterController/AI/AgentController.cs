using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public enum MovementID { Default = 0, Injured = -1, Drunk = -2 }

    public class AgentController : MonoBehaviour, IContextProvider
    {
        public Collider[] colliders = new Collider[50];

        [Header("--  Agent Parameters --")]
        [SerializeField]
        protected float m_FieldOfView = 135;
        [SerializeField]
        protected float m_SightRange = 20;
        [SerializeField]
        protected Transform m_LookTransform;


        [Space]
        [Header("--  Agent Context --")]
        [SerializeField]
        protected AgentContext m_Context;

        [Space]
        [SerializeField]
        private bool m_isDamaged;
        [SerializeField]
        private float m_damageDuration = 3;
        [SerializeField]
        private float m_damageDurationTimer;


        protected NavMeshAgentBridge m_NavMeshAgent;
        protected CharacterLocomotion m_Controller;
        protected CharacterHealth m_Health;
        protected Inventory m_Inventory;
        protected LayerManager m_Layers;
        protected Animator m_Animator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;

        protected MovementID m_MovementID = MovementID.Default;
        protected float m_deltaTime;



        //
        //  Properties
        // 
        public Transform LookTransform { get { return m_LookTransform; } }

        public float FieldOfView { get { return m_FieldOfView; } }

        public float SightRange { get { return m_SightRange; } }

        public Vector3 Position{ get { return m_Transform.position; } }

        public AgentContext Context{ get { return m_Context; } }







        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgentBridge>();
            m_Animator = GetComponent<Animator>();
            m_Context = new AgentContext(this, m_NavMeshAgent);

            m_Controller = GetComponent<CharacterLocomotion>();
            m_Layers = GetComponent<LayerManager>();
            m_Health = GetComponent<CharacterHealth>();
            m_Inventory = GetComponent<Inventory>();
            m_GameObject = gameObject;
            m_Transform = transform;

            m_LookTransform = m_Animator.GetBoneTransform(HumanBodyBones.Head);

            m_deltaTime = Time.deltaTime;
        }


		private void Start()
		{

		}


		private void OnEnable()
		{
            EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, OnTakeDamage);
		}


        private void OnDisable()
		{
            EventHandler.UnregisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, OnTakeDamage);
		}



		protected void Update()
		{
            //if(m_isDamaged){
            //    if(Time.timeSinceLevelLoad > m_damageDurationTimer){
            //        m_isDamaged = false;
            //        m_MovementID = MovementID.Default;
            //        m_Animator.SetInteger(HashID.MovementSetID, 0);
            //    }
            //}


		}




        protected void EquipItem(){
            
        }

        protected void ShootWeapon(){
            
        }

        protected void Reload(){
            
        }


        protected void OnTakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            //var directionForce = (force - position).normalized;
            //Debug.LogFormat("{0} took {1} at position {2} with a force of {3} by attacker {4}",gameObject.name, amount, position, directionForce, attacker);
            //m_MovementID = MovementID.Injured;
            //m_Animator.SetInteger(HashID.MovementSetID, (int)m_MovementID);

            //m_isDamaged = true;
            //m_damageDurationTimer = Time.timeSinceLevelLoad + m_damageDuration;
        }





        public virtual void OnAttackTargetChanged(GameObject newAttackTarget)
        {
            m_Context.target = newAttackTarget != null ? newAttackTarget.transform : null;
            //agentInput.LookAt(target);
        }



        public bool CanSeeTarget(Vector3 lookAtPoint, Vector3 target)
        {
            target.y = 1.5f;
            Vector3 direction = target - lookAtPoint;
            RaycastHit hit;
            if (Physics.Raycast(lookAtPoint, direction, out hit, m_SightRange))
            {
                if (hit.transform.GetComponent<CharacterHealth>()){
                    return true;
                }
            }
            return false;
        }


        public bool CanInteract(){

            return false;
        }




        ////  Calculates if npc can see target.
        //public bool CanSeeTarget(Vector3 target, float sightRange, float fieldOfView)
        //{
        //    //var targetPosition = new Vector3(target.position.x, (target.position.y + transform.position.y), target.position.z);
        //    target.y = 0.75f;
        //    var dirToPlayer = (target - m_Transform.position).normalized;

        //    var angleBetweenNpcAndPlayer = Vector3.Angle(m_Transform.forward, dirToPlayer);

        //    if (Vector3.Distance(m_Transform.position, target) < sightRange &&
        //        angleBetweenNpcAndPlayer < fieldOfView / 2f &&
        //        Physics.Linecast(m_Transform.position, target, m_Layers.GroundLayer) == false)
        //    {
        //        return true;
        //    }
        //    return false;
        //}



        ///// <summary>
        ///// Set the position to look at. Set to null is the Ai should stop looking
        ///// </summary>
        ///// <param name="lookAtTarget"></param>
        //public virtual void LookAtTarget(Transform lookAtTarget)
        //{
        //    target = lookAtTarget;
        //    if (lookAtTarget == null)
        //    {
        //        navAgent.updateRotation = true;
        //    }
        //    else
        //    {
        //        navAgent.updateRotation = false;
        //    }
        //}



        //private void RotateTowardsTarget()
        //{
        //    if (target != null)
        //    {
        //        Vector3 lookRotation = (target.position - transform.position);
        //        //  Errors out when roation method is called towards a vector zero.
        //        if (lookRotation != Vector3.zero)
        //        {
        //            // Create a quaternion (rotation) based on looking down the vector from the player to the target.
        //            Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
        //            transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * agent.stats.turnSpeed);
        //        }
        //    }
        //}




        //public void _SetAgentMoveType()
        //{
        //    int[] randmMoveTable = { 50, 25, 10 };
        //    float[] speedMultiplier = { 1, 1.5f, 2f };

        //    var totalValue = 0;
        //    for (int i = 0; i < randmMoveTable.Length; i++){
        //        totalValue += randmMoveTable[i];
        //    }
        //    var randomNumber = UnityEngine.Random.Range(0, totalValue);
        //    for (int index = 0; index < randmMoveTable.Length; index++){
        //        if (randomNumber <= randmMoveTable[index])
        //            m_Controller.SpeedChangeMultiplier = speedMultiplier[index];
        //        else
        //            randomNumber -= randmMoveTable[index];
        //    }


        //}





        public IAIContext GetContext()
        {
            return m_Context as IAIContext;
        }

        public IAIContext GetContext(Guid aiId)
        {
            return m_Context as IAIContext;
        }




		private void OnDrawGizmosSelected()
		{
            m_Context.DrawGizmos();
		}


	}
}

