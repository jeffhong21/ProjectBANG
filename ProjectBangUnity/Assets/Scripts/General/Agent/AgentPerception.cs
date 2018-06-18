namespace Bang
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.AI;
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public class AgentPerception : MonoBehaviour
    {

        AgentCtrl agentCtrl;
        private List<GameObject> units;


		protected void Awake()
		{
            agentCtrl = GetComponent<AgentCtrl>();
		}


		private void OnEnable()
        {
            units = new List<GameObject>();
        }



        public List<GameObject> FindNeighbors(float scanRadius)
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, scanRadius, Layers.entites);


            for (int i = 0; i < colliders.Length; i++)
            {
                var coll = colliders[i];
                if (coll == null)
                {
                    // ignore null entries
                    continue;
                }

                if (ReferenceEquals(coll.gameObject, this.gameObject))
                {
                    // Do not record 'self'
                    continue;
                }

                units.Add(coll.gameObject);
            }


            return units;
        }



        //  Calculates if npc can see target.
        public static bool CanSeeTarget(Transform transform, Vector3 target, float sightRange, float fieldOfView)
        {
            //var targetPosition = new Vector3(target.position.x, (target.position.y + transform.position.y), target.position.z);
            target.y = 0.75f;
            var dirToPlayer = (target - transform.position).normalized;

            var angleBetweenNpcAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (Vector3.Distance(transform.position, target) < sightRange &&
                angleBetweenNpcAndPlayer < fieldOfView / 2f &&
                Physics.Linecast(transform.position, target, Layers.cover) == false)
            {
                return true;
            }
            return false;
        }





        public static Vector3 DirFromAngle(Transform transform, float angleInDegrees, bool angleIsGlobal = false)
        {
            if (angleIsGlobal == false)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }



        //void OnDrawGizmos()
        //{
        //    //  View Arch
        //    //Handles.color = entity.hasLocationOfInterest ? targetInViewRangeColor : viewAngleColor;
        //    Handles.color = viewAngleColor;
        //    Handles.DrawSolidArc(transform.position + Vector3.up * yOffset, Vector3.up, DirFromAngle(transform.rotation.y), fieldOfView / 2, sightRange);
        //    Handles.DrawSolidArc(transform.position + Vector3.up * yOffset, Vector3.up, DirFromAngle(transform.rotation.y), -fieldOfView / 2, sightRange);

        //    //  Sight Range
        //    Handles.color = sightRangeColor;
        //    Handles.DrawWireArc(transform.position + Vector3.up * yOffset, Vector3.up, Vector3.forward, 360, sightRange);
        //}

    }

}