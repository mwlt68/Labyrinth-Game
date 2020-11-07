using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickEventsInGame : MonoBehaviour
{
    [SerializeField]
    public GameObject mainCameraInGame;
    private DataVisualizeInGame dataVisualizeInGame;
    void Start()
    {
        dataVisualizeInGame = mainCameraInGame.GetComponent<DataVisualizeInGame>();
    }
    void Update()
    {

    }
    public void GameMenuSwitch()
    {
        dataVisualizeInGame.MenuOpenOrClose();
    }
    public void VolumeSliderChange()
    {
        if (dataVisualizeInGame!= null)
        {
            dataVisualizeInGame.VolumeSliderChange();
        }
    }
    public void ContinueBtn()
    {
        dataVisualizeInGame.ContinueButton();
    }
    public void ExitBtn()
    {
        SceneManager.LoadScene("Assets/Scenes/Menu.unity", LoadSceneMode.Single);
    }
    public void RestartBtn()
    {
        SceneManager.LoadScene("Assets/Scenes/Game.unity", LoadSceneMode.Single);
    }
}
