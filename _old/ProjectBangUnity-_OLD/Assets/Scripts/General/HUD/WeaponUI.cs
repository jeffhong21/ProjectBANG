namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class WeaponUI : MonoBehaviour
    {
        private Text _text;


        private void Awake()
        {
            _text = GetComponentInChildren<Text>();
            SetWeapon("");
        }


        public void SetWeapon(string weaponName)
        {
            _text.text = weaponName;
        }


    }
}


