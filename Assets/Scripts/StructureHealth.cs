using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class StructureHealth : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            //destroy object
        }
    }
}
