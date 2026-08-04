using System;

/// <summary>
/// The data that we store on each lobby that is created
/// </summary>
[Serializable]
public class LobbyData
{
    public string id;

    public string name;

    public string hostIp;

    public int currentPlayers;

    public int maxPlayers;

    public bool inGame;

    public LobbyPlayerData[] players;
}
