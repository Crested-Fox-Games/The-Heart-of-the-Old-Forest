using UnityEngine;

public class MenuUiManager : MonoBehaviour
{
    public static MenuUiManager Instance { get; private set; }

    [SerializeField]
    private GameObject onlinePanel, browserPanel, lobbyPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OpenOnlinePanel()
    {
        onlinePanel.SetActive(true);
    }

    public void CloseOnlinePanel()
    {
        onlinePanel.SetActive(false);
    }

    public void OpenBrowserPanel()
    {
        browserPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void OpenLobbyPanel()
    {
        lobbyPanel.SetActive(true);
        browserPanel.SetActive(false);
    }

    
}
