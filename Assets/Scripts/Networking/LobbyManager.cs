using System;
using System.Collections;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private LobbyApi lobbyApi;

    /// <summary>
    /// Stores the current available lobbies
    /// </summary>
    public LobbyData[] CurrentLobbies { get; private set; }

    /// <summary>
    /// Lets ui scripts know when lobbies have been updated
    /// </summary>
    public event Action<LobbyData[]> OnLobbiesUpdated;

    /// <summary>
    /// Lets other systems know we joined a lobby
    /// </summary>
    public event Action<LobbyData> OnLobbyJoined;

    /// <summary>
    /// This will ask the server for updates on the lobby every few seconds
    /// </summary>
    private Coroutine lobbyPollingRoutine;

    public LobbyData CurrentLobby {  get; private set; }

    public event Action<LobbyData> OnLobbyUpdated;

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

        lobbyApi = FindFirstObjectByType<LobbyApi>();

        //Subscribe to api responses
        lobbyApi.OnLobbiesReceived += HandleLobbiesReceived;
    }

    //Called when the player pressed find games
    public void RefreshLobbies()
    {
        StartCoroutine(lobbyApi.GetLobbies());
    }

    /// <summary>
    /// Receives the data from LobbyApi
    /// </summary>
    /// <param name="lobbies"></param>
    private void HandleLobbiesReceived(LobbyData[] lobbies)
    {
        CurrentLobbies = lobbies;

        OnLobbiesUpdated?.Invoke(CurrentLobbies);
    }

    private void HandleLobbyJoined(LobbyData lobby)
    {
        CurrentLobby = lobby;

        StartLobbyPolling(lobby.id);

        OnLobbyJoined?.Invoke(lobby);
    }

    private void OnDestroy()
    {
        lobbyApi.OnLobbiesReceived -= HandleLobbiesReceived;
    }

    public void JoinLobby(LobbyData lobby)
    {
        StartCoroutine(lobbyApi.JoinLobby(lobby.id, HandleLobbyJoined));
    }

    public void LeaveLobby()
    {
        StopLobbyPolling();
    }

    public void UpdateLobby(LobbyData lobby)
    {
        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby);
    }

    public void StartLobbyPolling(string lobbyId)
    {
        if(lobbyPollingRoutine != null)
        {
            StopCoroutine(lobbyPollingRoutine);
        }

        lobbyPollingRoutine = StartCoroutine(PollLobby(lobbyId));
    }

    public void StopLobbyPolling()
    {
        if(lobbyPollingRoutine != null)
        {
            StopCoroutine(lobbyPollingRoutine);
            lobbyPollingRoutine = null;
        }
    }

    private IEnumerator PollLobby(string lobbyId)
    {
        while (true)
        {
            yield return StartCoroutine(lobbyApi.GetLobby(lobbyId));

            yield return new WaitForSeconds(5f);
        }
    }


}
