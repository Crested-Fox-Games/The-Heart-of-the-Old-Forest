using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject targetEnemy;

    [SerializeField]
    private float projSpeed;

    private float projDamage;

    private Enemy hitEnemy;

    public void InitializeProjectile(GameObject targetedEnemy, float projectileDamage)
    {
        targetEnemy = targetedEnemy;
        projDamage = projectileDamage;

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
        while(Vector3.Distance(transform.position, targetEnemy.transform.position) > 0.1f) //TODO: Determine if theres a check, or if the proj just travels until it hits target.
        {
            if (targetEnemy != null)
            {
                //Move the projectile towards the enemy
                transform.position = Vector3.MoveTowards(transform.position,
                    targetEnemy.transform.position,
                    projSpeed * Time.deltaTime);

                yield return null;
            }
        }
    }
}
