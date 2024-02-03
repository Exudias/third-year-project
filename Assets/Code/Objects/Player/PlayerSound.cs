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
        SpiritMovement.OnSpiritHitSolid += OnSpiritHitSolid;
        PlayerFormSwitcher.OnSwitchToSpirit += OnSwitchToSpirit;
        PlayerFormSwitcher.OnSwitchToBulb += OnSwitchToBulb;
        PlayerLogic.OnPlayerDeath += OnPlayerDeath;
        PlayerVisualsManager.OnPlayerFootstep += OnPlayerFootstep;
        OptionsManager.OnSoundVolumeChanged += OnSoundVolumeChanged;
    }

    private void OnDisable()
    {
        BulbMovement.OnPlayerJump -= OnPlayerJump;
        BulbMovement.OnPlayerWallJump -= OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround -= OnPlayerHitGround;
        SpiritMovement.OnSpiritHitSolid -= OnSpiritHitSolid;
        PlayerFormSwitcher.OnSwitchToSpirit -= OnSwitchToSpirit;
        PlayerFormSwitcher.OnSwitchToBulb -= OnSwitchToBulb;
        PlayerLogic.OnPlayerDeath -= OnPlayerDeath;
        PlayerVisualsManager.OnPlayerFootstep -= OnPlayerFootstep;
        OptionsManager.OnSoundVolumeChanged -= OnSoundVolumeChanged;
    }

    private const float TIME_BETWEEN_SPIRIT_BOUNCE = .2f;
    private float timeSinceSpiritBounce = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("soundVolume", 1f);
    }

    private void Update()
    {
        timeSinceSpiritBounce += Time.unscaledDeltaTime;
    }

    private void OnSoundVolumeChanged(float newValue)
    {
        audioSource.volume = newValue;
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

    private void OnSpiritHitSolid()
    {
        if (timeSinceSpiritBounce < TIME_BETWEEN_SPIRIT_BOUNCE) return;

        timeSinceSpiritBounce = 0;

        PlaySound(becomeSpiritSound, 0.5f, true);
    }

    public void PlayBulbSound()
    {
        OnSwitchToBulb();
    }
}
