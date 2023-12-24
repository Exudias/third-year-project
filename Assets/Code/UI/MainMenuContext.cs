using UnityEngine;

public class MainMenuContext : MonoBehaviour
{
    public void ActivateMenuContext()
    {
        MenuContext previousContext = MenuManager.instance.GetActiveContext();
        previousContext.Disable();
        previousContext.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GetComponent<MenuContext>().Enable();
    }
}
