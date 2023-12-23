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
    [Header("Parameters")]
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    private AudioSource audioSource;

    private void OnEnable()
    {
        BulbMovement.OnPlayerJump += OnPlayerJump;
        BulbMovement.OnPlayerWallJump += OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround += OnPlayerHitGround;
        PlayerFormSwitcher.OnSwitchToSpirit += OnSwitchToSpirit;
        PlayerFormSwitcher.OnSwitchToBulb += OnSwitchToBulb;
        PlayerLogic.OnPlayerDeath += OnPlayerDeath;
        PlayerVisualsManager.OnPlayerFootstep += OnPlayerFootstep;
    }

    private void OnDisable()
    {
        BulbMovement.OnPlayerJump -= OnPlayerJump;
        BulbMovement.OnPlayerWallJump -= OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround -= OnPlayerHitGround;
        PlayerFormSwitcher.OnSwitchToSpirit -= OnSwitchToSpirit;
        PlayerFormSwitcher.OnSwitchToBulb -= OnSwitchToBulb;
        PlayerLogic.OnPlayerDeath -= OnPlayerDeath;
        PlayerVisualsManager.OnPlayerFootstep -= OnPlayerFootstep;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip, float volume = 1, bool randomizePitch = false)
    {
        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            audioSource.pitch = 1;
        }
        audioSource.PlayOneShot(clip, volume);
    }

    private void OnPlayerJump()
    {
        PlaySound(jumpSound, 0.5f, true);
    }

    private void OnPlayerWallJump()
    {
        PlaySound(jumpSound);
    }

    private void OnPlayerHitGround()
    {
        PlaySound(walkSound, 0.5f, true);
    }

    private void OnSwitchToSpirit()
    {
        PlaySound(becomeSpiritSound);
    }

    private void OnSwitchToBulb()
    {
        PlaySound(becomeBulbSound, 0.5f, true);
    }

    private void OnPlayerDeath()
    {
        PlaySound(deathSound, 0.4f);
    }

    private void OnPlayerFootstep()
    {
        PlaySound(walkSound, 0.1f, true);
    }
}
