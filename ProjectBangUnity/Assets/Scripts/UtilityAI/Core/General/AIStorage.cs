namespace UtilityAI
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;




    [Serializable]
    public class AIStorage : ScriptableObject
    {
        [SerializeField]
        private bool debug;

        [Multiline]
        public string description;

        [ReadOnly]
        public float version = 1.0f;

        [ReadOnly]
        public string aiId;

        [HideInInspector]
        public UtilityAI configuration;

        [ReadOnly]
        public string guid;


        public string friendlyName{
            get{
                var assetDir = AssetDatabase.GetAssetPath(this);
                return Path.GetFileNameWithoutExtension(assetDir);
            }
        }


        ///<summary>
        ///Creates an instance for the specified AI.
        ///</summary>
        ///<param name = "aiId" > The name of the ai that gets registered.</param>
        ///<param name = "aiName" > Name of the asset file.</param>
        ///<returns></returns>
        public static AIStorage CreateAsset(string aiId, string aiName, bool isSelect = false)
        {
            AIStorage asset = ScriptableObject.CreateInstance<AIStorage>();

            string assetDir = AssetDatabase.GenerateUniqueAssetPath(AIManager.StorageFolder + "/" + aiName + ".asset");

            //  Generate unique friendly name.
            //asset.friendlyName = Path.GetFileNameWithoutExtension(assetDir);
            asset.aiId = AINameMap.GenerateUniqueID(aiId);
            asset.configuration = new UtilityAI(asset.aiId);


            AssetDatabase.CreateAsset(asset, assetDir);
            AssetDatabase.SaveAssets();

            asset.guid = AssetDatabase.AssetPathToGUID(assetDir);
            AINameMap.Register(AssetDatabase.AssetPathToGUID(assetDir), asset.aiId);


            if (isSelect){
                Selection.activeObject = asset;
            }

            return asset;
        }



        //public virtual AIStorage CreateAsset<T>(string aiId, string aiName, bool isSelect = true) where T : UtilityAIConfig
        //{
        //    AIStorage asset = CreateAsset(aiId, aiName, isSelect);
        //    SetAssetConfig<T>(asset);
        //    AssetDatabase.SaveAssets();
        //    return asset;
        //}


        //public void SetAssetConfig<TConfig>(AIStorage asset) where TConfig : UtilityAIConfig
        //{
        //    TConfig config = (TConfig)Activator.CreateInstance(typeof(TConfig), new object[] { asset });
        //    asset = config.asset;
        //}





		#region MockAIs

		//IAction a;
		//IScorer scorer;
		//List<IScorer> scorers;
		//IQualifier q;
		//Selector s;

		//List<IQualifier> qualifiers;
		//List<IScorer[]> allScorers;
		//List<IAction> actions;

		//void ConfigureClient(Selector rs)
		//{
		//    if (qualifiers == null) qualifiers = new List<IQualifier>();
		//    if (allScorers == null) allScorers = new List<IScorer[]>();
		//    if (actions == null) actions = new List<IAction>();


		//    a = new ScanForEntities();
		//    actions.Add(a);
		//    a = new ScanForPositions();
		//    actions.Add(a);

		//    scorers = new List<IScorer>();
		//    scorer = new HasEnemies();
		//    scorers.Add(scorer);
		//    allScorers.Add(scorers.ToArray());

		//    scorers = new List<IScorer>();
		//    scorer = new HasEnemiesInRange();
		//    scorers.Add(scorer);
		//    allScorers.Add(scorers.ToArray());

		//    q = new CompositeScoreQualifier();
		//    qualifiers.Add(q);
		//    q = new CompositeScoreQualifier();
		//    qualifiers.Add(q);

		//    //  Setup each qualifiers action and scorers.
		//    for (int index = 0; index < qualifiers.Count; index++)
		//    {
		//        //  Add qualifier to rootSelector.
		//        rs.qualifiers.Add(qualifiers[index]);
		//        var qualifier = rs.qualifiers[index];
		//        //  Set qualifier's action.
		//        qualifier.action = actions[index];
		//        //  Add scorers to qualifier.
		//        foreach (IScorer scorer in allScorers[index])
		//        {
		//            if (qualifier is CompositeQualifier)
		//            {
		//                var q = qualifier as CompositeQualifier;
		//                q.scorers.Add(scorer);
		//            }
		//        }
		//    }
		//}

		#endregion




        #region Original UtilityAI
        //  [Serializable]
        //  public class AIStorage : ScriptableObject
        //  {
        //      public string friendlyName;
        //      [Multiline]
        //      public string description;
        //      [ShowOnly]
        //      public float version = 1.0f;
        //      public string aiId;

        //      public UtilityAI configuration;
        //      public string editorConfiguration;

        //      ///<summary>
        //      ///Creates an instance for the specified AI.
        //      ///</summary>
        //      ///<param name = "aiId" > The name of the ai that gets registered.</param>
        //      ///<param name = "aiName" > Name of the asset file.</param>
        //      ///<returns></returns>
        //      public AIStorage CreateAsset(string aiId, string aiName, bool isMockAI = false)
        //      {
        //          string dir = AIManager.StorageFolder;
        //          string assetDir = AssetDatabase.GenerateUniqueAssetPath(dir + "/" + aiName + ".asset");
        //          aiName = Path.GetFileNameWithoutExtension(assetDir);

        //          AIStorage asset = ScriptableObject.CreateInstance<AIStorage>();
        //          asset.friendlyName = aiName;
        //          asset.aiId = aiId;
        //          asset.configuration = new UtilityAI(aiId);

        //          if(isMockAI)
        //              SetupAI(asset.configuration.selector);


        //          AssetDatabase.CreateAsset(asset, assetDir);
        //          AssetDatabase.SaveAssets();


        //          return asset;
        //      }


        //#region MockAIs

        //    IAction a;
        //    IScorer scorer;
        //    List<IScorer> scorers;
        //    IQualifier q;
        //    Selector s;

        //    List<IQualifier> qualifiers;
        //    List<IScorer[]> allScorers;
        //    List<IAction> actions;

        //    void SetupAI(Selector rs)
        //    {
        //        if(qualifiers == null) qualifiers = new List<IQualifier>();
        //        if (allScorers == null) allScorers = new List<IScorer[]>();
        //        if (actions == null) actions = new List<IAction>();


        //        a = new ScanForEntities();
        //        actions.Add(a);
        //        a = new ScanForPositions();
        //        actions.Add(a);
        //        scorers = new List<IScorer>();
        //        scorer = new HasEnemies();
        //        scorers.Add(scorer);
        //        allScorers.Add(scorers.ToArray());
        //        scorers = new List<IScorer>();
        //        scorer = new TestScorerB();
        //        scorers.Add(scorer);
        //        allScorers.Add(scorers.ToArray());
        //        q = new CompositeScoreQualifier();
        //        qualifiers.Add(q);
        //        q = new CompositeScoreQualifier();
        //        qualifiers.Add(q);

        //        //  Setup each qualifiers action and scorers.
        //        for (int index = 0; index < qualifiers.Count; index++)
        //        {
        //            //  Add qualifier to rootSelector.
        //            rs.qualifiers.Add(qualifiers[index]);
        //            var qualifier = rs.qualifiers[index];
        //            //  Set qualifier's action.
        //            qualifier.action = actions[index];
        //            //  Add scorers to qualifier.
        //            foreach (IScorer scorer in allScorers[index]){
        //                if (qualifier is CompositeQualifier){
        //                    var q = qualifier as CompositeQualifier;
        //                    q.scorers.Add(scorer);
        //                }
        //            }
        //        }
        //    }

        //    #endregion

        //}

        #endregion

	}

}

