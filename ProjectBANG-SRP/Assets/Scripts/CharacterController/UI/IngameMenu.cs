using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;



public class InGameMenu : MonoBehaviour
{
    //[SerializeField]
    private GameObject loadScenePanel;
    [SerializeField]
    private Button restartBtn;
    [SerializeField]
    private Button quitBtn;




    private void Start()
    {
        LevelManager.OnPause += SetPanelActive;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(restartBtn) restartBtn.onClick.AddListener(RestartScene);
        if (quitBtn) quitBtn.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        if (restartBtn) restartBtn.onClick.RemoveListener(RestartScene);
        if (quitBtn) quitBtn.onClick.RemoveListener(ExitGame);
    }

    private void OnDestroy()
    {
        LevelManager.OnPause -= SetPanelActive;
    }


    public void SetPanelActive(bool active)
    {
        gameObject.SetActive(active);
    }



    public void RestartScene()
    {
        Debug.Log(" ** Restarting Scene. **");
    }

    public void ExitGame()
    {
        Debug.Log("Quitting Game.");
        Application.Quit();
    }

}