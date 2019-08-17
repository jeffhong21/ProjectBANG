using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;



public class InGameMenu : MonoBehaviour
{

    public Button m_RestartBtn;
    public Button m_QuitBtn;




    private void Start()
    {
        Game.OnPause += SetPanelActive;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_RestartBtn.onClick.AddListener(RestartScene);
        m_QuitBtn.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        m_RestartBtn.onClick.RemoveListener(RestartScene);
        m_QuitBtn.onClick.RemoveListener(ExitGame);
    }

    private void OnDestroy()
    {
        Game.OnPause -= SetPanelActive;
    }


    public void SetPanelActive(bool active)
    {
        gameObject.SetActive(active);
    }



    public void RestartScene()
    {
        Debug.Log("Restarting Scene.");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}