namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public class Team
    {
        public string name;
        public int score;
        public int playerCount;
    }

    public class TeamManager : SingletonMonoBehaviour<TeamManager>
    {
     
        public List<Team> teams = new List<Team>();

        [HideInInspector]
        public string teamName0;
        [HideInInspector]
        public string teamName1;



        public void CreateTeam(string name)
        {
            var team = new Team();
            team.name = name;
            teams.Add(team);

            // Update clients
            var idx = teams.Count - 1;
            if (idx == 0) teamName0 = name;
            if (idx == 1) teamName1 = name;
        }


        public void AssignTeam(ActorManager player)
        {
            var players = DeathmatchManager.instance.players;
            //  How many players per team.
            int[] teamCount = new int[teams.Count];


            for (int i = 0, c = players.Length; i < c; i++)
            {
                //  If player teamId is less than the total team count, add that player to that teams player count.
                var idx = players[i].teamId;
                if (idx < teamCount.Length)
                    teamCount[idx]++;
            }


            int joinIndex = -1;
            int smallestTeamSize = 1000;
            for (int i = 0, c = teams.Count; i < c; i++)
            {
                //  If number of players in teamCount is less than smallestTeamSize, than thats the smallest team.
                if(teamCount[i] < smallestTeamSize){
                    smallestTeamSize = teamCount[i];
                    joinIndex = i;
                }
            }

            player.teamId = joinIndex < 0 ? 0 : joinIndex;
            //Debug.Log("Assigned team " + joinIndex + " to player " + player);
        }


        public void GetRandomSpawnTransform(int teamId, ref Vector3 pos, ref Quaternion rot)
        {
            
        }




    }

}
