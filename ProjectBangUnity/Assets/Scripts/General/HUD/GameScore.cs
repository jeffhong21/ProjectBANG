namespace Bang
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameScore : MonoBehaviour
    {
        public class GameScorePlayerIcon{
            public Image icon;
            public bool active;

            public GameScorePlayerIcon(Image i){
                icon = i;
                active = true;
            }
        }

        public Sprite spriteIcon;
        public Vector2 iconSize = new Vector2(50, 50);
        public RectTransform team1_holder;
        public RectTransform team2_holder;

        public Dictionary<int, GameScorePlayerIcon> ffa_lookup;
        public Dictionary<int, GameScorePlayerIcon> team1_lookup;
        public Dictionary<int, GameScorePlayerIcon> team2_lookup;


		private void Awake()
		{
            if(team1_holder == null || team2_holder == null){
                team1_holder = this.transform.GetChild(0) as RectTransform;
                team2_holder = this.transform.GetChild(1) as RectTransform;
                Debug.Log("GameScore missing holders.  Added in the missing holders.");
            }
		}


		public void Initialize()
		{
            ffa_lookup = new Dictionary<int, GameScorePlayerIcon>();
            team1_lookup = new Dictionary<int, GameScorePlayerIcon>();
            team2_lookup = new Dictionary<int, GameScorePlayerIcon>();

            for (int i = 0; i < DeathmatchManager.instance.players.Length; i++)
            {
                var player = DeathmatchManager.instance.players[i];
                var icon = new GameObject().AddComponent<Image>();
                icon.rectTransform.sizeDelta = iconSize;

                if (spriteIcon != null) icon.sprite = spriteIcon;

                if(DeathmatchManager.instance.teamGame)
                {
                    if (player.teamId == 0){
                        icon.rectTransform.parent = team1_holder;
                        team1_lookup.Add(player.playerId, new GameScorePlayerIcon(icon));
                        icon.color = DeathmatchManager.instance.primaryColorSettings.teamColor1;
                        continue;
                    }
                    if (player.teamId == 1){
                        icon.rectTransform.parent = team2_holder;
                        team2_lookup.Add(player.playerId, new GameScorePlayerIcon(icon));
                        icon.color = DeathmatchManager.instance.primaryColorSettings.teamColor2;
                        continue;
                    }

                    Debug.Log(player.playerName + " Has Wrong teamID");
                }
                else{
                    icon.color = Color.white;
                    ffa_lookup.Add(player.playerId, new GameScorePlayerIcon(icon));

                }
            }


            
        }


        public void UpdateScore(ActorManager player)
        {
            if(player.teamId == 0){
                if(team1_lookup.ContainsKey(player.playerId)){
                    team1_lookup[player.playerId].active = false;
                    team1_lookup[player.playerId].icon.color = Color.gray;
                }
            }
            else if (player.teamId == 1){
                if (team2_lookup.ContainsKey(player.playerId)){
                    team2_lookup[player.playerId].active = false;
                    team2_lookup[player.playerId].icon.color = Color.gray;
                }
            }
            else{
                if (ffa_lookup.ContainsKey(player.playerId)){
                    ffa_lookup[player.playerId].active = false;
                    ffa_lookup[player.playerId].icon.color = Color.gray;
                }
            }
        }



	}

}