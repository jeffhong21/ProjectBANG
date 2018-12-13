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

        public PrimaryTeamColors primaryColorSettings;
        public DeathmatchDebug debug;

        private PlayerController _player;
        public ActorManager[] players;

        [HideInInspector]
        public TeamManager teamManager;
        private GameObject parentObject;
        private GameObject[] spawnPoints;
        private WaitForSeconds startWait;
        private WaitForSeconds endWait;



        protected override void Awake()
		{
            base.Awake();
            //if (GameManager.instance.doNotDestroyOnLoad){
            //    DontDestroyOnLoad(this.gameObject);
            //}

            hud = Instantiate(hud) as HUDState;
            cameraPrefab = Instantiate(cameraPrefab) as CameraController;

            if (playerCount <= 0) playerCount = 1;
            players = new ActorManager[playerCount];
            //  Setup parent object.
            if (parentObject == null){
                if(string.IsNullOrWhiteSpace(parentName) == false){
                    parentObject = new GameObject();
                    parentObject.name = parentName;
                    parentObject.transform.localPosition = Vector3.zero;
                    parentObject.transform.localEulerAngles = Vector3.zero;
                    parentObject.transform.localScale = Vector3.one;
                }
            }


            if(teamGame) teamManager = GetComponent<TeamManager>();
           
            startWait = new WaitForSeconds(startDelay);
            endWait = new WaitForSeconds(endDelay);

            //  Get spawn points.
            spawnPoints = GameObject.FindGameObjectsWithTag(respawnPointTag);


		}


		private void Start()
		{
            Initialize();


            if (debug.doNotStartGameLoop){
                SetActorControls(true);
                return;
            }
            //  Start game loop.
            StartCoroutine(GameLoop());
		}

        void Team()
        {
            if (teamCount < 2) teamCount = 2;
            int remainderPlayers = playerCount % teamCount;
            int actorsPerTeam = (playerCount - remainderPlayers) / 2;
            //Debug.LogFormat(" Number of Players: {0} \n Players Per Team: {1} \n Remainder of Players: {2}", playerCount, actorsPerTeam, remainderPlayers);
        }


        private void Initialize()
        {
            if(teamGame){
                teamManager.CreateTeam("Team 1");
                teamManager.CreateTeam("Team 2");
            }

            Vector3 spawnLocation;
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new ActorManager();
                players[i].playerId = i;
                players[i].teamId = teamGame ? i % 2 : -1;
                teamManager.teams[players[i].teamId].playerCount++;

                //  Check if there are spawn points.
                if (spawnPoints.Length > 0 || spawnPoints[i] != null){
                    spawnLocation = spawnPoints[i].transform.position;
                } else {
                    spawnLocation = Random.insideUnitCircle * 5;
                    spawnLocation.y = 0;
                }

                var prefab = i == 0 ? playerPrefab : agentPrefab;
                var position = spawnLocation;
                var rotation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                var go = Instantiate(prefab, position, rotation);
                players[i].SetInstance(go);

                //if (teamGame) teamManager.AssignTeam(players[i]);


                //  Set controls.
                players[i].InitializeActor();
                players[i].DisableControls();
                if (parentObject != null) players[i].Instance.transform.parent = parentObject.transform;

                //  Set human player variable.
                if (i == 0) _player = players[0].Instance.GetComponent<PlayerController>();
            }
            //  Initialize Player HUD and Camera
            hud.InitializeHUD(_player);
            cameraPrefab.SetTarget(_player.transform);



            if(teamGame){
                Color teamColor1 = primaryColorSettings.teamColor1;
                Color teamColor2 = primaryColorSettings.teamColor2;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].teamId == 0)
                        players[i].SetTeamColor(teamColor1);
                    else
                        players[i].SetTeamColor(teamColor2);
                }
            }
        }



        private IEnumerator GameLoop()
        {
            if (debug.skipIntro == false){
                yield return StartCoroutine(RoundStarting());
            }

            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());

            yield return null;
        }


        private IEnumerator RoundStarting()
        {
            int count = 0;
            while (count < 3){
                hud.SetMessage(roundStartingText[count]);
                yield return startWait;
                count++;
            }
            //Debug.Log(roundStartingText[3]);
            hud.SetMessage(roundStartingText[3]);
            yield return startWait;
            hud.SetMessage("");
            yield return null;

        }


        string winMessage;
        private IEnumerator RoundPlaying()
        {
            SetActorControls(true);

            bool matchEnd = false;

            if(teamGame){
                while(matchEnd == false)
                {
                    //for (int i = 0; i < players.Length; i++){
                    //    var player = players[i];
                    //    if(player.IsDead){
                    //        OnPlayerKilled(player, player.killedBy);
                    //    }
                    //}

                    if (teamManager.teams[0].playerCount == 0){
                        matchEnd = true;
                        winMessage = "Team 2 WINS";
                    }
                    if (teamManager.teams[1].playerCount == 0){
                        matchEnd = true;
                        winMessage = "Team 1 WINS";
                    }

                    if (teamManager.teams[0].playerCount == 0 && teamManager.teams[1].playerCount == 0){
                        matchEnd = true;
                        winMessage = "Draw";
                    }
                    yield return null;
                }


            }
            else{
                int numPlayersLeft = playerCount;
                while (matchEnd == false)
                {
                    for (int i = 0; i < players.Length; i++){
                        if (players[i].deaths > 0)
                            numPlayersLeft--;
                    }
                    if(numPlayersLeft <= 1){
                        matchEnd = true;
                        winMessage = "DRAW";
                    }
                        
                    yield return null;
                }
            }

        }



        private IEnumerator RoundEnding()
        {
            SetActorControls(false);
            yield return new WaitForSeconds(1);

            ActorManager roundWinner = null;
            roundWinner = GetRoundWinner();

            string endMessage = winMessage;

            hud.SetMessage(endMessage);

            yield return endWait;
        }



        public void OnPlayerKilled(ActorManager victim, ActorManager killer)
        {
            if(killer != null){
                if(killer.teamId != victim.teamId){
                    killer.score++;
                    teamManager.teams[killer.teamId].score++;
                }
            }
        }


        private void SetActorControls(bool enable){
            for (int i = 0; i < players.Length; i++){
                if (enable){
                    players[i].EnableControls();}
                else{
                    players[i].DisableControls();
                }
            }
        }


        private bool OnePlayerLeft(){
            int numPlayersLeft = playerCount;
            // Go through all the players...
            for (int i = 0; i < players.Length; i++)
            {
                // ... and if they are active, increment the counter.
                // if (players[i].playerInstance.activeSelf){
                if (players[i].deaths > 0)
                    numPlayersLeft--;
            }
            // If there are one or fewer players remaining return true, otherwise return false.
            return numPlayersLeft <= 1; ;
        }


        private ActorManager GetRoundWinner()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].deaths == 0)
                    return players[i];
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
            public Color teamColor1 = new Color(1, 0, 0, 1);
            public Color teamColor2 = new Color(0, 0, 1, 1);
        }


        [System.Serializable]
        public class DeathmatchDebug
        {
            public bool skipIntro;
            public bool doNotStartGameLoop;
        }




        //  Killer, Killed, Color1, Colro2
        static string[] KillMessages = new string[]
        {
        "<color={2}>{0}</color> killed <color={3}>{1}</color>",
        "<color={2}>{0}</color> terminated <color={3}>{1}</color>",
        "<color={2}>{0}</color> ended <color={3}>{1}</color>",
        "<color={2}>{0}</color> owned <color={3}>{1}</color>",
        };

        static string[] SuicideMessages = new string[]
        {
        "<color={1}>{0}</color> rebooted",
        "<color={1}>{0}</color> gave up",
        "<color={1}>{0}</color> slipped and accidently killed himself",
        "<color={1}>{0}</color> wanted to give the enemy team an edge",
        };

        static string[] TeamColors = new string[]
        {
        "#1EA00000", //"#FF19E3FF",
        "#1EA00001", //"#00FFEAFF",
        };
	}

}
