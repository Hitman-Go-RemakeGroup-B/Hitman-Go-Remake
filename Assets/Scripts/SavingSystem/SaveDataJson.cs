using System.IO;
using UnityEngine;

public class SaveDataJson : MonoBehaviour
{
    SaveData data;
    void Start()
    {
        data= new SaveData();
    }

    public void SaveData()
    {
        string json=JsonUtility.ToJson(data);   
        Debug.Log(json);

        using(StreamWriter writer= new StreamWriter(Application.dataPath + "/" + "SaveDara.json"))
        {
            writer.Write(json);
        }

    }

    public void LoadData()
    {
        string json=string.Empty;

        using(StreamReader reader=new StreamReader(Application.dataPath+ "/" + "SaveData.json"))
        {
            json=reader.ReadToEnd();
        }
        SaveData data =JsonUtility.FromJson<SaveData>(json);
        //data.setdata
    }
}
