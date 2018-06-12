namespace Bang
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;



    public class AgentVisualizer : MonoBehaviour
    {
        AgentCtrl agentCtrl;
        AgentInput agentInput;
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


		private void Awake()
		{
            agentCtrl = GetComponent<AgentCtrl>();
            agentInput = GetComponent<AgentInput>();
		}


		private void OnDrawGizmos()
		{


            if (agentCtrl.attackTarget != null)
            {
                Gizmos.color = lineColor;
                Gizmos.DrawLine(agentCtrl.position + Vector3.up * yOffset, agentCtrl.attackTarget.position + Vector3.up * yOffset);

                Gizmos.color = focusTargetColor;
                Handles.DrawSolidDisc(agentCtrl.attackTarget.position + Vector3.up * yOffset, Vector3.up, locationRadius);

                //Gizmos.color = accuracyRangeColor;
                //Handles.DrawSolidDisc(agentCtrl.attackTarget.position + Vector3.up * yOffset, Vector3.up, agent.aimAccuracy);
            }
		}



	}
}








