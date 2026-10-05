using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is custom handling of Ui buttons used to make it easier to have click sounds that are run whenever any button is clicked
/// </summary>
public class SFXButton : Button
{
    protected override void OnEnable()
    {
        base.OnEnable();

        onClick.AddListener(PlayClickSound);
    }

    protected override void OnDisable()
    {
        onClick.RemoveListener(PlayClickSound);

        base.OnDisable();
    }

    /// <summary>
    /// Plays the click sound whenever a ui button is clicked
    /// </summary>
    private void PlayClickSound()
    {
        AudioManager.Instance.PlayUIAudioClip(AudioManager.Instance.UiButtonClickSound, Vector3.zero);
    }

}
