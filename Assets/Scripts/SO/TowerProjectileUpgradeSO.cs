using UnityEngine;

[CreateAssetMenu(fileName = "TowerProjectileUpgradeSO", menuName = "Towers/Upgrades/Projectile Upgrade")]
public class TowerProjectileUpgradeSO : TowerUpgradeSO
{
    /// <summary>
    /// Number of additional projectiles granted.
    /// </summary>
    [Header("--- Upgrade Info ---")]
    public int projectileAmount = 1;

    public override void GrantUpgrade(TowerSO towerSO)
    {
        TowerManager.Instance.AddProjectileUpgrade(towerSO, projectileAmount);
    }
}