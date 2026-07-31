using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

/// <summary>
/// This class handles communicating between unity and the program/site that is storing the data on the lobbies
/// </summary>
public class LobbyApi : MonoBehaviour
{
    /// <summary>
    /// this stores the address of the api
    /// This is currently hosted on a website as of 29/7/26
    /// </summary>
    [SerializeField]
    private const string BaseURL = "https://hotof-lobby-server.onrender.com/Lobby";

    /// <summary>
    /// An event for when the lobbies are received from the server
    /// </summary>
    public event Action<LobbyData[]> OnLobbiesReceived;

    //Temp
    private void Start()
    {
        //StartCoroutine(CreateLobby("Rax Lobby"));
    }

    /// <summary>
    /// This function lets us get the currently available lobbies
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetLobbies()
    {
        //This creates the GET request using the base url
        UnityWebRequest request = UnityWebRequest.Get(BaseURL);

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if(request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Gets a list of lobbies using the JsonHelper class
        LobbyData[] lobbies = JsonHelper.FromJson<LobbyData>(request.downloadHandler.text);

        //Fires the event for when the lobbies have been recieved
        OnLobbiesReceived?.Invoke(lobbies);
    }

    /// <summary>
    /// Function to get a specific lobby from its unique guid
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <returns></returns>
    public IEnumerator GetLobby(string lobbyId)
    {
        //Gets the url for the lobby
        string url = $"{BaseURL}/{lobbyId}";

        //This creates the GET request using the url
        UnityWebRequest request = UnityWebRequest.Get(url);

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Takes the Json response from the server and converts it to C#
        LobbyData lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);

        //Tells the lobby manager we have received new info on the lobby
        LobbyManager.Instance.UpdateLobby(lobby);
    }

    /// <summary>
    /// This function lets us ask the server to create a lobby for us
    /// </summary>
    /// <param name="lobbyName"></param>
    /// <returns></returns>
    public IEnumerator CreateLobby(string lobbyName, Action<LobbyData> callback)
    {
        //Creates an object with the data that the server needs 
        CreateLobbyRequest lobbyRequest = new CreateLobbyRequest()
        {
            name = lobbyName,
            hostIp = NetworkUtility.GetLocalIpAddress(),
            maxPlayers = 4,
            playerName = PlayerPrefs.GetString("PlayerName")
        };

        //Converts the lobbyrequest above to json
        string json = JsonUtility.ToJson(lobbyRequest);

        //creates a POST request to create the lobby
        UnityWebRequest request = new UnityWebRequest(BaseURL, "POST");

        //Convers JSON to bytes, this is as HTTP sends data as bytes
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        //This attaches the JSON we want to send
        request.uploadHandler = new UploadHandlerRaw(body);

        //This creates a place to store the response
        request.downloadHandler = new DownloadHandlerBuffer();

        //This tells the server the data format
        request.SetRequestHeader("Content-Type","application/json");

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Takes the Json response from the server and converts it to C#
        LobbyResponse response = JsonUtility.FromJson<LobbyResponse>(request.downloadHandler.text);

        //Tells the lobby manager what the players id is
        LobbyManager.Instance.SetPlayerId(response.playerId);

        //Calls the function that was passed in as a parameter when this function was called from LobbyManager
        callback?.Invoke(response.lobby);
    }

    /// <summary>
    /// Attempts to have a client join the server
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator JoinLobby(string lobbyId, Action<LobbyData> callback)
    {
        //Gets the url for the lobby with the name of the function on the server api
        string url = $"{BaseURL}/{lobbyId}/join";

        //Creates an object with the data that the server needs 
        JoinLobbyRequest joinRequest = new JoinLobbyRequest()
        {
            playerName = PlayerPrefs.GetString("PlayerName", "Player")
        };

        //Converts the ready request to a json file
        string json = JsonUtility.ToJson(joinRequest);

        //creates a POST request to join the lobby
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        //Convers JSON to bytes, this is as HTTP sends data as bytes
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        //This attaches the JSON we want to send
        request.uploadHandler = new UploadHandlerRaw(body);

        //This creates a place to store the response
        request.downloadHandler = new DownloadHandlerBuffer();

        //This tells the server the data format
        request.SetRequestHeader("Content-Type", "application/json");

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Takes the Json response from the server and converts it to C#
        LobbyResponse response = JsonUtility.FromJson<LobbyResponse>(request.downloadHandler.text);

        //Tells the lobby manager what the players id is
        LobbyManager.Instance.SetPlayerId(response.playerId);

        //Calls the function that was passed in as a parameter when this function was called from LobbyManager
        callback?.Invoke(response.lobby);
    }

    /// <summary>
    /// Sends the players ready status to the server and updates it for all players
    /// </summary>
    /// <param name="isReady"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator SetReady(bool isReady, Action<LobbyData> callback)
    {
        //Gets the url for the lobby with the name of the function on the server api
        string url = $"{BaseURL}/{LobbyManager.Instance.CurrentLobby.id}/ready";

        //Creates an object with the data that the server needs 
        ReadyRequest ready = new ReadyRequest()
        {
            playerId = LobbyManager.Instance.LocalPlayerId,
            isReady = isReady,
        };

        //Converts the ready request to a json file
        string json = JsonUtility.ToJson(ready);

        //creates a POST request to set the players ready status
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        //This attaches the JSON we want to send
        request.uploadHandler = new UploadHandlerRaw(body);

        //This creates a place to store the response
        request.downloadHandler = new DownloadHandlerBuffer();

        //This tells the server the data format
        request.SetRequestHeader("Content-Type", "application/json");

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Takes the Json response from the server and converts it to C#
        LobbyData lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);

        //Calls the function that was passed in as a parameter when this function was called from LobbyManager
        callback?.Invoke(lobby);
    }

    /// <summary>
    /// The function called by the host when we want to start the game
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator StartGame(Action<LobbyData> callback)
    {
        //Gets the url for the lobby with the name of the function on the server api
        string url = $"{BaseURL}/{LobbyManager.Instance.CurrentLobby.id}/start";

        //creates a POST request to start the game
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        //This creates a place to store the response
        request.downloadHandler = new DownloadHandlerBuffer();

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        //Takes the Json response from the server and converts it to C#
        LobbyData lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);

        //Calls the function that was passed in as a parameter when this function was called from LobbyManager
        callback?.Invoke(lobby);
    }

    /// <summary>
    /// This sends an update to the server every couple of seconds to let it know that the lobby is still active
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <returns></returns>
    public IEnumerator SendHeartbeat(string lobbyId)
    {
        //Gets the url for the lobby with the name of the function on the server api
        string url = $"{BaseURL}/{lobbyId}/heartbeat";

        //creates a POST request to send a heartbeat to the server
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        //This creates a place to store the response
        request.downloadHandler = new DownloadHandlerBuffer();

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
    }
}
