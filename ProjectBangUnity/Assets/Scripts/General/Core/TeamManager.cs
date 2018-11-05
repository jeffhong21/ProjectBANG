namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;


    public class TeamManager : SingletonMonoBehaviour<TeamManager>
    {
        public Team redTeam;






        public void InitializeTeams(int playersPerTeam)
        {
            redTeam = new Team(0, playersPerTeam);
        }







        public class Team
        {
            public int teamID;

            public int playersPerTeam;

            private List<ActorController> members;



            public Team(int teamID, int playersPerTeam)
            {
                this.teamID = teamID;
                this.playersPerTeam = playersPerTeam;

                members = new List<ActorController>();
            }


            public bool AddMember(ActorController player){

                if(members.Count > playersPerTeam){
                    return false;
                }

                members.Add(player);

                return true;
            }
        }
    }

}
