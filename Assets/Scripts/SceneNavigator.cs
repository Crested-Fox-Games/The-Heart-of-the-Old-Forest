using FishNet;
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

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("Gameplay");

        InstanceFinder.ServerManager.StartConnection();

        InstanceFinder.ClientManager.StartConnection();
    }

    //Need to figure out how to do fishnet scene stuff here for the main menu
}
