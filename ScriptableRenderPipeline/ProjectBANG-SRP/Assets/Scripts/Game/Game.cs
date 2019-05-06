using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterController;
using CharacterController.UI;

public class Game : SingletonMonoBehaviour<Game> 
{
    [SerializeField]
    private CameraController m_Camera;
    [SerializeField]
    private GameObject m_PlayerPrefab;
    private GameObject m_PlayerInstance;
    [SerializeField]
    private GameObject m_IngameHUD;



}
