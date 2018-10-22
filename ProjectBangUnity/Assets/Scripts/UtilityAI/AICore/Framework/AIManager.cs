namespace AtlasAI
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;

    using System.Linq;


    /// <summary>
    /// Given a Guid, AIManager can look up AIData or a List of IUtilityAIClient.
    /// </summary>
    public static class AIManager
    {
        public delegate IUtilityAIClient AIClientResolver(GameObject host, Guid aiId);


        public static string StorageFolder = Path.GetDirectoryName("Assets/Scripts/AtlasAI/Resources/AIStorage/");

        private static Dictionary<Guid, AIData> _aiLookup;
        //  UtilityAIComponent and its list of UtilityAIClients
        private static Dictionary<Guid, List<IUtilityAIClient>> _aiClients;


        public static AIClientResolver GetAIClient;



        //
        // Static Properties
        //
        public static IEnumerable<IUtilityAIClient> allClients
        {
            get{
                if (_aiClients != null)
                {
                    List<IUtilityAIClient> clients = new List<IUtilityAIClient>();
                    foreach (KeyValuePair<Guid, List<IUtilityAIClient>> client in _aiClients)
                    {
                        clients.Concat(client.Value);
                    }
                    return clients.ToArray();
                }
                else 
                { 
                    return null; 
                }


            }
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



        /// <summary>
        /// Gets the list of clients for a given AI through the AIStorage aiId.
        /// </summary>
        /// <param name="aiId">Ai identifier.</param>
        ///<returns> The list of clients for the specified AI. </returns>
        public static List<IUtilityAIClient> GetAllClients(Guid aiId)
        {
            if (_aiClients.ContainsKey(aiId))
            {
                return _aiClients[aiId];
            }
            else
            {
                return null;
            }

        }

        ///<summary>
        ///Gets a AtlasAI
        ///</summary>
        ///<param name = "id" > The ID.</param>
        ///<returns> The AI with the specified ID, or null if no match is found.</returns>
        public static IUtilityAI GetAI(Guid id)
        {
            if(_aiLookup.ContainsKey(id))
            {
                return _aiLookup[id].ai;
            }
            else{
                return null;
            }
        }


        public static void Register(IUtilityAIClient client)
        {
            if(_aiLookup == null) _aiLookup = new Dictionary<Guid, AIData>();
            if (_aiClients == null) _aiClients = new Dictionary<Guid, List<IUtilityAIClient>>();


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