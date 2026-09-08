using FishNet.Managing.Server;
using Unity.VisualScripting;
using UnityEngine;

public class RabbitBasicAttack : Ability
{
    private BaseProjectile projectile;

    private float damage = 20f;

    public RabbitBasicAttack(PlayerAbilities player, AbilitySO abilityData) : base(player, abilityData)
    {
        projectile = abilityData.Projectile;
    }


    protected override void Activate(Vector3 direction)
    {
        Debug.Log("Basic Attack Activated");
        // Implementation for the basic attack activation

        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)

        //Spawn Projectile that fires in the direction the player is aiming at
        owner.SpawnProjectile(projectile.gameObject, direction, damage, abilitySO);

    }

    
}
