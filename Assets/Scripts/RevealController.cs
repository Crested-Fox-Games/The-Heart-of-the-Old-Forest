using UnityEngine;

//This makes it so that any gameobject with this script requires a renderer
[RequireComponent(typeof(Renderer))]
public class RevealController : MonoBehaviour
{
    [Range(0, 1)]
    public float revealProgress;

    private Renderer rend;

    /// <summary>
    /// Container for custom material values
    /// </summary>
    private MaterialPropertyBlock block;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        block = new MaterialPropertyBlock();

        //Gets the objects size
        Bounds bounds = rend.bounds;

        //Gets the current shader properties from the renderer
        rend.GetPropertyBlock(block);

        //Sends the values to the shader
        block.SetFloat("_ObjectBottom", bounds.min.y);
        block.SetFloat("_ObjectHeight", bounds.size.y);

        //Applies changes to the renderer
        rend.SetPropertyBlock(block);
    }

    public void SetReveal(float value)
    {
        revealProgress = value;

        //Gets the current shader properties from the renderer
        rend.GetPropertyBlock(block);

        //Sends the values to the shader
        block.SetFloat("_Reveal", revealProgress);

        //Applies changes to the renderer
        rend.SetPropertyBlock(block);
    }
}
