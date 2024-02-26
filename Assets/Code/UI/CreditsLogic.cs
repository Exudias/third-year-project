using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsText;

    private void Awake()
    {
        float time = SpeedrunManager.time;
        string timeText = TimerLogic.TimeToString(time);
        creditsText.text += timeText;
    }
}
