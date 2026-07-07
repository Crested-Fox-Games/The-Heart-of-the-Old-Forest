using TMPro;
using UnityEngine;

/// <summary>
/// This class handles the ui updates
/// </summary>
public class UiManager : MonoBehaviour
{
    public static UiManager instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI interactText;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Used to display the text for the interaction popup
    /// </summary>
    /// <param name="interactionText"></param>
    public void ShowInteractionPopup(string text)
    {
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
}
