using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CharacterController.AI
{
    public class NavMeshAgentBridge : MonoBehaviour
    {
        [Header("--  NavMeshAgent Parameters --")]
        [SerializeField]
        private AnimationCurve m_ArriveRampDownCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1), new Keyframe(1, 0.25f) });
        [SerializeField]
        private float m_positionInterpolationSpeed = 30.0f;
        [SerializeField]
        private bool m_AutoBraking;

        [Header("--  NavMeshAgent Debug --")]
        [SerializeField] protected bool m_DrawDebugLines;
        private bool m_HideNavMeshAgent;
        [SerializeField]
        private Vector3 m_InputVector;
        [SerializeField]
        private float m_fwd, m_horizontal;




        [Header("--  NavMeshAgent Properties --")]
        private bool updatePosition = false;
        private bool updateRotation = true;



        private Vector3 m_lookDirection;
        private Quaternion m_LookRotation;

        private Vector3 m_targetPosition;
        private Vector3 m_interpolatedPosition;
        private Vector3 m_lastPosition;



        [SerializeField]
        Vector3 desiredVelocity, steeringTarget;
        [SerializeField]
        float velocitySpeed;
        [SerializeField]
        float rotationDifference;


        private AgentController m_Agent;
        private AgentContext m_Context;
        private NavMeshAgent m_NavMeshAgent;
        private CharacterLocomotion m_Controller;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_deltaTime;


        public NavMeshAgent NavAgent{
            get { return m_NavMeshAgent; }
        }

        public NavMeshPath NavPath{
            get { return m_NavMeshAgent.path; }
        }



        private void Awake()
        {
            m_Agent = GetComponent<AgentController>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            if (m_NavMeshAgent == null) m_NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            m_Controller = GetComponent<CharacterLocomotion>();
            //  Get reference to the context.
            m_Context = m_Agent.Context;
            m_GameObject = gameObject;
            m_Transform = transform;

            m_deltaTime = Time.deltaTime;
        }


        private void Start()
        {
            //  Get reference to the context.
            m_Context = m_Agent.Context;
            //  Setup NavMeshAgent.
            m_NavMeshAgent.speed = 1;
            m_NavMeshAgent.stoppingDistance = m_ArriveRampDownCurve.keys[0].value;
            m_NavMeshAgent.autoBraking = m_AutoBraking;
            // Give the unit a random avoidance priority so as to ensure that units will actually avoid each other (since same priority units will not try to avoid each other)
            m_NavMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);
            m_NavMeshAgent.updatePosition = updatePosition;
            m_NavMeshAgent.updateRotation = updateRotation;
        }


        private void OnEnable()
        {
            m_NavMeshAgent.enabled = true;
        }


        private void OnDisable()
        {
            m_NavMeshAgent.enabled = false;
        }



        [SerializeField]
        private float arriveRamp;
        [SerializeField]
        private float m_distance;
        private Vector3[] m_corners;


		private void FixedUpdate()
		{
            if (m_NavMeshAgent.hasPath)
            {
                desiredVelocity = m_NavMeshAgent.desiredVelocity;
                velocitySpeed = m_NavMeshAgent.desiredVelocity.sqrMagnitude;
                steeringTarget = m_NavMeshAgent.steeringTarget;



                m_lookDirection = m_NavMeshAgent.steeringTarget - m_Transform.position;
                m_lookDirection.y = 0;

                if (m_lookDirection != Vector3.zero){
                    m_LookRotation = Quaternion.LookRotation(m_lookDirection, m_Transform.up);
                } else {
                    m_LookRotation = m_Transform.rotation;
                }

                //Debug.DrawRay(m_Transform.position + (Vector3.up * 0.1f), m_lookDirection, Color.magenta);


                m_fwd = m_NavMeshAgent.desiredVelocity.sqrMagnitude;
                m_fwd = (float)Math.Round(Mathf.Clamp(m_fwd, -1, 1), 4);
                if (Mathf.Abs(m_fwd) < 0.1f) m_fwd = 0;
                if (Mathf.Abs(m_fwd) > 0.9f) m_fwd = 1;

                rotationDifference = m_LookRotation.eulerAngles.y - m_Transform.eulerAngles.y;
                if(rotationDifference < 5 || rotationDifference > -5 || m_NavMeshAgent.desiredVelocity == Vector3.zero){
                    m_horizontal = 0;
                } else {
                    m_horizontal = (float)Math.Round(Mathf.Clamp(rotationDifference, -1, 1), 4);
                }

            }
            else{
                m_fwd = 0f;
                m_horizontal = 0;
            }


            if(!m_NavMeshAgent.updatePosition){
                m_interpolatedPosition = Vector3.Lerp(m_Transform.position, m_targetPosition, m_deltaTime * m_positionInterpolationSpeed);
                m_NavMeshAgent.Move(m_interpolatedPosition - m_Transform.position);
                m_NavMeshAgent.nextPosition = m_interpolatedPosition;
            }


            ////  Check for edges.
            //NavMeshHit navMeshEdgeHit;
            //if (m_NavMeshAgent.FindClosestEdge(out navMeshEdgeHit)){
            //    if(navMeshEdgeHit.distance < m_NavMeshAgent.stoppingDistance && m_NavMeshAgent.remainingDistance > m_NavMeshAgent.stoppingDistance){
            //        Debug.Log("Closest edge distance." + navMeshEdgeHit.distance);
            //        //m_NavMeshAgent.CalculatePath(m_destination, new NavMeshPath());
            //    }
            //}



            m_InputVector.Set(m_horizontal, m_NavMeshAgent.desiredVelocity.y, m_fwd);
            m_Controller.InputVector = m_InputVector;


            m_targetPosition = m_Transform.position;
            //  Update waypoits.
            m_Context.WayPoints = m_NavMeshAgent.path.corners;
		}




        public void SetDestination(Vector3 destination, float maxDist = 2f, int areaMask = NavMesh.AllAreas) // maxDist is Sample within this distance from sourcePosition.
        {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(destination, out navMeshHit, maxDist, areaMask))
            {
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(navMeshHit.position);

                m_Context.WayPoints = m_NavMeshAgent.path.corners;
            }
        }



        public void StopMoving()
        {
            m_NavMeshAgent.velocity = Vector3.zero;
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.ResetPath();
        }


        public float GetDistanceRemaining()
        {
            m_distance = 0.0f;
            m_corners = m_NavMeshAgent.path.corners;
            for (int c = 0; c < m_corners.Length - 1; c++){
                m_distance += Mathf.Abs((m_corners[c] - m_corners[c + 1]).magnitude);
            }
            return m_distance;
        }


        public bool HasReachedDestination()
        {
            return GetDistanceRemaining() <= m_NavMeshAgent.stoppingDistance;
        }






        private void OnDrawGizmos()
        {
            //if (Application.isPlaying && m_NavMeshAgent != null)
            //{
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawSphere(m_NavMeshAgent.steeringTarget, .5f);

            //}

            //if (m_DrawDebugLines && Application.isPlaying)
            //{
                //if (m_NavMeshAgent != null)
                //{
                //    if (m_NavMeshAgent.path != null)
                //    {
                //        Vector3[] corners = m_NavMeshAgent.path.corners;
                //        for (int c = 0; c < corners.Length - 1; c++)
                //        {

                //            var corner1 = corners[c];
                //            corner1.y += 0.05f;
                //            var corner2 = corners[c + 1];
                //            corner2.y += 0.05f;
                //            Gizmos.color = Color.green;
                //            Gizmos.DrawLine(corner1, corner2);
                //            if (c + 1 <= corners.Length)
                //            {
                //                Gizmos.DrawSphere(corners[c + 1], 0.5f);
                //            }
                //        }

                //        //DrawGizmoString(string.Format("Distance: {0}", m_distance), corners[corners.Length-1], Color.green);
                //    }
                //    //else
                //    //{
                //    //    Gizmos.color = Color.gray;
                //    //    Gizmos.DrawSphere(m_Context.destination, 0.5f);
                //    //}


                //}
            //}
        }





	}
}

