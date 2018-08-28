namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using AtlasAI;

    [Serializable]
    public class AgentActionAI : UtilityAIAssetConfig
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



        public AgentActionAI()
        {
            name = "AgentActionAI";
        }


        public override void SetupAI(IUtilityAI asset)
        {
            //  Initializes the setup of AI.
            InitializeAI(asset);



            // ---- New Action ----
            a = new CompositeAction()
            {
                name = "FireAtTarget",
                actions = new List<IAction>()
                {
                    new StopMovement(){ name = "StopMovement"},
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
            scorer = new HasEnemies() { score = 5};
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 15, range = 10 };
            scorers.Add(scorer); 
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);





            // ---- New Action ----
            a = new EmptyAction()
            {
                name = "Null Action",
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new FixedScorer() { score = 10 };
            scorers.Add(scorer);  // --  Add to Scorers Group
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);


            //
            //  Default Qualifier
            //
            rs.defaultQualifier.action = new EmptyAction()
            {
                name = "Null Action"
            };


            //  
            //  Configure AI.  Setup each qualifiers action and scorers.
            //
            base.ConfigureAI();
        }





    }

}

