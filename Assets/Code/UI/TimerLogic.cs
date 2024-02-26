using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RectTransform rectTransform;

    private Camera mainCam;
    private RectTransform parentRect;

    private void Start()
    {
        mainCam = Camera.main;
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        timerText.text = TimeToString(SpeedrunManager.time);
        SyncToBlackBars();
    }

    private void SyncToBlackBars()
    {
        // Get canvas size
        float uiWidth = parentRect.rect.width;
        float uiHeight = parentRect.rect.height;
        // Calculate X displacement
        float verticalBlackBars = Screen.width - mainCam.pixelWidth;
        float singleBlackBarWidth = verticalBlackBars / 2;
        float singleBlackBarPortionOfScreenX = singleBlackBarWidth / Screen.width;
        float timerDisplacementX = singleBlackBarPortionOfScreenX * uiWidth;
        // Calculate Y displacement
        float horizontalBlackBars = Screen.height - mainCam.pixelHeight;
        float singleBlackBarHeight = horizontalBlackBars / 2;
        float singleBlackBarPortionOfScreenY = singleBlackBarHeight / Screen.height;
        float timerDisplacementY = singleBlackBarPortionOfScreenY * uiHeight;
        // Apply displacement
        rectTransform.anchoredPosition = new Vector3(timerDisplacementX, -timerDisplacementY, 0);
    }

    public static string TimeToString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        float milisecondsRaw = time - Mathf.Floor(time);
        int ms = (int)(milisecondsRaw * 100) % 100;

        string strMins = minutes.ToString("00");
        string strSeconds = seconds.ToString("00");
        string strMs = ms.ToString("00");

        return strMins + ":" + strSeconds + "." + strMs;
    }
}
