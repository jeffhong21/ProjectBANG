using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CharacterController.AI
{
    public class AgentController : MonoBehaviour
    {
        [Header("--  Agent Parameters --")]
        [SerializeField]
        protected float m_CheckRate = 1f;
        [SerializeField]
        protected float m_WanderRange = 20;


        [Header("--  Agent Context --")]
        [SerializeField]
        protected AgentContext m_Context;
        protected NavMeshAgentBridge m_NavMeshAgent;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform m_Transform;

        [Header("-- Debug Settings --")]

        [SerializeField] protected MeshRenderer m_stateIndicator;
        [SerializeField] private Color walkState = new Color32(255, 255, 0, 255);
        [SerializeField] private Color runningState = new Color32(255, 255, 128, 255);
        [SerializeField] private Color idleState = new Color32(255, 0, 20, 255);

        protected float m_deltaTime;
        protected float m_Time;
        protected float m_nextUpdate;


        public AgentContext Context{
            get { return m_Context; }
        }



        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgentBridge>();
            m_Context = new AgentContext(this, m_NavMeshAgent);
            m_Controller = GetComponent<CharacterLocomotion>();
            m_GameObject = gameObject;
            m_Transform = transform;


            m_deltaTime = Time.deltaTime;
            m_Time = Time.time;
        }

		private void Start()
		{

		}

		private void OnEnable()
		{
            m_NavMeshAgent.enabled = true;
		}

        private void OnDisable()
		{
            m_NavMeshAgent.enabled = false;
		}


		protected void Update()
		{
            m_Time = Time.time;
            if(m_Time > m_nextUpdate)
            {
                m_nextUpdate = m_Time + m_CheckRate;
                //  Agent is moving to destination.
                if(m_Context.hasDestination)
                {
                    if (m_NavMeshAgent.HasReachedDestination())
                    {
                        m_NavMeshAgent.StopMoving();
                        m_Context.hasDestination = false;
                        UpdateStateIndicator(idleState);
                        m_nextUpdate = m_Time + m_CheckRate + Random.Range(0f, 2f);
                    }

                }
                else if (m_Context.hasDestination == false){
                    if (RandomWanderTarget(m_WanderRange, out m_Context.destination))
                    {
                        m_NavMeshAgent.SetDestination(m_Context.destination);
                        m_Context.hasDestination = true;


                        m_Controller.Running = DetermineAgentMoveType(50);
                    }
                }

                //Debug.LogFormat("{0} | Position: {1}", Time.time, m_Transform.position);
            }


		}





        private bool RandomWanderTarget(float range, out Vector3 result)
        {
            NavMeshHit navHit;
            Vector3 randomPoint = m_Transform.position + (Random.onUnitSphere.normalized * range);
            randomPoint.y = 0;

            if (NavMesh.SamplePosition(randomPoint, out navHit, 5f, NavMesh.AllAreas))
            {
                result = navHit.position;
                return true;
            }
            result = m_Transform.position;
            return false;
        }




        public void UpdateStateIndicator(Color _color)
        {
            if (m_stateIndicator == null) return;
            m_stateIndicator.sharedMaterial.color = _color;
        }


        private bool DetermineAgentMoveType(int threshhold){
            var randomValue = Random.Range(0, 100);
            if (randomValue > threshhold){
                UpdateStateIndicator(runningState);
                return true;
            }

            UpdateStateIndicator(walkState);
            return false;
        } 



		//private void OnDrawGizmosSelected()
		//{
  //          if(m_DrawDebugLines && Application.isPlaying)
  //          {
  //              if (m_NavMeshAgent != null)
  //              {
  //                  if (m_NavMeshAgent.NavPath != null)
  //                  {
  //                      Vector3[] corners = m_NavMeshAgent.NavPath.corners;
  //                      for (int c = 0; c < corners.Length - 1; c++)
  //                      {

  //                          var corner1 = corners[c];
  //                          corner1.y += 0.05f;
  //                          var corner2 = corners[c + 1];
  //                          corner2.y += 0.05f;
  //                          Gizmos.color = Color.green;
  //                          Gizmos.DrawLine(corner1, corner2);
  //                          if (c + 1 <= corners.Length)
  //                          {
  //                              Gizmos.DrawSphere(corners[c + 1], 0.5f);
  //                          }
  //                      }
  //                  }
  //                  else
  //                  {
  //                      Gizmos.color = Color.gray;
  //                      Gizmos.DrawSphere(m_Context.destination, 0.5f);
  //                  }


  //              }
  //          }
		//}
	
    
    
    
    
    
    }
}

