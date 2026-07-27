using UnityEngine;

public class LobbyBrowserUI : MonoBehaviour
{
    private LobbyApi lobbyApi;

    private void Start()
    {
        lobbyApi = FindFirstObjectByType<LobbyApi>();

        lobbyApi.OnLobbiesReceived += DisplayLobbies;
    }

    public void FindLobbies()
    {
        StartCoroutine(lobbyApi.GetLobbies());
    }

    private void DisplayLobbies(LobbyData[] lobbies)
    {
        foreach (LobbyData lobby in lobbies)
        {
            Debug.Log($"{lobby.name} {lobby.currentPlayers}/{lobby.maxPlayers}");
        }
    }

    private void OnDestroy()
    {
        lobbyApi.OnLobbiesReceived -= DisplayLobbies;
    }
}
