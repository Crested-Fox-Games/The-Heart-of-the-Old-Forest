using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class StructureHealth : NetworkBehaviour, ITargetable, IRepairable, IInteractable
{
    //Max and current health of the structure
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private GameObject childObject;

    private readonly SyncVar<float> currentHealth = new();

    public Transform TargetTransform => transform;

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
        //Debug.Log("Structure has taken damage");
        if (currentHealth.Value <= 0)
        {
            Destroyed();
            return false;
        }
        return true;
    }

    [ObserversRpc]
    private void Destroyed()
    {
        Debug.Log($"{gameObject.name} has been destroyed");
        childObject.SetActive(false);
        CreateCollider();
    }

    public bool CanRepair()
    {
        return currentHealth.Value <= 0;
    }

    public void Rebuild()
    {
        if (!CanRepair())
            return;

        if (!IsServerStarted)
            return;

        currentHealth.Value = maxHealth;

        Rebuilt();
    }

    [ObserversRpc]
    private void Rebuilt()
    {
        childObject.SetActive(true);
    }

    public void Interact(NetworkObject player)
    {
        Rebuild();
    }

    public bool CanInteract(NetworkObject player)
    {
        return CanRepair();
    }

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
        boxCollider.isTrigger = true;
    }
}
