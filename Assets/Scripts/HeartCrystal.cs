using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

public class HeartCrystal : NetworkBehaviour, ITargetable
{
    //The current and starting health
    private float maxHealth = 100f;

    public readonly SyncVar<float> currentHealth = new();

    public Transform TargetTransform => transform;

    private HealthBar healthBar;

    /// <summary>
    /// Audio clip for when the player takes damage
    /// </summary>
    [SerializeField]
    private AudioClip takeDamageAudio;

    public override void OnStartServer()
    {
        //Initializes the health
        currentHealth.Value = maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        currentHealth.OnChange += UpdateHealthBar;
    }

    public bool IsAlive()
    {
        return currentHealth.Value > 0f;
    }

    public bool IsAttackable()
    {
        return IsAlive();
    }

    /// <summary>
    /// This will handle taking damage from enemies
    /// </summary>
    /// <param name="damage"></param>
    public bool TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return true;

        currentHealth.Value -= damage;

        if (takeDamageAudio != null)
        {
            AudioManager.Instance.PlayAudioClip(takeDamageAudio, transform.position);
        }
        else
        {
            Debug.LogWarning($"The audio clip doesnt exist for {takeDamageAudio.name}");
        }

        Debug.Log($"{gameObject.name} has been damaged");
        if (currentHealth.Value <= 0)
        {
            //Run some sort of game over function
            GameManager.Instance.GameOver();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Updates the health bar of the heart crystal
    /// </summary>
    private void UpdateHealthBar(float prev, float next, bool asServer)
    {
        healthBar.TriggerHealthBarUpdate(currentHealth.Value, maxHealth);
    }
}
