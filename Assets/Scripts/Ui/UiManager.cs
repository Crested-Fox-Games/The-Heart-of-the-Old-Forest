using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

/// <summary>
/// This class handles the ui updates
/// </summary>
public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    /// <summary>
    /// The text box used for the interaction popup
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI interactText;

    /// <summary>
    /// The text box used for the resource display
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI resourceText;

    /// <summary>
    /// The ui panel for the tower placement ui
    /// </summary>
    [SerializeField]
    private GameObject tmpTowerPlacementUi;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Used to display the text for the interaction popup
    /// </summary>
    /// <param name="interactionText"></param>
    public void ShowInteractionPopup(string text)
    {
        //TODO: Potentially make this able to display icons for use with other devices like controllers.
        interactText.text = text;
        interactText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Used to hide the text for interaction popup
    /// </summary>
    public void HideInteractionPopup()
    {
        interactText.text = "";
        interactText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Used to show the tower placement ui
    /// </summary>
    public void ShowTowerPlacementUi()
    {
        tmpTowerPlacementUi.SetActive(true);
    }

    /// <summary>
    /// Used to hide the tower placement ui
    /// </summary>
    public void HideTowerPlacementUi()
    {
        tmpTowerPlacementUi.SetActive(false);
    }

    /// <summary>
    /// Used to update the player's resource display
    /// </summary>
    /// <param name="resourceAmounts"></param>
    public void UpdatePlayerResourceUi(SyncDictionary<ResourceType, int> resourceAmounts)
    {
        string playerResources = "";
        foreach (var resource in resourceAmounts)
        {
            playerResources += $"{resource.Key}: {resource.Value}\n";
        }

        resourceText.text = playerResources;
    }
}
