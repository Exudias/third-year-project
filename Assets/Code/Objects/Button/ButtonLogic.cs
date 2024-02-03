using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private GameObject activateable;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip unpressSound;

    public void OnButtonPressed()
    {
        activateable.GetComponent<IActivatable>().Activate();
        AudioSource.PlayClipAtPoint(pressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 1f));
    }

    public void OnButtonUnpressed()
    {
        activateable.GetComponent<IActivatable>().Deactivate();
        AudioSource.PlayClipAtPoint(unpressSound, transform.position, PlayerPrefs.GetFloat("soundVolume", 1f));
    }
}
