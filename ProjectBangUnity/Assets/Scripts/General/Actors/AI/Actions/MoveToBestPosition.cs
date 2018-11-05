namespace Bang
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using AtlasAI;


    public sealed class MoveToBestPosition : ActionWithOptions<Vector3>
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            Vector3 best = GetBest(c, c.sampledPositions);

            //  Move to the best position...
            if (best.sqrMagnitude == 0f)
            {
                return;
            }

            //c._positionScores = scorers;


            c.agent.MoveTo(best);

            //c.PositionScores = this.GetAllScores(context, c.sampledPositions);

        }




//#if UNITY_EDITOR
//        public KeyValuePair<Vector3, float>[] GetScoredOptions(IAIContext context)
//        {
//            var c = context as AgentContext;
//            var allScores = ListBufferPool.GetBuffer<ScoredOption>(c.sampledPositions.Count);
//            this.GetAllScorers(context, c.sampledPositions, allScores);

//            var scoredOptions = (from score in allScores
//                                 select new KeyValuePair<Vector3, float>(score.option, score.score)).ToArray();

//            ListBufferPool.ReturnBuffer<ScoredOption>(allScores);

//            return scoredOptions;
//        }
//#endif




    }
}