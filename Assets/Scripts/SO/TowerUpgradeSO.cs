using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Towers/UpgradeSO")]
public abstract class TowerUpgradeSO : ScriptableObject
{
    /// <summary>
    /// The name displayed for the upgrade.
    /// </summary>
    [Header("--- UI Info ---")]
    public string UpgradeName;

    /// <summary>
    /// The ID of the upgrade.
    /// </summary>
    public int UpgradeId;

    /// <summary>
    /// The icon displayed for the upgrade.
    /// </summary>
    public Sprite UpgradeIcon;

    /// <summary>
    /// Grants the upgrade to the specified tower.
    /// </summary>
    public abstract void GrantUpgrade(TowerSO towerSO);
}