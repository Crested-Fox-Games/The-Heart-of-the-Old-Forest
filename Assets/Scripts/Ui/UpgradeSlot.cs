using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Scriptable object containing the upgrade's information
    /// </summary>
    [SerializeField]
    private TowerUpgradeSO towerUpgradeSO;

    /// <summary>
    /// Index of the upgrade path this slot belongs to
    /// </summary>
    private int pathIndex;

    /// <summary>
    /// Image used as the background for the upgrade slot
    /// </summary>
    [SerializeField]
    private Image towerUpgradePanel;

    /// <summary>
    /// Text displaying the name of the upgrade
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI upgradeName;

    /// <summary>
    /// Text displaying the resource and cost required to purchase the upgrade
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI resourceText;

    /// <summary>
    /// Text displaying the description of the upgrade
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    /// <summary>
    /// Reference to the main tower upgrade UI that manages this slot
    /// </summary>
    private TowerUpgradeUI towerUpgradeUi;

    /// <summary>
    /// Access to the upgrade path index used when selecting the upgrade
    /// </summary>
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
