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

    // /// <summary>
    // /// UtilityAI creates the rootSelector and initializes the tree.
    // /// </summary>
    // public interface IUtilityAI : ISerializationCallbackReceiver
    // {
    //     void AddSelector(Selector s);
    //     void RemoveSelector(Selector s);
    //     bool ReplaceSelector(Selector current, Selector replacement);
    //     Selector FindSelector(Selector s);
    //     IAction Select(IAIContext context);
    //     void RegenerateIds();
    //     //byte[] PrepareForSerialize();
    //     //void InitializeAfterDeserialize(byte[] data);
    //     Guid id { get; }
    //     string name { get; set; }
    //     Selector rootSelector { get; set; }
    //     int selectorCount { get; }
    //     Selector Item { get; }
    // }


    [Serializable]
    public class UtilityAI : IUtilityAI, IPrepareForSerialization, IInitializeAfterDeserialization
    {
        //[SerializeField] [HideInInspector]
        //private string jsonData;
        [HideInInspector]
        public string jsonData;

        [SerializeField, HideInInspector]
        private Guid _id;

        [SerializeField] [HideInInspector]
        private List<Selector> _selectors;

        private Selector _rootSelector;

        [SerializeField]
        private string _name;


        public Guid id { 
            get;
        }

        public string name{ 
            get{ return _name; } 
            set { _name = value; } 
        }

        public Selector rootSelector{ 
            get; 
            set;
        }

        public int selectorCount{
            get;
        }

        //public Selector this[int idx]{
        //    get;
        //}

        // //  Get Selector with specific index.
        // public Selector Item { get; private set; }



        public UtilityAI()
        {
            rootSelector = new ScoreSelector();
            //selector = new ScoreSelector();
            RegenerateIds();
        }

        public UtilityAI(string aiName)
        {
            rootSelector = new ScoreSelector();
            //selector = new ScoreSelector();
            name = aiName;

            RegenerateIds();
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
            _id = Guid.NewGuid();
            //Debug.Log(id);
        }


        /// <summary>
        /// Selects the action for execution.
        /// </summary>
        /// <returns>The select.</returns>
        /// <param name="context">Context.</param>
        public IAction Select(IAIContext context)
        {
            List<IQualifier> qualifiers = rootSelector.qualifiers;
            IDefaultQualifier defaultQualifier = rootSelector.defaultQualifier;
            IQualifier winner = rootSelector.Select(context, qualifiers, defaultQualifier);



            CompositeQualifier cq = winner as CompositeQualifier;
            // TODO:  What if there are no scoreres?
            //float score = cq.Score(context, cq.scorers);
            IAction action = winner.action;

            //if(Visualizer.VisualizerManager.EntityUpdate != null){
            //    Visualizer.VisualizerManager.EntityUpdate();
            //}

            return action;
        }




        public void PrepareForSerialization()
        {
            jsonData = JsonConvert.SerializeObject(rootSelector, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }

        int count = 0;
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
            if(rootSelector == null){
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

