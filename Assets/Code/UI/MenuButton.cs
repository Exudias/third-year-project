using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private UnityEvent OnActivate;
    [SerializeField] private bool isSlider = false;
    [SerializeField] private UnityEvent OnSlideDown;
    [SerializeField] private UnityEvent OnSlideUp;
    [SerializeField] private string playerPrefsFloatName;

    public void Hover()
    {
        if (bgImage == null) return;
        bgImage.sprite = hoverSprite;
    }

    public void UnHover()
    {
        if (bgImage == null) return;
        bgImage.sprite = defaultSprite;
    }

    public virtual void Activate()
    {
        OnActivate.Invoke();
    }

    public virtual void SlideDown()
    {
        OnSlideDown.Invoke();
        UpdateSliderDisplay();
    }

    public virtual void SlideUp()
    {
        OnSlideUp.Invoke();
        UpdateSliderDisplay();
    }

    private void Start()
    {
        if (isSlider)
        {
            UpdateSliderDisplay();
        }
    }

    private void UpdateSliderDisplay()
    {
        float newValue = PlayerPrefs.GetFloat(playerPrefsFloatName, 0.5f);
        GetComponent<SliderButton>().UpdateDisplayFloatAsPercentage(Mathf.Round(newValue * 100));
    }

    public bool IsSliderButton() => isSlider;
}
