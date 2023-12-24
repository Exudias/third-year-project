using UnityEngine;

public class FollowObjectWithTag : MonoBehaviour
{
    [SerializeField] private string tagToFollow;
    [SerializeField] private float timeBetweenPolls = 0.5f;

    private Transform following;

    private float timeUntilNextParse;

    private void Start()
    {
        following = FindTransformToFollow();
    }

    private void Update()
    {
        timeUntilNextParse -= Time.unscaledDeltaTime;
        if (following == null && timeUntilNextParse <= 0f)
        {
            timeUntilNextParse = timeBetweenPolls;
            following = FindTransformToFollow();
        }
        else if (following != null)
        {
            transform.position = following.position;
        }
    }

    private Transform FindTransformToFollow()
    {
        GameObject[] toFollows = GameObject.FindGameObjectsWithTag(tagToFollow);
        if (toFollows.Length == 0)
        {
            return null;
        }
        return toFollows[0].transform;
    }
}
