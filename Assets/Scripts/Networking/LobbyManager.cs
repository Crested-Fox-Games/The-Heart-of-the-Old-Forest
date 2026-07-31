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
    /// Stores the details of the current lobby
    /// </summary>
    public LobbyData CurrentLobby { get; private set; }

    /// <summary>
    /// Lets ui scripts know when lobbies have been updated
    /// </summary>
    public event Action<LobbyData[]> OnLobbiesUpdated;

    /// <summary>
    /// Lets other systems know we joined a lobby
    /// </summary>
    public event Action<LobbyData> OnLobbyJoined;

    /// <summary>
    /// Called whenever the current lobbys information is updated
    /// </summary>
    public event Action<LobbyData> OnLobbyUpdated;

    /// <summary>
    /// This will ask the server for updates on the lobby every few seconds
    /// </summary>
    private Coroutine lobbyPollingRoutine;

    /// <summary>
    /// The coroutine for telling the server that this lobby is still alive
    /// </summary>
    private Coroutine heartbeatRoutine;

    public string LocalPlayerId { get; private set; }

    private bool gameStarted = false;

    #region Unity Functions
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

    private void OnDestroy()
    {
        if (lobbyApi != null)
            lobbyApi.OnLobbiesReceived -= HandleLobbiesReceived;

        if (FishNetManager.Instance != null)
            FishNetManager.Instance.OnConnected -= HandleFishNetConnected;
    }

    #endregion

    #region Api Calls
    /// <summary>
    /// Called when the player pressed find games
    /// </summary>
    public void RefreshLobbies()
    {
        StartCoroutine(lobbyApi.GetLobbies());
    }

    /// <summary>
    /// Handles when we attempt to join a lobby
    /// </summary>
    /// <param name="lobby"></param>
    public void JoinLobby(LobbyData lobby)
    {
        StartCoroutine(lobbyApi.JoinLobby(lobby.id, HandleLobbyJoined));
    }

    /// <summary>
    /// Handles when we changes the ready state of the player
    /// </summary>
    /// <param name="ready"></param>
    public void SetReady(bool ready)
    {
        StartCoroutine(lobbyApi.SetReady(ready, HandleLobbyUpdated));
    }

    /// <summary>
    /// Called from the start game button on the host screen
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(lobbyApi.StartGame(HandleLobbyUpdated));
    }


    #endregion

    #region Handlers
    /// <summary>
    /// Receives the data from LobbyApi
    /// </summary>
    /// <param name="lobbies"></param>
    private void HandleLobbiesReceived(LobbyData[] lobbies)
    {
        CurrentLobbies = lobbies;

        OnLobbiesUpdated?.Invoke(CurrentLobbies);
    }

    /// <summary>
    /// Handles the logic for when we've joined a lobby
    /// </summary>
    /// <param name="lobby"></param>
    private void HandleLobbyJoined(LobbyData lobby)
    {
        CurrentLobby = lobby;

        StartLobbyPolling(lobby.id);

        FishNetManager.Instance.OnConnected += HandleFishNetConnected;

        FishNetManager.Instance.ConnectToHost(lobby.hostIp);
    }

    /// <summary>
    /// Handles when fishnet has been connected
    /// </summary>
    private void HandleFishNetConnected()
    {
        FishNetManager.Instance.OnConnected -= HandleFishNetConnected;

        OnLobbyJoined?.Invoke(CurrentLobby);
    }

    /// <summary>
    /// Handles what needs to be done when a lobby is created
    /// </summary>
    /// <param name="lobby"></param>
    private void HandleLobbyCreated(LobbyData lobby)
    {
        CurrentLobby = lobby;

        StartHeartbeat();

        StartLobbyPolling(CurrentLobby.id);

        OnLobbyJoined?.Invoke(CurrentLobby);

        MenuUiManager.Instance.OpenLobbyPanel();
    }

    /// <summary>
    /// Handles what happens when the lobby information is updated
    /// </summary>
    /// <param name="lobby"></param>
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

    #endregion

    #region Ienumerators
    /// <summary>
    /// Runs the lobby api for creating a lobby
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateHostLobby()
    {
        yield return StartCoroutine(lobbyApi.CreateLobby($"{PlayerPrefs.GetString("PlayerName", "Player")}'s lobby", HandleLobbyCreated));
    }

    /// <summary>
    /// Handles the polling of the server (asking it for updates) 
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Lets the server know that the lobby is still active
    /// </summary>
    /// <returns></returns>
    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            StartCoroutine(lobbyApi.SendHeartbeat(CurrentLobby.id));
        }
    }
    #endregion

    /// <summary>
    /// Handles the host starting a lobby
    /// </summary>
    public void HostLobby()
    {
        FishNetManager.Instance.StartHost();

        StartCoroutine(CreateHostLobby());
    }

    /// <summary>
    /// Handles when we leave a lobby
    /// </summary>
    public void LeaveLobby()
    {
        StopLobbyPolling();
    }

    /// <summary>
    /// Updates the lobbies info
    /// </summary>
    /// <param name="lobby"></param>
    public void UpdateLobby(LobbyData lobby)
    {
        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby);
    }

    /// <summary>
    /// Starts polling the server (asking it for updates) 
    /// </summary>
    /// <param name="lobbyId"></param>
    public void StartLobbyPolling(string lobbyId)
    {
        if (lobbyPollingRoutine != null)
        {
            StopCoroutine(lobbyPollingRoutine);
        }

        lobbyPollingRoutine = StartCoroutine(PollLobby(lobbyId));
    }

    /// <summary>
    /// Stops polling the server (asking it for updates) 
    /// </summary>
    public void StopLobbyPolling()
    {
        if(lobbyPollingRoutine != null)
        {
            StopCoroutine(lobbyPollingRoutine);
            lobbyPollingRoutine = null;
        }
    }
  
    /// <summary>
    /// Handles starting the game when the host clicks start
    /// </summary>
    private void StartGameScene()
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        SceneLoadData sceneLoadData = new SceneLoadData("Gameplay");

        sceneLoadData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    /// <summary>
    /// Sets the local players id
    /// </summary>
    /// <param name="playerId"></param>
    public void SetPlayerId(string playerId)
    {
        LocalPlayerId = playerId;
    }

    /// <summary>
    /// Checks if the local player is the host
    /// </summary>
    /// <returns></returns>
    private bool IsLocalPlayerHost()
    {
        if(CurrentLobby == null)
            return false;

        LobbyPlayerData playerData = CurrentLobby.players.FirstOrDefault(p => p.id == LocalPlayerId);

        return playerData != null && playerData.isHost;
    }

    /// <summary>
    /// Starts sending updates to the server
    /// </summary>
    public void StartHeartbeat()
    {
        if (!IsLocalPlayerHost())
            return;

        heartbeatRoutine = StartCoroutine(HeartbeatLoop());
    }
}
