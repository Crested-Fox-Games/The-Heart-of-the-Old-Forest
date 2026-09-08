using System.Collections;
using UnityEngine;

public class RelentlessVolley : Ability
{
    private BaseProjectile projectile;

    private float damage = 20f;

    GameObject normalProj;

    public RelentlessVolley(PlayerAbilities player, AbilitySO abilityData) : base(player, abilityData)
    {
        projectile = abilityData.Projectile;
    }


    protected override void Activate(Vector3 direction)
    {
        // Implementation for the basic attack activation

        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)
        Debug.Log($"Relentless Volley Activated");
        normalProj = owner.Projectile.gameObject;

        owner.SetProjectile(projectile.gameObject);
    }

    protected override void Deactivate()
    {
        owner.SetProjectile(normalProj);
    }

    
}
