using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Tracks")]
    [SerializeField] private AudioClip factoryTrack;
    [Header("Parameters")]
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float minPitch = 0.2f;

    private AudioSource audioSource;

    private void Start()
    {
        InitVariables();
        PlaySong(factoryTrack);
    }

    private void Update()
    {
        DoTimescaleEffects();
    }

    private void InitVariables()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void DoTimescaleEffects()
    {
        float timeScale = Time.timeScale;
        if (timeScale != 0)
        {
            audioSource.pitch = Mathf.Clamp(timeScale, minPitch, maxPitch);
        }
    }

    public void PlaySong(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
