using UnityEngine;

/// <summary>
/// Makes natural light sources (wisps) glow at night.
/// Follows the time of day from DynamicSkyController, runs locally on every client.
/// </summary>
public class NightLight : MonoBehaviour
{
    [Header("Light")]
    public Light targetLight; //uses a Light on this object or its children if empty
    public float nightIntensity = 2f;

    [Header("Glow")]
    public Renderer[] glowRenderers; // Materials need Emission ticked
    [ColorUsage(false, true)]
    public Color glowColour = new Color(0.4f, 2.5f, 2.2f);

    [Header("Particles")]
    public ParticleSystem[] nightParticles; // wisps etc. Only emit at night
    public float particleRate = 10f;

    [Header("Pulse")]
    [Range(0f, 0.5f)] public float pulseAmount = 0.1f; // 0 = steady glow
    public float pulseSpeed = 0.5f; // Pulses per second

    [Header("Daytime")]
    [Range(0f, 1f)] public float dayGlow = 0f; // How much glow is kept during the day
    public bool hideInDay = false; // Hides the glow renderers completely when they aren't glowing

    [Header("Float (for wisps)")]
    public bool floats = false;
    public float bobHeight = 0.3f;
    public float bobSpeed = 0.4f;
    public float wanderRadius = 0.5f;
    public float wanderSpeed = 0.3f;
    [Range(0f, 1f)] public float variation = 0.3f; // Randomises movement so wisps don't move in sync

    private static DynamicSkyController sky;
    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    private MaterialPropertyBlock block;
    private Vector3 origin;
    private float seed;
    private float bobMul, wanderMul, speedMul;

    private void Awake()
    {
        if (targetLight == null) targetLight = GetComponentInChildren<Light>();
        block = new MaterialPropertyBlock();

        //Random values so each object pulses and moves a little differently
        origin = transform.localPosition;
        seed = Random.value * 100f;
        bobMul = 1f + Random.Range(-variation, variation);
        wanderMul = 1f + Random.Range(-variation, variation);
        speedMul = 1f + Random.Range(-variation, variation);
    }

    private void Update()
    {
        UpdateGlow();

        if (floats)
        {
            UpdateFloat();
        }
    }

    private void UpdateGlow()
    {
        if (sky == null) sky = FindFirstObjectByType<DynamicSkyController>();

        //0 during the day, 1 at night
        float night = sky != null ? 1f - sky.DayFactor : 1f;

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f + seed) * pulseAmount;
        float strength = Mathf.Lerp(dayGlow, 1f, night) * pulse;

        if (targetLight != null)
        {
            targetLight.intensity = nightIntensity * strength;
            targetLight.enabled = targetLight.intensity > 0.01f;
        }

        if (glowRenderers != null)
        {
            foreach (var r in glowRenderers)
            {
                if (r == null) continue;
                if (hideInDay) r.enabled = strength > 0.01f;
                r.GetPropertyBlock(block);
                block.SetColor(EmissionId, glowColour * strength);
                r.SetPropertyBlock(block);
            }
        }

        if (nightParticles != null)
        {
            foreach (var ps in nightParticles)
            {
                if (ps == null) continue;
                var emission = ps.emission;
                emission.rateOverTime = particleRate * night;
            }
        }
    }

    private void UpdateFloat()
    {
        //Bob up and down, and wander slowly around the starting point
        float t = Time.time * speedMul;
        float bob = Mathf.Sin(t * bobSpeed * Mathf.PI * 2f + seed) * bobHeight * bobMul;
        float wx = (Mathf.PerlinNoise(seed, t * wanderSpeed) - 0.5f) * 2f;
        float wz = (Mathf.PerlinNoise(seed + 50f, t * wanderSpeed) - 0.5f) * 2f;

        transform.localPosition = origin + new Vector3(wx, 0f, wz) * wanderRadius * wanderMul + Vector3.up * bob;
    }
}