namespace Bang
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;

    using UtilityAI;


    public class AgentVisualizer : MonoBehaviour
    {
        AgentCtrl agentCtrl;
        AgentInput agentInput;
        AgentContext context;

        [SerializeField]
        float locationRadius = 0.5f;
        [SerializeField]
        Color lineColor = new Color32(180, 255, 0, 255);
        [SerializeField]
        Color focusTargetColor = new Color32(0, 180, 255, 128);
        //[SerializeField]
        //Color accuracyRangeColor = new Color32(0, 180, 255, 64);
        [SerializeField]
        float yOffset = 0.05f;


		private void Awake()
		{
            agentCtrl = GetComponent<AgentCtrl>();
            agentInput = GetComponent<AgentInput>();
            context = agentCtrl.context;
		}


		private void OnDrawGizmos()
		{

            DrawDestinationPath();

            DrawConverPosition(Color.blue);


		}






        private void DrawConverPosition(Color color)
        {
            //if (context.coverTarget != null && context.coverPosition != Vector3.zero)
            //{
            //    Handles.color = color;
            //    Handles.DrawSolidDisc(context.coverPosition + Vector3.up * yOffset, Vector3.up, 0.25f);
            //}
            if (context.coverPosition != Vector3.zero)
            {
                Handles.color = color;
                Handles.DrawSolidDisc(context.coverPosition + Vector3.up * yOffset, Vector3.up, 0.5f);
            }
        }


        private void DrawDestinationPath()
        {
            if (agentInput.path != null)
            {
                Vector3[] corners = agentInput.path.corners;
                for (int c = 0; c < corners.Length - 1; c++)
                {
                    Gizmos.color = lineColor;
                    Gizmos.DrawLine(corners[c] + Vector3.up * yOffset, corners[c + 1] + Vector3.up * yOffset);
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(agentCtrl.position + Vector3.up * yOffset, context.destination + Vector3.up * yOffset);
            }


            Handles.color = focusTargetColor;
            Handles.DrawSolidDisc(context.destination + Vector3.up * yOffset, Vector3.up, locationRadius);
        }


	}
}








