using TMPro;
using UnityEngine;

public class DayCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI dayCounterText;

    private void OnEnable()
    {
        dayCounterText.text = $"Day: {TimeCycleManager.Instance.CurrentDay}";
    }
}
