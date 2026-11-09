using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ResourceIndexUi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI resourceText, playerAmountText, baseAmountText;

    public void Initialize(ResourceType resource, float playerAmount = 0, float baseAmount = 0)
    {
        resourceText.text = resource.ToString();
        playerAmountText.text = playerAmount.ToString();
        baseAmountText.text = baseAmount.ToString();
    }

    public void UpdateUiText(float playerRes, float baseRes)
    {
        playerAmountText.text = playerRes.ToString();
        baseAmountText.text = baseRes.ToString();
    }
}
