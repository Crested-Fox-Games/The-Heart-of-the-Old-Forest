using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// The container gameobject, used to rotate the health bar to face the camera
    /// </summary>
    [SerializeField]
    private GameObject healthBarContainer;

    /// <summary>
    /// The renderer of the health bar object
    /// </summary>
    [SerializeField]
    private Renderer healthBarRenderer;

    /// <summary>
    /// The text that will display the health in the form of 'x/x'
    /// </summary>
    [SerializeField]
    private TextMeshPro healthText;

    /// <summary>
    /// The camera that the health bar will face
    /// </summary>
    private Camera playerCam;

    /// <summary>
    /// Property blocks are like how a regular script can have different values for different instances
    /// of the same script, but with materials. This allows us to share the same material across 
    /// multiple objects without them all changing whenever we update a value.
    /// </summary>
    private static readonly int FillProperty = Shader.PropertyToID("_Fill");

    private float lastHealth = -1;

    private MaterialPropertyBlock propertyBlock;

    private Coroutine healthAnimationCoroutine;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        StartCoroutine(GetPlayerCamera());
    }

    private IEnumerator GetPlayerCamera()
    {
        while (playerCam == null)
        {
            playerCam = Camera.main;
            yield return null;
        }
    }


    private void Update()
    {
        if (playerCam == null)
            return;

        Vector3 direction = (playerCam.transform.position - healthBarContainer.transform.position).normalized;
        healthBarContainer.transform.rotation = quaternion.LookRotation(direction, Vector3.up);
    }

    /// <summary>
    /// Function called to update the health bar visuals
    /// </summary>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    public void TriggerHealthBarUpdate(float currentHealth, float maxHealth)
    {
        if(lastHealth < 0)
            lastHealth = maxHealth;

        if (healthAnimationCoroutine == null)
        {
            healthAnimationCoroutine = StartCoroutine(ChangeHealthOverTime(lastHealth, currentHealth, maxHealth, 0.5f));

        }
        else
        {
            StopCoroutine(healthAnimationCoroutine);
            healthAnimationCoroutine = StartCoroutine(ChangeHealthOverTime(lastHealth, currentHealth, maxHealth, 0.5f));
        }

        UpdateHealthText(currentHealth, maxHealth);
    } 

    /// <summary>
    /// Updates the actual health bar itself
    /// </summary>
    /// <param name="healthPercentage"></param>
    private void UpdateHealthBar(float healthPercentage)
    {
        healthBarRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FillProperty, healthPercentage);
        healthBarRenderer.SetPropertyBlock(propertyBlock);
    }

    private IEnumerator ChangeHealthOverTime(float startingHealth, float targetHealth, float enemyMaxHealth, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float currentHealth = Mathf.Lerp(startingHealth, targetHealth, t);
            UpdateHealthBar(currentHealth / enemyMaxHealth);
            yield return null;
        }

        lastHealth = targetHealth;
        healthAnimationCoroutine = null;
    }

    /// <summary>
    /// Updates the health text
    /// </summary>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    private void UpdateHealthText(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            //Displays the health as an integer value, rounded down, in the form of 'x/x'
            healthText.text = $"{(int)currentHealth}/{(int)maxHealth}";
        }
    }
}
