using System.Collections;
using UnityEngine;

public class IntroTrigger : Trigger
{
    [SerializeField] private GameObject emptyBulb;
    [SerializeField] private GameObject particle;

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

        if (GameManager.GetSceneFrames() > 60) return;

        StartCoroutine(IntroCutscene(activator.gameObject));
    }

    private IEnumerator IntroCutscene(GameObject player)
    {
        player.SetActive(false);
        emptyBulb.SetActive(true);
        yield return new WaitForSeconds(3f);
        Vector2 particleStart = particle.transform.position;
        particle.transform.right = emptyBulb.transform.position - (Vector3)particleStart;
        float timeTravelled = 0;
        const float totalTravelTime = 0.5f;
        while (timeTravelled < totalTravelTime)
        {
            particle.transform.position = Vector3.Lerp(particleStart, emptyBulb.transform.position, timeTravelled / totalTravelTime);
            timeTravelled += Time.deltaTime;
            yield return null;
        }
        particle.SetActive(false);
        emptyBulb.SetActive(false);
        player.SetActive(true);
    }
}
