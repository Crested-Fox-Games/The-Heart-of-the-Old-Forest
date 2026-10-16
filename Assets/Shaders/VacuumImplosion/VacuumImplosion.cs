using UnityEngine;
using UnityEngine.UIElements;

public class VacuumImplosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject ExplosionLines;
    [SerializeField] private GameObject ExplosionSphere;
    [SerializeField] private float ExplosionLinesScale = 1f;
    [SerializeField] private float ExplosionSphereScale = 1f;
    private int testtimer = 30;
    void Start()
    {
        SpawnLines();
        SpawnSphere();
    }

    // Update is called once per frame
    void Update()
    {
        testtimer--;
        if (testtimer <= 0)
        {
            testtimer = 30;
            SpawnLines();
            SpawnSphere();
        }
    }

    private void SpawnLines()
    {
        GameObject explosionLines = Instantiate(ExplosionLines, transform.position, Quaternion.identity);
        ExplosionLines.transform.localScale = Vector3.one * ExplosionLinesScale;
    }


    private void SpawnSphere()
    {
        GameObject explosionSphere = Instantiate(ExplosionSphere, transform.position, Quaternion.identity);
        ExplosionSphere.transform.localScale = Vector3.one * ExplosionSphereScale;
    }
}
