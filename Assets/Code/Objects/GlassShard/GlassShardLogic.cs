using UnityEngine;

public class GlassShardLogic : MonoBehaviour
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float timeBetweenPoints;

    float currentLerp;

    public void Initialize(Vector2 start, Vector2 end, float timeBetween)
    {
        currentLerp = 0;
        startPoint = start;
        endPoint = end;
        timeBetweenPoints = timeBetween;
        // pick random sprite
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    private void Update()
    {
        currentLerp += Time.deltaTime / timeBetweenPoints;
        
        if (currentLerp >= 1)
        {
            currentLerp -= Mathf.Floor(currentLerp); // keep between 0 and 1
        }

        transform.position = Vector2.Lerp(startPoint, endPoint, currentLerp);
    }
}
