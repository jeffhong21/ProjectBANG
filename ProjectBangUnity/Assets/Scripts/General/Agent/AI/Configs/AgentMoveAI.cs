namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using UtilityAI;

    [Serializable]
    public class AgentMoveAI : UtilityAIAssetConfig
    {
        
        //private IAction a;
        //private IScorer scorer;
        //private List<IScorer> scorers;
        //private IQualifier q;
        //private Selector s;

        //private List<IQualifier> qualifiers;
        //private List<IScorer[]> allScorers;
        //private List<IAction> actions;



        public AgentMoveAI()
        {
            name = "AgentMoveAI";
        }


        public override void ConfigureAI(UtilityAI asset)
        {
            if (qualifiers == null) qualifiers = new List<IQualifier>();
            if (allScorers == null) allScorers = new List<IScorer[]>();
            if (actions == null) actions = new List<IAction>();

            Selector rs = asset.rootSelector;



            //a = new RandomWander()
            //{
            //    name = "RandomWander",
            //};
            //actions.Add(a);  // --  Add to Actions Group

            // ---- New Action ----
            a = new MoveToBestPosition()
            {
                name = "MoveToBestPosition",
                scorers = new List<IOptionScorer<Vector3>>()
                {
                    new PositionProximityToSelf(),
                    new ProximityToNearestEnemy(),
                    new OverRangeToClosestEnemy(),
                    new LineOfSightToAnyEnemy(),
                    new LineOfSightToClosestEnemy(),
                    new OverRangeToAnyEnemy(),
                    new OverRangeToAnyEnemySpawner(),
                    new ProximityToAgentSpawner()
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            // ---- New Scorer ----
            scorer = new FixedScorer() { score = 10 };
            scorers.Add(scorer);  // --  Add to Scorers Group
            allScorers.Add(scorers.ToArray());

            //scorers = new List<IScorer>();
            //scorer = new HasEnemies() { score = 15 };
            //scorers.Add(scorer);         // --------------- Add
            //allScorers.Add(scorers.ToArray());

            //// ---- New Qualifier ----
            //q = new CompositeScoreQualifier();
            //qualifiers.Add(q);

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);


            rs.defaultQualifier.action = new RandomWander() 
            {
                name = "RandomWander"
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

            Debug.Log("Finish Initializing " + this.GetType().Name);
        }





    }

}

