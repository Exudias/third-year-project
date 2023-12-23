using UnityEngine;

// This is garbage, sorry.

public class PlayerSound : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip becomeSpiritSound;
    [SerializeField] private AudioClip becomeBulbSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpSound);
    }

    public void PlayWalkSound()
    {
        PlaySound(walkSound);
    }

    public void PlayLandSound()
    {
        PlaySound(landSound);
    }

    public void PlayDeathSound()
    {
        PlaySound(deathSound);
    }

    public void PlayBecomeSpiritSound()
    {
        PlaySound(becomeSpiritSound);
    }

    public void PlayBecomeBulbSound()
    {
        PlaySound(becomeBulbSound);
    }
}
