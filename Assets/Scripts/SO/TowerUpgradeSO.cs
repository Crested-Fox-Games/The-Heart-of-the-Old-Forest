using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeEffectType
{
    ProjectileCount,
    Damage,
    Range,
    Health,
    FireRate
}

[Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType effectType;
    public float amount;
}

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Towers/UpgradeSO")]
public class TowerUpgradeSO : ScriptableObject
{
    [SerializeField]
    private string upgradeName;

    [SerializeField]
    private string upgradeID;

    [SerializeField, TextArea]
    private string upgradeDescription;

    [SerializeField]
    private List<UpgradeEffect> effects;

    /// <summary>
    /// Name of the upgrade
    /// </summary>
    public string UpgradeName => upgradeName;

    /// <summary>
    /// The ID of the upgrade SO
    /// </summary>
    public string UpgradeID => upgradeID;

    /// <summary>
    /// Description of the upgrade
    /// </summary>
    public string UpgradeDescription => upgradeDescription;

    /// <summary>
    /// List of upgrade effects and their amounts
    /// </summary>
    public List<UpgradeEffect> Effects => effects;
}