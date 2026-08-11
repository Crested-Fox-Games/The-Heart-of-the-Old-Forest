using System.Collections;
using UnityEngine;

public class BlightNodeGrowth : MonoBehaviour
{
    [SerializeField]
    private float growthDuration = 3f;

    private RevealController revealController;

    private void Awake()
    {
        revealController = GetComponent<RevealController>();
    }

    private void Start()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        float timer = 0f;

        while (timer < growthDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / growthDuration;

            revealController.SetReveal(progress);

            yield return null;
        }

        revealController.SetReveal(1f);
    }
}
