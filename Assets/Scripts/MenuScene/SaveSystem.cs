using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem 
{
    public static void SavePlayerData(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerdata.fun";
        FileStream stream = new FileStream(path,FileMode.Create);
        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream,data);
        stream.Close();
    }
    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerdata.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("Save file not found in" +path);
            return null;
        }
    }
    public static void SaveOptionData(OptionsData optionData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/optiondata.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, optionData);
        stream.Close();
    }
    public static OptionsData LoadOptionData()
    {
        string path = Application.persistentDataPath + "/optiondata.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            OptionsData data = formatter.Deserialize(stream) as OptionsData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("Save file not found in" + path);
            return null;
        }
    }
}
