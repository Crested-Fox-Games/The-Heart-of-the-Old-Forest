using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject sceneLight;

    private Vector3 lightRotation;

    private float cycleTime = 0f;
    private float cycleDuration = 720f; // 12 minutes in seconds
    private float cycleDayDuration = 480f; // 8 minutes in seconds
    private float sunAngle = 0; // Used in calculating sun position
    private int currentDay = 0;

    private void Start()
    {
        lightRotation = sceneLight.transform.localEulerAngles;
        lightRotation.x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCycle();
        UpdateLighting();
    }

    private void UpdateCycle()
    {
        if(cycleTime >= cycleDuration)
        {
            cycleTime -= cycleDuration;
            currentDay++;
        }
        else
        {
            cycleTime += Time.deltaTime;
        }
    }

    private void UpdateLighting()
    {
        //This will update the lighting and the skybox
        if(cycleTime < cycleDayDuration)
        {
            //Day period
            sunAngle = 180f / cycleDayDuration * cycleTime;
        }
        else
        {
            //Night period
            sunAngle = 180f / (cycleDuration - cycleDayDuration) * cycleTime;
        }

        if (sunAngle >= 360f)
            sunAngle -= 360f;

        sceneLight.transform.localRotation = Quaternion.Euler(sunAngle, lightRotation.y, lightRotation.z);
    }
}
