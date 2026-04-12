using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject sceneLight;

    private Quaternion lightRotation;

    private float cycleTime = 0f;
    private float cycleDuration = 720f; // 12 minutes in seconds
    private float cycleDayDuration = 480f; // 8 minutes in seconds
    private float dayPercent; //The percentage of time that the day covers

    private int currentDay = 0;

    private void Start()
    {
        lightRotation = sceneLight.transform.rotation;
        lightRotation.x = 0;
        dayPercent = 100 / cycleDuration * cycleDayDuration; //Should be around 66%
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCycle();
        UpdateLighting();
    }

    private void UpdateCycle()
    {
        if(cycleTime < cycleDuration)
        {
            cycleTime += Time.deltaTime;
        }
        else
        {
            cycleTime = 0f;
            currentDay++;
        }
    }

    private void UpdateLighting()
    {
        //TODO: This will update the lighting and the skybox
        if(cycleTime < cycleDayDuration)
        {
            //Day period
            lightRotation.x += 180 / cycleDayDuration * Time.deltaTime;
        }
        else
        {
            //Night period
            lightRotation.x += 180 / (cycleDuration - cycleDayDuration) * Time.deltaTime;
        }

        sceneLight.transform.rotation = lightRotation;
    }
}
