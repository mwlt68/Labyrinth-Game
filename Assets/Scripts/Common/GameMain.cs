using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    DataVisualize dataVisualize;
    public static Player player;
    public static OptionsData optionData;
    void Start()
    {
        dataVisualize = this.GetComponent<DataVisualize>();
        PlayerDataProcess();
        OptionDataProcess();
    }
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    private void PlayerDataProcess()
    {
        player = new Player();
        player.LoadPlayer();
        dataVisualize.PlayerInfoVisualize(player);
    }
    private void OptionDataProcess()
    {
        optionData = SaveSystem.LoadOptionData();
        if (optionData== null)
        {
           optionData = new OptionsData(100,20,true,false);
           SaveSystem.SaveOptionData(optionData);
        }
        dataVisualize.OptionDataVisualize(optionData);
    }

    public static  void CheckSlider (Slider slider, ref int value,ref bool changes)
    {
        if (slider.value != value)
        {
            value = (int)slider.value;
            changes = true;
        }
    }
    public static void CheckToggle(Toggle toggle,ref bool condition,ref bool changes)
    {
        if (toggle.isOn != condition)
        {
            condition = toggle.isOn;
            changes = true;
        }
    }

}
