using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    private bool ready = false;

    public void Toggle()
    {
        ready = !ready;
        LobbyManager.Instance.SetReady(ready);
    }
}
