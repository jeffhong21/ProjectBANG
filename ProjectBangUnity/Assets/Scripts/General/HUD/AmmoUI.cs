namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class AmmoUI : MonoBehaviour
    {
        private Text _text;


        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetAmmo(0, 0);
        }


        public void SetAmmo(int currentAmmo, int maxAmmo)
        {
            _text.text = string.Format("{0} / {1}", currentAmmo, maxAmmo);
        }


    }
}


