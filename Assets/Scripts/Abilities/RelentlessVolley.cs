using System.Collections;
using UnityEngine;

public class RelentlessVolley : Ability
{
    private BaseProjectile projectile;

    GameObject normalProj;

    private Ability basicAttack;

    private float timer = 5f;

    private void Start()
    {
        projectile = abilitySO.Projectile;
    }

    private void InitializeBasicAttack()
    {
        basicAttack = owner.BasicAttack;
    }

    protected override void Activate()
    {
        if(basicAttack == null)
        {
            InitializeBasicAttack();
        }

        // Implementation for the basic attack activation

        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)
        normalProj = owner.Projectile.gameObject;

        basicAttack.SetProjectile(projectile.gameObject);

        //Apply buffs
        //NOTE: Might need this to be its own upgrade buff
        owner.AddAbilityUpgrade(basicAttack.AbilitySO, AbilityStats.Damage, UpgradeType.Multiplacation, 0.5f);
        owner.AddAbilityUpgrade(basicAttack.AbilitySO, AbilityStats.Cooldown, UpgradeType.Multiplacation, 1f);

        StartCoroutine(ActiveTimerForAbility());
    }

    protected override void Deactivate()
    {
        basicAttack.SetProjectile(normalProj);

        //Remove buffs
        owner.AddAbilityUpgrade(basicAttack.AbilitySO, AbilityStats.Damage, UpgradeType.Multiplacation, -0.5f);
        owner.AddAbilityUpgrade(basicAttack.AbilitySO, AbilityStats.Cooldown, UpgradeType.Multiplacation, -1f);
    }

    private IEnumerator ActiveTimerForAbility()
    {
        yield return new WaitForSeconds(timer);

        Deactivate();
    }
}
