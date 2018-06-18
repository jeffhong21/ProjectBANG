namespace Bang
{
    using UnityEngine;
    using UnityEditor;

    using UtilityAI;
    using UtilityAI.Visualization;

    public class FocusTargetVisualizer : ContextGizmoGUIVisualizerComponent
    {
        [SerializeField]
        float locationRadius = 0.5f;
        [SerializeField]
        Color lineColor = new Color32(180, 255, 0, 255);
        [SerializeField]
        Color focusTargetColor = new Color32(0, 180, 255, 128);
        [SerializeField]
        Color accuracyRangeColor = new Color32(0, 180, 255, 64);
        [SerializeField]
        float yOffset = 0.05f;
        //[SerializeField]
        //Camera debugCam;

        //float drawRange = 2;        //  How far away from the destination does agent need to be before Gizmo is drawn.

        protected override void DrawGUI(IAIContext context)
        {

        }


        protected override void DrawGizmos(IAIContext context)
        {
            AgentContext c = context as AgentContext;
            var agent = c.agent;


            if(c.attackTarget != null)
            {
                Handles.color = lineColor;
                Handles.DrawLine(agent.position + Vector3.up * yOffset, c.attackTarget.position + Vector3.up * yOffset);

                Handles.color = focusTargetColor;
                Handles.DrawSolidDisc(c.attackTarget.position + Vector3.up * yOffset, Vector3.up, locationRadius);

                Handles.color = accuracyRangeColor;
                Handles.DrawSolidDisc(c.attackTarget.position + Vector3.up * yOffset, Vector3.up, agent.aimAccuracy);
            }



            //float distSqr = (c.destination - agent.transform.position).sqrMagnitude;


            //if (c.destination != Vector3.zero || distSqr > drawRange * drawRange)
            //{
            //    Vector3 height = Vector3.up * yOffset;

            //    if (path != null)
            //    {
            //        Vector3[] corners = path.corners;
            //        for (int c = 0; c < corners.Length - 1; c++)
            //        {
            //            //distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);
            //            Handles.color = lineColor;
            //            Handles.DrawLine(corners[c] + height, corners[c + 1] + height);
            //        }
            //    }


            //    Handles.color = focusTargetColor;
            //    Handles.DrawSolidDisc(c.destination + height, height, locationRadius);
            //    //  Stopping distance
            //    Handles.color = new Color(1, 1, 1, 0.25f); ;
            //    Handles.DrawSolidDisc(c.destination + height, height, agent.aiSteer.arrivalDistance);
            //}





        }
    }
}

