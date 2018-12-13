namespace Bang
{
    using UnityEngine;
    using System;


    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        //
        //  Fields
        //
        [SerializeField]
        public bool doNotDestroyOnLoad;
        [SerializeField]
        private PoolManager poolManager;
        [SerializeField]
        private WeaponManager weaponManager;
        [SerializeField]
        private string[] assetsWithTagsToRemove;


        //
        //  Properties
        //
        public PoolManager PoolManager{
            get { return poolManager; }
        }

        public WeaponManager WeaponManager
        {
            get { return weaponManager; }
        }



        //
        //  Methods
        //
        protected override void Awake()
		{
            base.Awake();
            //  Remove any player assets.
           // if(removeAssetsWithTag) RemoveAssetsWithTag(Tags.Player);

            if (FindObjectOfType<PoolManager>() == null){
                poolManager = Instantiate(PoolManager);
            }

            //if(doNotDestroyOnLoad){
            //    DontDestroyOnLoad(this.gameObject);
            //}
		}






		//  Removes any Gameobject with designated tag.
		private void RemoveAssetsWithTag(string assetTag, bool debugLog = false)
        {
            string debug_msg = "Assets with tag ("+ assetTag + ") that were removed:\n";
            GameObject[] assets = GameObject.FindGameObjectsWithTag(assetTag);

            if (assets.Length > 0){
                for (int i = 0; i < assets.Length; i++)
                {
                    debug_msg += assets[i].name + "\n";
                    Destroy(assets[i]);
                }
            }
            else{
                debug_msg = "No assets with tag (" + assetTag + ") removed.";
            }

            if(debugLog) Debug.Log(debug_msg);
        }




	}


}



