using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
    [SerializeField]
    private string abilityName;

    [SerializeField]
    private Sprite icon;
    
    [SerializeField]
    private float cooldown, castTime;
    
    /// <summary>
    /// The name of the ability
    /// </summary>
    public string AbilityName => abilityName;

    /// <summary>
    /// The icon for the ability that is displayed in the UI
    /// </summary>
    public Sprite Icon => icon;

    /// <summary>
    /// The cooldown for this ability
    /// </summary>
    public float Cooldown => cooldown;

    /// <summary>
    /// The amount of time it takes this ability to finish activating
    /// </summary>
    public float CastTime => castTime;
}
