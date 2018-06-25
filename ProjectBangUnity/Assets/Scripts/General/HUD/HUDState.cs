namespace Bang
{
    using UnityEngine;


    public class HUDState : MonoBehaviour
    {
        private Canvas _canvas;

        private static AmmoUI _ammoUI;
        private static HealthUI _healthUI;
        private static PlayerUI _playerUI;
        private static WeaponUI _weaponUI;
        private static ScoreUI _scoreUI;

        public Canvas canvas{ get { return _canvas; }}


        public static void UpdateAmmo(int currentAmmo, int maxAmmo)
        {
            _ammoUI.SetAmmo(currentAmmo, maxAmmo);
        }


        public static void UpdateHealth(int health)
        {
            _healthUI.SetHealth(health);
        }


        public static void UpdatePlayer(string playerName)
        {
            _playerUI.SetPlayerName(playerName);
        }


        public static void UpdateScore(int score)
        {
            _scoreUI.SetScore(score);
        }

        public static void UpdateWeapon(string weaponName)
        {
            _weaponUI.SetWeapon(weaponName);
        }




        private void Awake()
        {
            _ammoUI = GetComponentInChildren<AmmoUI>();
            _healthUI = GetComponentInChildren<HealthUI>();
            _playerUI = GetComponentInChildren<PlayerUI>();
            _scoreUI = GetComponentInChildren<ScoreUI>();
            _weaponUI = GetComponentInChildren<WeaponUI>();

            _canvas = GetComponent<Canvas>();
        }


		private void OnEnable()
		{
            _canvas.enabled = true;
		}


		private void OnDisable()
		{
            _canvas.enabled = false;
		}

	}
}


