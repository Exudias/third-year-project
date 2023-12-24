using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private float soundVolume;
    private float musicVolume;

    public delegate void OptionsFloatChangedEvent(float newValue);
    public static event OptionsFloatChangedEvent OnMusicVolumeChanged;
    public static event OptionsFloatChangedEvent OnSoundVolumeChanged;

    private void Start()
    {
        soundVolume = PlayerPrefs.GetFloat("soundVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
    }

    public void AddMusicVolume(float amount)
    {
        musicVolume = Mathf.Clamp01(musicVolume + amount);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        OnMusicVolumeChanged?.Invoke(musicVolume);
    }

    public void AddSoundVolume(float amount)
    {
        soundVolume = Mathf.Clamp01(soundVolume + amount);
        PlayerPrefs.SetFloat("soundVolume", soundVolume);
        OnSoundVolumeChanged?.Invoke(soundVolume);
    }
}
