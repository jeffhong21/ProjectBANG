namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerUI : MonoBehaviour
    {
        private Text _text;


        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetPlayerName("");
        }


        public void SetPlayerName(string playerName)
        {
            _text.text = playerName;
        }



    }
}


