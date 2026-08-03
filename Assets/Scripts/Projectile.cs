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

    private ArcherTower towerParent;

    //TODO: figure out if we need this/how to implement it properly
    private PlayerAbilities player;

    public void InitializeProjectile(Vector3 target, float projectileDamage, ArcherTower tower)
    {
        targetPosition = target;
        projDamage = projectileDamage;
        towerParent = tower;

        direction = (targetPosition - transform.position).normalized;
        StartCoroutine(MoveToTarget());
    }

    public void InitializeProjectile(Vector3 target, float projectileDamage, PlayerAbilities player)
    {
        targetPosition = target;
        projDamage = projectileDamage;
        this.player = player;

        direction = (targetPosition - transform.position).normalized;
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
        if (towerParent != null)
        {
            towerParent.AddToPool(this);
        }
        else if (player != null)
        {
            player.AddToPool(this);
        }

        ServerManager.Despawn(gameObject);

        gameObject.SetActive(false);
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
