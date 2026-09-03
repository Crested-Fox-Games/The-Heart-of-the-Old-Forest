using UnityEngine;

/// <summary>
/// This script just gives us references to the other player scripts. Used for Reward System
/// </summary>
public class PlayerRef : MonoBehaviour
{
    public PlayerAbilities playerAbilities {  get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerInteraction playerInteraction { get; private set; }
    public PlayerRPCHandler playerRPCHandler { get; private set; }
    public PlayerStatus playerStatus { get; private set; }

    private void Awake()
    {
        playerAbilities = GetComponent<PlayerAbilities>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();
        playerRPCHandler = GetComponent<PlayerRPCHandler>();
        playerStatus = GetComponent<PlayerStatus>();
    }
}
