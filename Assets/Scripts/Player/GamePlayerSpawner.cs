using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePlayerSpawner : MonoBehaviour
{
    public static GamePlayerSpawner Instance {  get; private set; }

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private NetworkObject playerPrefab;

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
    }

    private void Start()
    {
    #if UNITY_EDITOR
        if(!InstanceFinder.IsServerStarted && !InstanceFinder.IsClientStarted)
        {
            StartCoroutine(InSceneStartup());
            return;
        }
    #endif

        if (!InstanceFinder.IsServerStarted)
            return;

        RegisterConnections();
    }

    private IEnumerator InSceneStartup()
    {
        yield return InstanceFinder.ServerManager.StartConnection();

        while (!InstanceFinder.IsServerStarted)
            yield return null;

        yield return InstanceFinder.ClientManager.StartConnection();

        while (!InstanceFinder.IsClientStarted)
            yield return null;

        InstanceFinder.SceneManager.LoadGlobalScenes(new FishNet.Managing.Scened.SceneLoadData("Gameplay"));

        yield return null;


        RegisterConnections();
    }

    private void RegisterConnections()
    {
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            RegisterConnection(conn);
        }
    }

    private void RegisterConnection(NetworkConnection conn)
    {
        if(conn.LoadedStartScenes())
        {
            SpawnPlayer(conn);
            return;
        }

        conn.OnLoadedStartScenes -= OnConnectionLoadedStartScenes;
        conn.OnLoadedStartScenes += OnConnectionLoadedStartScenes;
    }


    private void OnConnectionLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (!asServer)
            return;

        conn.OnLoadedStartScenes -= OnConnectionLoadedStartScenes;

        SpawnPlayer(conn);

    }

    private void SpawnPlayer(NetworkConnection conn)
    {
        int spawnIndex = conn.ClientId % spawnPoints.Length;

        Transform spawnPoint = spawnPoints[spawnIndex];

        NetworkObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        InstanceFinder.ServerManager.Spawn(player, conn, gameObject.scene);
    }

    public void DespawnPlayers()
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        List<PlayerRef> players = FindObjectsByType<PlayerRef>(FindObjectsSortMode.None).ToList();

        foreach (PlayerRef player in players)
        {
            player.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnDestroy()
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            conn.OnLoadedStartScenes -= OnConnectionLoadedStartScenes;
        }
    }
}
