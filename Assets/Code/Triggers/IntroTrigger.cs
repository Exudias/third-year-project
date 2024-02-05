using System.Collections;
using UnityEngine;

public class IntroTrigger : Trigger
{
    [SerializeField] private GameObject emptyBulb;
    [SerializeField] private GameObject particle;
    [SerializeField] private PlayerSound playerSound;

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEntered;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEntered;
    }

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
    }

    private void OnEntered(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;

        if (!collisionIsPlayer) return;

        if (GameManager.GetSceneFrames() > 60) return; // for reentry from the right

        if (SpeedrunManager.time > 0.1f) return; // for retry

        StartCoroutine(IntroCutscene(activator.gameObject));
    }

    private IEnumerator IntroCutscene(GameObject player)
    {
        GameManager.SetCutscenePlaying(true);
        player.SetActive(false);
        emptyBulb.SetActive(true);
        yield return new WaitForSeconds(3f);
        Vector2 particleStart = particle.transform.position;
        particle.transform.right = emptyBulb.transform.position - (Vector3)particleStart;
        float timeTravelled = 0;
        const float totalTravelTime = 0.5f;
        while (timeTravelled < totalTravelTime)
        {
            float t = timeTravelled / totalTravelTime;
            float easedT = EaseInSine(t);
            particle.transform.position = Vector3.Lerp(particleStart, emptyBulb.transform.position, easedT);
            timeTravelled += Time.deltaTime;
            yield return null;
        }
        particle.SetActive(false);
        emptyBulb.SetActive(false);
        player.SetActive(true);
        playerSound.PlayBulbSound();
        GameManager.SetCutscenePlaying(false);
    }

    // From easings.net
    private float EaseInSine(float t)
    {
        return 1 - Mathf.Cos(t * Mathf.PI / 2);
    }
}
