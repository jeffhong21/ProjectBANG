namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;



    [RequireComponent(typeof(TeamManager))]
    public class DeathmatchManager : SingletonMonoBehaviour<DeathmatchManager>
    {
        private readonly string[] roundStartingText = { " 3 ", " 2 ", " 1 ", "Draw!" };
        protected readonly string respawnPointTag = "RespawnPoint";
        private readonly float startDelay = 0.75f;
        private readonly float endDelay = 3f;


        public int playerCount;
        public int playersPerTeam;
        public int teamCount;
        public bool teamGame;


        public GameObject playerPrefab;
        public CameraController cameraPrefab;
        public GameObject agentPrefab;
        public HUDState hud;
        public string parentName = "Players";


        public DeathmatchSettings settings;
        public PrimaryTeamColors primaryColorSettings;
        public DeathmatchDebug debug;

        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private ActorManager[] actors;


        private GameObject parentObject;
        private GameObject[] spawnPoints;
        private WaitForSeconds startWait;
        private WaitForSeconds endWait;



        protected override void Awake()
		{
            base.Awake();

            if (playerCount <= 0) playerCount = 1;


            startWait = new WaitForSeconds(startDelay);
            endWait = new WaitForSeconds(endDelay);

            //playerCount = playerCount + 1;
            actors = new ActorManager[playerCount];
            spawnPoints = GameObject.FindGameObjectsWithTag(respawnPointTag);

            //  Setup parent object.
            if (parentObject == null){
                if(string.IsNullOrWhiteSpace(parentName) == false){
                    parentObject = new GameObject();
                    parentObject.name = parentName;
                    parentObject.transform.localPosition = Vector3.zero;
                    parentObject.transform.localEulerAngles = Vector3.zero;
                    parentObject.transform.localScale = Vector3.one;
                }
                //else{
                //    Debug.Log("There was no ParentObject Name specified.  No ParentObject created.");
                //}
            }
		}


		private void Start()
		{
            //  Setup Teams.
            SetupTeams();
            //  Spawn players.
            SpawnPlayers();
            //  Initiate cameraa.
            SetupCamera(player.transform);
            //  Initiate Player.
            SetupHUD(player);
            //  Initiate Actors.
            SetupActors();



            if (debug.doNotStartGameLoop){
                SetActorControls(true);
                return;
            }
            //  Start game loop.
            StartCoroutine(GameLoop());
		}



        private void SetupTeams()
        {
            if (teamCount < 2) teamCount = 2;
            int remainderPlayers = playerCount % teamCount;
            int actorsPerTeam = (playerCount - remainderPlayers) / 2;
            //Debug.LogFormat(" Number of Players: {0} \n Players Per Team: {1} \n Remainder of Players: {2}", playerCount, actorsPerTeam, remainderPlayers);
        }




        private void SpawnPlayers()
        {
            Vector3 spawnLocation;

            for (int i = 0; i < actors.Length; i++)
            {
                actors[i] = new ActorManager();

                //  Check if there are spawn points.
                if (spawnPoints.Length > 0 || spawnPoints[i] != null){
                    spawnLocation = spawnPoints[i].transform.position;
                }
                else{
                    spawnLocation = Random.insideUnitCircle * 5;
                    spawnLocation.y = 0;
                }

                //  Instantiate actors.
                if (i == 0)
                {
                    var go = Instantiate(playerPrefab, spawnLocation, Quaternion.Euler(0, 180, 0));
                    actors[i].SetInstance(go);
                    player = actors[i].Instance.GetComponent<PlayerController>();
                }
                else
                {
                    var go = Instantiate(agentPrefab, spawnLocation, Quaternion.Euler(0, 180, 0));
                    actors[i].SetInstance(go);
                    //actors[i].instance = Instantiate(agentPrefab, spawnPoints[i].transform.position, Quaternion.Euler(0, 180, 0));
                }

                //  Disable controls.
                actors[i].DisableControls();


                if(parentObject != null){
                    //  Parent the Actors.
                    actors[i].Instance.transform.parent = parentObject.transform;
                }
            }

            //  Disable controls.
            SetActorControls(false);
        }


        private void SetupHUD(PlayerController p){
            hud.InitializeHUD(p);
        }


        private void SetupCamera(Transform t){
            cameraPrefab.SetTarget(t);
        }


        private void SetupActors(){
            for (int i = 0; i < actors.Length; i++)
            {
                //  Initialize Actors:
                actors[i].InitializeActor();
            }
        }





        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());

            yield return StartCoroutine(RoundPlaying());

            yield return StartCoroutine(RoundEnding());


            yield return null;
        }


        private IEnumerator RoundStarting()
        {
            yield return new WaitForSeconds(0.5f);
            //Debug.Log(roundStartingText[0]);
            hud.SetMessage(roundStartingText[0]);
            yield return startWait;
            //Debug.Log(roundStartingText[1]);
            hud.SetMessage(roundStartingText[1]);
            yield return startWait;
            //Debug.Log(roundStartingText[2]);
            hud.SetMessage(roundStartingText[2]);
            yield return startWait;
            //Debug.Log(roundStartingText[3]);
            hud.SetMessage(roundStartingText[3]);
            yield return startWait;
            hud.SetMessage("");

            yield return null;
        }



        private IEnumerator RoundPlaying()
        {
            UpdateSideMEssage();

            SetActorControls(true);

            while(OnePlayerLeft() == false)
            {
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            Debug.Log("Round Ending");
            UpdateSideMEssage();


            SetActorControls(false);
            yield return new WaitForSeconds(1);

            ActorManager roundWinner = null;
            roundWinner = GetRoundWinner();

            string endMessage = roundWinner.Instance.name + " WINS!";

            hud.SetMessage(endMessage);

            yield return endWait;
        }


        private string sideMessage = "";
        private void UpdateSideMEssage()
        {
            sideMessage = "";
            for (int i = 0; i < actors.Length; i++)
            {
                var actorHealth = actors[i].Instance.GetComponent<ActorHealth>();
                sideMessage += string.Format("{0} | Is Dead:  {1}\n", actors[i].Instance.name, actorHealth.IsDead);
            }
            hud.UpdateSideMessage(sideMessage);
        }


        /// <summary>
        /// Enables or disables actors controls.
        /// </summary>
        /// <param name="enable">If set to <c>true</c> enable.</param>
        private void SetActorControls(bool enable)
        {
            for (int i = 0; i < actors.Length; i++){
                if (enable){
                    actors[i].EnableControls();}
                else{
                    actors[i].DisableControls();
                }
            }
        }



        private bool OnePlayerLeft()
        {
            int numPlayersLeft = playerCount;
            // Go through all the players...
            for (int i = 0; i < actors.Length; i++)
            {
                // ... and if they are active, increment the counter.
                // if (players[i].playerInstance.activeSelf){
                if (actors[i].Deaths > 0)
                    numPlayersLeft--;
                
                //if (actors[i].lives == 0)
                //{
                //    numPlayersLeft--;
                //}
            }
            // If there are one or fewer players remaining return true, otherwise return false.
            return numPlayersLeft <= 1; ;
        }


        private ActorManager GetRoundWinner()
        {
            for (int i = 0; i < actors.Length; i++)
            {
                if (actors[i].Deaths == 0)
                    return actors[i];
            }
            return null;
        }



        [System.Serializable]
        public class DeathmatchSettings
        {
            public float timePerRound = 180f;
        }


        [System.Serializable]
        public class PrimaryTeamColors
        {
            public Color redTeam = Color.red;
            public Color blueTeam = Color.blue;
        }


        [System.Serializable]
        public class DeathmatchDebug
        {
            public bool skipIntro;
            public bool doNotStartGameLoop;
        }

	}

}
