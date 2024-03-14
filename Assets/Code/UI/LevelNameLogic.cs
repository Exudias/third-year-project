using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelNameLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private RectTransform rectTransform;

    private Camera mainCam;
    private RectTransform parentRect;

    private Dictionary<string, string> levelToName = new Dictionary<string, string>
    {
        { "1-0", "Beginnings" },
        { "1-1", "Luigi" },
        { "1-2", "The Word" },
        { "1-3", "Zig" },
        { "1-4", "Zag" },
        { "1-5", "The Nile" },
        { "1-6", "No, Se7en" },
        { "1-7", "Uriel" },
        { "1-8", "G-Dawg" },
        { "1-9", "So Below" },
        { "1-10", "The Ladder" },
        { "1-11", "Lúcio" },
        { "1-12", "Yoshi" }
    };

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

        string number = stage + "-" + level;
        string levelName = number + " " + levelToName[number];

        return levelName;
    }
}
