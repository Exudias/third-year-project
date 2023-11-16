using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    public void OnButtonPressed()
    {
        // Open door or whatever
        Debug.Log("Pressed!");
    }

    public void OnButtonUnpressed()
    {
        // Close door or whatever
        Debug.Log("Unpressed!");
    }
}
