namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;


    public class CharacterHealthUI : MonoBehaviour
    {
        public Slider slider;
        public Image fillImage;
        public bool useTeamColor;
        [HideInInspector]
        public Color teamColor;
        public Color fullHealthColor = new Color(0, 1, 0, 1);
        public Color lowHealthColor = new Color(1, 0, 0, 1);

        public bool useRelativeRotation = true;


        private float startingHealth;
        private float currentHealth;
        private Quaternion relativeRotation;

        private bool isInitialized;





		private void Start()
		{
            relativeRotation = transform.parent.localRotation;
            fillImage.color = useTeamColor ? teamColor : fullHealthColor;
		}


		private void Update()
		{
            if (useRelativeRotation)
                transform.rotation = relativeRotation;
		}


		public void Initialize(float maxHealth)
        {
            startingHealth = maxHealth;
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            fillImage.color = useTeamColor ? teamColor : fullHealthColor;
        }



        public void SetHealthUI(float health)
        {
            currentHealth = health;
            slider.value = currentHealth;

            fillImage.color = useTeamColor ? teamColor : Color.Lerp(lowHealthColor, fullHealthColor, currentHealth / startingHealth);

        }
    }
}


