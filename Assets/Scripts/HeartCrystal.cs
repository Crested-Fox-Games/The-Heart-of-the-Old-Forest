using UnityEngine;

public class HeartCrystal : MonoBehaviour
{
    //Needs to be able to take damage

    //The current and starting health
    private float startingHealth = 100f, currentHealth;

    private void Start()
    {
        //Initializes the health
        currentHealth = startingHealth;
    }

    /// <summary>
    /// This will handle taking damage from enemies
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    //TODO: update some sort of in scene ui that displays a health bar
}
