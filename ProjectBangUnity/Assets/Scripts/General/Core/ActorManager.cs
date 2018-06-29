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
        public GameObject prefab;
        [HideInInspector]
        public GameObject instance;
        [ReadOnly]
        public int id;
        [ReadOnly]
        public int lives;
        [ReadOnly]
        public int wins;
        [ReadOnly]
        public int kills;
        [ReadOnly]
        public int deaths;



        public void Reset(){
            
        }


        public void EnableControls()
        {
            instance.GetComponent<IActorController>().EnableControls();
        }


        public void DisableControls()
        {
            instance.GetComponent<IActorController>().DisableControls();
        }

    }






}


