namespace CharacterController.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;

    public class IngameHUD : MonoBehaviour
    {
        public ItemHUD m_ItemHUD;
        public HUDCrosshair m_HUDCrosshair;

        Canvas m_Canvas;


        public void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
        }

        public void SetPanelActive(bool active)
        {
            gameObject.SetActive(active);
        }


    }
}