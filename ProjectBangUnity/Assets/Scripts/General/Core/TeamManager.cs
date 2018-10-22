namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;


    public class TeamManager : MonoBehaviour
    {

        public GameObject[] actors;


        //  playerID, teamID
        private Dictionary<string, int> players = new Dictionary<string, int>();



        public void RegisterPlayer()
        {
            
        }


        public void UnRegisterPlayer()
        {
            
        }


        //private void SpawnPlayers()
        //{
        //    for (int i = 0; i < Actors.Length; i++)
        //    {
        //        if (respawnPoints.Length > 0)
        //        {
        //            //SuffleSpawnPoints(RespawnPoints);
        //            Actors[i].instance = Instantiate(Actors[i].prefab, respawnPoints[i].transform.position, Quaternion.Euler(0, 180, 0));
        //            Actors[i].DisableControls();
        //        }
        //        else
        //        {
        //            Vector3 defaultSpawn = Random.insideUnitCircle * 5;
        //            defaultSpawn.y = 0;
        //            Actors[i].instance = Instantiate(Actors[i].prefab, defaultSpawn, Quaternion.Euler(0, 180, 0));
        //            Actors[i].DisableControls();
        //        }
        //    }
        //    SetActorControls(false);
        //}



        public class Teams
        {
            public GameObject[] players;


            public Teams(int numberPlayers)
            {
                players = new GameObject[numberPlayers];
            }
        }
    }

}
