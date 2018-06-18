namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using UtilityAI;

    [Serializable]
    public class AgentActionAI : UtilityAIAssetConfig
    {

        //private IAction a;
        //private IScorer scorer;
        //private List<IScorer> scorers;
        //private IQualifier q;
        //private Selector s;

        //private List<IQualifier> qualifiers;
        //private List<IScorer[]> allScorers;
        //private List<IAction> actions;

        public AgentActionAI()
        {
            name = "AgentActionAI";
        }


        public override void ConfigureAI(IUtilityAI asset)
        {
            if (qualifiers == null) qualifiers = new List<IQualifier>();
            if (allScorers == null) allScorers = new List<IScorer[]>();
            if (actions == null) actions = new List<IAction>();

            Selector rs = asset.rootSelector;



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
                        scorers = new List<IOptionScorer<IHasHealth>>()
                        {
                            new IsAttackTargetAlive(){score = 100f},
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
            // ---- Add Scorers ----
            scorer = new HasEnemies() { score = 5};
            scorers.Add(scorer);  // --  Add to Scorers Group
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 15, range = 10 };
            scorers.Add(scorer);  // --  Add to Scorers Group
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);
            //CompositeScoreQualifier csq = q as CompositeScoreQualifier;
            //csq.debugScore = true;




            // ---- New Action ----
            a = new EmptyAction()
            {
                name = "Null Action",
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            // ---- New Scorer ----
            scorer = new FixedScorer() { score = 10 };
            scorers.Add(scorer);  // --  Add to Scorers Group
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);



            rs.defaultQualifier.action = new EmptyAction()
            {
                name = "Null Action"
            };



            //  Setup each qualifiers action and scorers.
            for (int index = 0; index < qualifiers.Count; index++)
            {
                //  Add qualifier to rootSelector.
                rs.qualifiers.Add(qualifiers[index]);
                var qualifier = rs.qualifiers[index];
                //  Set qualifier's action.
                qualifier.action = actions[index];
                //  Add scorers to qualifier.
                foreach (IScorer scorer in allScorers[index])
                {
                    if (qualifier is CompositeQualifier)
                    {
                        var q = qualifier as CompositeQualifier;
                        q.scorers.Add(scorer);
                    }
                }
            }

            //Debug.Log("Finish Initializing " + this.GetType().Name);
        }





    }

}

