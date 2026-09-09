using System.Collections;
using TMPro;
using UnityEngine;

public class EventPopups : MonoBehaviour
{
    [SerializeField]
    private GameObject dayCountPopup;

    [SerializeField]
    private GameObject enemiesComingPopup;

    /// <summary>
    /// The amount of time it takes for the popup to fade in and out
    /// </summary>
    private float fadeTimer = 1f;

    /// <summary>
    /// The amount of time the popup stays on the screen
    /// </summary>
    private float stayTimer = 3f;

    private void Start()
    {
        TimeCycleManager.Instance.OnNightEnd += DayCountPopup;
        TimeCycleManager.Instance.OnNightStart += EnemiesComingPopup;
    }

    private void DayCountPopup()
    {
        StartCoroutine(FadeIn(dayCountPopup));
    }

    private void EnemiesComingPopup()
    {
        StartCoroutine(FadeIn(enemiesComingPopup));
    }

    private IEnumerator FadeIn(GameObject go)
    {
        go.SetActive(true);

        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        cg.alpha = 0f;

        float timer = 0f;

        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;

            cg.alpha = timer / fadeTimer;

            yield return null;
        }

        cg.alpha = 1f;

        yield return new WaitForSeconds(stayTimer);

        StartCoroutine(FadeOut(go));
    }

    private IEnumerator FadeOut(GameObject go) 
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        float timer = 0f;

        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;

            cg.alpha = 1 - timer / fadeTimer;

            yield return null;
        }

        cg.alpha = 0f;

        go.SetActive(false);
    }
}
