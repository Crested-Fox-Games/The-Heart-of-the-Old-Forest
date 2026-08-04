using UnityEngine;

/// <summary>
/// Data container for passing the players ready state to and from the server
/// </summary>
[System.Serializable]
public class ReadyRequest
{
    public string playerId;
    public bool isReady;
}
