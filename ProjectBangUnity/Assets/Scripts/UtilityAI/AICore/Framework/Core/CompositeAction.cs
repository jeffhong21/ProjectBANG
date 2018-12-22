namespace AtlasAI
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Composite action.
    /// </summary>
    public sealed class CompositeAction : ActionBase
    {
        //
        // Fields
        //
        private List<IAction> _actions;


        //
        // Properties
        //
        public List<IAction> actions 
        {
            get { return _actions; }
            set { _actions = value; }
        }


        //
        // Constructors
        //
        public CompositeAction()
        {
            _actions = new List<IAction>();
        }

        public CompositeAction(CompositeAction other)
        {
            actions = other.actions;
        }


        //
        // Methods
        //
        public override void Execute(IAIContext context)
        {
            for (int i = 0; i < actions.Count; i++){
                actions[i].Execute(context);
            }
        }

    }



}