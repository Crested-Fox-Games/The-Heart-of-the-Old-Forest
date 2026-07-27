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
        OpenOnlinePanel();
        browserPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void OpenLobbyPanel()
    {
        OpenOnlinePanel();
        lobbyPanel.SetActive(true);
        browserPanel.SetActive(false);
    }

    public void UpdatePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }
}
