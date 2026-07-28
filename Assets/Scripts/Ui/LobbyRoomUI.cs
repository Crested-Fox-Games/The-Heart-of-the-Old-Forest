using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyRoomUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerListParent;

    [SerializeField]
    private PlayerEntryUI playerEntryPrefab;

    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private TextMeshProUGUI lobbyName;

    private bool isHost;

    private void Start()
    {
        LobbyManager.Instance.OnLobbyUpdated += UpdateLobby;
    }

    private void UpdateLobby(LobbyData lobby)
    {
        CheckHost(lobby);

        lobbyName.text = lobby.name;

        ClearPlayers();

        foreach(LobbyPlayerData player in lobby.players)
        {
            PlayerEntryUI playerEntry = Instantiate(playerEntryPrefab, playerListParent);

            playerEntry.Setup(player);
        }
    }

    private void CheckHost(LobbyData lobby)
    {
        LobbyPlayerData player = lobby.players.FirstOrDefault(p => p.id == LobbyManager.Instance.LocalPlayerId);

        if (player == null)
        {
            Debug.Log("Player not found");
            startButton.SetActive(false);
            return;
        }

        startButton.SetActive(player.isHost);
    }

    private void ClearPlayers()
    {
        foreach(Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }
    }
}
