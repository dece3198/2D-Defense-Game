using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int rank = 0;
    public int ruby = 0;
    public int dia = 0;
    public List<int> cardLevel;
    public int unitLevel;
    public int unitAtkLevel;
    public int unitAtkSpeedLevel;
    public int unitSkillPercentLevel;
    public int unitSkillDamageLevel;
    public int monsterLevel;
    public string lastCheckTimeString;
    public bool isTimeCompensation = true;


}

public class DataManager : Singleton<DataManager>
{
    public PlayerData curData = new PlayerData();
    public string path;
    public string userID;

    private new void Awake()
    {
        base.Awake();
        path = Application.persistentDataPath + "/";
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(curData);
        File.WriteAllText(path + userID, data);
    }

    public void LoadData()
    {
        if(File.Exists(path + userID))
        {
            string data = File.ReadAllText(path + userID);
            curData = JsonUtility.FromJson<PlayerData>(data);
            GameManager.instance.LoadData();
        }
        else
        {
            SaveData();
        }
        curData.lastCheckTimeString = DateTime.Now.ToString("O");
        curData.isTimeCompensation = true;
        GameManager.instance.CheckTime();
    }

    public void DeleteData(string name)
    {
        File.Delete(path + name);
    }

    public void ClearData()
    {
        curData = new PlayerData();
    }

}
