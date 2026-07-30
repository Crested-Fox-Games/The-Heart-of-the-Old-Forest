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

    public void InitializeProjectile(GameObject targetedEnemy, float projectileDamage)
    {
        targetEnemy = targetedEnemy;
        projDamage = projectileDamage;

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

            //TODO: This will probably need to be changed for optimization
            //Destroy projectile.
            Destroy(gameObject);
        }
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

        //TODO: This will probably need to be changed for optimization
        //Destroy projectile.
        Destroy(gameObject);
    }
}
