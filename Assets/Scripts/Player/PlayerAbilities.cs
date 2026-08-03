using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum AbilitySlot
{
    BasicAttack,
    FirstAbility,
    SecondAbility
}

public class PlayerAbilities : NetworkBehaviour
{
    [SerializeField]
    private AbilitySO basicAttackSO, firstAbilitySO, secondAbilitySO;
    private Ability basicAttack, firstAbility, secondAbility;

    /// <summary>
    /// This syncvar is used to let the client know how long is left on the cooldown, used for UI purposes
    /// </summary>
    private readonly SyncVar<float> basicAttackCooldownRemaining = new SyncVar<float>();

    /// <summary>
    /// This is the projectile for the player basic attack.
    /// This will probably need to be changed later for when we have multiple different characters
    /// </summary>
    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    private Transform firingPosition;

    private void Awake()
    {
        basicAttack = new RabbitBasicAttack(this, basicAttackSO, projectile);

    }

    private void Update()
    {
        if (!IsServerStarted)
            return;

        //Ticks down the basic attacks cooldown timer
        basicAttack.Tick(Time.deltaTime);

        //Updates the sync var to let the client know how much time is left on the cooldown
        basicAttackCooldownRemaining.Value = basicAttack.CooldownRemaining;
    }

    private void TryUseAbility(AbilitySlot abilitySlot)
    {
       
        if(GetAbilityFromSlot(abilitySlot).AbilitySO.NeedDirection)
        {
            UseAbility(abilitySlot, GetAimPoint());
        }
        else
        {
            UseAbility(abilitySlot);
        }
    }


    /// <summary>
    /// The server validates to see if the ability can be used and then uses it if it can
    /// </summary>
    /// <param name="abilitySlot"></param>
    [ServerRpc]
    private void UseAbility(AbilitySlot abilitySlot)
    {
        Ability ability = GetAbilityFromSlot(abilitySlot);

        if (ability == null)
            return;

        if(!ability.CanUseAbility())
            return;



        ability.UseAbility();
    }

    [ServerRpc]
    private void UseAbility(AbilitySlot abilitySlot, Vector3 direction)
    {
        Ability ability = GetAbilityFromSlot(abilitySlot);

        if (ability == null)
            return;

        if(!ability.CanUseAbility())
            return;

        ability.UseAbility(direction);
    }

    public void TryUseBasicAttack(InputAction.CallbackContext context)
    {
        TryUseAbility(AbilitySlot.BasicAttack);
    }

    //public void TryUseFirstAbility()
    //{
    //    TryUseAbility(AbilitySlot.FirstAbility);
    //}

    //public void TryUseSecondAbility()
    //{
    //    TryUseAbility(AbilitySlot.SecondAbility);
    //}

    /// <summary>
    /// Gets the ability from the specified slot.
    /// </summary>
    /// <param name="abilitySlot"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    private Ability GetAbilityFromSlot(AbilitySlot abilitySlot)
    {
        //A switch statement which returns the ability in the specified slot
        return abilitySlot switch
        {
            AbilitySlot.BasicAttack => basicAttack,
            AbilitySlot.FirstAbility => firstAbility,
            AbilitySlot.SecondAbility => secondAbility,
            _ => throw new System.ArgumentOutOfRangeException(nameof(abilitySlot), abilitySlot, null)
        };
    }

    private Vector3 GetAimPoint()
    {
        //Gets the players camera
        Camera playerCamera = GetComponentInChildren<Camera>();

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * 100f; // Default to a point 100 units away if nothing is hit
    }

    public void SpawnProjectile(GameObject projectilePrefab, Vector3 target, float damage)
    {
        // Spawns the projectile on the server
        Projectile newProjectile = Instantiate(projectilePrefab, firingPosition.position, transform.rotation).GetComponent<Projectile>();

        newProjectile.InitializeProjectile(target, damage, this);

        Spawn(newProjectile.gameObject);
    }

}
