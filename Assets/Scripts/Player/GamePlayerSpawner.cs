using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class GamePlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private PlayerRPCHandler playerPrefab;

    private void Start()
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        int spawnIndex = 0;
        Debug.Log($"Clients connected: {InstanceFinder.ServerManager.Clients.Count}");
        foreach (NetworkConnection connection in InstanceFinder.ServerManager.Clients.Values)
        {
            Debug.Log($"Spawning player for connection: {connection.ClientId}");
            Transform spawnpoint = spawnPoints[spawnIndex % spawnPoints.Length];

            NetworkObject player = Instantiate(playerPrefab, spawnpoint.position, spawnpoint.rotation);

            InstanceFinder.ServerManager.Spawn(player, connection);

            spawnIndex++;
        }
    }
}
