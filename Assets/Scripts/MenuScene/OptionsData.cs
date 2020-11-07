using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsData 
{
    public int volumeLevel;
    public int sensitivityLevel;
    public bool leftSide;
    public bool reverseMovement;
    public OptionsData()
    {

    }
    public OptionsData(int volumeLevel, int sensitivityLevel, bool leftSide, bool reverseMovement)
    {
        this.volumeLevel = volumeLevel;
        this.sensitivityLevel = sensitivityLevel;
        this.leftSide = leftSide;
        this.reverseMovement = reverseMovement;
    }
}
