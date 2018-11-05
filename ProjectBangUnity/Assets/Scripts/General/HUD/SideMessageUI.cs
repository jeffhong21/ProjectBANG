namespace Bang
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class SideMessageUI : MonoBehaviour
    {
        private Text _text;



        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetMessage("");
        }


		public void SetMessage(string message)
        {
            _text.text = message;
        }




    }
}


