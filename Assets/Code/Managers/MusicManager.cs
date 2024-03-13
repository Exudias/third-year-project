using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Tracks")]
    [SerializeField] private AudioClip factoryTrack;
    [SerializeField] private AudioClip menuTrack;
    [Header("Parameters")]
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.2f;

    private AudioSource audioSource;

    private float volume;

    private void Start()
    {
        InitVariables();
        if (GameManager.IsInMenu())
        {
            PlaySong(menuTrack);
        }
        else
        {
            PlaySong(factoryTrack);
        }
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
        if (!GameManager.GetCreditsPlaying())
        {
            DoTimescaleEffects();
        }
        else
        {
            audioSource.pitch = 1.5f;
        }
        CheckForPauseSong();
    }

    private void InitVariables()
    {
        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        audioSource.volume = volume;
    }

    private void CheckForPauseSong()
    {
        bool isCutscenePlaying = GameManager.GetPlayingCutscene();
        if (isCutscenePlaying && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else if (!isCutscenePlaying && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
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
