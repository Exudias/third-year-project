using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] activateables;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip unpressSound;

    private const float SOUND_VOLUME = 0.5f;

    public void OnButtonPressed()
    {
        foreach (GameObject a in activateables)
        {
            a.GetComponent<IActivatable>().Activate();
        }
        
        AudioSource.PlayClipAtPoint(pressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 0.5f) * SOUND_VOLUME);
    }

    public void OnButtonUnpressed()
    {
        foreach (GameObject a in activateables)
        {
            a.GetComponent<IActivatable>().Deactivate();
        }
        AudioSource.PlayClipAtPoint(unpressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 0.5f) * SOUND_VOLUME);
    }
}
