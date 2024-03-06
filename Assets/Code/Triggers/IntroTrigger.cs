using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroTrigger : Trigger
{
    [SerializeField] private GameObject emptyBulb;
    [SerializeField] private GameObject particle;
    [SerializeField] private PlayerSound playerSound;
    [SerializeField] private Image flashImage;
    [SerializeField] private Image chapterTitleBG;
    [SerializeField] private TextMeshProUGUI chapterTitleText;

    private bool activated = false;

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

        if (activated) return;

        activated = true;

        StartCoroutine(IntroCutscene(activator.gameObject));
    }

    private IEnumerator IntroCutscene(GameObject player)
    {
        // Init, spawn dummy empty bulb, hide player
        GameManager.SetCutscenePlaying(true);
        player.SetActive(false);
        emptyBulb.SetActive(true);
        // Wait 3 seconds
        yield return new WaitForSeconds(3f);
        // Init particle
        Vector2 particleStart = particle.transform.position;
        particle.transform.right = emptyBulb.transform.position - (Vector3)particleStart;
        float timeTravelled = 0;
        const float totalTravelTime = 0.5f;
        // Move particle towards empty bulb, with easing
        while (timeTravelled < totalTravelTime)
        {
            float t = timeTravelled / totalTravelTime;
            float easedT = EaseInSine(t);
            particle.transform.position = Vector3.Lerp(particleStart, emptyBulb.transform.position, easedT);
            timeTravelled += Time.deltaTime;
            yield return null;
        }
        // When particle hits bulb, hide it, hide bulb, show player
        particle.SetActive(false);
        emptyBulb.SetActive(false);
        player.SetActive(true);
        playerSound.PlayBulbSound();
        // Screen Flash
        float flashTime = 0;
        const float totalFlashTime = 0.25f;
        float inTime = totalFlashTime / 8;
        float outTime = totalFlashTime - inTime;
        while (flashTime < totalFlashTime)
        {
            float flashAlpha;
            if (flashTime <= inTime)
            {
                flashAlpha = Mathf.Lerp(0, 1, flashTime / inTime);
            }
            else
            {
                flashAlpha = Mathf.Lerp(1, 0, (flashTime - inTime) / outTime);
            }
            flashAlpha = EaseInSine(flashAlpha);
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, flashAlpha);
            flashTime += Time.deltaTime;
            yield return null;
        }
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
        // Update cutscene state
        GameManager.SetCutscenePlaying(false);
        // Show chapter title card
        float titleTime = 0f;
        const float titleMaxTime = 3f;
        float titleInTime = titleMaxTime / 4;
        float titleStayTime = titleMaxTime / 2;
        float titleOutTime = titleMaxTime - titleInTime - titleStayTime;
        while (titleTime < titleMaxTime)
        {
            Color bgc = chapterTitleBG.color;
            Color tc = chapterTitleText.color;
            if (titleTime <= titleInTime)
            {
                float t = titleTime / titleInTime;

                bgc.a = t / 2;
                chapterTitleBG.color = bgc;

                tc.a = t;
                chapterTitleText.color = tc;
            }
            else if (titleTime > titleInTime && titleTime <= titleInTime + titleStayTime)
            {
                bgc.a = 0.5f;
                chapterTitleBG.color = bgc;

                tc.a = 1;
                chapterTitleText.color = tc;
            }
            else
            {
                float t = 1 - ((titleTime - titleInTime - titleStayTime) / titleOutTime);

                bgc.a = t / 2;
                chapterTitleBG.color = bgc;

                tc.a = t;
                chapterTitleText.color = tc;
            }
            titleTime += Time.deltaTime;
            yield return null;
        }
        Color bg = chapterTitleBG.color;
        Color text = chapterTitleText.color;
        bg.a = 0;
        text.a = 0;
        chapterTitleBG.color = bg;
        chapterTitleText.color = text;
        Destroy(gameObject); // remove self after completion
    }

    // From easings.net
    private float EaseInSine(float t)
    {
        return 1 - Mathf.Cos(t * Mathf.PI / 2);
    }
}
