using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerPathProgress
{
    public int upgradeCount;
    public int pendingUpgradeID;
}

[CreateAssetMenu(fileName = "TowerUpgradePath", menuName = "Towers/Tower Upgrade Path")]
public class TowerUpgradePathSO : ScriptableObject
{
    [SerializeField]
    private string pathName;

    [SerializeField]
    private TowerUpgradeSO milestoneUpgrade;

    [SerializeField]
    private List<TowerUpgradeSO> randomUpgradePool;

    [SerializeField]
    private int randomUpgradesBetweenMilestones = 2;

    public string PathName => pathName;
    public TowerUpgradeSO MilestoneUpgrade => milestoneUpgrade;
    public List<TowerUpgradeSO> RandomUpgradePool => randomUpgradePool;
    public int RandomUpgradesBetweenMilestones => randomUpgradesBetweenMilestones;

    public TowerUpgradeSO GetNextUpgrade(int upgradeCount)
    {
        int positionInCycle = upgradeCount % (randomUpgradesBetweenMilestones + 1);

        if (positionInCycle == 0)
        {
            return milestoneUpgrade;
        }

        if (randomUpgradePool.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, randomUpgradePool.Count);

        return randomUpgradePool[randomIndex];
    }

    public TowerUpgradeSO GetRandomUpgrade()
    {
        if (randomUpgradePool == null || randomUpgradePool.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, randomUpgradePool.Count);

        return randomUpgradePool[randomIndex];
    }
}
