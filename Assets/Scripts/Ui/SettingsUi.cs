using UnityEngine;
using UnityEngine.UI;

public class SettingsUi : MonoBehaviour
{
    //The sliders for the different volume settings
    [SerializeField]
    private Slider masterVolSlider;

    [SerializeField]
    private Slider musicVolSlider;

    [SerializeField]
    private Slider sfxVolSlider;

    [SerializeField]
    private Slider uiVolSlider;

    private void OnEnable()
    {
        //Sets the sliders to the values in the settings manager when the settings ui is enabled
        masterVolSlider.value = SettingsManager.Instance.MasterVolumeSliderVal;
        musicVolSlider.value = SettingsManager.Instance.MusicVolumeSliderVal;
        sfxVolSlider.value = SettingsManager.Instance.SFXVolumeSliderVal;
        uiVolSlider.value = SettingsManager.Instance.UIVolumeSliderVal;
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance.MasterVolumeSliderVal = value;
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.MusicVolumeSliderVal = value;
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SFXVolumeSliderVal = value;
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnUIVolumeChanged(float value)
    {
        SettingsManager.Instance.UIVolumeSliderVal = value;
    }
}
