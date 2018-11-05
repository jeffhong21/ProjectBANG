namespace Bang
{
    using UnityEngine;


    public class WeaponBase : MonoBehaviour
    {
        
        protected ActorController owner;
        [SerializeField]
        protected string nameID;
        [SerializeField]
        protected WeaponTypes weaponType;



        public string NameID{
            get { return nameID; }
            set { nameID = value; }
        }
    }
}


