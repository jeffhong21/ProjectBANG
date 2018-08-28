namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class IsSearchingForTargets : ScorerBase
    {
        [SerializeField]
        public bool not = false;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;


            if (c.isSearching)
            {
                return this.not ? 0f : this.score;
            }

            return this.not ? this.score : 0f;
        }


    }
}