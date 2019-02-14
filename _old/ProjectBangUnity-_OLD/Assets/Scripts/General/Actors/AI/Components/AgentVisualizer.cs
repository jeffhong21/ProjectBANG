namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEditor;
    using System.Collections.Generic;



    public class AgentVisualizer : MonoBehaviour
    {
        public enum GizmoShape { Sphere, Cube, WiredSphere, WiredCube}

        [System.Serializable]
        public class DestinationSettings
        {
            public bool show = true;
            public float gizmoSize = 0.25f;
            public Color gizmoColor = new Color32(180, 255, 0, 255);
            public GizmoShape gizmoShape;
        }
        [System.Serializable]
        public class AttackTargetSettings
        {
            public bool show = true;
            public float gizmoSize = 0.5f;
            public Color gizmoColor = new Color32(255, 0, 20, 128);
            public GizmoShape gizmoShape;
        }
        [System.Serializable]
        public class CoverPositionSettings
        {
            public bool show = true;
            public float gizmoSize = 0.5f;
            public Color gizmoColor = new Color32(0, 180, 255, 255);
            public GizmoShape gizmoShape;
        }
        [System.Serializable]
        public class PerceptionSettings
        {
            public bool show = true;
        }



        private AgentController agent;
        private AgentContext context;
        private NavMeshAgent navAgent;

        [SerializeField]
        private bool onDrawSelected = true;
        [Space]
        public DestinationSettings destination;
        public AttackTargetSettings attackTarget;
        public CoverPositionSettings coverPosition;
        public PerceptionSettings perception;


        [SerializeField, Header("Settings")]
        private float yOffset = 0.05f;
        [SerializeField]
        private Color sightRangeColor = new Color(1, 1, 1, 0.5f);
        [SerializeField]
        private Color rangeAttackRangeColor = new Color(1, 1, 1, 0.5f);
        [SerializeField]
        private Color viewAngleColor = new Color(1, 1, 1, 0.15f);
        [SerializeField]
        private Color targetInViewRangeColor = new Color(1, 0, 0, 0.15f);





		private void Awake()
		{
            agent = GetComponent<AgentController>();
            context = GetComponent<AIContextProvider>().GetContext() as AgentContext;
            navAgent = GetComponent<NavMeshAgent>();
		}



		private void OnDrawGizmos()
		{
            if ( !onDrawSelected)
                DrawGizmos();
		}

		private void OnDrawGizmosSelected()
		{
            if (onDrawSelected)
                DrawGizmos();
		}


        private void DrawGizmos()
        {
            if(destination.show){
                DrawDestinationPath();
            }

            if (attackTarget.show){
                if(context.attackTarget != null){
                    DrawSphereGizmo(context.attackTarget.position, attackTarget.gizmoSize, attackTarget.gizmoColor);
                    DrawLine(agent.position, context.attackTarget.position, yOffset, attackTarget.gizmoColor);
                }
            }

            if (coverPosition.show)
            {
                if (context.coverPosition != Vector3.zero){
                    DrawGizmoShape(coverPosition.gizmoShape, context.coverPosition, coverPosition.gizmoSize, coverPosition.gizmoColor);
                }
            }

            if(perception.show){
                DrawViewArch();
                DrawSightRange();
            }
        }


        private void DrawDestinationPath()
        {
            if(navAgent != null){
                if (navAgent.path != null)
                {
                    Vector3[] corners = navAgent.path.corners;
                    for (int c = 0; c < corners.Length - 1; c++)
                    {
                        DrawLine(corners[c], corners[c + 1], yOffset, destination.gizmoColor);
                        if (c + 1 <= corners.Length)
                        {
                            DrawGizmoShape(destination.gizmoShape, corners[c + 1], destination.gizmoSize, destination.gizmoColor);
                        }
                    }
                }
                else
                {
                    DrawGizmoShape(destination.gizmoShape, context.destination, destination.gizmoSize * 1.5f, destination.gizmoColor);
                }
            }
        }



        private void DrawViewArch()
        {
            //float halfFOV = agent.stats.fieldOfView / 2.0f;
            //Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            //Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            //Vector3 leftRayDirection = leftRayRotation * transform.forward;
            //Vector3 rightRayDirection = rightRayRotation * transform.forward;

            //  View Arch
            Handles.color = viewAngleColor;

            //Gizmos.DrawRay(transform.position, leftRayDirection * agent.stats.sightRange);
            //Gizmos.DrawRay(transform.position, rightRayDirection * agent.stats.sightRange);

            //Handles.DrawSolidArc(agent.position + Vector3.up * yOffset, Vector3.up, rightRayDirection, -agent.stats.fieldOfView / 2, agent.stats.sightRange);
            //Handles.DrawSolidArc(agent.position + Vector3.up * yOffset, Vector3.up, leftRayDirection, agent.stats.fieldOfView / 2, agent.stats.sightRange);

            //Handles.color = new Color(1, 0, 0, 0.15f);
            Handles.DrawSolidArc(agent.position + Vector3.up * yOffset, Vector3.up, DirFromAngle(agent.rotation.y), agent.stats.fieldOfView / 2, agent.stats.sightRange);
            Handles.DrawSolidArc(agent.position + Vector3.up * yOffset, Vector3.up, DirFromAngle(agent.rotation.y), -agent.stats.fieldOfView / 2, agent.stats.sightRange);


            //Handles.DrawSolidArc(npc.transform.position + Vector3.up * offset, Vector3.up, npc.DirFromAngle(npc.transform.rotation.y), -npc.viewAngle / 2, npc.sightRange);


        }

        private void DrawSightRange()
        {
            //  Sight Range
            Handles.color = sightRangeColor;
            Handles.DrawWireArc(agent.position + Vector3.up * yOffset, Vector3.up, Vector3.forward, 360, agent.stats.sightRange);
            Handles.color = rangeAttackRangeColor;
            Handles.DrawWireArc(agent.position + Vector3.up * yOffset, Vector3.up, Vector3.forward, 360, agent.stats.sightRange);
        }



        //  Get direction from angle.
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal = false)
        {
            if (angleIsGlobal == false){
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }





        private void DrawLine(Vector3 origin, Vector3 target, float heightOffset, Color color)
        {
            origin.Set(origin.x, origin.y + heightOffset, origin.z);
            target.Set(target.x, target.y + heightOffset, target.z);

            Gizmos.color = color;
            Gizmos.DrawLine(origin, target);
        }


        private void DrawGizmoShape(GizmoShape gizmo, Vector3 position, float size, Color color)
        {
            switch (gizmo)
            {
                case GizmoShape.Cube:
                    DrawCubeGizmo(position, size, color);
                    break;
                case GizmoShape.Sphere:
                    DrawSphereGizmo(position, size, color);
                    break;
                case GizmoShape.WiredCube:
                    DrawWiredCubeGizmo(position, size, color);
                    break;
                case GizmoShape.WiredSphere:
                    DrawWiredSphereGizmo(position, size, color);
                    break;
                default:
                    break;
            }
        }


        private void DrawSphereGizmo(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, size);
        }

        private void DrawWiredSphereGizmo(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, size);
        }

        private void DrawCubeGizmo(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(position, new Vector3(size, size, size));
        }

        private void DrawWiredCubeGizmo(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(position, new Vector3(size, size, size));
        }



	}
}








