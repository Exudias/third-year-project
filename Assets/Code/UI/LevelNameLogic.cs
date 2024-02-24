using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelNameLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
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
        levelText.text = LevelNameToString();
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
        rectTransform.anchoredPosition = new Vector3(timerDisplacementX, timerDisplacementY, 0);
    }

    private string LevelNameToString()
    {
        string sceneName = GameManager.GetCurrentLevel().name;

        string[] parts = sceneName.Split("_");

        string stage = parts[1];
        string level = int.Parse(parts[2]).ToString();

        return stage + " - " + level;
    }
}
