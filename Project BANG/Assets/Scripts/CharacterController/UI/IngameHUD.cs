namespace CharacterController.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;

    public class InGameHUD : MonoBehaviour
    {
        public HUDCrosshair m_HUDCrosshair;
        //public HUDNotifications 

        Canvas m_Canvas;


        public void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            SetPanelActive(true);
        }

        public void SetPanelActive(bool active)
        {
            gameObject.SetActive(active);
        }


        public void RegisterCharacter(RigidbodyCharacterController controller)
        {

        }



    }
}