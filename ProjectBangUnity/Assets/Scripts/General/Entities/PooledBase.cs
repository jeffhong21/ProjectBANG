namespace Bang
{
    using UnityEngine;


    public abstract class PooledBase : MonoBehaviour, IPooled
    {
        
        [SerializeField, Util.ReadOnly, Tooltip("The instance pool ID. Guaranteed to be unique.")]
        protected uint _poolId;

        /// <summary>
        /// Gets or sets the pooled identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public uint id{
            get { return _poolId; }
        }


        protected virtual void OnEnable()
        {

        }



    }
}


