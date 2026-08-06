using System;
using System.ComponentModel.Design;
using UnityEngine;

public class LobbyBrowserUI : MonoBehaviour
{
    [SerializeField]
    private Transform lobbyListParent;

    [SerializeField]
    private LobbyEntryUI lobbyEntryPrefab;

    private void Start()
    {
        LobbyManager.Instance.OnLobbiesUpdated += DisplayLobbies;
        LobbyManager.Instance.OnLobbyJoined += JoinedLobby;
    }

    /// <summary>
    /// Tells the lobby manager we want to look for games
    /// </summary>
    public void FindGames()
    {
        LobbyManager.Instance.RefreshLobbies();
    }

    /// <summary>
    /// Shows the lobbies in the browser
    /// </summary>
    /// <param name="lobbies"></param>
    private void DisplayLobbies(LobbyData[] lobbies)
    {
        ClearLobbyList();

        foreach (LobbyData lobby in lobbies)
        {
            LobbyEntryUI entry = Instantiate(lobbyEntryPrefab, lobbyListParent);

            entry.Setup(lobby);
        }
    }

    private void ClearLobbyList()
    {
        foreach(Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        if(LobbyManager.Instance != null)
            LobbyManager.Instance.OnLobbiesUpdated -= DisplayLobbies;
    }

    private void JoinedLobby(LobbyData lobby)
    {
        //Hide Browser & Show Lobby Room
        MenuUiManager.Instance.OpenLobbyPanel();
    }

}
