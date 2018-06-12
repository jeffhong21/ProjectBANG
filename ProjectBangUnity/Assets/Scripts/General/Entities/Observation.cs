namespace Bang
{
    using UnityEngine;

    public class Observation
    {
        public Observation(IEntity entity)
        {
            this.entity = entity;
            this.position = entity.position;
            this.timestamp = Time.timeSinceLevelLoad;
        }

        public IEntity entity
        {
            get;
            private set;
        }

        public Vector3 position
        {
            get;
            set;
        }

        public float timestamp
        {
            get;
            set;
        }
    }

}

