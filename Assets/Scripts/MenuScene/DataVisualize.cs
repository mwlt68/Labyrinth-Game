using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataVisualize : MonoBehaviour
{
    public Slider experienceSlider;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public TextMeshProUGUI levelTextMesh;
    public Toggle leftSideToggle;
    public Toggle reverseMovementToggle;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void PlayerInfoVisualize(Player player)
    {
        levelTextMesh.text = player.level.ToString();
        int experienceRate = (int)((player.experience * 100) / player.level);
        experienceSlider.value = experienceRate;
    }
    public void OptionDataVisualize(OptionsData optionData)
    {
        volumeSlider.value = optionData.volumeLevel;
        sensitivitySlider.value = optionData.sensitivityLevel;
        leftSideToggle.isOn = optionData.leftSide;
        reverseMovementToggle.isOn = optionData.reverseMovement;
    }
    public  void SaveOptionsChanges()
    {

        bool changes = false;
        GameMain.CheckSlider(volumeSlider, ref GameMain.optionData.volumeLevel, ref changes);
        GameMain.CheckSlider(sensitivitySlider, ref GameMain.optionData.sensitivityLevel, ref changes);
        GameMain.CheckToggle(leftSideToggle, ref GameMain.optionData.leftSide, ref changes);
        GameMain.CheckToggle(reverseMovementToggle, ref GameMain.optionData.reverseMovement, ref changes);
        if (changes)
        {
            SaveSystem.SaveOptionData(GameMain.optionData);
        }
    }
}
