using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;
using FishNet.Object;
using System;
using System.Collections;
using UnityEngine;

public class GamePlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private NetworkObject playerPrefab;

    private void Start()
    {
    #if UNITY_EDITOR
        StartCoroutine(InSceneStartup());

    #endif

        if (!InstanceFinder.IsServerStarted)
            return;

        SpawnPlayers();
    }

    private IEnumerator InSceneStartup()
    {
        if (!InstanceFinder.IsServerStarted)
        {
            yield return InstanceFinder.ServerManager.StartConnection();

            yield return InstanceFinder.ClientManager.StartConnection();

            InstanceFinder.SceneManager.LoadGlobalScenes(new SceneLoadData("Gameplay"));
        }

        while(!InstanceFinder.IsServerStarted)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        SpawnPlayers();
    }



    private void SpawnPlayers()
    {
        Debug.Log("spawning player");
        int spawnIndex = 0;

        foreach (NetworkConnection connection in InstanceFinder.ServerManager.Clients.Values)
        {
            Transform spawnpoint = spawnPoints[spawnIndex % spawnPoints.Length];

            NetworkObject player = Instantiate(playerPrefab, spawnpoint.position, spawnpoint.rotation);

            InstanceFinder.ServerManager.Spawn(player, connection, gameObject.scene);

            spawnIndex++;
        }
    }
}
