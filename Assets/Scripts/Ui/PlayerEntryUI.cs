using TMPro;
using UnityEngine;

public class PlayerEntryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameText, hostText, readyText;

    public void Setup(LobbyPlayerData player)
    {
        playerNameText.text = player.name;

        hostText.text = player.isHost ? "Host" : "";

        readyText.text = player.isReady ? "Ready" : "Not Ready";
    }
}
