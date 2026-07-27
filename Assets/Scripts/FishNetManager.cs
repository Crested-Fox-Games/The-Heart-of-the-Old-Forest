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
        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    public void JoinGame(string ip)
    {
        InstanceFinder.ClientManager.StartConnection(ip);
    }
}
