using UnityEngine;
using FishNet.Object;

public class NewNetworkBehaviourTemplate : NetworkBehaviour
{
    private WeaponHitbox weaponHitbox;

    public void CallStartAttack()
    {
        weaponHitbox = GetComponent<WeaponHitbox>();

        weaponHitbox.StartAttack();
    }

    public void CallEndAttack()
    {
        weaponHitbox = GetComponent<WeaponHitbox>();

        weaponHitbox.EndAttack();
    }
}
