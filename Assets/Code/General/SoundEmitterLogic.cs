using UnityEngine;

public class SoundEmitterLogic : MonoBehaviour
{
    public bool useTimescale = true;
    public bool destroyAfterSound = true;

    [SerializeField] private AudioSource source;

    private bool playedSound = false;

    private void Update()
    {
        if (useTimescale && Time.timeScale != 0)
        {
            source.pitch = Time.timeScale;
        }
        if (destroyAfterSound && playedSound && !source.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip sound, float volume)
    {
        source.PlayOneShot(sound, volume * PlayerPrefs.GetFloat("soundVolume", 0.5f));
        playedSound = true;
    }
}
