using UnityEngine;
using TMPro;

public class SliderButton : MonoBehaviour
{
    [SerializeField] private string variableDisplayName;
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateDisplayFloatAsPercentage(float toDisplay)
    {
        text.text = variableDisplayName + ": " + toDisplay + "%";
    }
}
