namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using UtilityAI;

    [Serializable]
    public class AgentScanAI : UtilityAIAssetConfig
    {

        //private IAction a;
        //private IScorer scorer;
        //private List<IScorer> scorers;
        //private IQualifier q;
        //private Selector s;

        //private List<IQualifier> qualifiers;
        //private List<IScorer[]> allScorers;
        //private List<IAction> actions;

        public AgentScanAI()
        {
            name = "AgentScanAI";
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
                name = "Scan",
                actions = new List<IAction>()
                {
                    new ScanForEntities(){name = "ScanForEntitites"},
                    new ScanForPositions(){ name = "ScanForPositions", samplingRange = 20, samplingDensity = 2.5f }
                }
            };
            actions.Add(a);  // --  Add to Actions Group


            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            // ---- New Scorer ----
            scorer = new FixedScorer() { score = 5 };
            scorers.Add(scorer);  // --  Add to Scorers Group
            allScorers.Add(scorers.ToArray());


            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);



            rs.defaultQualifier.action = new ScanForEntities()
            {
                name = "DefaultScanForPositions",
                samplingRange = 20,
                samplingDensity = 2.5f
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

