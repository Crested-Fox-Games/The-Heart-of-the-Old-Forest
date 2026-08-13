using FishNet.Managing.Server;
using Unity.VisualScripting;
using UnityEngine;

public class RabbitBasicAttack : Ability
{
    private Projectile projectile;

    private float damage = 20f;

    public RabbitBasicAttack(PlayerAbilities player, AbilitySO abilityData, Projectile projectile) : base(player, abilityData)
    {
        this.projectile = projectile;
    }


    protected override void Activate(Vector3 direction)
    {
        Debug.Log("Basic Attack Activated");
        // Implementation for the basic attack activation

        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)

        //Spawn Projectile that fires in the direction the player is aiming at
        
        owner.GetComponent<PlayerAbilities>().SpawnProjectile(projectile.gameObject, direction, damage);

    }

    
}
