using UnityEngine;
using System.Collections;
using CharacterController.UI;

public class Game : SingletonMonoBehaviour<Game>
{
    [Header("UI")]
    [SerializeField]
    private InGameMenu m_InGameMenu;
    [SerializeField]
    private InGameHUD m_inGameHUD;

    [Header("Managers")]
    [SerializeField]
    private LevelManager m_levelManager;
    [SerializeField]
    private ObjectPool m_objectPoolManager;
    [SerializeField]
    private SpawnPointManager m_spawnPointManager;
    [SerializeField]
    private ObjectManager m_objectManager;





    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);



    }


}
