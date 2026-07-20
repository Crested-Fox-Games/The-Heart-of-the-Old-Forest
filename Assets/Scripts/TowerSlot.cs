using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TowerSO towerSO;

    [SerializeField]
    private Image towerSlotPanel;

    [SerializeField]
    private TextMeshProUGUI towerName;

    [SerializeField]
    private TextMeshProUGUI resourceText;

    [SerializeField]
    private Image towerImage;

    private TowerPlacementUi towerPlacementUi;

    public void Initialize(TowerPlacementUi tpUi)
    {
        towerPlacementUi = tpUi;

        towerName.text = towerSO.TowerName;

        //TODO: add an image field
        //towerImage = towerSO

        string resourceString = "";

        foreach(var resource in towerSO.RequiredResources)
        {
            resourceString += $"{resource.resource.ToString()}: {resource.cost.ToString()}\n";
        }

        resourceText.text = resourceString;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Check if can afford, if yes, green, if no, red
        foreach(var resource in towerSO.RequiredResources)
        {
            if(!BaseResourceController.Instance.CheckEnoughResources(resource.resource, resource.cost))
            {
                towerSlotPanel.color = Color.red;
                return;
            }
        }

        towerSlotPanel.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Make white when we stop hovering over the tower
        towerSlotPanel.color = Color.white;
    }

    public void OnClick()
    {
        towerPlacementUi.SelectTower(towerSO);
    }
}
