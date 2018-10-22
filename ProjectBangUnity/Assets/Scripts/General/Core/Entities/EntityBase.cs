namespace Bang
{
    using UnityEngine;


    public abstract class EntityBase : PooledBase, IEntity
    {

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Quaternion rotation
        {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Bang.IEntity"/> is active.
        /// </summary>
        /// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
        public bool isActive
        {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); }
        }



    }
}


