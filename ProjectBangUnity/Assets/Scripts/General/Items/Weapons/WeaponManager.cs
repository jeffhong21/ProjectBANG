namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;

    public class WeaponManager : SingletonMonoBehaviour<WeaponManager>
    {

        public Weapon[] weapons;

        private Dictionary<string, int> weaponLookup = new Dictionary<string, int>();



        protected override void Awake()
        {
            base.Awake();
            //  Stay Persistent throught scenes.



            Init();
        }


        public void Init()
        {
            for (int i = 0; i < weapons.Length; i ++)
            {
                if(weaponLookup.ContainsKey(weapons[i].id))
                {
                    
                }
                else
                {
                    weaponLookup.Add(weapons[i].id, i);
                }
            }
        }


        public Weapon GetWeapon(string id)
        {
            Weapon obj = null;
            int index = -1;
            if(weaponLookup.TryGetValue(id, out index))
            {
                obj = weapons[index];
            }

            return obj;
        }





    }


    [System.Serializable]
    public class Weapon
    {
        public string id;

        public GameObject prefab;

        public float fireRate = 1f;

        public int ammo = 6;

        public int maxAmmo = 18;


    }
}


