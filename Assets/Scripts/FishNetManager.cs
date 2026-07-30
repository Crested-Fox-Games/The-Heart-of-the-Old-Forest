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

    /// <summary>
    /// Starts up the fishnet server for the host
    /// </summary>
    public void StartHost()
    {
        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    /// <summary>
    /// Connects the client to the host via the ip address
    /// </summary>
    /// <param name="ip"></param>
    public void ConnectToHost(string ip)
    {
        InstanceFinder.ClientManager.StartConnection(ip);
    }

    /// <summary>
    /// Runs when the clients connection state is updated
    /// </summary>
    /// <param name="args"></param>
    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if(args.ConnectionState == LocalConnectionState.Started)
        {
            OnConnected?.Invoke();
        }
    }

    private void OnDestroy()
    {
        //Unsubscribes when the object is destroyed
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }
}
