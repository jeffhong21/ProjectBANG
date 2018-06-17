namespace UtilityAI
{
    using UnityEngine;


    public abstract class SingleInstanceComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        //
        // Static Fields
        //
        //private static int _instanceMark;

        //
        // Constructors
        //
        //protected SingleInstanceComponent();

        //
        // Methods
        //
        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnDestroy()
        {

        }
    }






}

