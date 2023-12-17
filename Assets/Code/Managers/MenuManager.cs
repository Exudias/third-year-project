using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private MenuContext[] contexts;
    [SerializeField] private GameObject dimmer;
    [SerializeField] private Animator screenTransition;
    [SerializeField] private MenuContext pauseContext;

    private MenuContext activeContext = null;
    private MenuButton hoveredButton = null;
    private int activeButtonIndex = 0;

    private InputManager input;

    private void OnEnable()
    {
        MenuContext.OnContextEnabled += OnContextEnabled;
        MenuContext.OnContextDisabled += OnContextDisabled;
    }

    private void OnDisable()
    {
        MenuContext.OnContextEnabled -= OnContextEnabled;
    }

    private void Start()
    {
        instance = this;
        input = InputManager.instance;
        for (int i = 0; i < contexts.Length; i++)
        {
            if (contexts[i].IsEnabled())
            {
                ActivateContext(contexts[i]);
                break;
            }
        }
    }

    private void Update()
    {
        if (activeContext == null) return;
        // Both enters or space press selected button
        if (input.IsDown(KeyCode.Return) || input.IsDown(KeyCode.Space) || input.IsDown(KeyCode.KeypadEnter))
        {
            hoveredButton.Activate();
        }
        if (input.IsDown(KeyCode.DownArrow) || input.IsDown(KeyCode.S))
        {
            activeButtonIndex++;
            hoveredButton.UnHover();
            MenuButton[] ctxBtns = activeContext.GetButtons();
            // Menu wrap
            if (activeButtonIndex == ctxBtns.Length)
            {
                activeButtonIndex = 0;
            }
            hoveredButton = activeContext.GetButtons()[activeButtonIndex];
            hoveredButton.Hover();
        }
        if (input.IsDown(KeyCode.UpArrow) || input.IsDown(KeyCode.W))
        {
            activeButtonIndex--;
            hoveredButton.UnHover();
            MenuButton[] ctxBtns = activeContext.GetButtons();
            // Menu wrap
            if (activeButtonIndex < 0)
            {
                activeButtonIndex = ctxBtns.Length - 1;
            }
            hoveredButton = activeContext.GetButtons()[activeButtonIndex];
            hoveredButton.Hover();
        }
    }

    public float PlayTransitionOut()
    {
        screenTransition.SetTrigger("Out");
        return screenTransition.GetCurrentAnimatorStateInfo(0).length;
    }

    public bool ShowPauseMenu()
    {
        if (pauseContext == null) return false;
        SetDimmerActive(true);
        pauseContext.Enable();
        return true;
    }

    public void HidePauseMenu()
    {
        if (pauseContext == null) return;
        SetDimmerActive(false);
        pauseContext.Disable();
    }

    private void SetDimmerActive(bool active)
    {
        if (dimmer == null) return;
        dimmer.SetActive(active);
    }

    private void ActivateContext(MenuContext context)
    {
        hoveredButton?.UnHover();
        activeContext = context;
        hoveredButton = context.GetButtons()[0];
        hoveredButton.Hover();
    }

    private void DeactivateContext(MenuContext context)
    {
        hoveredButton?.UnHover();
        activeContext = null;
        hoveredButton = null;
    }

    private void OnContextDisabled(MenuContext context)
    {
        DeactivateContext(context);
    }

    private void OnContextEnabled(MenuContext context)
    {
        ActivateContext(context);
    }
}
