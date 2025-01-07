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
        if (!dataJson.data.Music)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(1) * 20);
            dataJson.data.Music = true;
            SaveVolume(dataJson);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
            dataJson.data.Music = false;
            SaveVolume(dataJson);
        }
    }

    public void SwapSFXVolume()
    {
        if (!dataJson.data.SFX)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
            dataJson.data.SFX = true;
            SaveVolume(dataJson);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
            dataJson.data.SFX = false;
            SaveVolume(dataJson);
        }           
    }

    public void SetMusicVolume()
    {
        if (dataJson.data.Music)
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
        if (dataJson.data.SFX)
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
