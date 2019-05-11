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


        private CharacterLocomotion m_Controller;



		private void Awake()
		{
            if(m_Character == null){
                m_Character = GameObject.FindGameObjectWithTag("Player");
            }
            m_Controller = m_Character.GetComponent<CharacterLocomotion>();
		}


		private void OnEnable()
        {
            m_Controller.OnAim += CrosshairsSetActive;

            m_DefaultCrosshairsImage.color = m_CrosshairsColor;
            m_DefaultCrosshairsImage.enabled = !m_OnlyVisibleOnAim;
		}


		private void OnDisable()
		{
            m_Controller.OnAim -= CrosshairsSetActive;
		}


		private void CrosshairsSetActive(bool aim)
        {
            if(m_OnlyVisibleOnAim)
                m_DefaultCrosshairsImage.enabled = aim;

        }
	}
}