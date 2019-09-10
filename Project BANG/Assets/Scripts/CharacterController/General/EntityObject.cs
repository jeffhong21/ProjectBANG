using UnityEngine;
using System.Collections;

namespace CharacterController
{

    public interface IEntityObject
    {
        int EntityID { get; }



        void Update(float deltaTime);
    }


    public abstract class EntityObject : MonoBehaviour, IEntityObject
    {
        public abstract int EntityID { get; }

        public abstract void Update(float deltaTime);
    }

}
