using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This class handles communicating between unity and the program/site that is storing the data on the lobbies
/// </summary>
public class LobbyApi : MonoBehaviour
{
    /// <summary>
    /// this stores the address of the api
    /// </summary>
    private const string BaseURL = "http://localhost:5038/Lobby";

    //Temp
    private void Start()
    {
        StartCoroutine(CreateLobby("Rax Lobby"));
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

        //Prints the request as a string
        Debug.Log(request.downloadHandler.text);
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

        //Prints the request as a string
        Debug.Log(request.downloadHandler.text);
    }
}
