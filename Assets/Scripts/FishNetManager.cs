using FishNet;
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
        Debug.Log("StartingHost");
        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    public void ConnectToHost(string ip)
    {
        Debug.Log($"Joining game on ip {ip}");

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
