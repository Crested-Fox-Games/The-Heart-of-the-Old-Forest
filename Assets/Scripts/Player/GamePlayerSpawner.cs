using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
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
            Debug.Log("Starting game from in scene");
            StartCoroutine(InSceneStartup());
            return;
        }
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
}
