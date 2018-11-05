namespace Bang
{
    using UnityEngine;
    using System;


    [Serializable]
    public class ActorManager
    {
        //
        //  Fields
        //
        public string playerName;

        public int teamID;

        private GameObject instance;

        private ActorController controller;
        [SerializeField]
        private int deaths;



        //
        //  Properties
        //
        public GameObject Instance{
            get { return instance; }
        }

        public int Deaths{
            get { return deaths; }
        }


        //
        //  Methods
        //

        public void SetInstance(GameObject instance){
            this.instance = instance;
        }


        public void InitializeActor()
        {
            controller = instance.GetComponent<ActorController>();
            controller.InitializeActor(this);
            playerName = instance.name;
            RegisterEvents();
        }


        public void RegisterEvents(){
            controller.OnDeathEvent += OnDeath;
        }

        public void UnregisterEvents(){
            controller.OnDeathEvent -= OnDeath;
        }


        public void SetTeam(string teamID)
        {
            //instance.tag = teamID;
            //instance.layer = LayerMask.NameToLayer(teamID);
        }


        private void OnRespawn(Vector3 position){
            RegisterEvents();
        }


        private void OnDeath(GameObject attacker)
        {
            //Debug.LogFormat("{0} was killed by {1}", instance.name, attacker.name);
            deaths++;

            UnregisterEvents();
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


