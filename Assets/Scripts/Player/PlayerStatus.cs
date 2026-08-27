using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour, ITargetable
{
    [SerializeField] private float maxHealth = 100f;

    private readonly SyncVar<float> currentHealth = new();

    public Transform TargetTransform => transform;

    public override void OnStartServer()
    {
        //Initialises the health of the structure
        currentHealth.Value = maxHealth;
    }

    public bool IsAlive()
    {
        return currentHealth.Value > 0;
    }

    public bool IsAttackable()
    {
        //Can change later to add protection mechanics
        return IsAlive();
    }

    public bool TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return true;

        currentHealth.Value -= damage;
        //Debug.Log("Structure has taken damage");
        if (currentHealth.Value <= 0)
        {
            Destroyed();
            return false;
        }
        return true;
    }

    [ObserversRpc]
    public void Destroyed()
    {
        Debug.Log($"{gameObject.name} has been destroyed");
        gameObject.SetActive(false);
    }
}
