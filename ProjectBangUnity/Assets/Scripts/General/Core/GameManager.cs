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
        private PoolManager _poolManager;
        [SerializeField]
        private HUDState _hudState;
        [SerializeField]
        private PauseManager _pauseManager;
        [SerializeField]
        private CameraController _playerCamera;
        [SerializeField]
        private FirearmBase _defaultWeapon;


        //
        //  Properties
        //
        public PoolManager PoolManager{
            get { return _poolManager; }
        }

        public HUDState HudState{ 
            get { return _hudState;}
        }

        public PauseManager PauseManager { 
            get { return _pauseManager; } 
        }

        public CameraController PlayerCamera
        {
            get { return _playerCamera; }
        }

        public FirearmBase defaultWeapon
        {
            get { return _defaultWeapon; }
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
                _poolManager = Instantiate(PoolManager);
            }

            if(FindObjectOfType<HUDState>() == null){
                _hudState = Instantiate(HudState);
            }

            if (FindObjectOfType<PauseManager>() == null){
                _pauseManager = Instantiate(PauseManager);
            }

            _playerCamera = Camera.main.GetComponent<CameraController>();


            if (_defaultWeapon == null) throw new ArgumentNullException("GameManager has no default weapon.");

            //InstantiatePlayers();
            //  Stay Persistent throught scenes.
            DontDestroyOnLoad(gameObject);
		}


		//private void Start()
		//{
        //   //InstantiatePlayers();
		//}


		////  Removes any Gameobject with designated tag.
		//private void RemoveAssetsWithTag(string assetTag, bool debugLog = false)
  //      {
  //          string assetsRemoved = "Assets with tag ("+ assetTag + ") that were removed:\n";
  //          GameObject[] assets = GameObject.FindGameObjectsWithTag(assetTag);
  //          if (assets.Length > 0){
  //              foreach (GameObject asset in assets){
  //                  assetsRemoved += asset.name + "\n";
  //                  Destroy(asset);
  //              }
  //          }
  //          else{
  //              assetsRemoved = "No assets with tag (" + assetTag + ") removed.";
  //          }
  //          if(debugLog) Debug.Log(assetsRemoved);
  //      }


		//private void InstantiatePlayers()
        //{
        //    if(players.doNotSpawn == false)
        //        players.playerInstance = Instantiate(players.playerPrefab, Vector3.zero, Quaternion.Euler(0,180,0));
        //}



	}




    //[Serializable]
    //public class PlayerManager
    //{
    //    public bool doNotSpawn;
    //    //public string playerName;
    //    public PlayerInput playerPrefab;
    //    //[HideInInspector]
    //    public PlayerInput playerInstance;
    //    public float startingHealth = 4f;

    //}
}



