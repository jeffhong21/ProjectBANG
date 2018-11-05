namespace AtlasAI
{
    using System.Collections.Generic;

    public interface IUtilityAIAssetConfig
    {
        //IAction a { get; set; }
        //IContextualScorer scorer { get; set; }
        //List<IContextualScorer> scorers { get; set; }
        //IQualifier q { get; set; }
        //Selector s { get; set; }

        //List<IQualifier> qualifiers { get; set; }
        //List<IContextualScorer[]> allScorers { get; set; }
        //List<IAction> actions { get; set; }

        string name
        {
            get;
            set;
        }

        void SetupAI(IUtilityAI asset);
    }
}