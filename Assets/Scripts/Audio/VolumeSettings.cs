using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField]SaveDataJson dataJson;   

    private void Start()
    {
        LoadVolume(dataJson);
        SetMusicVolume();
        SetSFXVolume();
    }


    public void SwapMusicVolume()
    {
        if (!dataJson.Data.Music)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(1) * 20);
            dataJson.Data.Music = true;
            SaveVolume(dataJson);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
            dataJson.Data.Music = false;
            SaveVolume(dataJson);
        }
    }

    public void SwapSFXVolume()
    {
        if (!dataJson.Data.SFX)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
            dataJson.Data.SFX = true;
            SaveVolume(dataJson);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
            dataJson.Data.SFX = false;
            SaveVolume(dataJson);
        }           
    }

    public void SetMusicVolume()
    {
        if (dataJson.Data.Music)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(1) * 20);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
        }
    }

    public void SetSFXVolume()
    {
        if (dataJson.Data.SFX)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
        }
    }

    private void LoadVolume(SaveDataJson data)
    {
        data.LoadData();     
    }
    private void SaveVolume(SaveDataJson data)
    {
        data.SaveData();
    }

}
