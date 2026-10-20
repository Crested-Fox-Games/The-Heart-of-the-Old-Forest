using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MovementVFXManager : MonoBehaviour
{
    public static MovementVFXManager Instance { get; private set; }

    /// <summary>
    /// This is the file used for the on screen movement vfx
    /// </summary>
    [SerializeField]
    private ScriptableRendererFeature movementVFXFeature;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DisableVFX();
    }

    public void EnableVFX()
    {
        movementVFXFeature.SetActive(true);
    }

    public void DisableVFX()
    {
        movementVFXFeature.SetActive(false);
    }
}
