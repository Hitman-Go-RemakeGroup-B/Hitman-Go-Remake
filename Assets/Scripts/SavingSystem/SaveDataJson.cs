using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveDataJson : MonoBehaviour
{
    SaveData data;
    byte[] key = Encoding.ASCII.GetBytes("Picodead"); 
    byte[] iv = Encoding.ASCII.GetBytes("abcdefgh"); 

    
    void Start()
    {
        data= new SaveData();
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
        string json=JsonUtility.ToJson(data);   
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
        SaveData data =JsonUtility.FromJson<SaveData>(decryptedString);
        Debug.Log(decryptedString);
        //data.setdata
    }
}
