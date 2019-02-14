namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;


    public static class WeaponNameIDs
    {
        public static string Revolver_01 = "Single Action Revolver";
        public static string Revolver_02 = "Colt Action Revolver";
        public static string Rifle_01 = "Springfield Rifle";
        public static string IK_Rifle = "IK_Rifle";
        public static string Shotgun = "Shotgun";
    }



    [CreateAssetMenu(menuName = "Resources Manager/WeaponsManager")]
    public class WeaponManager : ScriptableObject
    {
        [SerializeField]
        private WeaponObject[] weapons;


        private static Dictionary<string, int> weaponLookup;


		private void OnEnable(){
            Initialize();
		}


		public void Initialize()
        {
            if(weaponLookup == null){
                weaponLookup = new Dictionary<string, int>();
            }

            for (int i = 0; i < weapons.Length; i ++)
            {
                if(weaponLookup.ContainsKey(weapons[i].nameID))
                {
                    
                }
                else
                {
                    weaponLookup.Add(weapons[i].nameID, i);
                }
            }

        }


        public WeaponObject GetWeapon(string id)
        {
            WeaponObject obj = null;
            int index = -1;
            if(weaponLookup.TryGetValue(id, out index)){
                obj = weapons[index];
            }

            return obj;
        }





    }


}


