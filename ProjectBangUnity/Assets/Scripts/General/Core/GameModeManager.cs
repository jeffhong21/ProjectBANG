namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class GameModeManager : SingletonMonoBehaviour<GameModeManager>
    {

        public GameModeTypes gameMode;
        public string[] roundStartingText = { "3", "2", "1", "Draw!" };
        public GameModeBase shootout = new GameModeBase();



        public TeamManager roundWinner;
        public TeamManager gameWinner;


		protected override void Awake()
		{
            base.Awake();


            DontDestroyOnLoad(gameObject);
		}



        protected virtual void Start()
        {
            switch (gameMode)
            {
                case GameModeTypes.Shootout:
                    shootout.Setup();
                    break;
            }

            //StartCoroutine(GameLoop());
        }

        protected virtual IEnumerator GameLoop()
        {
            //if (GameManager.Instance.skipIntro == false)
            //{
            //    yield return StartCoroutine(RoundStarting());
            //}
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

        protected virtual IEnumerator RoundStarting()
        {
            //Debug.Log("RoundStarting");
            //if (GameManager.Instance.isRoundPlaying == false)
            //{
            //    ResetAllPlayers();
            //    DisablePlayerControl();

            //    GameManager.Instance.messageText.gameObject.SetActive(true);
            //    yield return new WaitForSeconds(0.5f);
            //    GameManager.Instance.messageText.text = "3";
            //    yield return new WaitForSeconds(1f);
            //    GameManager.Instance.messageText.text = "2";
            //    yield return new WaitForSeconds(1f);
            //    GameManager.Instance.messageText.text = "1";
            //    yield return new WaitForSeconds(1f);
            //    GameManager.Instance.messageText.text = "GO!";
            //}
            //else
            //{
            //    yield return null;
            //}

            yield return null;
        }

        protected virtual IEnumerator RoundPlaying()
        {
            //Debug.Log("RoundPlaying");
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

        protected virtual IEnumerator RoundEnding()
        {
            //Debug.Log("Round Ending");
            //GameManager.Instance.isRoundPlaying = false;
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


        //void DisablePlayerControl()
        //{
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        players[i].DisableControl();
        //    }
        //}


        //void EnablePlayerControl()
        //{
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        players[i].EnableControl();
        //    }
        //}



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


    }
}


