using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool ignoreZ = true;

    private void LateUpdate()
    {
        if (followTransform == null) return;

        Vector3 newPos = followTransform.position + offset;
        
        if (ignoreZ)
        {
            newPos.z = transform.position.z;
        }

        transform.position = newPos;
    }

    public void UpdateFollowTransform(Transform newFollow)
    {
        followTransform = newFollow;
    }
}
