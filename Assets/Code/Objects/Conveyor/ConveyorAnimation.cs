using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class ConveyorAnimation : MonoBehaviour
{
    [SerializeField] private PushTrigger pushTrigger;
    [SerializeField] private float baseSpeed = 15;

    private Animator anim;
    private SpriteRenderer sprite;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 pushVelocity = pushTrigger.GetVelocity();

        sprite.flipX = pushVelocity.x < 0;

        anim.speed = Mathf.Abs(pushVelocity.x) / baseSpeed;
    }
}
