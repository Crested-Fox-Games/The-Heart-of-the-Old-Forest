using FishNet.Object;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public interface ITargetable
{
   //priority
   //is it alive
   //is it attackable
   void SetPriority(float priority);

    bool IsAlive();

    bool IsAttackable();

    void TakeDamage(float damage);

}
