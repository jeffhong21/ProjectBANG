namespace UtilityAI
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using Random = UnityEngine.Random;

    using Bang;
    /// <summary>
    /// TaskNetworkComponent
    /// </summary>
    public class TaskNetworkComponent : MonoBehaviour
    {
        
        [HideInInspector, SerializeField]
        public List<UtilityAIConfig> aiConfigs;

        [SerializeField]
        private List<UtilityAIClient> _clients = new List<UtilityAIClient>();

        //  AddClientWindow
        public List<UtilityAIClient> clients
        {
            get{
                return _clients;
            }
            set{
                _clients = value;
            }
        }

        //  For OnStartAndEnable;
        [SerializeField]
        private bool _hasStarted;


        [Header(" -------- Debug -------- ")]
        [SerializeField]
        bool debugNextIntervalTime;
        bool isRunning = true;
        [HideInInspector] public bool showDefaultInspector, showDeleteAssetOption, selectAiAssetOnCreate;





        private void Awake()
        {

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
            foreach (UtilityAIClient client in clients)
            {
                client.Stop();
            }
		}

		private void OnStartAndEnable()
        {
            if (_hasStarted) return;  //  If already initialized, return from function.
            _hasStarted = true;


            foreach(UtilityAIClient client in clients)
            {
                client.Start();
                StartCoroutine(ExecuteUpdate(client));
            }
        }


        public IEnumerator ExecuteUpdate(UtilityAIClient client)
        {
            float nextInterval = 0f;
            //IEnumerator activeAction = null;

            yield return new WaitForSeconds(Random.Range(client.startDelayMin, client.startDelayMax));

            while (isRunning)
            {   
                if (Time.timeSinceLevelLoad > nextInterval)
                {
                    client.Execute();
                    nextInterval = Time.timeSinceLevelLoad + Random.Range(client.intervalMin, client.intervalMax);
                    if (debugNextIntervalTime) Debug.Log("Current Time:  " + Time.timeSinceLevelLoad + " | Next interval in:  " + (nextInterval - Time.timeSinceLevelLoad));

                }

                yield return null;
            }
        }


        public void GetClient(Guid aiId)
        {

        }


        public void Pause()
        {
            //isRunning = false;
            foreach (UtilityAIClient client in clients)
            {
                client.Pause();
            }
        }


        public void Resume()
        {
            //isRunning = true;
            foreach (UtilityAIClient client in clients)
            {
                client.Resume();
            }
        }

        internal void ToggleActive(int idx, bool active)
        {
            
        }








		#region For Testing AI
		//public void InitializeTestAI()
		//{
		//    var scanAi = new MockScanningAI("ScanningAI");
		//    var moveAi = new MockMoveAI("MovementAI");
		//    moveAi.visualizer = visualizer;
		//    //  * Initialize TestClient
		//    var scanAiClient = InitializeAI(scanAi);
		//    var moveAiClient = InitializeAI(moveAi, 1f, 1f, 0f, 0f);

		//    scanAiClient.debug = false;

		//    //  Starting all clients.
		//    foreach (UtilityAIClient client in clients)
		//    {
		//        //client.ActionSelected += taskNetworkDebugger.GetSelectorResults;
		//        client.Start();
		//        StartCoroutine(ExecuteUpdate(client));
		//    }
		//}

		//public UtilityAIClient InitializeAI(UtilityAI utilityAI, float iMin = 1f, float iMax = 1f, float sMin = 0f, float sMax = 0f)
		//{
		//    var aiClient = new UtilityAIClient(this, utilityAI, iMin, iMax, sMin, sMax);
		//    clients.Add(aiClient);
		//    return aiClient;
		//}



		//public IEnumerator _ExecuteUpdate(UtilityAIClient client)
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
		//public IEnumerator OnRunning(UtilityAIClient client)
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

