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
    /// The image component that will fill to represent the health bar
    /// </summary>
    [SerializeField]
    private Image healthBarImage;

    /// <summary>
    /// The text that will display the health in the form of 'x/x'
    /// </summary>
    [SerializeField]
    private TextMeshPro healthText;

    /// <summary>
    /// The camera that the health bar will face
    /// </summary>
    private Camera playerCam;

    private void Start()
    {
        playerCam = Camera.main;
    }


    private void Update()
    {
        healthBarContainer.transform.rotation = quaternion.LookRotation(playerCam.transform.forward, Vector3.up);
    }

    public void TriggerHealthBarUpdate(float currentHealth, float maxHealth)
    {
        float percentage = currentHealth / maxHealth;

        UpdateHealthBar(percentage);
        UpdateHealthText(currentHealth, maxHealth);
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.Clamp01(healthPercentage);
        }

        if(healthText != null)
        {
            healthText.text = $"";
        }
    }

    private void UpdateHealthText(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
