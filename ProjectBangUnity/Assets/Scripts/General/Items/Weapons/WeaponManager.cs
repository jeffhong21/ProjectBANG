namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;


    public static class WeaponNameIDs
    {
        public static string Revolver_01 = "Revolver_01";
        public static string Rifle_01 = "Rifle_01";
    }



    [CreateAssetMenu(menuName = "Resources Manager/WeaponsManager")]
    public class WeaponManager : ScriptableObject
    {
        [Header("******* Guns ********")]
        public Weapon[] weapons;


        private static Dictionary<string, int> weaponLookup = new Dictionary<string, int>();


		private void OnEnable()
		{
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

        public Gun prefab;

        public float fireRate = 1f;

        public int ammo = 6;

        public int maxAmmo = 6;
    }

}


