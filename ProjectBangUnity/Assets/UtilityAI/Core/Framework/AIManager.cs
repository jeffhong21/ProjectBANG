namespace UtilityAI
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;


    public static class AIManager
    {
        public static string StorageFolder = Path.GetDirectoryName("Assets/UtilityAI/Resources/AIStorage/");

        private static Dictionary<Guid, AIData> _aiLookup;

        private static Dictionary<Guid, List<IUtilityAIClient>> _aiClients = new Dictionary<Guid, List<IUtilityAIClient>>();





        /// <summary>
        /// Gets all registered clients.
        /// </summary>
        /// <value>All clients.</value>
        //public static HashSet<UtilityAIClient> allClients { get; private set; } = new HashSet<UtilityAIClient>();


        /// <summary>
        /// Gets the list of clients for a given AI.
        /// </summary>
        /// <param name="aiID">Ai identifier.</param>
        ///<returns> The list of clients for the specified AI. </returns>
        public static List<UtilityAIClient> GetAllClients(string aiID){
            List<UtilityAIClient> clients = new List<UtilityAIClient>();

            return clients;
        }

        ///<summary>
        ///Gets an AI by ID.
        ///</summary>
        ///<param name = "id" > The ID.</param>
        ///<returns> The AI with the specified ID, or null if no match is found.</returns>
        public static IUtilityAI GetAI(Guid id)
        {
            if(_aiLookup.ContainsKey(id) == false){
                return null;
            }
            else{
                return _aiLookup[id].ai;
            }
        }

        //<summary>
        //Executes the specified AI once.
        //</summary>
        //<param name = "id" > The AI ID.</param>
        //<param name = "context" > The context.</param>
        //<returns><c>true</c> if the AI was found and executed; otherwise<c>false</c>.</returns>
        public static bool ExecuteAI(string id, IAIContext context){
            return true;
        }


        private static void ReadAndInit(AIData data)
        {
            
        }


        public static void Register(IUtilityAIClient client)
        {
            if(_aiLookup == null)
            {
                _aiLookup = new Dictionary<Guid, AIData>();
            }

            if(_aiLookup.ContainsKey(client.ai.id) == false) // || _aiLookup[client.ai.id] == null)
            {
                AIData aiData = new AIData()
                {
                    ai = client.ai
                };
                _aiLookup.Add(aiData.ai.id, aiData);
            }

        }


        public static void UnRegister(IUtilityAIClient client)
        {
            if (_aiLookup == null)
            {
                return;
            }

            if (_aiLookup.ContainsKey(client.ai.id) == true) // || _aiLookup[client.ai.id] != null)
            {
                _aiLookup.Remove(client.ai.id);

            }
        }



        private class AIData
        {
            public IUtilityAI ai;

            public AIStorage storedData;

            public AIData()
            {
                
            }

        }


    }



}