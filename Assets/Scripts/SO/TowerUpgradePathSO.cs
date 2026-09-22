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

    public string PathName => pathName;
    public int MaxUniqueUpgrades => maxUniqueUpgrades;
    public TowerUpgradeSO MilestoneUpgrade => milestoneUpgrade;
    public List<TowerUpgradeSO> RandomUpgradePool => randomUpgradePool;
    public int RandomUpgradesBetweenMilestones => randomUpgradesBetweenMilestones;
    public TowerUpgradeSO NextRandomUpgrade => nextRandomUpgrade;

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
