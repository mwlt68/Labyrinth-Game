using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataVisualizeInGame : MonoBehaviour
{
    public  Text timeText;
    public  Button pauseButton;
    public  GameObject MenuPanel;
    public  Slider volumeSlider;
    public  Button continueBtn;
    public  Button exitBtn;
    public TextMeshProUGUI timeResultText;
    public TextMeshProUGUI gameResultText;
    public GameObject pausePanel;
    public GameObject timePanel;
    public GameObject gameFinishPanel;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public  void MenuOpenOrClose()
    {
        if (MenuPanel.active==false)
        {
            MenuPanel.SetActive(true);
            pausePanel.SetActive(false);
  //          timePanel.SetActive(false);
        }
        else
            MenuPanel.SetActive(false);

    }
    public  void VolumeSliderChange()
    {
        GameMain.optionData.volumeLevel = (int)volumeSlider.value;
        SaveSystem.SaveOptionData(GameMain.optionData);
    }
    public  void ContinueButton()
    {
        MenuPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void GameFinishComponentVisualize(bool doesWin,int time)
    {
        MenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        timePanel.SetActive(false);
        gameFinishPanel.SetActive(true);
        if (doesWin)
            gameResultText.text = "WIN";
        else
            gameResultText.text = "LOSE";
        string timeText = "Time : " + time.ToString();
        timeResultText.text = timeText;
    }
}
