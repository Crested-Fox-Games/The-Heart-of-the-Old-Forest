using UnityEngine;

public class TowerGrow : MonoBehaviour
{
    /// <summary>
    /// The vine mesh
    /// </summary>
    [SerializeField] private SkinnedMeshRenderer Vines;

    /// <summary>
    /// The blendshape we want to change
    /// </summary>
    [SerializeField] private int BlendShapeIndex = 0;

    /// <summary>
    /// Time it takes to grow
    /// </summary>
    [SerializeField] private float GrowTime = 200f;

    void Awake()
    {
        ///<summary>
        ///Make sure the vines start at 0 growth
        /// </summary>
        Vines.SetBlendShapeWeight(BlendShapeIndex, 100);
    }

    // Update is called once per frame
    void Update()
    {
        ///<summary>
        ///Smoothly change the weight so it grows
        /// </summary>
        float CurrentWeight = Vines.GetBlendShapeWeight(BlendShapeIndex);
        float NewWeight = Mathf.MoveTowards(CurrentWeight, 0f, GrowTime * Time.deltaTime);
        Vines.SetBlendShapeWeight(BlendShapeIndex, NewWeight);

        if (NewWeight <= 0)
        {
            enabled = false; //disable the script when growing is done (at 0)
        }
    }
}
