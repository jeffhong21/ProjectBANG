namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;

    public class GameModeManager : SingletonMonoBehaviour<GameModeManager>
    {
        //
        //  Readonly
        //
        protected readonly string respawnPointTag = "RespawnPoint";
        protected readonly string[] roundStartingText = { " 3 ", " 2 ", " 1 ", "Draw!" };


        //
        //  Fields
        //
        public int numberRoundsToWin = 1;
        public float timePerRound = 180f;
        public float respawnTime = 3f;
        public bool skipIntro;
        public bool placeAgents;

        [SerializeField, ReadOnly]
        protected int currentRound;
        [SerializeField]
        protected ActorManager[] actors;
        [SerializeField, ReadOnly]
        protected GameObject[] respawnPoints;

        public Text displayMessage;

            
        //
        //  Properties
        //
        public int CurrentRound{
            get { return currentRound; }
        }

        public ActorManager[] Actors{
            get { return actors; }
        }

        public GameObject[] RespawnPoints{
            get { return respawnPoints; }
        }


        //
        //  Methods
        //
        protected override void Awake()
		{
            base.Awake();
            //DontDestroyOnLoad(gameObject);
		}


        private void Start()
        {
            respawnPoints = GameObject.FindGameObjectsWithTag(respawnPointTag);

            if(!placeAgents)
                SpawnPlayers();

            //  Start the game.
            StartCoroutine(GameLoop());
        }



        private void SpawnPlayers()
        {
            for (int i = 0; i < Actors.Length; i++)
            {
                if (respawnPoints.Length > 0)
                {
                    //SuffleSpawnPoints(RespawnPoints);
                    Actors[i].instance = Instantiate(Actors[i].prefab, respawnPoints[i].transform.position, Quaternion.Euler(0, 180, 0));
                    Actors[i].DisableControls();
                }
                else
                {
                    Vector3 defaultSpawn = Random.insideUnitCircle * 5;
                    defaultSpawn.y = 0;
                    Actors[i].instance = Instantiate(Actors[i].prefab, defaultSpawn, Quaternion.Euler(0, 180, 0));
                    Actors[i].DisableControls();
                }
            }
            SetActorControls(false);
        }


        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(PlaceAgents());

            if (skipIntro == false)
            {
                yield return StartCoroutine(RoundStarting());
            }

            yield return StartCoroutine(RoundPlaying());

            yield return StartCoroutine(RoundEnding());

            //if (gameWinner != null)
            //{
            //    Debug.Log(string.Format("Winner of the game is {0}, Starting Next Game", gameWinner.playerName));
            //    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //}
            //else
            //{
            //    Debug.Log(" No Game Winner, Starting Next Round");
            //    StartCoroutine(GameLoop());
            //}
        }



        private IEnumerator PlaceAgents()
        {
            yield return null;
        }




        private IEnumerator RoundStarting()
        {
            Debug.Log("RoundStarting");

            //for (int i = 0; i < roundStartingText.Length; i ++)
            //{
            //    //  Set Round Starting TExt to be active.
            //    yield return new WaitForSeconds(0.5f);
            //    //  Set the text for each second.
            //    //  
            //}

            //if (GameManager.Instance.isRoundPlaying == false)
            //{
            //    //ResetAllPlayers();
            //    //DisablePlayerControl();

            //    displayMessage.gameObject.SetActive(true);
            //    yield return new WaitForSeconds(0.5f);
            //    displayMessage.text = "3";
            //    yield return new WaitForSeconds(1f);
            //    displayMessage.text = "2";
            //    yield return new WaitForSeconds(1f);
            //    displayMessage.text = "1";
            //    yield return new WaitForSeconds(1f);
            //    displayMessage.text = "GO!";
            //}
            //else
            //{
            //    yield return null;
            //}


            //displayMessage.gameObject.SetActive(true);
            //yield return new WaitForSeconds(0.5f);
            //displayMessage.text = "3";
            //yield return new WaitForSeconds(1f);
            //displayMessage.text = "2";
            //yield return new WaitForSeconds(1f);
            //displayMessage.text = "1";
            //yield return new WaitForSeconds(1f);
            //displayMessage.text = "GO!";

            yield return null;
        }

        private IEnumerator RoundPlaying()
        {
            //Debug.Log("RoundPlaying");
            SetActorControls(true);

            //EnablePlayerControl();
            //GameManager.Instance.isRoundPlaying = true;
            //yield return new WaitForSeconds(1);
            //GameManager.Instance.messageText.text = "";
            ////GameManager.Instance.messageText.gameObject.SetActive(false);


            //while (OnePlayerLeft() == false)
            //{
            //    yield return null;
            //}
            yield return null;
        }

        private IEnumerator RoundEnding()
        {
            //Debug.Log("Round Ending");
            //SetActorControls(false);


            //yield return new WaitForSeconds(1);
            //DisablePlayerControl();

            //roundWinner = null;

            //roundWinner = GetRoundWinner();

            //string endMessage = roundWinner.playerName + " WINS!";

            //GameManager.Instance.messageText.text = endMessage;

            //yield return new WaitForSeconds(3);

            //gameWinner = roundWinner;

            yield return null;
        }


        //void ResetAllPlayers()
        //{

        //}



        private void SetActorControls(bool enable)
        {
            for (int i = 0; i < Actors.Length; i++)
            {
                if(enable)
                {
                    Actors[i].EnableControls();
                }
                else
                {
                    Actors[i].DisableControls();
                }
            }
        }


        //bool OnePlayerLeft()
        //{
        //    int numPlayersLeft = players.Length;
        //    // Go through all the players...
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        // ... and if they are active, increment the counter.
        //        // if (players[i].playerInstance.activeSelf){
        //        if (players[i].lives == 0)
        //        {
        //            numPlayersLeft--;
        //        }
        //    }
        //    // If there are one or fewer players remaining return true, otherwise return false.
        //    return numPlayersLeft <= 1; ;
        //}


        //PlayerManager GetRoundWinner()
        //{
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        if (players[i].lives > 0)
        //            return players[i];
        //    }

        //    return null;
        //}


        //PlayerManager GetGameWinner()
        //{
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        if (players[i].wins == numRoundstoWin)
        //            return players[i];
        //    }

        //    return null;
        //}



        //private GameObject[] SuffleSpawnPoints(GameObject[] a)
        //{
        //    for (int i = 0; i < a.Length - 1; i--)
        //    {
        //        // Randomize a number between 0 and i (so that the range decreases each time)
        //        int rnd = Random.Range(0, i);
        //        // Save the value of the current i, otherwise it'll overright when we swap the values
        //        GameObject temp = a[i];
        //
        //        a[i] = a[rnd];
        //        a[rnd] = temp;
        //    }
        //
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        Debug.Log(a[i]);
        //    }
        //    return a;
        //}
    }
}


