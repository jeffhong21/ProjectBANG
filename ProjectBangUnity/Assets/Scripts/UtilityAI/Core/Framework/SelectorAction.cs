namespace UtilityAI
{
    using UnityEngine;
    using System;

    using Serialization;
    using Newtonsoft.Json;

    public class SelectorAction : IAction //, ISerializationCallbackReceiver, IPrepareForSerialization, IInitializeAfterDeserialization
    {
        [HideInInspector]
        public string jsonData;  //  Serialization data.
        [SerializeField]
        private string _name;

        private Guid _selectorId;
        private Selector _selector;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Selector selector
        {
            get { return _selector; }
            set { _selector = value; }
        }


        //
        // Constructors
        //
        public SelectorAction()
        {
        }

        public SelectorAction(Selector selector)
        {
            _selector = selector;
            _selectorId = selector.id;
        }


        //
        // Methods
        //
        public void Execute(IAIContext context)
        {
            selector.Select(context, selector.qualifiers, selector.defaultQualifier);
        }


        //public void PrepareForSerialization()
        //{
        //    jsonData = JsonConvert.SerializeObject(selector, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        TypeNameHandling = TypeNameHandling.Auto,
        //        PreserveReferencesHandling = PreserveReferencesHandling.All
        //    });
        //}


        //public void InitializeAfterDeserialization(object rootObject)
        //{
        //    //Debug.Log(jsonData);
        //    if (jsonData == null) return;


        //    selector = JsonConvert.DeserializeObject<ScoreSelector>(jsonData, new JsonSerializerSettings
        //    {
        //        TypeNameHandling = TypeNameHandling.Auto,
        //        NullValueHandling = NullValueHandling.Ignore,
        //        PreserveReferencesHandling = PreserveReferencesHandling.All
        //    });
        //    //rootSelector = rootObject as ScoreSelector;

        //}


        ///// <summary>
        ///// Part of ISerializationCallbackReceiver.
        ///// </summary>
        //public void OnBeforeSerialize()
        //{
        //    //Debug.Log("Prepare for serialization");
        //    PrepareForSerialization();
        //}


        ///// <summary>
        ///// Part of ISerializationCallbackReceiver.
        ///// </summary>
        //public void OnAfterDeserialize()
        //{
        //    //Debug.Log("After deserialize");
        //    if (selector == null)
        //    {
        //        selector = new ScoreSelector();
        //    }
        //    InitializeAfterDeserialization(selector);
        //}



    }



}