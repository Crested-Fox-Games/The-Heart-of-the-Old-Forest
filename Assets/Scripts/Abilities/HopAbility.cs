using System.Collections;
using UnityEngine;

public class HopAbility : Ability
{
    private Coroutine passiveCoroutine;

    private float currentPassiveTimer = 0f;

    private float passiveActiveTime = 3f;

    private float speedMult = 0.5f;

    public override void PassiveAbilitySetup()
    {
        //Subscribes the passive to its trigger
        owner.GetComponent<PlayerMovement>().onPlayerJump += PassiveTriggered;
    }

    protected override void PassiveTriggered()
    {
        //Resets the current passive time
        currentPassiveTimer = 0f;

        if(passiveCoroutine == null)
        {
            passiveCoroutine = StartCoroutine(PassiveEffect(owner.GetComponent<PlayerMovement>()));
        }
    }

    /// <summary>
    /// The Ienumerator that handles the effect of the passive
    /// </summary>
    /// <param name="playerMovement"></param>
    /// <returns></returns>
    private IEnumerator PassiveEffect(PlayerMovement playerMovement)
    {
        //Increase the player speed
        playerMovement.SetMovementSpeedMultiplier(playerMovement.MovementSpeedMultiplier +  speedMult);

        //Loops through while the timer is below the active time
        while (currentPassiveTimer < passiveActiveTime)
        {
            currentPassiveTimer += Time.deltaTime;

            yield return null;
        }

        //Decrease the player speed
        playerMovement.SetMovementSpeedMultiplier(playerMovement.MovementSpeedMultiplier - speedMult);
    }
}
