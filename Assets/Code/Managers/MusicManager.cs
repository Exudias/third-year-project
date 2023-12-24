using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Tracks")]
    [SerializeField] private AudioClip factoryTrack;
    [Header("Parameters")]
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.2f;

    private AudioSource audioSource;

    private float volume;

    private void Start()
    {
        InitVariables();
        PlaySong(factoryTrack);
    }

    private void OnEnable()
    {
        OptionsManager.OnMusicVolumeChanged += OnMusicVolumeChanged;
    }

    private void OnDisable()
    {
        OptionsManager.OnMusicVolumeChanged -= OnMusicVolumeChanged;
    }

    private void OnMusicVolumeChanged(float newValue)
    {
        volume = newValue;
        audioSource.volume = volume;
    }

    private void Update()
    {
        DoTimescaleEffects();
    }

    private void InitVariables()
    {
        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        audioSource.volume = volume;
    }

    private void DoTimescaleEffects()
    {
        float timeScale = Time.timeScale;
        if (timeScale != 0)
        {
            float targetPitch = Mathf.Clamp(timeScale, minPitch, maxPitch);
            audioSource.pitch = Mathf.MoveTowards(audioSource.pitch, targetPitch, 0.1f);
        }
    }

    public void PlaySong(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
