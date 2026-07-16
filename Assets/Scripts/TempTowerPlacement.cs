using FishNet.Object;
using UnityEngine;

public class TempTowerPlacement : MonoBehaviour, IInteractable
{


    public bool CanInteract(NetworkObject player)
    {
        throw new System.NotImplementedException();

        //Check if tower placement UI is already open, also check if player is close enough
    }

    public void Interact(NetworkObject player)
    {
        throw new System.NotImplementedException();

        //TODO: Bring up the tower placement UI
    }
}
