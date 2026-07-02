using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

public class TimeManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject sceneLight;

    private Vector3 lightRotation;

    public readonly SyncVar<float> cycleTime = new();
    private float cycleDuration = 720f; // 12 minutes in seconds
    private float cycleDayDuration = 480f; // 8 minutes in seconds
    private float cycleNightDuration = 240f; // 4 minutes in seconds
    private float sunAngle = 0; // Used in calculating sun position
    private int currentDay = 0;

    /// <summary>
    /// This event fires when the night starts
    /// </summary>
    public event Action OnNightStart;
    /// <summary>
    /// This event fires when the night ends
    /// </summary>
    public event Action OnNightEnd;

    /// <summary>
    /// Internal bool for tracking if its night time
    /// </summary>
    private bool isNight = false;

    public override void OnStartServer()
    {
        //Set the time to 0 when the server starts up
        cycleTime.Value = 0f;

        lightRotation = sceneLight.transform.localEulerAngles;
        lightRotation.x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServerStarted)
            return;

        UpdateCycle();
        UpdateLighting();
    }

    private void UpdateCycle()
    {
        if(cycleTime.Value >= cycleDuration)
        {
            cycleTime.Value -= cycleDuration;
            currentDay++;
        }
        else
        {
            cycleTime.Value += Time.deltaTime;
        }
    }

    private void UpdateLighting()
    {
        //This will update the lighting and the skybox
        if (cycleTime.Value < cycleDayDuration)
        {
            //Day period
            sunAngle = 180f / cycleDayDuration * cycleTime.Value;
            if(isNight)
            {
                isNight = false;
                OnNightEnd?.Invoke();
            }
        }
        else
        {
            //Night period
            sunAngle = 180f + (180f / cycleNightDuration * (cycleTime.Value - cycleDayDuration));

            if (!isNight)
            {
                isNight = true;
                OnNightStart?.Invoke();
            }
        }

        if (sunAngle >= 360f)
        {
            sunAngle -= 360f;
        }

        sceneLight.transform.localRotation = Quaternion.Euler(sunAngle, lightRotation.y, lightRotation.z);
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void SkipToNight()
        {
            cycleTime.Value = cycleDayDuration;
        }
    #endif
}
