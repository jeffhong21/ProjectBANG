namespace AtlasAI
{
    using UnityEngine;
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using Random = UnityEngine.Random;

    using Bang;
    /// <summary>
    /// TaskNetworkComponent
    /// </summary>
    public class TaskNetworkComponent : MonoBehaviour
    {
        
        //[HideInInspector, SerializeField]
        [SerializeField]
        public UtilityAIConfig[] aiConfigs;

        [SerializeField]
        private List<IUtilityAIClient> _clients;

        //  AddClientWindow
        public List<IUtilityAIClient> clients
        {
            get{
                return _clients;
            }
        }


        [SerializeField]
        private bool _hasStarted;  //  For OnStartAndEnable;


        [Header(" -------- Debug -------- ")]
        [SerializeField]
        bool debugNextIntervalTime;
        bool isRunning = true;
        [HideInInspector] public bool showDefaultInspector, showDeleteAssetOption, selectAiAssetOnCreate;





        private void Awake()
        {
            //_clients = new List<IUtilityAIClient>();

            AINameMap.RegenerateNameMap();

            if (aiConfigs.Length == 0)
                return;

            _clients = new List<IUtilityAIClient>();


            if (aiConfigs.Length != clients.Count)
            {
                clients.Clear();
                for (int i = 0; i < aiConfigs.Length; i++)
                {
                    
                    //  Get Guid of aiID.
                    string aiClientName = aiConfigs[i].aiId;
                    var field = typeof(AINameMapHelper).GetField(aiClientName, BindingFlags.Public | BindingFlags.Static);
                    Guid guid = (Guid)field.GetValue(null);

                    //Guid guid = AINameMap.GetGUID(aiConfigs[i].aiId);
                    if (guid == Guid.Empty)
                    {
                        //Debug.LogWarningFormat("Guid for {0} does not exist.", aiConfigs[i].aiId);
                        Debug.LogFormat("<color=red> AINameMap does not contain aiId ( {0} ) </color>", aiConfigs[i].aiId);
                        continue;
                    }

                    //  Create a new IUtilityAIClient
                    IUtilityAIClient client = new UtilityAIClient(guid, GetComponent<IContextProvider>(),
                                                                  aiConfigs[i].intervalMin, aiConfigs[i].intervalMax,
                                                                  aiConfigs[i].startDelayMin, aiConfigs[i].startDelayMax)
                    {
                        debugClient = aiConfigs[i].debug
                    };


                    //  Add the client to the TaskNetwork.
                    clients.Add(client);                          


                    if (aiConfigs[i].isPredefined)
                    {
                        //  Initialize AIConfig so we can get the name of the predefined config.
                        Type type = Type.GetType(aiConfigs[i].type);
                        IUtilityAIConfig config = (IUtilityAIConfig)Activator.CreateInstance(type);
                        //  Configure the predefined settings.
                        config.SetupAI(client.ai);
                    }
                }
            }

        }

		private void Start()
		{
            OnStartAndEnable();
		}

		
        private void OnEnable()
        {
            OnStartAndEnable();
        }

		
        private void OnDisable()
		{
            _hasStarted = false;
            foreach (IUtilityAIClient client in clients)
            {
                client.Stop();
            }
		}


		private void OnStartAndEnable()
        {
            if (_hasStarted) return;  //  If already initialized, return from function.
            _hasStarted = true;


            for (int i = 0; i < aiConfigs.Length; i ++)
            {
                if(aiConfigs[i].isActive)
                {
                    //Debug.Log(aiConfigs[i].aiId + " is active and running.\nDebugClient is set to " + aiConfigs[i]._debugClient);
                    if(clients[i].state != UtilityAIClientState.Running)
                    {
                        clients[i].Start();
                    }
                    StartCoroutine(ExecuteUpdate((UtilityAIClient)clients[i]));
                }    
            }
        }



		//private void Update()
		//{
  //          for (int i = 0; i < clients.Count; i ++)
  //          {
  //              if(clients[i].state == UtilityAIClientState.Running)
  //              {
  //                  clients[i].Execute();
  //              }
  //          }
		//}


		public IEnumerator ExecuteUpdate(UtilityAIClient client)
        {
            float nextInterval = 0f;

            //Debug.Log(" Waiting for startDelay ");
            //yield return new WaitForSeconds(Random.Range(client.startDelayMin, client.startDelayMax));

            while (isRunning)
            {   
                if (Time.timeSinceLevelLoad + 0.25f > nextInterval)
                {
                    client.Execute();
                    nextInterval = Time.timeSinceLevelLoad + Random.Range(client.intervalMin, client.intervalMax);
                    if (debugNextIntervalTime) Debug.Log("Current Time:  " + Time.timeSinceLevelLoad + " | Next interval in:  " + (nextInterval - Time.timeSinceLevelLoad));

                }
                yield return null;
            }
        }


        public IUtilityAIClient GetClient(Guid aiId)
        {
            foreach (IUtilityAIClient client in clients)
            {
                if(client.ai.id == aiId){
                    Debug.Log("Client:  " + client);
                    return null;
                }
            }

            Debug.Log("Did not find a client");

            return null;
        }


        public void Pause()
        {
            //isRunning = false;
            foreach (IUtilityAIClient client in clients)
            {
                client.Pause();
            }
        }


        public void Resume()
        {
            //isRunning = true;
            foreach (IUtilityAIClient client in clients)
            {
                client.Resume();
            }
        }

        internal void ToggleActive(int idx, bool active)
        {
            if(idx < aiConfigs.Length)
                aiConfigs[idx].isActive = active;
            
        }


        ////  For LoadBalancer
        //private void Update()
        //{

        //    for (int index = 0; index < clients.Count; index++)
        //    {
        //        float nextInterval = 0f;

        //        if (Time.time > nextInterval)
        //        {
        //            clients[index].Execute();
        //            nextInterval = Time.timeSinceLevelLoad + Random.Range(clients[index].intervalMin, clients[index].intervalMax);
        //        }
        //    }
        //}




		#region For Testing AI
		//public void InitializeTestAI()
		//{
		//    var scanAi = new MockScanningAI("ScanningAI");
		//    var moveAi = new MockMoveAI("MovementAI");
		//    moveAi.visualizer = visualizer;
		//    //  * Initialize TestClient
		//    var scanAiClient = ConfigureAI(scanAi);
		//    var moveAiClient = ConfigureAI(moveAi, 1f, 1f, 0f, 0f);

		//    scanAiClient.debug = false;

		//    //  Starting all clients.
		//    foreach (IUtilityAIClient client in clients)
		//    {
		//        //client.ActionSelected += taskNetworkDebugger.GetSelectorResults;
		//        client.Start();
		//        StartCoroutine(ExecuteUpdate(client));
		//    }
		//}

		//public IUtilityAIClient ConfigureAI(AtlasAI utilityAI, float iMin = 1f, float iMax = 1f, float sMin = 0f, float sMax = 0f)
		//{
		//    var aiClient = new IUtilityAIClient(this, utilityAI, iMin, iMax, sMin, sMax);
		//    clients.Add(aiClient);
		//    return aiClient;
		//}



		//public IEnumerator _ExecuteUpdate(IUtilityAIClient client)
		//{
		//    float nextInterval = 0f;
		//    IEnumerator activeAction = null;
		//    IEnumerator onRunning = null;

		//    yield return new WaitForSeconds(Random.Range(client.startDelayMin, client.startDelayMax));
		//    while(isRunning)
		//    {
		//        if (client.state == UtilityAIClientState.Stopped){
		//            if (Time.time > nextInterval)
		//            {
		//                //  **  For Debugging
		//                if(debugNavMesh){
		//                    onRunning = OnRunning(client);
		//                    StartCoroutine(onRunning);
		//                }

		//                activeAction = client.ExecuteAction();
		//                yield return StartCoroutine(activeAction);
		//                activeAction = null;

		//                //  **  For Debugging
		//                if(onRunning != null)
		//                    StopCoroutine(onRunning);

		//                nextInterval = Time.time + Random.Range(client.intervalMin - 0.5f, client.intervalMax + 1f);
		//                Debug.Log("Next interval in:  " + (nextInterval - Time.time));
		//                yield return null;
		//            }
		//        }
		//        else if (client.state == UtilityAIClientState.Running){
		//            yield return null;
		//        }
		//        else if (client.state == UtilityAIClientState.Pause)
		//        {
		//            while(client.state == UtilityAIClientState.Pause){
		//                yield return null;  //  Pause Coroutine.                            
		//            }
		//        }


		//        yield return null;
		//    }
		//}


		///// <summary>
		///// For Debugging Purposes
		///// </summary>
		///// <returns>The running.</returns>
		///// <param name="client">Client.</param>
		//public IEnumerator OnRunning(IUtilityAIClient client)
		//{
		//    while (isRunning)
		//    {
		//        if (client.state == UtilityAIClientState.Running)
		//        {
		//            AIContext aiContext = context as AIContext;
		//            if(aiContext.navMeshAgent.remainingDistance == Mathf.Infinity){
		//                Debug.Log(string.Format("PathPending: < {0} > | PathStatus: < {1} > | Path: < {2} >", aiContext.navMeshAgent.pathPending, aiContext.navMeshAgent.pathStatus, aiContext.navMeshAgent.path));
		//                // Debug.Break();
		//            }
		//            else{
		//                Debug.Log(string.Format("DistanceRemaining is: < {0} > | PathStatus:  < {1} > |  < {2} >", aiContext.navMeshAgent.remainingDistance.ToString("n4"), aiContext.navMeshAgent.pathStatus, Time.time));
		//            }

		//            yield return new WaitForSeconds(1.5f);
		//        }

		//        yield return null;
		//    }

		//}
		#endregion




	}
}

