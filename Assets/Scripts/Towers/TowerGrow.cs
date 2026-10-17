using UnityEngine;

public class TowerGrow : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer Vines;
    [SerializeField] private int BlendShapeIndex = 0;
    [SerializeField] private float GrowTime = 200f;

    void Awake()
    {
        Vines.SetBlendShapeWeight(BlendShapeIndex, 100);
    }

    // Update is called once per frame
    void Update()
    {
        float CurrentWeight = Vines.GetBlendShapeWeight(BlendShapeIndex);
        float NewWeight = Mathf.MoveTowards(CurrentWeight, 0f, GrowTime * Time.deltaTime);
        Vines.SetBlendShapeWeight(BlendShapeIndex, NewWeight);

        if (NewWeight <= 0)
        {
            enabled = false;
        }
    }
}
