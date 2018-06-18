namespace Bang
{
    using UnityEngine;
    using System;


    [Serializable]
    public class ActorManager
    {
        
        public string name;

        public TeamTypes team;

        public bool isBot;
        [ReadOnly]
        public int id;
        [ReadOnly]
        public int wins;
        [ReadOnly]
        public int kills;
        [ReadOnly]
        public int deaths;





        public void Reset(){
            
        }


        public void EnableControls(){
            
        }


        public void DisableControls(){
            
        }

    }






}


