using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
    [SerializeField]
    private string abilityName;

    [SerializeField]
    private Sprite abilityIcon;

    [SerializeField]
    private float cooldown, castTime;

    [SerializeField]
    private BaseProjectile projectile;

    [SerializeField]
    private bool needDirection;

    [SerializeField]
    private string abilityTypeName;

    [SerializeField]
    private AudioClip activationAudio;

    [SerializeField]
    private AudioClip effectAudio;

    /// <summary>
    /// The name of the ability
    /// </summary>
    public string AbilityName => abilityName;

    /// <summary>
    /// The icon for the ability that is displayed in the UI
    /// </summary>
    public Sprite AbilityIcon => abilityIcon;

    /// <summary>
    /// The cooldown for this ability
    /// </summary>
    public float Cooldown => cooldown;

    /// <summary>
    /// The amount of time it takes this ability to finish activating
    /// </summary>
    public float CastTime => castTime;

    /// <summary>
    /// The projectile for if the ability has a projectile
    /// </summary>
    public BaseProjectile Projectile => projectile;

    /// <summary>
    /// Returns whether or not this ability needs a direction to be used
    /// </summary>
    public bool NeedDirection => needDirection;

    /// <summary>
    /// The c# type name
    /// </summary>
    public string AbilityTypeName => abilityTypeName;

    /// <summary>
    /// The audio that is played when the ability is activated
    /// </summary>
    public AudioClip ActivationAudio => activationAudio;

    /// <summary>
    /// The audio that is played for a specific effect of the ability
    /// </summary>
    public AudioClip EffectAudio => effectAudio;
}
