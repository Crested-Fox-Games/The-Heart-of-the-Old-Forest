using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerPathProgress
{
    public int upgradeCount;
    public int pendingUpgradeID;
    public int milestoneUpgradeCount;
}

[CreateAssetMenu(fileName = "TowerUpgradePath", menuName = "Towers/Tower Upgrade Path")]
public class TowerUpgradePathSO : ScriptableObject
{
    [SerializeField]
    private string pathName;

    [SerializeField]
    private int maxUniqueUpgrades = 3;

    [SerializeField]
    private TowerUpgradeSO milestoneUpgrade;

    [SerializeField]
    private List<TowerUpgradeSO> randomUpgradePool;

    [SerializeField]
    private int randomUpgradesBetweenMilestones = 2;

    private TowerUpgradeSO nextRandomUpgrade;

    /// <summary>
    /// The name displayed for the upgrade path
    /// </summary>
    public string PathName => pathName;

    /// <summary>
    /// Maximum amount this unique upgrade can be purchased
    /// </summary>
    public int MaxUniqueUpgrades => maxUniqueUpgrades;

    /// <summary>
    /// The unique milestone upgrade for this path
    /// </summary>
    public TowerUpgradeSO MilestoneUpgrade => milestoneUpgrade;

    /// <summary>
    /// Pool of random upgrades
    /// </summary>
    public List<TowerUpgradeSO> RandomUpgradePool => randomUpgradePool;

    /// <summary>
    /// Number of upgrades between milestone upgrades   
    /// </summary>
    public int RandomUpgradesBetweenMilestones => randomUpgradesBetweenMilestones;

    /// <summary>
    /// The next random upgrade in the upgrade pool
    /// </summary>
    public TowerUpgradeSO NextRandomUpgrade => nextRandomUpgrade;

    /// <summary>
    /// Returns the next random upgrade in the pool
    /// </summary>
    /// <returns></returns>
    public TowerUpgradeSO GetRandomUpgrade()
    {
        if (randomUpgradePool == null || randomUpgradePool.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, randomUpgradePool.Count);

        nextRandomUpgrade = randomUpgradePool[randomIndex];

        return nextRandomUpgrade;
    }
}
