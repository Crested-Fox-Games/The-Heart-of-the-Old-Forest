using UnityEngine;

[CreateAssetMenu(fileName = "TowerStatUpgradeSO", menuName = "Towers/Upgrades/Tower Stat Upgrade")]
public class TowerStatUpgradeSO : TowerUpgradeSO
{
    /// <summary>
    /// The tower stat affected by the upgrade.
    /// </summary>
    [Header("--- Upgrade Info ---")]
    public TowerStats towerStat;

    /// <summary>
    /// Whether the upgrade is additive or multiplicative.
    /// </summary>
    public UpgradeType upgradeType;

    /// <summary>
    /// The amount the stat is upgraded by.
    /// </summary>
    public float upgradeAmount;

    public override void GrantUpgrade(TowerSO towerSO)
    {
        TowerManager.Instance.AddGlobalUpgrade(
            towerSO,
            towerStat,
            upgradeType,
            upgradeAmount
        );
    }
}
