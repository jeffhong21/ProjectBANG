namespace Bang
{
    using UnityEngine;


    public class HUDState : MonoBehaviour
    {
        private PlayerController _player;
        private Canvas _canvas;
        private AmmoUI _ammoUI;
        private HealthUI _healthUI;
        private PlayerUI _playerUI;
        private WeaponUI _weaponUI;
        private ScoreUI _scoreUI;
        private MessageUI _messageUI;
        private SideMessageUI _sideMessageUI;

        public Canvas Canvas{ get { return _canvas; }}



        private void Awake()
        {
            _ammoUI = GetComponentInChildren<AmmoUI>();
            _healthUI = GetComponentInChildren<HealthUI>();
            _playerUI = GetComponentInChildren<PlayerUI>();
            _weaponUI = GetComponentInChildren<WeaponUI>();
            _scoreUI = GetComponentInChildren<ScoreUI>();
            _messageUI = GetComponentInChildren<MessageUI>();
            _sideMessageUI = GetComponentInChildren<SideMessageUI>();

            _canvas = GetComponent<Canvas>();
        }


		private void OnEnable()
		{
            _canvas.enabled = true;

            if(_player != null){
                Debug.Log("Registering Events via OnEnable");
                _player.CurrentAmmoEvent += UpdateAmmo;
                _player.DamageEvent += UpdateHealth;
                _player.EquipWeaponEvent += UpdateWeapon;
            }

		}


		private void OnDisable()
		{
            _canvas.enabled = false;

            if (_player != null)
            {
                _player.CurrentAmmoEvent -= UpdateAmmo;
                _player.DamageEvent -= UpdateHealth;
                _player.EquipWeaponEvent -= UpdateWeapon;
            }

		}


        public void InitializeHUD(PlayerController player)
        {
            _player = player;
            _player.CurrentAmmoEvent += UpdateAmmo;
            _player.DamageEvent += UpdateHealth;
            _player.EquipWeaponEvent += UpdateWeapon;

            UpdatePlayer(player.name);
            UpdateHealth(player.Health.CurrentHealth);
        }


        public void UpdateAmmo(int currentAmmo, int maxAmmo)
        {
            _ammoUI.SetAmmo(currentAmmo, maxAmmo);
        }


        public void UpdateHealth(float health)
        {
            _healthUI.SetHealth(health);
        }


        public void UpdatePlayer(string playerName)
        {
            _playerUI.SetPlayerName(playerName);
        }


        public void UpdateScore(int score)
        {
            _scoreUI.SetScore(score);
        }

        public void UpdateWeapon(string weaponName)
        {
            _weaponUI.SetWeapon(weaponName);
        }


        public void SetMessage(string message)
        {
            _messageUI.SetMessage(message);
        }

        public void SetMessage(string message, float time)
        {
            _messageUI.SetMessage(message, time);
        }

        public void UpdateSideMessage(string message)
        {
            _sideMessageUI.SetMessage(message);
        }
	}
}


