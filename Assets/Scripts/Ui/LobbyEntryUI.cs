using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lobbyNameText, playerCountText;

    [SerializeField]
    private Button joinButton;

    private LobbyData lobby;

    public void Setup(LobbyData lobbyData)
    {
        lobby = lobbyData;

        lobbyNameText.text = lobby.name;

        playerCountText.text = $"{lobby.currentPlayers}/{lobby.maxPlayers}";

        joinButton.onClick.AddListener(JoinLobby);
    }

    private void JoinLobby()
    {
        LobbyManager.Instance.JoinLobby(lobby);
    }
}
