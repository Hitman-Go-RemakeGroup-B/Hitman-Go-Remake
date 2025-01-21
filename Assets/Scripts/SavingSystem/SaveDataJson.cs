using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveDataJson : MonoBehaviour
{

    [SerializeField]LevelScriptableObject level;

    public int starCount=0;

    public static Action<SaveLevel> levelAction;
    public SaveData Data;
    byte[] key = Encoding.ASCII.GetBytes("Picodead"); 
    byte[] iv = Encoding.ASCII.GetBytes("abcdefgh");

    private void OnEnable()
    {
        levelAction += SaveLevel;
    }

    private void OnDisable()
    {
        levelAction  -= SaveLevel;
    }

    public void RestoreGame()
    {
        Data = new SaveData(level.Nlevel);
        SaveData();
        Debug.Log(JsonUtility.ToJson(Data));
    }

    public static string Encrypt(string plainText, byte[] key, byte[] iv)
    {
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            ICryptoTransform encryptor = des.CreateEncryptor(key, iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    public static string Decrypt(string encryptedText, byte[] key, byte[] iv)
    {
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            ICryptoTransform decryptor = des.CreateDecryptor(key, iv);
            using (MemoryStream ms = new(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(Data);

        Debug.Log(json);
        string encryptedString = Encrypt(json, key, iv);

        using (StreamWriter writer= new StreamWriter(Application.dataPath + "/" + "SaveData.json"))
        {
            writer.Write(encryptedString);
            //writer.Write(json);
        }

    }

    public void LoadData()
    {
        string json=string.Empty;
        using (StreamReader reader=new StreamReader(Application.dataPath+ "/" + "SaveData.json"))
        {
            json=reader.ReadToEnd();
        }
        string decryptedString = Decrypt(json, key, iv);
        Data =JsonUtility.FromJson<SaveData>(decryptedString);
        //Data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log(decryptedString);
    }
    public void SaveLevel(SaveLevel level)
    {
        if (Data==null||level.LevelIndex >= Data.LevelIndex)
        {
            Data = new SaveData(this.level.Nlevel);
            Data.LevelIndex = level.LevelIndex;
        }
        Data.KillKing[level.LevelIndex] = level.KillKing;
        Data.MinTurns[level.LevelIndex] = level.MinTurns;
        Data.NoEnemy[level.LevelIndex] = level.NoEnemy;
        Data.EveryEnemy[level.LevelIndex] = level.EveryEnemy;
        Data.QueenEnding[level.LevelIndex] = level.QueenEnding;
        //Debug.Log(Data.ToString());
        SaveData();
    }

    public void VisualizeStar(int i)
    {
        starCount = 0;
        if (Data == null)
        {
            Data=new SaveData(this.level.Nlevel);
            SaveData();
        }
        LoadData();
        if (Data.EveryEnemy[i])
        {
            starCount++;
        }else if (Data.KillKing[i])
        {
            starCount++; 
        }else if (Data.QueenEnding[i])
        {
            starCount++;
        }else if (Data.NoEnemy[i])
        {
            starCount++;
        }else if (Data.MinTurns[i])
        {
            starCount++;
        }



    }

}
