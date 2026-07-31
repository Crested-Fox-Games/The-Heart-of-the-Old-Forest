using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject targetEnemy;

    [SerializeField]
    private float projSpeed;

    private float projDamage;

    private Enemy hitEnemy;

    private Vector3 direction;

    private float projectileMaxTime = 5f;

    private ArcherTower towerParent;

    public void InitializeProjectile(GameObject targetedEnemy, float projectileDamage, ArcherTower tower)
    {
        targetEnemy = targetedEnemy;
        projDamage = projectileDamage;
        towerParent = tower;

        direction = (targetEnemy.transform.position - transform.position).normalized;
        StartCoroutine(MoveToTarget());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Enemy>(out hitEnemy))
        {
            Debug.Log("Hit enemy");
            //Deal damage
            hitEnemy.TakeDamage(projDamage);


            HandleProjectileFinished();
        }
    }

    /// <summary>
    /// Handles what happens when the projectile hits its target or times out
    /// </summary>
    private void HandleProjectileFinished()
    {
        gameObject.SetActive(false);


        towerParent.AddToPool(this);
    }

    //TODO: Determine if it moves to the enemy or if it moves to where the enemy was/will be
    private IEnumerator MoveToTarget()
    {
        float timer = 0;
        //TODO: Determine if theres a check, or if the proj just travels until it hits target.
        while (timer < projectileMaxTime) 
        {
            timer += Time.deltaTime;

            //Move the projectile towards the enemy
            transform.position += direction * projSpeed * Time.deltaTime;

            yield return null;
        }

       HandleProjectileFinished();
    }
}
