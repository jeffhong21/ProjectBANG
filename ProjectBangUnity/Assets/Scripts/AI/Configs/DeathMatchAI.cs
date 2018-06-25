//namespace Bang
//{
//    using UnityEngine;
//    using System;
//    using System.Collections.Generic;

//    using UtilityAI;

//    [Serializable]
//    public class DeathMatchAI : UtilityAIAssetConfig
//    {

//        /*
//        private IAction a;
//        private IScorer scorer;
//        private List<IScorer> scorers;
//        private IQualifier q;
//        private Selector s;

//        private List<IQualifier> qualifiers;
//        private List<IScorer[]> allScorers;
//        private List<IAction> actions;
//        */


//        public DeathMatchAI()
//        {
//            name = "DeathMatchAI";
//        }

//        public override void SetupAI(IUtilityAI asset)
//        {
//            //  Initializes the setup of AI.
//            InitializeAI(asset);



//            #region New Action

//            // ---- New Action ----
//            a = new CompositeAction()
//            {
//                name = "SearchForTarget",
//                actions = new List<IAction>()
//                {
//                    new SearchForTargets(){name = "SearchForTarget"},
//                }
//            };
//            actions.Add(a);  // --  Add to Actions Group

//            // ---- New Scorers Group ----
//            scorers = new List<IScorer>();
//            //
//            // ---- New Scorers ----
//            scorer = new FixedScorer() { score = 10 };
//            scorers.Add(scorer);
//            //
//            // ---- Add All Scorers to Scorers Group ----
//            allScorers.Add(scorers.ToArray());

//            // ---- New Qualifier ----
//            q = new CompositeScoreQualifier();
//            qualifiers.Add(q);



//            #endregion



//            #region Damaged




//            #endregion



//            #region SearchForHealth




//            #endregion



//            #region CanSee
//            //

//            #endregion



//            #region Attack

//            // ---- New Action ----
//            a = new CompositeAction()
//            {
//                name = "FireAtTarget",
//                actions = new List<IAction>()
//                {
//                    new SetBestAttackTarget()
//                    {
//                        name = "SetBestAttackTarget",
//                        scorers = new List<IOptionScorer<IHasHealth>>()
//                        {
//                            new IsAttackTargetAlive(){score = 100f},
//                            new EnemyProximityToSelf(){multiplier = 1f, score = 50f},
//                            new IsCurrentTargetScorer()
//                        }
//                    },
//                    new FireAtAttackTarget(){ name = "FireAtAttackTarget" }
//                }
//            };
//            actions.Add(a);  // --  Add to Actions Group

//            // ---- New Scorers Group ----
//            scorers = new List<IScorer>();
//            //
//            // ---- New Scorers ----
//            scorer = new HasEnemies() { score = 5 };
//            scorers.Add(scorer);
//            // ---- New Scorer ----
//            scorer = new HasEnemiesInRange() { score = 15, range = 10 };
//            scorers.Add(scorer);
//            //
//            // ---- Add All Scorers to Scorers Group ----
//            allScorers.Add(scorers.ToArray());

//            // ---- New Qualifier ----
//            q = new CompositeScoreQualifier();
//            qualifiers.Add(q);

//            #endregion



//            #region Cover

//            // ---- New Action ----
//            a = new CompositeAction()
//            {
//                name = "MoveToCover",
//                actions = new List<IAction>()
//                {
//                    new FindClosestCover(){name = "FindClosestCover"},
//                    new MoveToCover(){name = "MoveToCover"}
//                }
//            };
//            actions.Add(a);  // --  Add to Actions Group

//            // ---- New Scorers Group ----
//            scorers = new List<IScorer>();
//            //
//            // ---- New Scorer ----
//            scorer = new HasEnemiesInRange() { score = 10, range = 4 };
//            scorers.Add(scorer);
//            // ---- New Scorer ----
//            scorer = new HasCoverPosition() { score = 10 };
//            scorers.Add(scorer);
//            //
//            // ---- Add All Scorers to Scorers Group ----
//            allScorers.Add(scorers.ToArray());

//            // ---- New Qualifier ----
//            q = new CompositeScoreQualifier();
//            qualifiers.Add(q);

//            #endregion



//            #region CanHear
//            //

//            #endregion



//            #region SearchForLostTarget
//            //

//            #endregion



//            #region IsAmmoLow
//            //

//            #endregion



//            #region Default-SearchForTarget
//            //
//            //  Default Qualifier
//            //
//            rs.defaultQualifier.action = new SearchForTargets()
//            {
//                name = "Search For Target"
//            };
//            #endregion


//            //  
//            //  Configure AI.  Setup each qualifiers action and scorers.
//            //
//            base.ConfigureAI();
//        }






//    }

//}

