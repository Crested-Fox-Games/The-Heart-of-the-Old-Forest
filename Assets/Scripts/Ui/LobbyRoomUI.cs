using UnityEngine;

public class LobbyRoomUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerListParent;

    [SerializeField]
    private PlayerEntryUI playerEntryPrefab;

    private void Awake()
    {
        LobbyManager.Instance.OnLobbyUpdated += UpdateLobby;
    }

    private void UpdateLobby(LobbyData lobby)
    {
        ClearPlayers();

        foreach(LobbyPlayerData player in lobby.players)
        {
            PlayerEntryUI playerEntry = Instantiate(playerEntryPrefab, playerListParent);

            playerEntry.Setup(player);
        }
    }

    private void ClearPlayers()
    {
        foreach(Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }
    }
}
