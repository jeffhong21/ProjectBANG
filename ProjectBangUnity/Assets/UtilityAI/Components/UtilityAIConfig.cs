namespace UtilityAI
{
    using System;

	[Serializable]
	public class UtilityAIConfig
	{
		//
		// Fields
		//
		public string aiId;

		public float intervalMin;

		public float intervalMax;

		public float startDelayMin;

		public float startDelayMax;

		public bool isActive;

        public bool _debugClient;

        public bool isPredefined;

        public string type;

		//
		// Constructors
		//
        public UtilityAIConfig ()
        {
            aiId = "New AI";
            intervalMin = intervalMax = 1;
            startDelayMin = startDelayMax = 0;
            isActive = true;
        }
	}
}