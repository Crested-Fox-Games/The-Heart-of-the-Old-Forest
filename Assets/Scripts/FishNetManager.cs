using FishNet;
using UnityEngine;

public class FishNetManager : MonoBehaviour
{
    public static FishNetManager Instance { get; private set; }

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

    public void StartHost()
    {
        Debug.Log("StartingHost");
        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    public void JoinGame(string ip)
    {
        Debug.Log($"Joining game on ip {ip}");
        InstanceFinder.ClientManager.StartConnection(ip);
    }
}
