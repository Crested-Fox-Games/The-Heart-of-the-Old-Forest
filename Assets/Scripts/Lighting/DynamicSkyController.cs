using UnityEngine;

/// <summary>
/// Controls the skybox, sun, moon and fog based on the time of day.
/// Time is set by TimeCycleManager, this script just handles the visuals.
/// </summary>
[ExecuteAlways]
public class DynamicSkyController : MonoBehaviour
{
    [Header("References")]
    public Light sun;
    public Light moon;
    public Material skyMaterial;

    /// <summary>
    /// 0 = midnight, 0.25 = sunrise, 0.5 = noon, 0.75 = sunset. Drag in edit mode to preview
    /// </summary>
    [Header("Time")]
    [Range(0f, 1f)] public float timeOfDay = 0.35f;
    /// <summary>
    /// Length of dusk/dawn in degrees, set by TimeCycleManager
    /// </summary>
    [Range(1f, 30f)] public float twilightDegrees = 11f;

    [Header("Sky Orientation")]
    public float sunYaw = 30f; // Direction the sun rises from
    [Range(0f, 60f)] public float sunPathTilt = 20f; // How much the sun's arc leans

    [Header("Lighting")]
    public float maxSunIntensity = 1f;
    public float maxMoonIntensity = 0.2f;
    public Gradient sunColour = DefaultSunGradient(); // Left = horizon, right = overhead
    public float ambientRefreshInterval = 0.5f; // Seconds between ambient lighting updates

    [Header("Fog (Linear)")]
    public Color dayFogColour = new Color(0.52f, 0.55f, 0.58f);
    public Color twilightFogColour = new Color(0.35f, 0.27f, 0.25f);
    public Color nightFogColour = new Color(0.02f, 0.025f, 0.035f);
    public float dayFogStart = 5f;
    public float dayFogEnd = 120f;
    public float nightFogStart = 10f;
    public float nightFogEnd = 55f;

    /// <summary>
    /// 1 = full day, 0 = full night, blends during dusk/dawn
    /// </summary>
    public float DayFactor { get; private set; } = 1f;

    static readonly int SunDirId = Shader.PropertyToID("_SunDir");
    static readonly int MoonDirId = Shader.PropertyToID("_MoonDir");
    static readonly int StarRotId = Shader.PropertyToID("_StarRotation");
    static readonly int TwilightId = Shader.PropertyToID("_TwilightWidth");
    static readonly int SkyFogId = Shader.PropertyToID("_SkyFogColor");

    private float ambientTimer;

    public void SetTime(float normalisedTime) => timeOfDay = Mathf.Repeat(normalisedTime, 1f);

    private void OnEnable()
    {
        if (skyMaterial != null) RenderSettings.skybox = skyMaterial;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    }

    private void Update()
    {
        //Rotate the sun and moon around a tilted path, moon is always opposite the sun
        Quaternion orbitBasis = Quaternion.Euler(0f, sunYaw, 0f) * Quaternion.AngleAxis(sunPathTilt, Vector3.forward);
        float angle = timeOfDay * 360f - 90f;
        Quaternion sunRot = orbitBasis * Quaternion.Euler(angle, 0f, 0f);
        Quaternion moonRot = orbitBasis * Quaternion.Euler(angle + 180f, 0f, 0f);

        if (sun != null) sun.transform.rotation = sunRot;
        if (moon != null) moon.transform.rotation = moonRot;

        //Directions pointing towards the sun and moon
        Vector3 sunDir = sunRot * Vector3.back;
        Vector3 moonDir = moonRot * Vector3.back;

        //How far above/below the horizon dusk/dawn blends over
        float twilightWidth = Mathf.Max(Mathf.Sin(twilightDegrees * Mathf.Deg2Rad) * Mathf.Cos(sunPathTilt * Mathf.Deg2Rad), 0.01f);
        float t = Mathf.InverseLerp(-twilightWidth, twilightWidth, sunDir.y);
        DayFactor = t * t * (3f - 2f * t);
        float twilightFactor = Mathf.Clamp01(1f - Mathf.Abs(sunDir.y) / (twilightWidth * 1.7f));

        //Send values to the sky shader
        Shader.SetGlobalVector(SunDirId, sunDir);
        Shader.SetGlobalVector(MoonDirId, moonDir);
        Shader.SetGlobalMatrix(StarRotId, Matrix4x4.Rotate(Quaternion.Inverse(sunRot)));
        Shader.SetGlobalFloat(TwilightId, twilightWidth);

        UpdateLights(sunDir.y, moonDir.y, twilightWidth);
        UpdateFog(twilightFactor);
        UpdateAmbient();
    }

    private void UpdateLights(float sunHeight, float moonHeight, float twilightWidth)
    {
        //Fade lights out as they reach the horizon, and turn off when below it
        if (sun != null)
        {
            sun.intensity = maxSunIntensity * Mathf.Clamp01(sunHeight / twilightWidth);
            sun.color = sunColour.Evaluate(Mathf.Clamp01(sunHeight));
            sun.enabled = sun.intensity > 0.001f;
        }

        if (moon != null)
        {
            moon.intensity = maxMoonIntensity * Mathf.Clamp01(moonHeight / twilightWidth);
            moon.enabled = moon.intensity > 0.001f;
        }

        //Main light swaps to the moon at night
        RenderSettings.sun = (sunHeight > 0f || moon == null) ? sun : moon;
    }

    private void UpdateFog(float twilightFactor)
    {
        Color fog = Color.Lerp(nightFogColour, dayFogColour, DayFactor);
        fog = Color.Lerp(fog, twilightFogColour, twilightFactor * 0.6f);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fog;
        RenderSettings.fogStartDistance = Mathf.Lerp(nightFogStart, dayFogStart, DayFactor);
        RenderSettings.fogEndDistance = Mathf.Lerp(nightFogEnd, dayFogEnd, DayFactor);

        //Sky horizon blends into the fog colour
        Shader.SetGlobalColor(SkyFogId, fog);
    }

    private void UpdateAmbient()
    {
        //Ambient lighting is expensive to update, so only do it every so often (always in edit mode)
        ambientTimer -= Application.isPlaying ? Time.deltaTime : ambientRefreshInterval;
        if (ambientTimer > 0f) return;
        ambientTimer = ambientRefreshInterval;
        DynamicGI.UpdateEnvironment();
    }

    private static Gradient DefaultSunGradient()
    {
        //Orange at the horizon, white overhead
        var g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.4f, 0.15f), 0f),
                new GradientColorKey(new Color(1f, 0.8f, 0.6f), 0.15f),
                new GradientColorKey(new Color(1f, 0.97f, 0.92f), 0.5f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        return g;
    }
}