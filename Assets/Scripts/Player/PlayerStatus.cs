using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

/// <summary>
/// The stats of the player that can be upgraded
/// </summary>
public enum PlayerStats
{
    Health,
    MoveSpeed
}

public enum AbilityStats
{
    Damage,
    Cooldown,
    Range,
    CritChance,
    CritDamage
}   

public class PlayerStatus : NetworkBehaviour, ITargetable
{
    /// <summary>
    /// The starting health for the player before upgrades are applied
    /// </summary>
    [SerializeField]
    [Tooltip("The starting health for the player before upgrades are applied")]
    private float baseMaxHealth = 100f;

    /// <summary>
    /// The delay in seconds before the players health starts coming back
    /// </summary>
    [SerializeField]
    [Tooltip("The delay in seconds before the players health starts coming back")]
    private float healthRegenDelay = 5f;

    /// <summary>
    /// The amount of health the player gets back every second
    /// </summary>
    [SerializeField]
    [Tooltip("The amount of health the player gets back every second")]
    private float healthRegenAmount = 5f;

    private float currentMaxHealth;

    private float moveSpeed = 5f;

    private Coroutine healthRegenCoroutine;

    private readonly SyncVar<float> currentHealth = new();

    /// <summary>
    /// A dictionary that holds the values for the players additive upgrades
    /// </summary>
    private readonly SyncDictionary<PlayerStats, float> playerAdditiveUpgrades = new();

    /// <summary>
    /// A dictionary that holds the values for the players multiplicative upgrades
    /// </summary>
    private readonly SyncDictionary<PlayerStats, float> playerMultiplicativeUpgrades = new();

    public Transform TargetTransform => transform;

    public override void OnStartServer()
    {
        InitializeUpgradeDictionaries();

        //Initialises the health of the structure
        currentMaxHealth = baseMaxHealth;
        currentHealth.Value = currentMaxHealth;

        currentHealth.OnChange += UpdateHealthBar;

        playerAdditiveUpgrades.OnChange += OnUpgradesChanged;
        playerMultiplicativeUpgrades.OnChange += OnUpgradesChanged;
    }

    override public void OnStopServer()
    {
        currentHealth.OnChange -= UpdateHealthBar;
        playerAdditiveUpgrades.OnChange -= OnUpgradesChanged;
        playerMultiplicativeUpgrades.OnChange -= OnUpgradesChanged;
    }

    public bool IsAlive()
    {
        return currentHealth.Value > 0;
    }

    public bool IsAttackable()
    {
        //Can change later to add protection mechanics
        return IsAlive();
    }

    public bool TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return true;

        currentHealth.Value -= damage;


        //Debug.Log("Structure has taken damage");
        if (currentHealth.Value <= 0)
        {
            Destroyed();
            return false;
        }
        else
        {
            //If the regen coroutine is started we end it
            if(healthRegenCoroutine != null)
            {
                StopCoroutine(healthRegenCoroutine);
            }

            //Starts the health regen coroutine
            healthRegenCoroutine = StartCoroutine(HealthRegen());
        }

        return true;
    }

    [ObserversRpc]
    public void Destroyed()
    {
        Debug.Log($"{gameObject.name} has been destroyed");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Tells the ui manager that the health of the player has changed
    /// </summary>
    /// <param name="prev"></param>
    /// <param name="next"></param>
    /// <param name="asServer"></param>
    private void UpdateHealthBar(float prev, float next, bool asServer)
    {
        UiManager.Instance.UpdatePlayerHealthBar(currentHealth.Value, currentMaxHealth);
    }

    /// <summary>
    /// Initialize the values for the dictionaries
    /// </summary>
    private void InitializeUpgradeDictionaries()
    {
        playerAdditiveUpgrades[PlayerStats.Health] = 0f;
        playerAdditiveUpgrades[PlayerStats.MoveSpeed] = 0f;

        playerMultiplicativeUpgrades[PlayerStats.Health] = 1f;
        playerMultiplicativeUpgrades[PlayerStats.MoveSpeed] = 1f;
    }

    public void AddUpgrade(PlayerStats stat, UpgradeType upgradeType, float amount)
    {
        if (upgradeType == UpgradeType.Addition)
        {
            playerAdditiveUpgrades[stat] += amount;
        }
        else if (upgradeType == UpgradeType.Multiplacation)
        {
            playerMultiplicativeUpgrades[stat] += amount;
        }
    }

    private void OnUpgradesChanged(SyncDictionaryOperation op, PlayerStats key, float value, bool asServer)
    {
        // Handle the changes to the upgrade dictionaries here
        // For example, you can update the player's stats based on the new values

        switch(key)
        {
            case PlayerStats.Health:
                // Update current and max health based on the new value
                float newMaxHealth = GetHealth();

                float difference = newMaxHealth - currentMaxHealth;

                currentMaxHealth = newMaxHealth;
                currentHealth.Value += difference;
                break;
            case PlayerStats.MoveSpeed:
                // Update move speed based on the new value
                break; 
        }
    }

    private float GetHealth()
    {
        return (baseMaxHealth + playerAdditiveUpgrades[PlayerStats.Health]) * playerMultiplicativeUpgrades[PlayerStats.Health];
    }

    private float GetSpeed()
    {
        return (moveSpeed + playerAdditiveUpgrades[PlayerStats.MoveSpeed]) * playerMultiplicativeUpgrades[PlayerStats.MoveSpeed];
    }


    private IEnumerator HealthRegen()
    {
        //Waits for a delay after being hit to force the player to play safer to regen
        yield return new WaitForSeconds(healthRegenDelay);

        //Loops while health isnt at max
        while(currentHealth.Value < currentMaxHealth)
        {
            //Currently gives players a flat amount of hp back per tick
            //Also ensures we dont overflow the health regen over max health
            currentHealth.Value = Mathf.Min(currentMaxHealth, currentHealth.Value + healthRegenAmount);

            //The time between regen ticks
            yield return new WaitForSecondsRealtime(1f);
        }


    }
}
