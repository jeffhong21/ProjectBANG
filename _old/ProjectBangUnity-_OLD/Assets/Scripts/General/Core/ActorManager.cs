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
        public int playerId;
        public string playerName;
        public int teamId;

        public int score;
        public int deaths;

        public GameObject killedBy;

        private GameObject instance;
        private ActorController controller;



        //
        //  Properties
        //
        public GameObject Instance{
            get { return instance; }
        }

        public bool IsDead{
            get;
            private set;
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


        public void SetTeamColor(Color teamColor)
        {
            var charHealthUI = instance.GetComponentInChildren<CharacterHealthUI>();
            if(charHealthUI != null){
                charHealthUI.useTeamColor = true;
                charHealthUI.teamColor = teamColor;
            }

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


        private void OnRespawn(Vector3 position)
        {
            killedBy = null;
            IsDead = false;

            RegisterEvents();
        }


        private void OnDeath(GameObject attacker)
        {
            Debug.LogFormat("<color=#008000ff>{0}</color> was killed by <color=#008000ff>{1}</color>", instance.name, attacker.name);
            deaths++;
            IsDead = true;
            killedBy = attacker;

            DeathmatchManager.instance.hud.UpdateGameScore(this);
            DeathmatchManager.instance.teamManager.teams[teamId].playerCount--;
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


