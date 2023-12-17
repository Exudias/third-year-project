using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RestartSceneOnFinishAnim : MonoBehaviour
{
    private Animator animator;
    private bool reset = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (normalizedTime > 1 && !reset)
        {
            reset = true;
            GameManager.Instance.ResetScene();
            Destroy(gameObject);
        }
    }
}
