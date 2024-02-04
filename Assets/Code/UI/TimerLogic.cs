using UnityEngine;
using TMPro;

public class TimerLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        timerText.text = TimeToString(SpeedrunManager.time);
    }

    private string TimeToString(float time)
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
