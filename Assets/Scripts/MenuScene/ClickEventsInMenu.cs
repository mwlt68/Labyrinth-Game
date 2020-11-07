using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickEventsInMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject mainCameraInMenu;
    private SoundSystem soundSystem;
    DataVisualize dataVisualize;
    void Start()
    {
        dataVisualize = mainCameraInMenu.GetComponent<DataVisualize>();
        soundSystem = mainCameraInMenu.GetComponent<SoundSystem>();
    }

    public void StartGame(int gameMode)
    {
        switch (gameMode)
        {
            case 0:
                GameSceneInit.gameMode = GameSceneInit.GameMode.Easy;
                break;
            case 1:
                GameSceneInit.gameMode = GameSceneInit.GameMode.Normal;
                break;
            case 2:
                GameSceneInit.gameMode = GameSceneInit.GameMode.Hard;
                break;
            default:
                GameSceneInit.gameMode = GameSceneInit.GameMode.Easy;
                break;
        }
        SceneManager.LoadScene("Assets/Scenes/Game.unity", LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void BackInOptions()
    {
        dataVisualize.SaveOptionsChanges();
        if (soundSystem != null)
        {
            soundSystem.SetAudioClipVolume();
        }
        
    }
}