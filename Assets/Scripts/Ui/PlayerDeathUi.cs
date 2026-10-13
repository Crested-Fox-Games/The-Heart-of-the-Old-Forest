using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerDeathUi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    /// <summary>
    /// Handles what ui events happen when this script is called
    /// </summary>
    /// <param name="respawnTimer"></param>
    public void PlayerDeath(float respawnTimer)
    {
        StartCoroutine(PlayerRespawnUi(respawnTimer));
    }

    /// <summary>
    /// Handles the timer for the players respawn
    /// </summary>
    /// <param name="respawnTimer"></param>
    /// <returns></returns>
    private IEnumerator PlayerRespawnUi(float respawnTimer)
    {
        float timer = 0;


        while (timer < respawnTimer)
        {
            timer += Time.deltaTime;

            timerText.text = (respawnTimer - timer).ToString("F1") + "s";
            yield return null;
        }

        UiManager.Instance.ClosePlayerDeathUi();
    }
}
