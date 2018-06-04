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

		//
		// Constructors
		//
        public UtilityAIConfig (){
            
        }
	}
}