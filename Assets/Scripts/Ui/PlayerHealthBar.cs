using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private TextMeshProUGUI healthText;

    private float lastHealth = -1f;

    private Coroutine healthAnimationCoroutine;

    /// <summary>
    /// Function called to update the health bar visuals
    /// </summary>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    public void TriggerHealthBarUpdate(float currentHealth, float maxHealth)
    {
        if (lastHealth < 0)
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

    private IEnumerator ChangeHealthOverTime(float startingHealth, float targetHealth, float enemyMaxHealth, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float currentHealth = Mathf.Lerp(startingHealth, targetHealth, t);
            UpdateHealthFill(currentHealth / enemyMaxHealth);
            yield return null;
        }

        lastHealth = targetHealth;
        healthAnimationCoroutine = null;
    }

    private void UpdateHealthFill(float healthPercentage)
    {
        healthBarFill.fillAmount = healthPercentage;
    }

    private void UpdateHealthText(float currentHealth, float maxHealth)
    {
        healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
    }
}
