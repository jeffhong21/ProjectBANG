using System;
using System.Collections.Generic;

namespace AtlasAI.Visualization
{
    public class UtilityAIVisualizer : IUtilityAI, ISelect
    {
        //
        // Fields
        //
        private IUtilityAI _ai;

        private List<Action> _postExecute;

        private SelectorVisualizer _visualizerRootSelector;

        private List<SelectorVisualizer> _selectorVisualizers;

        //private List<UtilityAIVisualizer> _linkedAIs;


        //
        // Properties
        //
        public Guid id{
            get;
            private set;
        }

        public string name{
            get;
            set;
        }

        public Selector rootSelector{
            get;
            set;
        }

        public int selectorCount{
            get { return _ai.selectorCount; }
        }


        //
        // Indexer
        //
        public Selector this[int idx]{
            get { return _selectorVisualizers[idx]; }
        }


        //
        // Constructors
        //
        public UtilityAIVisualizer(IUtilityAI ai)
        {
            id = ai.id;
            name = ai.name;
            rootSelector = ai.rootSelector;

            _ai = ai;
            _visualizerRootSelector = new SelectorVisualizer(ai.rootSelector, this);


            UnityEngine.Debug.LogFormat("Initializing UtilityAI Visualizer.");
        }


        //
        // Static Methods
        //
        private static bool TryFindActionVisualizer(IAction source, IAction target, out ActionVisualizer result)
        {
            throw new NotImplementedException();
        }



        //
        // Methods
        //
        void IUtilityAI.AddSelector(Selector s)
        {
            _selectorVisualizers.Add(new SelectorVisualizer(s, this));
        }


        public ActionVisualizer FindActionVisualizer(IAction target)
        {
            throw new NotImplementedException();
        }


        public IQualifierVisualizer FindQualifierVisualizer(IQualifier target)
        {
            throw new NotImplementedException();
        }


        public Selector FindSelector(Guid id)
        {
            for (int i = 0; i < _selectorVisualizers.Count; i++){
                if (_selectorVisualizers[i].selector.id == id){
                    return _selectorVisualizers[i].selector;
                }
            }
            return null;
        }


        public void Hook(Action postExecute)
        {
            throw new NotImplementedException();
        }


        public void PostExecute()
        {
            throw new NotImplementedException();
        }


        void IUtilityAI.RegenerateIds()
        {
            throw new NotImplementedException();
        }


        void IUtilityAI.RemoveSelector(Selector s)
        {
            for (int index = 0; index < _selectorVisualizers.Count; index++){
                //  If selector id matches given selector id, remove the selector.
                if (_selectorVisualizers[index].selector.id == s.id){
                    //  If the root selector is to be removed, replace the rootSelector field with the next selector.
                    if (index == 0){
                        rootSelector = _selectorVisualizers[1].selector;
                    }
                    _selectorVisualizers.RemoveAt(index);
                    return;
                }
            }
        }


        bool IUtilityAI.ReplaceSelector(Selector current, Selector replacement)
        {
            throw new NotImplementedException();
        }


        public void Reset()
        {
            throw new NotImplementedException();
        }


        public IAction Select(IAIContext context)
        {
            throw new NotImplementedException();
        }


        public void Unhook(Action postExecute)
        {
            throw new NotImplementedException();
        }


    }
}
