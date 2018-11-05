namespace Bang
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class MessageUI : MonoBehaviour
    {
        private Text _text;

        private IEnumerator messageCoroutine;

        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetMessage("");
        }

		private void OnEnable()
		{
            //gameObject.SetActive(false);
		}

		public void SetMessage(string message)
        {
            //gameObject.SetActive(true);
            _text.text = message;
        }


        public void SetMessage(string message, float time)
        {
            if(messageCoroutine != null){
                StopCoroutine(messageCoroutine);
            }

            messageCoroutine = DisplayMessage(message, time);
            StartCoroutine(messageCoroutine);
        }


        private IEnumerator DisplayMessage(string message, float time)
        {
            _text.text = message;
            yield return new WaitForSeconds(time);
            _text.text = "";
            yield return null;
        }

    }
}


