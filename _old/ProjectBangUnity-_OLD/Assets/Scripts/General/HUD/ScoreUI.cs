namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreUI : MonoBehaviour
    {
        private Text _text;


        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetScore(0);
        }


        public void SetScore(int score)
        {
            _text.text = string.Format("Score: {0}", score);
        }


    }
}


