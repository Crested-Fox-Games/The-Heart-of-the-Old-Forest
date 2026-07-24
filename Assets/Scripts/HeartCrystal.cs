using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class HeartCrystal : NetworkBehaviour
{
    //The current and starting health
    private float startingHealth = 100f;

    public readonly SyncVar<float> currentHealth = new();

    public override void OnStartServer()
    {
        //Initializes the health
        currentHealth.Value = startingHealth;
    }

    /// <summary>
    /// This will handle taking damage from enemies
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return;

        currentHealth.Value -= damage;

        if(currentHealth.Value <= 0)
        {
            //Run some sort of game over function
            GameManager.Instance.GameOver();
        }
    }

    //TODO: update some sort of in scene ui that displays a health bar
}
