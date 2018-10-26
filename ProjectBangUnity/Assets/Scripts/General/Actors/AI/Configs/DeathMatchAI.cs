namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using AtlasAI;

    [Serializable]
    public class DeathMatchAI : UtilityAIAssetConfig
    {

        /*
        private IAction a;
        private IScorer scorer;
        private List<IScorer> scorers;
        private IQualifier q;
        private Selector s;

        private List<IQualifier> qualifiers;
        private List<IScorer[]> allScorers;
        private List<IAction> actions;
        */


        public DeathMatchAI()
        {
            name = "DeathMatchAI";
        }

        public override void SetupAI(IUtilityAI asset)
        {
            //  Initializes the setup of AI.
            InitializeAI(asset);
            rs.debugScores = true;


            #region Damaged

            // ---- New Action ----
            Selector selector = new ScoreSelector();
            a = new CompositeAction()
            {
                name = "FireAtTarget",
                actions = new List<IAction>()
                {
                    new SetBestAttackTarget()
                    {
                        name = "SetBestAttackTarget",
                        scorers = new List<IOptionScorer<ActorHealth>>()
                        {
                            new IsTargetAlive(){score = 100f},
                            new EnemyProximityToSelf(){multiplier = 1f, score = 50f},
                            new IsCurrentTargetScorer()
                        }
                    },
                    new FireAtAttackTarget(){ name = "FireAtAttackTarget" }
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorers ----
            scorer = new HasEnemies() { score = 15 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 25, range = 10 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 20 };
            qualifiers.Add(q);



            a = new SelectorAction(selector);
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 10, range = 10 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasCoverPosition() { score = 10 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion



            #region SearchForHealth


            #endregion



            #region CanSee


            #endregion



            #region MoveToBestAttackPosition

            // ---- New Action ----
            a = new CompositeAction()
            {
                name = "MoveToBestAttackPosition",
                actions = new List<IAction>()
                {
                    new ScanForPositions(){ name = "ScanForPositions", samplingRange = 20, samplingDensity = 2.5f },
                    new MoveToBestPosition()
                    {
                        name = "MoveToBestAttackPosition",
                        scorers = new List<IOptionScorer<Vector3>>()
                        {
                            new PositionProximityToSelf(),      //  How close each point is to agent.
                            new ProximityToNearestEnemy(),      //  How close each point is to each enemy.
                            new OverRangeToClosestEnemy(),      //  If point is over a certain range to each enemy.
                            new LineOfSightToAnyEnemy(),        //  Does each point have line of sight to each enemy.
                            new LineOfSightToClosestEnemy(),    //  Does each point have line of sight to closest enemy.
                            new OverRangeToAnyEnemy()           //  If point is over range to any enemy.
                        }
                    }
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 22, range = 8 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            //q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion



            #region Attack

            // ---- New Action ----
            a = new CompositeAction()
            {
                name = "FireAtTarget",
                actions = new List<IAction>()
                {
                    new SetBestAttackTarget()
                    {
                        name = "SetBestAttackTarget",
                        scorers = new List<IOptionScorer<ActorHealth>>()
                        {
                            new IsTargetAlive(){score = 100f},
                            new EnemyProximityToSelf(){multiplier = 1f, score = 50f},
                            new IsCurrentTargetScorer()
                        }
                    },
                    new FireAtAttackTarget(){ name = "FireAtAttackTarget" }
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorers ----
            scorer = new HasEnemies() { score = 15 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 25, range = 10 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 20 };
            qualifiers.Add(q);

            #endregion



            #region SearchForLostTarget
            //

            #endregion



            #region IsAmmoLow
            //

            #endregion



            #region Default-Scan
            //
            //  Default Qualifier
            //
            rs.defaultQualifier.action = new ScanForEntities()
            { 
                name = "ScanForEntitites" 
            };
            #endregion


            //  
            //  Configure AI.  Setup each qualifiers action and scorers.
            //
            base.ConfigureAI();
        }






    }

}

