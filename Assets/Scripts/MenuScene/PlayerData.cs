using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int level;
    public int experience;

    public PlayerData(Player player)
    {
        this.level = player.level;
        this.experience = player.experience;
    }
}
