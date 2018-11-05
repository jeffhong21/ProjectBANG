namespace Bang
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEditor;

    [CustomEditor(typeof(NavMeshAgentBridge))]
    public class NavMeshAgentBridgeEditor : Editor
    {

        NavMeshAgentBridge castedTarget;

        private void OnEnable()
        {
            castedTarget = target as NavMeshAgentBridge;

            if(castedTarget.hideNavMeshAgentComponent){
                if(castedTarget.GetComponent<NavMeshAgent>()){
                    castedTarget.GetComponent<NavMeshAgent>().hideFlags = HideFlags.HideInInspector;
                }
            }
            else{
                if (castedTarget.GetComponent<NavMeshAgent>()){
                    castedTarget.GetComponent<NavMeshAgent>().hideFlags = HideFlags.None;
                }
            }

        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }

    }

}
