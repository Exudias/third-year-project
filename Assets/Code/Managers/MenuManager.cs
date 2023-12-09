using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuContext[] contexts;

    private MenuContext activeContext = null;
    private MenuButton hoveredButton = null;
    private int activeButtonIndex = 0;

    private InputManager input;

    private void OnEnable()
    {
        MenuContext.OnContextEnabled += OnContextEnabled;
    }

    private void OnDisable()
    {
        MenuContext.OnContextEnabled -= OnContextEnabled;
    }

    private void Start()
    {
        input = InputManager.instance;
        ActivateContext(contexts[0]);
    }

    private void Update()
    {
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

    private void ActivateContext(MenuContext context)
    {
        hoveredButton?.UnHover();
        activeContext = context;
        hoveredButton = context.GetButtons()[0];
        hoveredButton.Hover();
    }

    private void OnContextEnabled(MenuContext context)
    {
        ActivateContext(context);
    }
}
