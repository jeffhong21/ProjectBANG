namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HealthUI : MonoBehaviour
    {
        private Text _text;


        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetHealth(0);
        }


        public void SetHealth(float currentHealth)
        {
            _text.text = currentHealth.ToString();
        }

    }
}


