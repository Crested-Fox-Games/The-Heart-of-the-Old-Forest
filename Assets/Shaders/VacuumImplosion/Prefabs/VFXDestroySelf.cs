using UnityEngine;

public class VFXDestroySelf : MonoBehaviour
{

    [SerializeField] private float delay = 1f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
