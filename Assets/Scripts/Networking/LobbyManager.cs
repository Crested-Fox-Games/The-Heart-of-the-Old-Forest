using FishNet;
using FishNet.Managing.Scened;
using System;
using System.Collections;
using System.Linq;
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

    private Coroutine heartbeatRoutine;

    public LobbyData CurrentLobby {  get; private set; }

    public string LocalPlayerId { get; private set; }

    public event Action<LobbyData> OnLobbyUpdated;

    private bool gameStarted = false;

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

        FishNetManager.Instance.OnConnected += HandleFishNetConnected;

        FishNetManager.Instance.ConnectToHost(lobby.hostIp);
    }

    private void OnDestroy()
    {
        if(lobbyApi != null)
            lobbyApi.OnLobbiesReceived -= HandleLobbiesReceived;

        if(FishNetManager.Instance != null)
            FishNetManager.Instance.OnConnected -= HandleFishNetConnected;
    }

    private void HandleFishNetConnected()
    {
        FishNetManager.Instance.OnConnected -= HandleFishNetConnected;

        OnLobbyJoined?.Invoke(CurrentLobby);
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

    public void HostLobby()
    {
        FishNetManager.Instance.StartHost();

        StartCoroutine(CreateHostLobby());
    }

    private IEnumerator CreateHostLobby()
    {
        yield return StartCoroutine(lobbyApi.CreateLobby($"{PlayerPrefs.GetString("PlayerName", "Player")}'s lobby", HandleLobbyCreated));
    }

    private void HandleLobbyCreated(LobbyData lobby)
    {
        CurrentLobby = lobby;

        StartHeartbeat();

        StartLobbyPolling(CurrentLobby.id);

        OnLobbyJoined?.Invoke(CurrentLobby);

        MenuUiManager.Instance.OpenLobbyPanel();
    }

    public void StartLobbyPolling(string lobbyId)
    {
        if (lobbyPollingRoutine != null)
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

            //This is the amount of time between updates on the server
            //If we want realtime updates we need to change from polling to a response system
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetReady(bool ready)
    {
        StartCoroutine(lobbyApi.SetReady(ready, HandleLobbyUpdated));
    }

    private void HandleLobbyUpdated(LobbyData lobby)
    {
        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby);

        if (CurrentLobby.inGame && !gameStarted)
        {
            //Run fishnet scene loading
            gameStarted = true;
            StartGameScene();
        }
    }

    private void StartGameScene()
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        SceneLoadData sceneLoadData = new SceneLoadData("Gameplay");

        sceneLoadData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    public void SetPlayerId(string playerId)
    {
        LocalPlayerId = playerId;
    }

    public void StartGame()
    {
        StartCoroutine(lobbyApi.StartGame(HandleLobbyUpdated));
    }

    private bool IsLocalPlayerHost()
    {
        if(CurrentLobby == null)
            return false;

        LobbyPlayerData playerData = CurrentLobby.players.FirstOrDefault(p => p.id == LocalPlayerId);

        return playerData != null && playerData.isHost;
    }

    public void StartHeartbeat()
    {
        if (!IsLocalPlayerHost())
            return;

        heartbeatRoutine = StartCoroutine(HeartbeatLoop());
    }

    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            StartCoroutine(lobbyApi.SendHeartbeat(CurrentLobby.id));
        }
    }
}
