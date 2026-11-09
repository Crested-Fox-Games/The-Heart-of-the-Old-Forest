using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlightSpreadShader : MonoBehaviour
{
    /// <summary>
    /// Time it takes to spread to max
    /// </summary>
    [SerializeField] private float SpreadTime = 2f;

    /// <summary>
    /// Max size
    /// </summary>
    [SerializeField] private float MaxSpread = 0.46f;

    /// <summary>
    /// Make radius an editable variable
    /// </summary>
    private int SpreadRadius = Shader.PropertyToID("_Radius");

    /// <summary>
    /// The material used for the shader
    /// </summary>
    private Material Material;

    /// <summary>
    /// The shader's current radius
    /// </summary>
    private float CurrentRadius;

    /// <summary>
    /// The shader's target radius
    /// </summary>
    private float TargetRadius;

    private void Awake()
    {
        ///<summary>
        ///Set up variables for usage
        /// </summary>
        DecalProjector projector = GetComponentInChildren<DecalProjector>();
        Material = new Material(projector.material); //Make a new material so the shader's radius doesn't effect ALL of the spread shaders, and just this one
        projector.material = Material;
        TargetRadius = MaxSpread;
    }

    public void Shrink() 
    {
        ///<summary>
        ///Tell the shader its target is to shrink to 0 now
        /// </summary>
        TargetRadius = 0f;
    } 

    private void Update()
    {
        ///<summary>
        ///Update the shaders to spread toward the target
        /// </summary>
        CurrentRadius = Mathf.MoveTowards(CurrentRadius, TargetRadius, MaxSpread * Time.deltaTime / SpreadTime);
        Material.SetFloat(SpreadRadius, CurrentRadius);
        ///<summary>
        ///Destroy the shader if its target is 0 and its radius is already 0, so it is destroyed when shrink
        /// </summary>
        if (CurrentRadius <= 0 && TargetRadius <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ///<summary>
        ///Destroy the material we made to avoid lag
        /// </summary>
        Destroy(Material);
    }
}
