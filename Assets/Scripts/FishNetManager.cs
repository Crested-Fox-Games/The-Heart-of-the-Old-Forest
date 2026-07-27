using FishNet;
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

        OnConnected?.Invoke();
    }
}
