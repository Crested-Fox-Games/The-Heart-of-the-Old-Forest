using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilityUiElement : MonoBehaviour
{
    /// <summary>
    /// Text that shows what hotkey the button is linked to
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI abilityHotkeyText;

    /// <summary>
    /// The icon for the ability
    /// </summary>
    [SerializeField]
    private Image abilityIcon;

    /// <summary>
    /// The countdown text for the cooldown
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI cooldownText;

    /// <summary>
    /// The overlay that shows the cooldown visually
    /// </summary>
    [SerializeField]
    private Image cooldownOverlay;

    /// <summary>
    /// The slot this ui element is linked to
    /// </summary>
    [SerializeField]
    private AbilitySlot abilitySlot;

    /// <summary>
    /// The player this is linked to
    /// </summary>
    private PlayerAbilities owner;

    /// <summary>
    /// The ability this is linked to
    /// </summary>
    private Ability ability;

    /// <summary>
    /// The SO for the ability this is linked to
    /// </summary>
    private AbilitySO abilitySO;


    private void OnEnable()
    {
        if(owner == null)
            StartCoroutine(GetPlayer());
    }

    /// <summary>
    /// Runs until it finds the camera which is only on the local player
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetPlayer()
    {
        while(owner == null)
        {
            Camera playerCam = FindFirstObjectByType<Camera>();

            if(playerCam != null)
            {
                owner = playerCam.GetComponentInParent<PlayerAbilities>();
            }

            yield return new WaitForSeconds(0.2f);
        }

        Initialize();
    }

    /// <summary>
    /// Sets up all the initial values
    /// </summary>
    private void Initialize()
    {
        ability = owner.GetAbilityFromSlot(abilitySlot);
        abilitySO = ability.AbilitySO;

        //NOTE: Might need a way to update this at runtime if we add in key rebinds
        //Sets the hotkey
        InputAction action = owner.GetComponent<PlayerInput>().GetHotkeyFromSlot(abilitySlot);

        abilityHotkeyText.text = GetKeybindString(action);

        //Gets the sprite if we have one
        //Might need better validation for this
        if(owner.GetAbilityFromSlot(abilitySlot).AbilitySO.AbilityIcon != null)
        {
            abilityIcon.sprite = abilitySO.AbilityIcon;
        }

        //Subscribes the cooldown event to the correct slot
        var cooldown = owner.GetCooldownSyncVarFromSlot(abilitySlot);
        cooldown.OnChange += UpdateCooldown;
    }

    /// <summary>
    /// Updates the cooldown visuals
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="cooldownRemaining"></param>
    /// <param name="asServer"></param>
    private void UpdateCooldown(float previous, float cooldownRemaining, bool asServer)
    {
        if(ability.HasActive() && ability.ActiveRemaining() > 0)
        {
            cooldownText.gameObject.SetActive(true);

            cooldownText.text = ability.ActiveRemaining().ToString("F1");
            cooldownOverlay.fillAmount = 1f - ability.ActiveRemaining() / ability.ActiveTime();

            return;
        }

        if(cooldownRemaining > 0) 
        {
            cooldownText.gameObject.SetActive(true);
        }
        else
        {
            cooldownText.gameObject.SetActive(false);
        }

        cooldownText.text = (cooldownRemaining).ToString("F1");

        cooldownOverlay.fillAmount = cooldownRemaining / abilitySO.Cooldown;
    }

    /// <summary>
    /// Gives us custom key bindings if needed
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private string GetKeybindString(InputAction action)
    {
        string currentBindingGroup = owner.GetComponent<PlayerInteraction>().GetCurrentBindingGroup();

        //TODO: This doesnt account for controllers yet, need to figure that out
        foreach(InputBinding binding in action.bindings)
        {
            //Check to ensure its in the correct input bindings
            if (!binding.groups.Contains(currentBindingGroup))
                continue;

            //Set custom text in if statements here
            if (binding.effectivePath == "<Mouse>/leftButton")
                return "LMB";
        }

        //Returns the base binding string if none of the checks above are true
        return action.GetBindingDisplayString(group: currentBindingGroup);
    }
}