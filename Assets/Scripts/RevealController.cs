using Unity.VisualScripting;
using UnityEngine;

//This makes it so that any gameobject with this script requires a renderer
[RequireComponent(typeof(Renderer))]
public class RevealController : MonoBehaviour
{
    [Range(0, 1)]
    public float revealProgress;

    private Renderer[] renderers;

    /// <summary>
    /// Container for custom material values
    /// </summary>
    private MaterialPropertyBlock block;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        block = new MaterialPropertyBlock();

        UpdateRender();
    }

    public void UpdateRender()
    {
        if (renderers.Length == 0)
            return;

        //Gets the initial bounds for the renderer
        Bounds bounds = renderers[0].bounds;

        //Combine all the bounds for the renderers
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        //Apply the bounds to every renderer
        foreach (Renderer renderer in renderers)
        {
            //Gets the current shader properties from the renderer
            renderer.GetPropertyBlock(block);

            //Sends the values to the shader
            block.SetFloat("_ObjectBottom", bounds.min.y);
            block.SetFloat("_ObjectHeight", bounds.size.y);

            //Applies changes to the renderer
            renderer.SetPropertyBlock(block);
        }
    }

    public void SetReveal(float value)
    {
        revealProgress = value;

        //Apply reveal to all renderers
        foreach (Renderer renderer in renderers)
        {
            //Gets the current shader properties from the renderer
            renderer.GetPropertyBlock(block);

            //Sends the values to the shader
            block.SetFloat("_Reveal", revealProgress);

            //Applies changes to the renderer
            renderer.SetPropertyBlock(block);
        }
    }
}
