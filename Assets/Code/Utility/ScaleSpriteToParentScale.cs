using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class ScaleSpriteToParentScale : MonoBehaviour
{
    [SerializeField] private int xSlices = 3;
    [SerializeField] private int ySlices = 3;
    private SpriteRenderer sprite;
    private Transform parent;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        parent = transform.parent;
    }

    private void Update()
    {
        transform.localScale = new Vector3(1 / parent.localScale.x, 1 / parent.localScale.y, 1);

        float xSize = sprite.sprite.bounds.max.x - sprite.sprite.bounds.min.x;
        float ySize = sprite.sprite.bounds.max.y - sprite.sprite.bounds.min.y;

        sprite.size = new Vector3(parent.localScale.x * xSize / xSlices, parent.localScale.y * ySize / ySlices, 1);
    }
}
