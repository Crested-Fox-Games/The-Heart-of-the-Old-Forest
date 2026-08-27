using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class StructureHealth : NetworkBehaviour, ITargetable, IRepairable, IInteractable
{
    //Max and current health of the structure
    [SerializeField] private float maxHealth = 100f;
    private readonly SyncVar<float> currentHealth = new();

    //References
    [SerializeField] private GameObject childObject;
    public Transform TargetTransform => transform;

    //Player interaction
    [SerializeField] private float interactTime = 3f;
    public float InteractTime => interactTime;

    public override void OnStartServer()
    {
        //Initialises the health of the structure
        currentHealth.Value = maxHealth;
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

    /// <summary>
    /// Handles taking damage from sources such as enemies
    /// </summary>
    /// <param name="damage"></param>
    public bool TakeDamage(float damage)
    {
        if (!IsServerStarted)
            return true;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Destroyed();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Disable visual of structure
    /// </summary>
    [ObserversRpc]
    private void Destroyed()
    {
        Debug.Log($"{gameObject.name} has been destroyed");
        childObject.SetActive(false);

        //Create repair collider for player to interact with
        CreateCollider();
    }

    /// <summary>
    /// Checks if structure is destroyed
    /// </summary>
    /// <returns></returns>
    public bool CanRepair()
    {
        return currentHealth.Value <= 0;
    }

    /// <summary>
    /// Enable wall when interacted with by player
    /// </summary>
    public void Rebuild()
    {
        if (!IsServerStarted)
            return;

        if (!CanRepair())
            return;

        currentHealth.Value = maxHealth;

        StructureEnable();
    }

    /// <summary>
    /// Activates wall visual
    /// </summary>
    [ObserversRpc]
    private void StructureEnable()
    {
        //delete the interactable collider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        DestroyCollider(boxCollider);

        childObject.SetActive(true);
    }

    /// <summary>
    /// Calls to rebuild on player interaction
    /// </summary>
    /// <param name="player"></param>
    public void Interact(NetworkObject player)
    {
        Rebuild();
    }

    /// <summary>
    /// Checks if player can interact with repairable structure
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool CanInteract(NetworkObject player)
    {
        return CanRepair();
    }

    /// <summary>
    /// Builds a collider for the structure after it is destroyed by enemies, for player interaction
    /// </summary>
    private void CreateCollider()
    {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        Transform child = childObject.GetComponent<Transform>();

        boxCollider.center = child.localPosition;
        boxCollider.size = child.localScale;
        boxCollider.isTrigger = false;
    }

    /// <summary>
    /// Destroy the player interactable collider
    /// </summary>
    /// <param name="boxCollider"></param>
    private void DestroyCollider(BoxCollider boxCollider)
    {
        Destroy(boxCollider);
    }
}
