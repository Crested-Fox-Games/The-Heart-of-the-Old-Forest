using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    #region Sound Settings
    /// <summary>
    /// The audio mixer that controls volumes
    /// </summary>
    [Header("Audio Settings")]
    [SerializeField]
    private AudioMixer audioMixer;
    
    /// <summary>
    /// The value that is shown on the slider
    /// </summary>
    private float masterVolumeSliderVal = 50;
    private float musicVolumeSliderVal = 50;
    private float sfxVolumeSliderVal = 50;
    private float uiVolumeSliderVal = 50;

    /// <summary>
    /// The decibels for the mixer group
    /// </summary>
    private float masterVolumeDb;
    private float musicVolumeDb;
    private float sfxVolumeDb;
    private float uiVolumeDb;

    #endregion



    private void Start()
    {
        InitializeSettings();
    }

    /// <summary>
    /// Initializes all the settings
    /// </summary>
    private void InitializeSettings()
    {
        // --- Sound Initialization ---

        //Sets the master volume slider val based on the player pref, if the player pref doesnt exist yet, it sets it to 50
        masterVolumeSliderVal = PlayerPrefs.GetFloat("MasterVolume", 50);
        //Calls the function for setting the master volume in the audio mixer
        SetMasterVolumeDecibels(masterVolumeSliderVal);

        musicVolumeSliderVal = PlayerPrefs.GetFloat("MusicVolume", 50);
        SetMusicVolumeDecibels(musicVolumeSliderVal);

        sfxVolumeSliderVal = PlayerPrefs.GetFloat("SFXVolume", 50);
        SetSFXVolumeDecibels(sfxVolumeSliderVal);

        uiVolumeSliderVal = PlayerPrefs.GetFloat("UIVolume", 50);
        SetUIVolumeDecibels(uiVolumeSliderVal);
    }

    /// <summary>
    /// Sets the volume for the Master mixer group in the audio mixer
    /// </summary>
    /// <param name="sliderValue"></param>
    private void SetMasterVolumeDecibels(float sliderValue)
    {
        masterVolumeDb = ConvertSliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("Master", masterVolumeDb);
    }

    /// <summary>
    /// Sets the volume for the Music mixer group in the audio mixer
    /// </summary>
    /// <param name="sliderValue"></param>
    private void SetMusicVolumeDecibels(float sliderValue)
    {
        musicVolumeDb = ConvertSliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("Music", musicVolumeDb);
    }

    /// <summary>
    /// Sets the volume for the SFX mixer group in the audio mixer
    /// </summary>
    /// <param name="sliderValue"></param>
    private void SetSFXVolumeDecibels(float sliderValue)
    {
        sfxVolumeDb = ConvertSliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("SFX", sfxVolumeDb);
    }

    /// <summary>
    /// Sets the volume for the UI mixer group in the audio mixer
    /// </summary>
    /// <param name="sliderValue"></param>
    private void SetUIVolumeDecibels(float sliderValue)
    {
        uiVolumeDb = ConvertSliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("UI", uiVolumeDb);
    }

    private float ConvertSliderValueToDecibels(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0f, 100f); 

        if(sliderValue <= 0f)
        {
            return -80f; // Minimum decibel value
        }

        return Mathf.Log10(sliderValue / 100) * 20;
    }
}
