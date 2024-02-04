using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private GameObject activateable;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip unpressSound;

    private const float SOUND_VOLUME = 0.5f;

    public void OnButtonPressed()
    {
        activateable.GetComponent<IActivatable>().Activate();
        AudioSource.PlayClipAtPoint(pressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 1f) * SOUND_VOLUME);
    }

    public void OnButtonUnpressed()
    {
        activateable.GetComponent<IActivatable>().Deactivate();
        AudioSource.PlayClipAtPoint(unpressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 1f) * SOUND_VOLUME);
    }
}
