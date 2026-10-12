using FishNet.Object;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerRef localPlayer;

    /// <summary>
    /// The event that triggers game over across subscribed systems
    /// </summary>
    public event Action OnGameOver;

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

    private void Start()
    {
        List<PlayerRef> players = FindObjectsByType<PlayerRef>(FindObjectsSortMode.None).ToList();

        //Finds the local player and sets it to the localPlayer variable
        foreach (PlayerRef player in players)
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                localPlayer = player;
                break;
            }
        }
    }


    /// <summary>
    /// The function that invokes the game over event, anything we decide that happens upon game ending 
    /// will subscribe to the event so that it happens when this is called
    /// </summary>
    public void GameOver()
    {
        //TODO: Decide if players and towers are killed or what happens to them

        OnGameOver?.Invoke();

        //Disables the player map, stopping all gameplay inputs and activating ui inputs
        InputSystem.actions.FindActionMap("Player").Disable();

        //Enable the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
