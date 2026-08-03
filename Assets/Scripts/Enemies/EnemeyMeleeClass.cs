using UnityEngine;

public class EnemeyMeleeClass : Enemy
{
    [SerializeField] private EnemySO enemyData;

    public float EnemyDamage => enemyData.EnemyDamage;


    ITargetable targetable;

}
