namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using UtilityAI;

    [Serializable]
    public class ActionTester : UtilityAIAssetConfig
    {

        //private IAction a;
        //private IScorer scorer;
        //private List<IScorer> scorers;
        //private IQualifier q;
        //private Selector s;

        //private List<IQualifier> qualifiers;
        //private List<IScorer[]> allScorers;
        //private List<IAction> actions;

        public ActionTester()
        {
            name = "ActionTester";
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
                name = "SearchForTarget",
                actions = new List<IAction>()
                {
                    new SearchForTargets(){name = "SearchForTarget"},
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            // ---- Add Scorers ----
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



            //
            //  Configure the AI;
            //

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

