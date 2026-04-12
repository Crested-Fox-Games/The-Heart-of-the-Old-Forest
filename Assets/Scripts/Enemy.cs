using UnityEngine;

public class Enemy : MonoBehaviour
{
    //This class will later be made a abstract parent

    float health = 5;

    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} damage dealt");
    }
}
