namespace AtlasAI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Given a Guid, AIManager can look up AIData or a List of IUtilityAIClient.
    /// </summary>
    public static class AIManager
    {
        public delegate IUtilityAIClient AIClientResolver(GameObject host, Guid aiId);


        //
        // Static Fields
        //
        public static string StorageFolder = Path.GetDirectoryName("Assets/Scripts/AtlasAI/Resources/AIStorage/");

        /// <summary>
        /// Lookup for each UtilityAI and which StoredData to use.  Uses the Guid of the UtilityAI.
        /// </summary>
        private static Dictionary<Guid, AIData> _aiLookup;

        /// <summary>
        /// Lookup for each gameobject that contains a list of UtilityAIClients.
        /// </summary>
        private static Dictionary<Guid, List<IUtilityAIClient>> _aiClients;


        public static AIClientResolver GetAIClient = 
            (GameObject host, Guid aiId) => {
            if(host.GetComponent<UtilityAIComponent>())
                    return host.GetComponent<UtilityAIComponent>().GetClient(aiId);
            else
                return null;
            };



        //
        // Static Properties
        //
        public static IEnumerable<IUtilityAIClient> allClients
        {
            get{
                var allComponents = Utilities.ComponentHelper.FindAllComponentsInScene<UtilityAIComponent>();
                foreach (var utilityAIComponent in allComponents){
                    for (int i = 0; i < utilityAIComponent.clients.Length; i++)
                        yield return utilityAIComponent.clients[i];
                }
            }
        }



        /// <summary>
        /// Loads and initializes all AIs. This means that calling <see cref="M:Apex.AI.AIManager.GetAI(System.Guid)"/> 
        /// will never load AIs on demand and thus won't allocate memory.
        /// </summary>
        public static void EagerLoadAll()
        {
            
        }


        //<summary>
        //Executes the specified AI once.
        //</summary>
        //<param name = "id" > The AI ID.</param>
        //<param name = "context" > The context.</param>
        //<returns><c>true</c> if the AI was found and executed; otherwise<c>false</c>.</returns>
        public static bool ExecuteAI(Guid id, IAIContext context)
        {
            return true;
        }


        private static void ReadAndInit(AIData data)
        {

        }


        ///<summary>
        ///  Gets a UtilityAI via its id
        ///</summary>
        ///<param name = "id" > The ID.</param>
        ///<returns> The AI with the specified ID, or null if no match is found.</returns>
        public static IUtilityAI GetAI(Guid id)
        {
            if (_aiLookup == null){
                _aiLookup = new Dictionary<Guid, AIData>();
                return null;
            }

            if(_aiLookup.ContainsKey(id)){
                return _aiLookup[id].ai;
            }
            else{
                return null;
            }
        }


        /// <summary>
        /// Gets the list of clients for a given AI. Please note that this is a live list that should not be modified directly.
        /// </summary>
        /// <param name="aiId">Ai identifier.</param>
        ///<returns> The list of clients for the specified AI. </returns>
        public static IList<IUtilityAIClient> GetAllClients(Guid aiId)
        {
            if (_aiClients == null){
                _aiClients = new Dictionary<Guid, List<IUtilityAIClient>>();
                return null;
            }

            if (_aiClients.ContainsKey(aiId)){
                return _aiClients[aiId];
            }
            else{
                return null;
            }
        }


        public static void Register(IUtilityAIClient client)
        {
            if(_aiLookup == null) _aiLookup = new Dictionary<Guid, AIData>();

            //  If aiLookup does not have a client registered.
            if(_aiLookup.ContainsKey(client.ai.id) == false) // || _aiLookup[client.ai.id] == null)
            {
                AIData aiData = new AIData();
                aiData.ai = client.ai;
                aiData.storedData = null;
                //  Add AIData to aiLookup.
                _aiLookup.Add(aiData.ai.id, aiData);
            }

        }


        public static void UnRegister(IUtilityAIClient client)
        {
            //  If aiLookup has a client registered.
            if (_aiLookup.ContainsKey(client.ai.id)){
                _aiLookup.Remove(client.ai.id);
            }
        }



        private class AIData
        {
            public IUtilityAI ai;

            public AIStorage storedData;

            public AIData()
            {
                ai = null;
                storedData = null;
            }
        }


    }



}