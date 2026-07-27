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
    /// </summary>
    private const string BaseURL = "http://localhost:5038/Lobby";

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

        OnLobbiesReceived?.Invoke(lobbies);
    }

    /// <summary>
    /// This function lets us ask the server to create a lobby for us
    /// </summary>
    /// <param name="lobbyName"></param>
    /// <returns></returns>
    public IEnumerator CreateLobby(string lobbyName)
    {
        //Creates an object with the data that the server needs 
        CreateLobbyRequest lobbyRequest = new CreateLobbyRequest()
        {
            name = lobbyName,
            hostIp = "127.0.0.1",
            maxPlayers = 4
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
        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        //This sends the request, then the coroutine pauses until it gets a response
        yield return request.SendWebRequest();

        //Checks to see if the response succeeded 
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }


        LobbyData lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);

        Debug.Log($"Created lobby: {lobby.name}");
    }

    public IEnumerator JoinLobby(string lobbyId, Action<LobbyData> callback)
    {
        string url = $"{BaseURL}/{lobbyId}/join";

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        LobbyData lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);

        callback?.Invoke(lobby);
    }
}
