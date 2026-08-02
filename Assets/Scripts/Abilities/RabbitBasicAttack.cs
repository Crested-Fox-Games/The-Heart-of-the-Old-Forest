using UnityEngine;

public class RabbitBasicAttack : Ability
{
    public RabbitBasicAttack(PlayerAbilities player) : base(player)
    {
    }

    protected override void Activate()
    {
        // Implementation for the basic attack activation

        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)

        //Spawn Projectile that fires in the direction the player is aiming at
        //TODO: decide how this will actually work due to third person camera
    }
}
