﻿namespace AtlasAI
{
    using UnityEngine;
    using System;

    using Serialization;
    using Newtonsoft.Json;

    public class SelectorAction : IAction //, ISerializationCallbackReceiver, IPrepareForSerialization, IInitializeAfterDeserialization
    {
        //
        // Fields
        //

        private Guid _selectorId;

        private Selector _selector;


        //
        // Properties
        //
        public string name {
            get;
            set;
        }

        public Selector selector
        {
            get { return _selector; }
            set { _selector = value; }
        }


        //
        // Constructors
        //
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





    }



}