namespace Bang
{
    using UnityEngine;
    using System;


    [Serializable]
    public class ActorManager
    {
        
        public string name;

        public int teamID;

        public bool isBot;

        public GameObject prefab;
        [HideInInspector]
        public GameObject instance;
        [HideInInspector]
        public int lives;

        public void Reset(){
            
        }


        public void InitializeActor()
        {
            instance.GetComponent<ActorController>().Init(this);   
        }


        public void SetTeam(string teamID)
        {
            //instance.tag = teamID;
            //instance.layer = LayerMask.NameToLayer(teamID);
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


