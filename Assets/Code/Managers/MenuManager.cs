using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private MenuContext[] contexts;
    [SerializeField] private GameObject dimmer;
    [SerializeField] private Animator screenTransition;
    [SerializeField] private MenuContext pauseContext;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip buttonMove;
    [SerializeField] private AudioClip sliderMove;

    private MenuContext activeContext = null;
    private MenuButton hoveredButton = null;
    private int activeButtonIndex = 0;

    private InputManager input;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (activeContext == null) return;

        if (hoveredButton.IsSliderButton())
        {
            if (input.IsDown(KeyCode.LeftArrow) || input.IsDown(KeyCode.A))
            {
                hoveredButton.SlideDown();
                audioSource.PlayOneShot(sliderMove, PlayerPrefs.GetFloat("soundVolume", 1f));
            }
            else if (input.IsDown(KeyCode.RightArrow) || input.IsDown(KeyCode.D))
            {
                hoveredButton.SlideUp();
                audioSource.PlayOneShot(sliderMove, PlayerPrefs.GetFloat("soundVolume", 1f));
            }
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            // Both enters or space press selected button
            if (input.IsDown(KeyCode.Return) || input.IsDown(KeyCode.Space) || input.IsDown(KeyCode.KeypadEnter))
            {
                hoveredButton.Activate();
                audioSource.PlayOneShot(buttonClick, PlayerPrefs.GetFloat("soundVolume", 1f));
            }
#elif (UNITY_WEBGL)
        // Only space to press selected button on WebGL
        if (input.IsDown(KeyCode.Space))
        {
            hoveredButton.Activate();
            audioSource.PlayOneShot(buttonClick, PlayerPrefs.GetFloat("soundVolume", 1f));
        }
#endif
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
            // Sounds
            audioSource.PlayOneShot(buttonMove, PlayerPrefs.GetFloat("soundVolume", 1f));
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
            // Sounds
            audioSource.PlayOneShot(buttonMove, PlayerPrefs.GetFloat("soundVolume", 1f));
        }
    }

    public float PlayTransitionOut()
    {
        screenTransition.SetTrigger("Out");
        return screenTransition.GetCurrentAnimatorStateInfo(0).length;
    }

    public bool ShowPauseMenu()
    {
        if (pauseContext == null || GameManager.IsLoadingScene()) return false;
        SetDimmerActive(true);
        pauseContext.Enable();
        audioSource.PlayOneShot(buttonClick, PlayerPrefs.GetFloat("soundVolume", 1f));
        return true;
    }

    public void HidePauseMenu(bool playSound = false)
    {
        if (pauseContext == null) return;
        SetDimmerActive(false);
        if (playSound)
        {
            audioSource.PlayOneShot(buttonClick, PlayerPrefs.GetFloat("soundVolume", 1f));
        }
        activeContext?.Disable();
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
        activeButtonIndex = 0;
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

    public MenuContext GetActiveContext() => activeContext;
}
