using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerSniperUpgradeSO", menuName = "Towers/Upgrades/Sniper Upgrade")]
public class TowerSniperUpgradeSO : TowerUpgradeSO
{
    [Header("--- Sniper Upgrade ---")]
    public float damageIncrease = 10f;
    public float rangeIncrease = 5f;

    public override void GrantUpgrade(Tower tower)
    {
        tower.AddLocalStatUpgrade(TowerStats.Attack, UpgradeType.Addition, damageIncrease);

        tower.AddLocalStatUpgrade(TowerStats.Range, UpgradeType.Addition, rangeIncrease);

        //Update range
        tower.OnUpgradesChanged();
    }
}
