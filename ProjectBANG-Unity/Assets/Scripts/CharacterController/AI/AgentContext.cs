using System;
using System.Collections.Generic;
using UnityEngine;
using uUtilityAI;

namespace CharacterController.AI
{
    [Serializable]
    public class AgentContext : IAIContext
    {
        [HideInInspector]
        public AgentController agent;
        [HideInInspector]
        public NavMeshAgentBridge navMeshAgent;

        public Transform target;

        public Vector3 destination;

        public bool hasDestination;

        public Vector3[] wayPoints = new Vector3[0];

        public List<Vector3> sampledPositions = new List<Vector3>();

        public List<GameObject> hostiles = new List<GameObject>();
        [HideInInspector]
        public LayerMask hostilesLayer;




        public Vector3[] WayPoints
        {
            get { return wayPoints; }
            set{
                wayPoints = value;
                if (wayPoints.Length == 0) hasDestination = false;
            }
        }




        public AgentContext()
        {

        }

        public AgentContext(AgentController a, NavMeshAgentBridge bridge)
        {
            agent = a;
            navMeshAgent = bridge;
            hostilesLayer = agent.GetComponent<LayerManager>().EnemyLayer;
        }












        #region Debug


        Color waypointColor = new Color(0, 1, 0, 0.5f);
        Color destinationColor = new Color(1, 0.92f, 0.016f, 0.5f);
        public void DrawGizmos()
        {
            if (Application.isPlaying)
            {
                DrawWayPointGizmo(0.5f);
            }
        }

        private void DrawWayPointGizmo(float radius)
        {
            if (wayPoints.Length > 0)
            {
                for (int i = 0; i < wayPoints.Length - 1; i++)
                {
                    var waypoint1 = wayPoints[i];
                    waypoint1.y += 0.05f;
                    var waypoint2 = wayPoints[i + 1];
                    waypoint2.y += 0.05f;
                    Gizmos.color = waypointColor;
                    Gizmos.DrawLine(waypoint1, waypoint2);
                    if (i + 1 <= wayPoints.Length)
                    {
                        if (i + 2 == wayPoints.Length && hasDestination == true)
                        {
                            Gizmos.color = destination != wayPoints[i + 1] ? Color.red : destinationColor;
                            Gizmos.DrawSphere(destination, radius);
                            GizmosUtils.DrawString(string.Format("Destination ({0})", destination.ToString()), destination + Vector3.up * 1, destination != wayPoints[i + 1] ? Color.red : Color.yellow);
                        }
                        else
                        {
                            Gizmos.color = waypointColor;
                            Gizmos.DrawSphere(wayPoints[i + 1], radius);
                            GizmosUtils.DrawString(string.Format("Waypoint({0}): {1}", i + 1, wayPoints[i + 1].ToString()), wayPoints[i + 1] + Vector3.up * 1, Color.white);
                        }
                    }
                }
            }


        }

        #endregion



    }
}

