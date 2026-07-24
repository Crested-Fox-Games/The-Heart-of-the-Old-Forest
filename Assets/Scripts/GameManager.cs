using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    /// <summary>
    /// The function that invokes the game over event, anything we decide that happens upon game ending 
    /// will subscribe to the event so that it happens when this is called
    /// </summary>
    public void GameOver()
    {
        //TODO: Decide if players and towers are killed or what happens to them

        //TODO: Decide if this takes the players back to the main menu or to a lobby with the current players

        OnGameOver?.Invoke();

        //Disables the player map, stopping all gameplay inputs and activating ui inputs
        InputSystem.actions.FindActionMap("Player").Disable();

        //Enable the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
