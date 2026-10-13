using TMPro;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour
{
    /// <summary>
    /// The text box that houses the stats
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI statsText;

    private void OnEnable()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        PlayerRef player = GameManager.Instance.LocalPlayer;


        string playerStatsString = $"Player Stats \n" +
            $"Health: {player.playerStatus.GetHealth()} \n" +
            $"Speed: {player.playerStatus.GetSpeed()} \n" +
            $"Regen: 5% \n\n" +
            $"{player.playerAbilities.GetAbilityFromSlot(AbilitySlot.BasicAttack).GetStatString()} \n\n" +
            $"{player.playerAbilities.GetAbilityFromSlot(AbilitySlot.MovementAbility).GetStatString()}\n\n" +
            $"{player.playerAbilities.GetAbilityFromSlot(AbilitySlot.SpecialAbility).GetStatString()} \n\n" +
            $"{player.playerAbilities.GetAbilityFromSlot(AbilitySlot.UltimateAbility).GetStatString()} ";

        statsText.text = playerStatsString;
    }
}
