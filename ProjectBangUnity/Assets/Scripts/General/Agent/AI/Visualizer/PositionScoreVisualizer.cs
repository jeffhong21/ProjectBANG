namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;

    using UtilityAI.Visualization;
    using UtilityAI;

    public class PositionScoreVisualizer : ActionWithOptionsVisualizerComponent<MoveToBestPosition, Vector3>
    {
        [Range(0.1f, 2f)]
        public float sphereSize = 0.25f;
        [Range(0.01f, 0.99f)]
        public float sphereAlpha = 0.5f;



        protected override List<Vector3> GetOptions(IAIContext context){
            AgentContext c = context as AgentContext;
            return c.sampledPositions;
        }


        protected override void DrawGUI(List<OptionScorer<Vector3>> data)
        {
            var cam = Camera.main;

            if (cam == null)
                return;

            foreach (var scoredOption in data)
            {
                var score = scoredOption.score;

                var p = cam.WorldToScreenPoint(scoredOption.option);
                p.y = Screen.height - p.y;


                if (score < 0f)
                {
                    GUI.color = Color.red;
                }
                else if (score == 0f)
                {
                    GUI.color = Color.black;
                }
                else
                {
                    GUI.color = Color.green;
                }


                var content = new GUIContent(score.ToString("F0"));
                var size = new GUIStyle(GUI.skin.label).CalcSize(content);

                GUI.Label(new Rect(p.x, p.y, size.x, size.y), content);
            }
        }

        protected override void DrawGizmos(List<OptionScorer<Vector3>> data)
        {
            float maxScore = 0f;
            float minScore = Mathf.Infinity;

            foreach (var scoredOption in data)
            {
                var value = scoredOption.score;
                if (value > maxScore)
                {
                    maxScore = value;
                }

                if (value < minScore)
                {
                    minScore = value;
                }
            }

            var diffScore = maxScore - minScore;

            foreach (var scoredOption in data)
            {
                var pos = scoredOption.option;
                var score = scoredOption.score;

                var normScore = score - minScore;

                Gizmos.color = GetColor(normScore, diffScore, sphereAlpha);
                Gizmos.DrawSphere(pos, sphereSize);
            }
        }



        private static Color GetColor(float score, float maxScore, float alpha = 1f)
        {
            if (maxScore <= 0)
            {
                return Color.green;
            }

            if (score == maxScore)
            {
                return Color.cyan;
            }

            var quotient = score / maxScore;

            return new Color((1 - quotient), quotient, 0, alpha);
        }


    }
}
