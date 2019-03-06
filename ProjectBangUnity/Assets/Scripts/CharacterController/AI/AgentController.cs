using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{



    public class AgentController : MonoBehaviour, IContextProvider
    {
        //[Header("--  Agent Parameters --")]

        public enum MovementID { Default, Injured = -1, Drunk = -2}


        [Header("--  Agent Context --")]
        [SerializeField]
        protected AgentContext m_Context;

        protected NavMeshAgentBridge m_NavMeshAgent;
        protected CharacterLocomotion m_Controller;
        protected CharacterHealth m_Health;
        protected Animator m_Animator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;

        protected MovementID m_MovementID = MovementID.Default;
        protected float m_deltaTime;



        public Vector3 Position{
            get { return m_Transform.position; }
        }

        public AgentContext Context{
            get { return m_Context; }
        }







        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgentBridge>();
            m_Animator = GetComponent<Animator>();
            m_Context = new AgentContext(this, m_NavMeshAgent);
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            m_Transform = transform;



            m_deltaTime = Time.deltaTime;
        }


		private void Start()
		{
            //var movementSetID = DetermineMovementSetID(75);
            //m_Animator.SetInteger(HashID.MovementSetID, movementSetID);
		}


		private void OnEnable()
		{
            m_Controller.Running = DetermineAgentMoveType(75);

            EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, OnTakeDamage);
		}


        private void OnDisable()
		{
            EventHandler.UnregisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, OnTakeDamage);
		}


		protected void Update()
		{

		}





        protected void OnTakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            //var directionForce = (force - position).normalized;
            //Debug.LogFormat("{0} took {1} at position {2} with a force of {3} by attacker {4}",gameObject.name, amount, position, directionForce, attacker);
            m_MovementID = MovementID.Injured;
            m_Animator.SetInteger(HashID.MovementSetID, (int)m_MovementID);
        }















        public void _SetAgentMoveType()
        {
            m_Controller.Running = DetermineAgentMoveType(75);
        }


        private int DetermineMovementSetID(int threshhold)
        {
            var movementSetID = 0;
            var percentage = UnityEngine.Random.Range(0, 101);
            if (percentage > threshhold){
                if (UnityEngine.Random.Range(0, 101) > 50)
                    movementSetID = -1;
                else
                    movementSetID = -2;
            } 
            return movementSetID;
        } 

        private bool DetermineAgentMoveType(int threshhold){
            var randomValue = UnityEngine.Random.Range(0, 100);
            if (randomValue > threshhold){
                return true;
            }
            return false;
        } 






        public IAIContext GetContext()
        {
            return m_Context as IAIContext;
        }

        public IAIContext GetContext(Guid aiId)
        {
            return m_Context as IAIContext;
        }

    
    
    
    }
}

