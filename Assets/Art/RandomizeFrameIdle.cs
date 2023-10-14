using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomizeFrameIdle : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        animator.Play(clip.name, 0, Random.value);
    }
}
