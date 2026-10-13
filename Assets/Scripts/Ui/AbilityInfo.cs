using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInfo : MonoBehaviour
{
    /// <summary>
    /// The slot this ui is displaying
    /// </summary>
    [SerializeField]
    private AbilitySlot slotOfAbility;

    /// <summary>
    /// The icon of the ability
    /// </summary>
    [SerializeField]
    private Image abilityIcon;

    /// <summary>
    /// The text box for the name of the ability
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI abilityName;

    /// <summary>
    /// The text box for the description of the ability
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI abilityDesc;

    private void Start()
    {
        Initialize();

    }

    private void Initialize()
    {
        PlayerAbilities player = GetPlayerAbilities();

        if (player == null)
        {
            Debug.LogError("PlayerAbilities component not found on local player.");
            return;
        }

        Ability ability =  player.GetAbilityFromSlot(slotOfAbility);

        Debug.Log($"Initializing AbilityInfo for slot {slotOfAbility} with ability: {ability?.AbilitySO.AbilityName ?? "None"}");
        if (ability != null)
        {
            abilityIcon.sprite = ability.AbilitySO.AbilityIcon;
            abilityName.text = ability.AbilitySO.AbilityName;
            abilityDesc.text = ability.AbilitySO.AbilityDescription;
        }
        else
        {
            Debug.LogError($"No ability found in slot {slotOfAbility}.");
        }

    }

    private PlayerAbilities GetPlayerAbilities()
    {
        return GameManager.Instance.LocalPlayer.GetComponent<PlayerAbilities>();
    }
}
