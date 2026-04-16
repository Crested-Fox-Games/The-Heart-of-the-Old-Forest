using UnityEngine;

//TODO: make this class an abstract parent when implementing enemy types
public class Enemy : MonoBehaviour
{
    #region SO Fields
    [SerializeField]
    private EnemySO enemySO;

    #endregion

    float health = 5;

    //TODO: Probably add an enum for proj types to easily trigger effects
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} damage dealt");
    }
}
