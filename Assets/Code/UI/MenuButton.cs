using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private UnityEvent OnActivate;

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
}
