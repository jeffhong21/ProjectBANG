namespace CharacterController.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class HUDCrosshair : MonoBehaviour
    {

        protected GameObject m_Character;
        [SerializeField]
        protected Color m_CrosshairsColor = Color.white;
        [SerializeField]
        protected LayerMask m_CrosshairsTargetLayer;
        [SerializeField]
        protected Image m_DefaultCrosshairsImage;

        //public float hitIndicatorShowDuration = 0.5f;

        [SerializeField]
        protected bool m_OnlyVisibleOnAim = true;


        private RigidbodyCharacterController m_controller;



		private void Awake()
		{
            if(m_Character == null){
                m_Character = GameObject.FindGameObjectWithTag("Player");
            }
            m_controller = m_Character.GetComponent<RigidbodyCharacterController>();
		}


		private void OnEnable()
        {
            m_controller.OnAim += CrosshairsSetActive;

            m_DefaultCrosshairsImage.color = m_CrosshairsColor;
            m_DefaultCrosshairsImage.enabled = !m_OnlyVisibleOnAim;
		}


		private void OnDisable()
		{
            m_controller.OnAim -= CrosshairsSetActive;
		}


		private void CrosshairsSetActive(bool aim)
        {
            if(m_OnlyVisibleOnAim)
                m_DefaultCrosshairsImage.enabled = aim;

        }
	}
}