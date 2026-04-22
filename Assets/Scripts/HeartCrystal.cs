using UnityEngine;

public class HeartCrystal : MonoBehaviour
{

    //Need it to be able to take resources from the players when within a certain range (maybe seperate script)
    //Needs to be able to take damage

    //The current and starting health
    private float startingHealth = 100f, currentHealth;

    private void Start()
    {
        //Initializes the health
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;


    }
}
