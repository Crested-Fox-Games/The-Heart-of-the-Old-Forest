using System;

/// <summary>
/// Stores a list of all the lobbies the server has
/// </summary>
[Serializable]
public class LobbyListResponse
{
    public LobbyData[] lobbies;
}
