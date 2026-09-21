using UnityEngine;

public class EmpoweredShot : Ability
{
    private BaseProjectile projectile;

    private Transform firingPosition;

    private float damage = 20f;

    private void Start()
    {
        firingPosition = owner.FiringPosition;
        projectile = abilitySO.Projectile;
    }

    protected override void Activate(Vector3 direction)
    {
        //TODO: Trigger animation (Also might want to do the thing Marcus said like with enemy attacks)

        //Spawn Projectile that fires in the direction the player is aiming at
        owner.SpawnProjectile(projectile.gameObject, direction, damage, abilitySO);
    }

    /// <summary>
    /// Handles spawning projectiles for player abilities
    /// </summary>
    /// <param name="projectilePrefab"></param>
    /// <param name="target"></param>
    /// <param name="baseDamage"></param>
    /// <param name="abilitySO"></param>
    public void SpawnProjectile(GameObject projectilePrefab, Vector3 target, float baseDamage, AbilitySO abilitySO)
    {
        //Gets the direction the projectile should be facing
        Vector3 dir = (target - firingPosition.position).normalized;

        //Creates a rotation for the projectile to face the target
        Quaternion projRotation = Quaternion.LookRotation(dir);

        // Spawns the projectile on the server
        BaseProjectile newProjectile = Instantiate(projectilePrefab, firingPosition.position, projRotation).GetComponent<BaseProjectile>();

        newProjectile.InitializeProjectile(target, owner.GetDamage(abilitySO, baseDamage));

        //Spawns the projectile on the network
        Spawn(newProjectile.gameObject);
    }
}
