using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int level;
    public int experience;

    public Player()
    {

    }
    public Player(int level, int experience)
    {
        this.level = level;
        this.experience = experience;
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayerData(this);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayerData();
        if (data==null)
        {
            this.level = 1;
            this.experience = 0;
            SavePlayer();
        }
        else
        {
            this.level = data.level;
            this.experience = data.experience;
        }
    }
    public void AddExperience(int val)
    {
        if (val > 0)
        {
            this.experience += val;
            if (this.experience >=this.level)
            {
                this.experience -= this.level;
                this.level += 1;
            }
            SavePlayer();
        }
    }
}
