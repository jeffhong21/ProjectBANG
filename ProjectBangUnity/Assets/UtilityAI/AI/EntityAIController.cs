namespace UtilityAI
{
    using UnityEngine;
    using UnityEngine.AI;


    public class EntityAIController : MonoBehaviour
    {

        [SerializeField, Range(1f, 50f)]
        private float _scanRadius = 20f;

        public float scanRadius
        {
            get { return _scanRadius; }
        }


        TaskNetworkComponent taskNetwork;
        AIContextProvider contextProvider;

        [HideInInspector]
        public AIPerceptionComponent aiSight;
        [HideInInspector]
        public EntityAISteering aiSteer;


        public bool isMoving;
        [HideInInspector]
        public bool isDead;
        [SerializeField]
        private bool useRandomColor;




		protected void Awake()
		{
            taskNetwork = GetComponent<TaskNetworkComponent>();
            contextProvider = GetComponent<AIContextProvider>();
            //aiSight = GetComponent<AIPerceptionComponent>();
            aiSteer = GetComponent<EntityAISteering>();


            if(useRandomColor) ColorRenderers(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
		}








		public void MoveTo(Vector3 destination)
        {
            AIContext context = contextProvider.GetContext() as AIContext;
            context.destination = destination;

            //aiMove.MoveTo(destination);
            aiSteer.MoveTo(destination);
            isMoving = true;
            Debug.LogFormat("Current Move Time is: {0}", Time.timeSinceLevelLoad);
        }

        public void StopMoving()
        {
            aiSteer.StopWalking();
            isMoving = false;
            //Debug.Log(string.Format("{0} has stopped moving", this.gameObject.name));
        }


        public void Shoot()
        {
            Debug.Log(string.Format("{0} is shooting", this.gameObject.name));
        }

        public void Reload()
        {
            Debug.Log(string.Format("{0} is reloading", this.gameObject.name));
        }

        public void AddAmmo(int _amount)
        {
            Debug.Log(string.Format("{0} is adding ammo", this.gameObject.name));
        }

        public void OnDeath()
        {
            //  Turn off the movement and shooting scripts.
        }

        protected virtual void OnAttackTargetChange()
        {
            //  When a new attack target is set, we want to turn towards the target.
        }

        protected virtual void OnAttackTargetDead()
        {
            //  When target dies, stop shooting

        }


        public void ColorRenderers(Color color, bool ignoreParticleSystems = true)
        {
            string eyes_mat_name = "entity_eyes_mat (Instance)";

            // first iterate through all renderes on the game object itself
            var renderers = this.GetComponents<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if ( !ignoreParticleSystems || renderers[i].GetComponent<ParticleSystem>() == null)
                {
                    if (renderers[i].material.name != eyes_mat_name)
                    {
                        //Debug.Log(renderers[i].material.name);
                        renderers[i].material.color = color;
                    }

                }
            }

            // then iterate through all renderers in child game objects
            renderers = this.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if ( !ignoreParticleSystems || renderers[i].GetComponent<ParticleSystem>() == null)
                {
                    if(renderers[i].material.name != eyes_mat_name){
                        //Debug.Log(renderers[i].material.name);
                        renderers[i].material.color = color;
                    }
                        

                }
            }
        }


    }
}