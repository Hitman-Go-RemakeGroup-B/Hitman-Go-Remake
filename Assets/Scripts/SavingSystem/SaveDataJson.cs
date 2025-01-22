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
        if (Data == null)
        {
            Data=new SaveData(level.Nlevel);
        }
        else
        {
            LoadData();
        }
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
        string encryptedString = Encrypt(json, key, iv);

        using (StreamWriter writer= new StreamWriter(Application.persistentDataPath + "/" + "SaveData.json"))
        {
            writer.Write(encryptedString);
        }
    }

    public void LoadData()
    {
        string json=string.Empty;
        using (StreamReader reader=new StreamReader(Application.persistentDataPath+ "/" + "SaveData.json"))
        {
            json=reader.ReadToEnd();
        }
        string decryptedString = Decrypt(json, key, iv);
        Data =JsonUtility.FromJson<SaveData>(decryptedString);
    }
    public void SaveLevel(SaveLevel level)
    {
        if (Data.LevelIndex<level.LevelIndex)
        {
            Data.LevelIndex=level.LevelIndex;
        }

        Data.KillKing[level.LevelIndex-1] = level.KillKing;
        Data.MinTurns[level.LevelIndex-1] = level.MinTurns;
        Data.NoEnemy[level.LevelIndex-1] = level.NoEnemy;
        Data.EveryEnemy[level.LevelIndex-1] = level.EveryEnemy;
        Data.QueenEnding[level.LevelIndex - 1] = level.QueenEnding;
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
