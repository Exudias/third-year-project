using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float maxTime = 45f;
    [SerializeField] private TextMeshProUGUI timeText;

    private float currentTime;

    private void Start()
    {
        currentTime = maxTime;
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            TimeOut();
        }

        UpdateUI();
    }

    private void TimeOut()
    {
        GameManager.ResetScene();
    }

    private void UpdateUI()
    {
        string formattedTimeLeft = string.Format("{0:0.000}", currentTime);
        string formattedTimeScale = string.Format("{0:0.0}", Time.timeScale);
        timeText.text = formattedTimeLeft + " x" + formattedTimeScale;
    }
}
