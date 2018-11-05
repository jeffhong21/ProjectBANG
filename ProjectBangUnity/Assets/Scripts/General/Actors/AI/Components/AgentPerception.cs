namespace Bang
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using SharedExtensions;

    public class AgentPerception : MonoBehaviour
    {
        private AgentController agent;

        private float fieldOfView;
        private float rayRange;
        public int amountOfRays = 9;


        private RaycastHit hit;
        private Vector3[] directions;


		private void Awake()
		{
            fieldOfView = agent.stats.fieldOfView;
            rayRange = agent.stats.sightRange;
		}


		public void GeneralSightCheck()
        {
            directions = transform.forward.GetDirectionsFrom(amountOfRays, fieldOfView);
            for (int i = 0; i < directions.Length; i++)
            {
                Debug.DrawRay(transform.position, directions[i] * rayRange, Color.white, 1f);

                if(Physics.Raycast(transform.position, directions[i], out hit, rayRange, Layers.entites, QueryTriggerInteraction.Ignore)){
                    
                }
            }
        }


        public void FocusSightCheck(){
            
        }


        private void OnDrawGizmosSelected()
        {
            if(directions != null){
                for (int i = 0; i < directions.Length; i++){
                    Gizmos.DrawRay(transform.position, directions[i] * rayRange);
                }
            }
        }


        public void ConicalBulletRaycast(Transform bulletSpawn, float coneSize, float range)
        {
            float xSpread = Random.Range(-1, 1);
            float ySpread = Random.Range(-1, 1);
            //normalize the spread vector to keep it conical
            Vector3 spread = new Vector3(xSpread, ySpread, 0.0f).normalized * coneSize;
            Vector3 direction = ((Quaternion.Euler(spread) * bulletSpawn.rotation).eulerAngles).normalized;
            RaycastHit hit;
            if (Physics.Raycast(bulletSpawn.position, direction, out hit, range))
            {
                //handle collision
            }
        }


    }

}