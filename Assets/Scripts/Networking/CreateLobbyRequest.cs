using System;

/// <summary>
/// This class is used as a data container for when we try to create a lobby
/// </summary>
[Serializable]
public class CreateLobbyRequest
{
    public string name;
    public string hostIp;
    public int maxPlayers;
    public string playerName;
}
