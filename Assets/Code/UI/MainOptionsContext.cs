using UnityEngine;

public class MainOptionsContext : MonoBehaviour
{
    MenuContext menuContext;

    private void Start()
    {
        menuContext = GetComponent<MenuContext>();
    }

    public void ActivateMenuContext()
    {
        MenuManager.instance.GetActiveContext().Disable();
        MenuManager.instance.GetActiveContext().gameObject.SetActive(false);
        menuContext.gameObject.SetActive(true);
        menuContext.Enable();
    }
}
