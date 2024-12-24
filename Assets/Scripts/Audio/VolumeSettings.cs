using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    private int musicMute=1;
    private int SFXMute=1;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume")){
            LoadVolume();
            LoadSFX();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SwapMusicVolume()
    {
        if (musicMute == 0)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(1) * 20);
            musicMute = 1;
            PlayerPrefs.SetInt("MusicVolume", musicMute);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
            musicMute = 0;
            PlayerPrefs.SetInt("MusicVolume", musicMute);
        }
    }

    public void SwapSFXVolume()
    {
        if (SFXMute==0)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
            SFXMute=1;
            PlayerPrefs.SetInt("SFXVolume", SFXMute);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
            SFXMute =0;
            PlayerPrefs.SetInt("SFXVolume", SFXMute);
        }           
    }

    private void SetMusicVolume()
    {
        if (musicMute == 1)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(1) * 20);
            musicMute = 1;
            PlayerPrefs.SetInt("MusicVolume", musicMute);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(0.0001f) * 20);
            musicMute = 0;
            PlayerPrefs.SetInt("MusicVolume", musicMute);
        }
    }
    private void SetSFXVolume()
    {
        if (SFXMute==1)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(1) * 20);
            SFXMute = 1;
            PlayerPrefs.SetInt("SFXVolume", SFXMute);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(0.0001f) * 20);
            SFXMute = 0;
            PlayerPrefs.SetInt("SFXVolume", SFXMute);
        }
    }

    private void LoadVolume()
    {
        musicMute = PlayerPrefs.GetInt("MusicVolume");

        SetMusicVolume();
    }

    private void LoadSFX()
    {
        SFXMute = PlayerPrefs.GetInt("SFXVolume");

        SetSFXVolume();
    }

}
