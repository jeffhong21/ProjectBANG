namespace AtlasAI
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Random = UnityEngine.Random;

    using Bang;
    /// <summary>
    /// UtilityAIComponent
    /// </summary>
    public class UtilityAIComponent : ExtendedMonoBehaviour
    {
        //
        // Fields
        //

        [SerializeField]
        private string serializedGuid;
        public UtilityAIConfig[] aiConfigs;
        private IUtilityAIClient[] _clients;

        # region Debug
        [Header(" -------- Debug -------- ")]
        [SerializeField] bool debugNextIntervalTime;
        bool isRunning = true;
        [HideInInspector] public bool showDefaultInspector, showDeleteAssetOption, selectAiAssetOnCreate;
        # endregion


        //
        // Properties
        //

        /// <summary>
        /// For AddClientWindow
        /// </summary>
        /// <value>The clients.</value>
        public IUtilityAIClient[] clients{
            get { return _clients; }
        }


        //
        // Methods
        //

        private void OnDisable()
        {
            for (int i = 0; i < clients.Length; i++){
                clients[i].Stop();
            }
        }


        /// <summary>
        /// Instantiates and starts the Utility AI Client
        /// </summary>
		protected override void OnStartAndEnable()
        {
            _clients = new IUtilityAIClient[aiConfigs.Length];

            for (int i = 0; i < aiConfigs.Length; i++){
                ////  Get Guid of the aiConfig.
                //string aiClientName = aiConfigs[i].aiId;
                //var field = typeof(AINameMapHelper).GetField(aiClientName, BindingFlags.Public | BindingFlags.Static);
                //Guid guid = (Guid)field.GetValue(null);

                IUtilityAI utilityAI = new UtilityAI();
                utilityAI.name = aiConfigs[i].aiId;
                //  Create a new IUtilityAIClient
                clients[i] = new UtilityAIClient(utilityAI, GetComponent<IContextProvider>(), 
                                                 aiConfigs[i].intervalMin, aiConfigs[i].intervalMax,
                                                 aiConfigs[i].startDelayMin, aiConfigs[i].startDelayMax);

                //  Resolve client.
                //AIManager.GetAIClient = GetClient(guid);

                // **** TEMP ****
                if (aiConfigs[i].isPredefined){
                    //  Initialize AIConfig so we can get the name of the predefined config.
                    Type type = Type.GetType(aiConfigs[i].type);
                    IUtilityAIAssetConfig config = (IUtilityAIAssetConfig)Activator.CreateInstance(type);
                    //  Configure the predefined settings.
                    config.SetupAI(clients[i].ai);
                }


                //  Start UtilityAIClient
                clients[i].Start();
                //  If aiConfig is not active, pauce client.
                if(aiConfigs[i].isActive == false){
                    clients[i].Pause();
                    continue;
                }
                    
                // **** TEMP ****
                //StartCoroutine(ExecuteUpdate((UtilityAIClient)clients[i]));
            }
        }


        public IUtilityAIClient GetClient(Guid aiId)
        {
            for (int i = 0; i < clients.Length; i++){
                if (clients[i].ai.id == aiId){
                    Debug.Log("Client:  " + clients[i]);
                    return null;
                }
            }

            Debug.Log("Did not find a client");
            return null;
        }


        public void Pause()
        {
            //isRunning = false;
            for (int i = 0; i < clients.Length; i++){
                clients[i].Pause();
            }
        }


        public void Resume()
        {
            //isRunning = true;
            for (int i = 0; i < clients.Length; i++){
                clients[i].Resume();
            }
        }


        public void ToggleActive(int idx, bool active)
        {
            if (idx < aiConfigs.Length){
                aiConfigs[idx].isActive = active;
                if (active && clients[idx].state == UtilityAIClientState.Pause)
                    clients[idx].Resume();
                else
                    clients[idx].Pause();
            }                  
        }






        public IEnumerator ExecuteUpdate(UtilityAIClient client)
        {
            float nextInterval = 1f;

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


        #region OnStartAndEnable

        //protected void OnStartAndEnable_1()
        //{
        //    if (aiConfigs.Length != clients.Count)
        //    {
        //        //AINameMap.RegenerateNameMap();
        //        _clients.Clear();

        //        for (int i = 0; i < aiConfigs.Length; i++)
        //        {
        //            //  Get Guid of aiID.
        //            string aiClientName = aiConfigs[i].aiId;
        //            var field = typeof(AINameMapHelper).GetField(aiClientName, BindingFlags.Public | BindingFlags.Static);
        //            Guid guid = (Guid)field.GetValue(null);

        //            //Guid guid = AINameMap.GetGUID(aiConfigs[i].aiId);
        //            if (guid == Guid.Empty)
        //            {
        //                Debug.LogFormat("<color=red> AINameMap does not contain aiId ( {0} ) </color>", aiConfigs[i].aiId);
        //                continue;
        //            }

        //            //  Create a new IUtilityAIClient
        //            IUtilityAIClient client = new UtilityAIClient(guid, GetComponent<IContextProvider>(),
        //                                                          aiConfigs[i].intervalMin, aiConfigs[i].intervalMax,
        //                                                          aiConfigs[i].startDelayMin, aiConfigs[i].startDelayMax);
        //            client.debugClient = aiConfigs[i].debug;

        //            //  Add the client to the TaskNetwork.
        //            clients.Add(client);


        //            if (aiConfigs[i].isPredefined)
        //            {
        //                //  Initialize AIConfig so we can get the name of the predefined config.
        //                Type type = Type.GetType(aiConfigs[i].type);
        //                IUtilityAIAssetConfig config = (IUtilityAIAssetConfig)Activator.CreateInstance(type);
        //                //  Configure the predefined settings.
        //                config.SetupAI(client.ai);
        //            }
        //        }
        //    }


        //    for (int i = 0; i < aiConfigs.Length; i++)
        //    {
        //        if (aiConfigs[i].isActive)
        //        {
        //            //Debug.Log(aiConfigs[i].aiId + " is active and running.\nDebugClient is set to " + aiConfigs[i]._debugClient);
        //            if (clients[i].state != UtilityAIClientState.Running)
        //            {
        //                clients[i].Start();
        //            }
        //            StartCoroutine(ExecuteUpdate((UtilityAIClient)clients[i]));
        //        }
        //    }
        //}

        #endregion

    }
}

