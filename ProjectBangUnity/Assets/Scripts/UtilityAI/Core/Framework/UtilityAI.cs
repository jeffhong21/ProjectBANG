namespace UtilityAI
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Serialization;
    using Newtonsoft.Json;


    [Serializable]
    public class UtilityAI : IUtilityAI, ISerializationCallbackReceiver, IPrepareForSerialization, IInitializeAfterDeserialization
    {
        public bool debug;

        //[SerializeField] [HideInInspector]
        //private string jsonData;
        [HideInInspector]
        public string jsonData;  //  Serialization data.


        [SerializeField, HideInInspector]
        private Guid _rootSelectorId;

        [SerializeField, HideInInspector]
        private Guid _id;

        [SerializeField, HideInInspector]
        private List<Selector> _selectors;

        private Selector _rootSelector;

        [SerializeField]
        private string _name;



        public Guid id
        {
            get { return _id; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Selector rootSelector
        {
            get{ return _rootSelector;}
            set{ _rootSelector = value;}
        }

        public int selectorCount
        {
            get { return _selectors.Count; }
        }

        //
        // Indexer
        //
        public Selector this[int idx]
        {
            get { return _selectors[idx]; }
        }


        public UtilityAI()
        {
            _rootSelector = new ScoreSelector();
            _selectors = new List<Selector>();
            RegenerateIds();
        }

        public UtilityAI(string aiName)
        {
            _rootSelector = new ScoreSelector();
            _selectors = new List<Selector>();
            RegenerateIds();

            name = aiName;
        }

        public void AddSelector(Selector s)
        {
            throw new NotImplementedException();
        }

        public Selector FindSelector(Selector s)
        {
            throw new NotImplementedException();
        }

        public void RemoveSelector(Selector s)
        {
            throw new NotImplementedException();
        }

        public bool ReplaceSelector(Selector current, Selector replacement)
        {
            throw new NotImplementedException();
        }

        public void RegenerateIds()
        {
            _rootSelector.RegenerateId();
            _rootSelectorId = _rootSelector.id;
            _id = Guid.NewGuid();

            for (int i = 0; i < _selectors.Count; i++){
                _selectors[i].RegenerateId();
            }
            //Debug.Log(id);
        }


        /// <summary>
        /// Selects the action for execution.
        /// </summary>
        /// <returns>The select.</returns>
        /// <param name="context">Context.</param>
        public IAction Select(IAIContext context)
        {
            
            IList<IQualifier> qualifiers = rootSelector.qualifiers;
            IDefaultQualifier defaultQualifier = rootSelector.defaultQualifier;
            IQualifier winner = rootSelector.Select(context, qualifiers, defaultQualifier);
            IAction action = winner.action;


            //if (debug)
            //{
            //    Debug.LogFormat(" Best Qualifier: {0} (<color=#800080ff>{1}</color>)\n", action.name, ((CompositeQualifier)winner).Score(context, ((CompositeQualifier)winner).scorers));
            //}


            //CompositeQualifier cq = winner as CompositeQualifier;
            // TODO:  What if there are no scoreres?
            //float score = cq.Score(context, cq.scorers);


            //if(Visualizer.VisualizerManager.EntityUpdate != null){
            //    Visualizer.VisualizerManager.EntityUpdate();
            //}

            return action;
        }



        //
        // Serialization
        //
        int count = 0;


        public void PrepareForSerialization()
        {
            jsonData = JsonConvert.SerializeObject(rootSelector, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }


        public void InitializeAfterDeserialization(object rootObject)
        {
            //Debug.Log(jsonData);
            if (jsonData == null) return;


            rootSelector = JsonConvert.DeserializeObject<ScoreSelector>(jsonData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
            //rootSelector = rootObject as ScoreSelector;

        }


        /// <summary>
        /// Part of ISerializationCallbackReceiver.
        /// </summary>
        public void OnBeforeSerialize()
        {
            //Debug.Log("Prepare for serialization");
            PrepareForSerialization();
        }


        /// <summary>
        /// Part of ISerializationCallbackReceiver.
        /// </summary>
        public void OnAfterDeserialize()
        {
            //Debug.Log("After deserialize");
            if (rootSelector == null)
            {
                rootSelector = new ScoreSelector();
            }
            InitializeAfterDeserialization(rootSelector);
        }










        #region Serialization

        //public byte[] PrepareForSerialize()
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();

        //    //  Serialize utilityAI to memoryStream
        //    binaryFormatter.Serialize(memoryStream, rootSelector);

        //    memoryStream.Close();
        //    return memoryStream.ToArray();
        //}

        ///// <summary>
        ///// Part of ISerializationCallbackReceiver.
        ///// </summary>
        //public void OnBeforeSerialize()
        //{
        //    //Debug.Log("Prepare for serialization");
        //    data = PrepareForSerialize();
        //}


        //public void InitializeAfterDeserialize(byte[] data)
        //{
        //    int count = 0;

        //    if (data != null)
        //    {
        //        object obj = new BinaryFormatter().Deserialize(new MemoryStream(data));

        //        if (obj is Selector)
        //        {
        //            Selector root = obj as Selector;
        //            rootSelector = obj as Selector;
        //            //selector = root as ScoreSelector;

        //            //if (count < 1){
        //            //    Debug.Log("Succesfully Serialized");
        //            //    count++;
        //            //}
        //            return;
        //        }

        //        else throw new ApplicationException("Unable to deserialize type " + obj.GetType());
        //    }

        //    //if (count < 1){
        //    //    Debug.Log("Did not Deserialize");
        //    //    count++;
        //    //}
        //}


        ///// <summary>
        ///// Part of ISerializationCallbackReceiver.
        ///// </summary>
        //public void OnAfterDeserialize()
        //{
        //    //Debug.Log("After deserialize");
        //    InitializeAfterDeserialize(data);
        //    // InitializeAfterDeserialize(object rootObject);

        //}

        #endregion





    }
}

