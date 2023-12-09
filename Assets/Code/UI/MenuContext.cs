using UnityEngine;

public class MenuContext : MonoBehaviour
{
    [SerializeField] private MenuButton[] buttons;

    public delegate void MenuContextEvent(MenuContext context);
    public static event MenuContextEvent OnContextEnabled;
    public static event MenuContextEvent OnContextDisabled;

    public void Enable()
    {
        OnContextEnabled?.Invoke(this);
    }

    public void Disable()
    {
        OnContextDisabled?.Invoke(this);
    }

    public MenuButton[] GetButtons() => buttons;
}
