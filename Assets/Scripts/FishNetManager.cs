using FishNet;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using System;
using UnityEngine;

public class FishNetManager : MonoBehaviour
{
    public static FishNetManager Instance { get; private set; }

    public event Action OnConnected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    public void StartHost()
    {
        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    public void ConnectToHost(string ip)
    {
        InstanceFinder.ClientManager.StartConnection(ip);
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if(args.ConnectionState == LocalConnectionState.Started)
        {
            OnConnected?.Invoke();
        }
    }

    private void OnDestroy()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }
}
