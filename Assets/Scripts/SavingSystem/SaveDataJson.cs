using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveDataJson : MonoBehaviour
{
    public SaveData Data;
    public SaveLevel Level;
    byte[] key = Encoding.ASCII.GetBytes("Picodead"); 
    byte[] iv = Encoding.ASCII.GetBytes("abcdefgh"); 

    void Start()
    {
        Data= new SaveData();
    }

    public void RestoreGame()
    {
        Data = new SaveData();
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
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
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
        string json=JsonUtility.ToJson(Data);   
        Debug.Log(json);
        string encryptedString = Encrypt(json, key, iv);

        using (StreamWriter writer= new StreamWriter(Application.dataPath + "/" + "SaveData.json"))
        {
            writer.Write(encryptedString);
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
        Debug.Log(decryptedString);
    }

    public void SaveLevel()
    {
        SaveLevel level=this.Level;
        if (level.LevelIndex>=Data.LevelIndex)
        {
            Data.LevelIndex = level.LevelIndex;
        }
        Data.KillKing[level.LevelIndex] = level.KillKing;
        Data.MinTurns[level.LevelIndex] =level.MinTurns;
        Data.NoEnemy[level.LevelIndex] = level.NoEnemy;
        Data.EveryEnemy[level.LevelIndex] = level.EveryEnemy;
        Data.QueenEnding[level.LevelIndex]=level.QueenEnding;
    }

}
