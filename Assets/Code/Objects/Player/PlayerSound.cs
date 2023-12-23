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

    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void OnPlayerJump()
    {
        PlaySound(jumpSound);
    }

    private void OnPlayerWallJump()
    {
        PlaySound(jumpSound);
    }

    private void OnPlayerHitGround()
    {
        PlaySound(landSound);
    }

    private void OnSwitchToSpirit()
    {
        PlaySound(becomeSpiritSound);
    }

    private void OnSwitchToBulb()
    {
        PlaySound(becomeBulbSound);
    }

    private void OnPlayerDeath()
    {
        PlaySound(deathSound);
    }

    private void OnPlayerFootstep()
    {
        PlaySound(walkSound);
    }
}
