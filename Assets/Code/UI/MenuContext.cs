using UnityEngine;

public class MenuContext : MonoBehaviour
{
    [SerializeField] private bool isEnabled;
    [SerializeField] private MenuButton[] buttons;   

    public delegate void MenuContextEvent(MenuContext context);
    public static event MenuContextEvent OnContextEnabled;
    public static event MenuContextEvent OnContextDisabled;

    public void Enable()
    {
        isEnabled = true;
        gameObject.SetActive(true);
        OnContextEnabled?.Invoke(this);
    }

    public void Disable()
    {
        isEnabled = false;
        gameObject.SetActive(false);
        OnContextDisabled?.Invoke(this);
    }

    public MenuButton[] GetButtons() => buttons;

    public bool IsEnabled() => isEnabled;
}
