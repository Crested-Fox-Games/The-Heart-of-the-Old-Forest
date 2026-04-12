using UnityEngine;

public class Enemy : MonoBehaviour
{
    //This class will later be made an abstract parent

    float health = 5;

    //TODO: Probably add an enum for proj types to easily trigger effects
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} damage dealt");
    }
}
