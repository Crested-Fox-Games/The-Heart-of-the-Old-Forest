using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUi : MonoBehaviour
{
    //The sliders for the different volume settings
    [Header("Volume Sliders")]
    [SerializeField]
    private Slider masterVolSlider;

    [SerializeField]
    private Slider musicVolSlider;

    [SerializeField]
    private Slider sfxVolSlider;

    [SerializeField]
    private Slider uiVolSlider;

    [Header("Volume Texts")]
    [SerializeField]
    private TextMeshProUGUI masterVolText;

    [SerializeField]
    private TextMeshProUGUI musicVolText;

    [SerializeField]
    private TextMeshProUGUI sfxVolText;

    [SerializeField]
    private TextMeshProUGUI uiVolText;

    private void OnEnable()
    {
        //Sets the sliders to the values in the settings manager when the settings ui is enabled
        masterVolSlider.value = SettingsManager.Instance.MasterVolumeSliderVal;
        musicVolSlider.value = SettingsManager.Instance.MusicVolumeSliderVal;
        sfxVolSlider.value = SettingsManager.Instance.SFXVolumeSliderVal;
        uiVolSlider.value = SettingsManager.Instance.UIVolumeSliderVal;

        //Sets the text to the values in the settings manager when the settings ui is enabled
        masterVolText.text = SettingsManager.Instance.MasterVolumeSliderVal.ToString("0");
        musicVolText.text = SettingsManager.Instance.MusicVolumeSliderVal.ToString("0");
        sfxVolText.text = SettingsManager.Instance.SFXVolumeSliderVal.ToString("0");
        uiVolText.text = SettingsManager.Instance.UIVolumeSliderVal.ToString("0");
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnMasterVolumeChanged()
    {
        float value = masterVolSlider.value;
        SettingsManager.Instance.MasterVolumeSliderVal = value;
        masterVolText.text = value.ToString("0");
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnMusicVolumeChanged()
    {
        float value = musicVolSlider.value;
        SettingsManager.Instance.MusicVolumeSliderVal = value;
        musicVolText.text = value.ToString("0");
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnSFXVolumeChanged()
    {
        float value = sfxVolSlider.value;
        SettingsManager.Instance.SFXVolumeSliderVal = value;
        sfxVolText.text = value.ToString("0");
    }

    //Updates the values in the settings manager when the sliders are changed
    public void OnUIVolumeChanged()
    {
        float value = uiVolSlider.value;
        SettingsManager.Instance.UIVolumeSliderVal = value;
        uiVolText.text = value.ToString("0");
    }
}
