namespace Bang
{
    using UnityEngine;


    public class WeaponBase : MonoBehaviour
    {
        
        protected ActorController owner;
        [SerializeField]
        protected string id;
        [SerializeField]
        protected WeaponTypes weaponType;



        public string Id{
            get { return id; }
            set { id = value; }
        }
    }
}


