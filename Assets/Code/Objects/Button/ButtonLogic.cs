using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private GameObject activateable;

    public void OnButtonPressed()
    {
        activateable.GetComponent<IActivatable>().Activate();
    }

    public void OnButtonUnpressed()
    {
        activateable.GetComponent<IActivatable>().Deactivate();
    }
}
