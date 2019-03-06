namespace CharacterController.AI
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using uUtilityAI;

    [Serializable]
    public class GenericAI : UtilityAIAssetConfig
    {

        /*
        private IAction a;
        private IContextualScorer scorer;
        private List<IContextualScorer> scorers;
        private IQualifier q;
        private Selector s;

        private List<IQualifier> qualifiers;
        private List<IContextualScorer[]> allScorers;
        private List<IAction> actions;
        */


        //public DeathMatchAI()
        //{
        //    name = "DeathMatchAI";
        //}
        public GenericAI(string aiId) : base(aiId) { }


        protected override void ConfigureAI(IUtilityAI ai)
        {






            // ---- New Action ----
            a = new CompositeAction()
            {
                actions = new List<IAction>()
                {
                    new RandomWander(){range = 20 }
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IContextualScorer>();
            //
            // ---- New Scorer ----
            scorer = new CanMoveToNewPosition() { score = 20 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);




            #region Default-Scan
            //
            //  Default Qualifier
            //
            //rs.defaultQualifier.action = new EmptyAction();
            #endregion
        }






    }

}

