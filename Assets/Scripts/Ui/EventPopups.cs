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

        //Gets the canvas group so that the text can be faded in
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        //Sets the alpha to 0, making it invisible
        cg.alpha = 0f;

        float timer = 0f;

        //Fades in over time
        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;

            cg.alpha = timer / fadeTimer;

            yield return null;
        }

        //Failsafe to ensure the alpha is correct after loop
        cg.alpha = 1f;

        yield return new WaitForSeconds(stayTimer);

        StartCoroutine(FadeOut(go));
    }

    private IEnumerator FadeOut(GameObject go) 
    {
        //Gets the canvas group so that text can be faded out
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        float timer = 0f;

        //Fades out over time
        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;

            cg.alpha = 1 - timer / fadeTimer;

            yield return null;
        }

        //Failsafe to ensure the alpha is correct after loop
        cg.alpha = 0f;

        go.SetActive(false);
    }
}
