using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class StructureHealth : NetworkBehaviour
{
    //Max and current health of the structure
    [SerializeField] private float maxHealth = 100f;

    private readonly SyncVar<float> currentHealth = new();

    public override void OnStartServer()
    {
        //Initialises the health of the structure
        currentHealth.Value = maxHealth;
    }


    /// <summary>
    /// Handles taking damage from sources such as enemies
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return;

        currentHealth.Value -= damage;
        //Debug.Log("Structure has taken damage");
        if (currentHealth.Value <= 0)
        {
            //TODO destroy object, different logic for heart crystal
            Debug.Log($"{gameObject.name} has been destroyed");
        }
    }
}
