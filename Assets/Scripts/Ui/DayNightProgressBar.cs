using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayNightProgressBar : MonoBehaviour
{
    /// <summary>
    /// The bar for filling up to night time
    /// </summary>
    [SerializeField]
    private Image nightBar;

    /// <summary>
    /// The bar for filling up to day time
    /// </summary>
    [SerializeField]
    private Image dayBar;

    [SerializeField]
    private TextMeshProUGUI dayText;

    private TimeCycleManager timeManager;

    private void Start()
    {
        timeManager = TimeCycleManager.Instance;

        timeManager.OnNightEnd += OnNewDay;
    }

    private void Update()
    {
        //Checks to see if its day time
        if(timeManager.cycleTime.Value < timeManager.CycleDayDuration)
        {
            nightBar.fillAmount = timeManager.cycleTime.Value / timeManager.CycleDayDuration;
        }
        else
        {
            nightBar.fillAmount = 1;

            dayBar.fillAmount = (timeManager.cycleTime.Value - timeManager.CycleDayDuration) / timeManager.CycleNightDuration;
        }
    }

    private void OnNewDay()
    {
        nightBar.fillAmount = 0;
        dayBar.fillAmount = 0;
        dayText.text = $"Day {TimeCycleManager.Instance.CurrentDay}";
    }
}