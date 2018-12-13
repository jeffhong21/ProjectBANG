namespace Items
{
    using UnityEngine;
    using System.Collections;


    public abstract class ItemBase : MonoBehaviour
    {

        public Vector3 position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        public Quaternion rotation
        {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; }
        }

        public bool isActive
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); }
        }
    }


    public interface IItemBase
    {
        
    }

}