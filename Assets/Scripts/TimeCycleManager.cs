using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

public class TimeCycleManager : NetworkBehaviour
{
    public static TimeCycleManager Instance { get; private set; }

    [SerializeField]
    private GameObject sceneLight;

    /// <summary>
    /// Seconds of dusk at the end of the day and dawn at the end of the night
    /// </summary>
    [SerializeField]
    private float transitionSeconds = 15f;

    /// <summary>
    /// Found at runtime since it lives in the Environment prefab
    /// </summary>
    private DynamicSkyController skyController;

    private Vector3 lightRotation;

    public readonly SyncVar<float> cycleTime = new();
    private float cycleDuration = 240f; // 4 minutes in seconds
    private float cycleDayDuration = 120f; // 2 minutes in seconds
    private float cycleNightDuration = 120f; // 2 minutes in seconds
    private float sunAngle = 0; // Used in calculating sun position
    private int currentDay = 0;

    public float CycleDuration => cycleDuration;
    public float CycleDayDuration => cycleDayDuration;
    public float CycleNightDuration => cycleNightDuration;

    public int CurrentDay => currentDay;

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



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        //Done here instead of OnStartServer so clients have it too
        if (sceneLight != null)
        {
            lightRotation = sceneLight.transform.localEulerAngles;
            lightRotation.x = 0;
        }
    }

    public override void OnStartServer()
    {
        //Set the time to 0 when the server starts up
        cycleTime.Value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Only the server advances time
        if (IsServerStarted)
        {
            UpdateCycle();
        }

        //Everyone updates visuals from the synced cycleTime
        UpdateLighting();
    }

    private void UpdateCycle()
    {
        if (cycleTime.Value >= cycleDuration)
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
        bool nightNow;

        if (cycleTime.Value < cycleDayDuration)
        {
            //Day period
            sunAngle = 180f / cycleDayDuration * cycleTime.Value;
            nightNow = false;
        }
        else
        {
            //Night period
            sunAngle = 180f + (180f / cycleNightDuration * (cycleTime.Value - cycleDayDuration));
            nightNow = true;
        }

        if (sunAngle >= 360f)
        {
            sunAngle -= 360f;
        }

        //Night events only fire on the server
        if (IsServerStarted && nightNow != isNight)
        {
            isNight = nightNow;
            if (isNight)
                OnNightStart?.Invoke();
            else
                OnNightEnd?.Invoke();
        }

        if (skyController == null)
        {
            skyController = FindFirstObjectByType<DynamicSkyController>();
        }

        if (skyController != null)
        {
            //Shift the sky forward by half the transition so dusk/dawn finish when night starts/ends
            float twilightDegrees = Mathf.Clamp(transitionSeconds * 0.5f * (180f / cycleDayDuration), 1f, 30f);
            skyController.twilightDegrees = twilightDegrees;

            //Convert sunAngle (0 = sunrise) to the sky's time of day (0.25 = sunrise)
            skyController.SetTime((sunAngle + twilightDegrees + 90f) / 360f);
        }
        else if (sceneLight != null)
        {
            //No sky controller found, just rotate the light like before
            sceneLight.transform.localRotation = Quaternion.Euler(sunAngle, lightRotation.y, lightRotation.z);
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void SkipToNight()
    {
        cycleTime.Value = cycleDayDuration;
    }

    public void QuickEndDay()
    {
        cycleTime.Value = cycleDuration - 1;
    }
#endif
}