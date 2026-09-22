using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TowerUpgradeSO towerUpgradeSO;

    private int pathIndex;

    [SerializeField]
    private Image towerUpgradePanel;

    [SerializeField]
    private TextMeshProUGUI upgradeName;

    [SerializeField]
    private TextMeshProUGUI resourceText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private TowerUpgradeUI towerUpgradeUi;

    public int PathIndex => pathIndex;

    /// <summary>
    /// Sets the relevant values of the text in the prefab slot
    /// </summary>
    /// <param name="towerUpgradeUi"></param>
    /// <param name="so"></param>
    /// <param name="pathIndex"></param>
    public void Initialize(TowerUpgradeUI towerUpgradeUi, TowerUpgradeSO so, int pathIndex)
    {
        this.towerUpgradeUi = towerUpgradeUi;
        this.towerUpgradeSO = so;
        this.pathIndex = pathIndex;


        upgradeName.text = so.UpgradeName;
        descriptionText.text = so.Description;

        string resourceString = "";

        foreach (var resource in so.RequiredResources)
        {
            resourceString += $"{resource.resource.ToString()}: {resource.cost}\n";
        }

        resourceText.text = resourceString;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Check if can afford, if yes, green, if no, red
        foreach (var resource in towerUpgradeSO.RequiredResources)
        {
            if (!BaseResourceController.Instance.CheckEnoughResources(resource.resource, resource.cost))
            {
                towerUpgradePanel.color = Color.red;
                return;
            }
        }

        towerUpgradePanel.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Make white when we stop hovering over the tower
        towerUpgradePanel.color = Color.white;
    }

    public void OnClick()
    {
        towerUpgradeUi.SelectUpgrade(pathIndex);
    }
}
