namespace UtilityAI
{
    using UnityEngine;

    public interface IUtilityAI : ISerializationCallbackReceiver
    {
        string name{
            get;
            set;
        }

        Selector rootSelector{
            get;
            set;
        }

        int selectorCount { 
            get; 
        }

        // Selector Item { 
        //     get; 
        // }

        //Selector this[int idx]{
        //    get;
        //}

        void AddSelector(Selector s);

        Selector FindSelector(Selector s);

        void RemoveSelector(Selector s);

        bool ReplaceSelector(Selector current, Selector replacement);
        
        void RegenerateIds();
        //byte[] PrepareForSerialize();
        //void InitializeAfterDeserialize(byte[] data);

    }
}