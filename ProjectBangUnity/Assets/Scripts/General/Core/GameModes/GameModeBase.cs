//namespace Bang
//{
//    using UnityEngine;
//    using System;

//    [Serializable]
//    public class GameModeBase : IGameMode
//    {
//        [SerializeField, HideInInspector]
//        protected int _playerCount = 1;
//        [SerializeField]
//        protected int _aiCount;
//        [SerializeField]
//        protected int _numberRoundsToWin = 1;
//        [SerializeField]
//        protected float _timePerRound = 180f;
//        [SerializeField]
//        protected float _respawnTime = 3f;
//        [SerializeField, ReadOnly]
//        protected int _currentRound;
//        [SerializeField]
//        protected ActorManager[] _actors; 


//        public int playerCount
//        {
//            get { return _playerCount; }
//            set { _playerCount = value; }
//        }


//        public int aiCount
//        {
//            get { return _aiCount; }
//            set { _aiCount = value; }
//        }

//        public int numberRoundsToWin
//        {
//            get { return _numberRoundsToWin; }
//            set { _numberRoundsToWin = value; }
//        }

//        public float timePerRound
//        {
//            get { return _timePerRound; }
//            set { _timePerRound = value; }
//        }

//        public float respawnTime
//        {
//            get { return _respawnTime; }
//            set { _respawnTime = value; }
//        }

//        public int currentRound
//        {
//            get{
//                return _currentRound;
//            }
//        }

//        public ActorManager[] actors
//        {
//            get { return _actors; }
//            //set { _actors = value; }
//        }






//        public virtual void Setup()
//        {
//            int actorCount = playerCount + aiCount;
//            _actors = new ActorManager[actorCount];

//            for (int i = 0; i < actors.Length; i++)
//            {
//                var actor = actors[i] = new ActorManager();
//                actor.id = i;
//                if(i > playerCount - 1)
//                {
//                    actor.isBot = true;    
//                }
//            }
//        }


//    }
//}


