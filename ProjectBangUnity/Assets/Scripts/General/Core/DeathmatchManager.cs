namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


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
        public GameObject cameraPrefab;
        public GameObject agentPrefab;


        public DeathmatchSettings settings;
        public DeathmatchDebug debug;



        private ActorManager[] actors;
        private GameObject[] spawnPoints;
        private WaitForSeconds startWait;
        private WaitForSeconds endWait;



		private void Start()
		{
            startWait = new WaitForSeconds(startDelay);
            endWait = new WaitForSeconds(endDelay);

            actors = new ActorManager[playerCount + 1];
            spawnPoints = GameObject.FindGameObjectsWithTag(respawnPointTag);


            //  Spawn players.
            SpawnPlayers();


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
        }


        private void SpawnPlayers()
        {
            if(spawnPoints.Length > 0){
                for (int i = 0; i < actors.Length; i++){
                    actors[i] = new ActorManager();

                    //  Instantiate actors.
                    if(i == 0){
                        actors[i].instance = Instantiate(playerPrefab, spawnPoints[i].transform.position, Quaternion.Euler(0, 180, 0));
                    } else {
                        actors[i].instance = Instantiate(agentPrefab, spawnPoints[i].transform.position, Quaternion.Euler(0, 180, 0));
                    }

                    //  Initialize Actors:
                    actors[i].InitializeActor();

                    //  Disable controls.
                    actors[i].DisableControls();
                }
            }
            else{
                Debug.Log(" DeathmatchManager:  No Spawn Points");
                for (int i = 0; i < actors.Length; i++){
                    actors[i] = new ActorManager();
                    Vector3 defaultSpawn = Random.insideUnitCircle * 5;
                    defaultSpawn.y = 0;

                    //  Instantiate actors.
                    if(i == 0){
                        actors[i].instance = Instantiate(playerPrefab, defaultSpawn, Quaternion.Euler(0, 180, 0));
                    } else {
                        actors[i].instance = Instantiate(agentPrefab, defaultSpawn, Quaternion.Euler(0, 180, 0));
                    }

                    //  Initialize Actors:
                    actors[i].InitializeActor();

                    //  Disable controls.
                    actors[i].DisableControls();
                }
            }

            SetActorControls(false);
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
            Debug.Log(roundStartingText[0]);
            yield return startWait;
            Debug.Log(roundStartingText[1]);
            yield return startWait;
            Debug.Log(roundStartingText[2]);
            yield return startWait;
            Debug.Log(roundStartingText[3]);


            yield return null;
        }


        private IEnumerator RoundPlaying()
        {
            SetActorControls(true);

            while(OnePlayerLeft() == false)
            {
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            SetActorControls(false);

            Debug.Log("Round Ending");

            yield return endWait;
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
            int numPlayersLeft = 0;
            // Go through all the players...
            for (int i = 0; i < actors.Length; i++)
            {
                // ... and if they are active, increment the counter.
                // if (players[i].playerInstance.activeSelf){
                if (actors[i].instance.activeSelf)
                    numPlayersLeft++;
                
                //if (actors[i].lives == 0)
                //{
                //    numPlayersLeft--;
                //}
            }
            // If there are one or fewer players remaining return true, otherwise return false.
            return numPlayersLeft <= 1; ;
        }




        [System.Serializable]
        public class DeathmatchSettings
        {
            public float timePerRound = 180f;
        }

        [System.Serializable]
        public class DeathmatchDebug
        {
            public bool skipIntro;
            public bool doNotStartGameLoop;
        }

	}

}
