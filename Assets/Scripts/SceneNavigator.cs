using FishNet;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public static SceneNavigator Instance { get; private set; }

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

    public void OpenMainMenuFromGameplay()
    {
        GamePlayerSpawner.Instance.DespawnPlayers();

        InstanceFinder.ServerManager.StopConnection(true);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void StartSinglePlayer()
    {
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;

        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if(args.ConnectionState != LocalConnectionState.Started)
        {
            return;
        }

        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;

        SceneLoadData sld = new SceneLoadData("Gameplay");

        sld.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    //Need to figure out how to do fishnet scene stuff here for the main menu
}
