using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Vector3 targetPosition;

    [SerializeField]
    private float projSpeed;

    private float projDamage;

    private Enemy hitEnemy;

    private Vector3 direction;

    private float projectileMaxTime = 5f;

    /// <summary>
    /// Initializes the projectiles initial values
    /// </summary>
    /// <param name="target"></param>
    /// <param name="projectileDamage"></param>
    /// <param name="tower"></param>
    public void InitializeProjectile(Vector3 target, float projectileDamage)
    {
        targetPosition = target;
        projDamage = projectileDamage;

        direction = (targetPosition - transform.position).normalized;
        StartCoroutine(MoveToTarget());
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy hitEnemy = other.GetComponentInParent<Enemy>();

        if(hitEnemy != null)
        {
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
        ServerManager.Despawn(gameObject);
    }

    private IEnumerator MoveToTarget()
    {
        float timer = 0;

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
