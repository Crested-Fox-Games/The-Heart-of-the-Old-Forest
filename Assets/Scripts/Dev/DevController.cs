using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevController : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset inputActions;

    private InputActionMap devMap;

    private InputActionMap devTimeMap;

    private int currentDevMapIndex = 0;

    private List<InputActionMap> actionMapsList = new List<InputActionMap>();

    [SerializeField]
    private GameObject devControlPanel;

    private InputAction OpenDevControls;

    /// <summary>
    /// Skips to night time, for testing night time mechanics
    /// </summary>
    private InputAction skipToNight;

    /// <summary>
    /// Goes to the end of the day night cycle and starts a new day
    /// </summary>
    private InputAction quickEndDay;

    private void Awake()
    {
        GetActionMaps();

        //Checks if the game is being played in editor
        //NOTE: Might want to add a check for dev build so we can also test in build
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        InitializeDevControls();
    #endif
    }

    private void GetActionMaps()
    {
        devMap = inputActions.FindActionMap("Dev");

        //Controls for time
        devTimeMap = inputActions.FindActionMap("DevTime");
        actionMapsList.Add(devTimeMap);
    }

    /// <summary>
    /// This #if will only run if the game is being played in the editor
    /// </summary>
    #if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void InitializeDevControls()
    {
        devMap.Enable();

        OpenDevControls = devMap.FindAction("OpenDevControls");
        OpenDevControls.performed += ActivateDevControls;

        //Find actions for all the action maps(add more functions below)
        FindDevTimeMapActions();
    }

    private void ActivateDevControls(InputAction.CallbackContext context)
    {
        devControlPanel.SetActive(true);

        //Activate default map
        ActivateDevTimeMapControls();

        //TODO: we might want to have it unsubcribe from this and subscribe to a deactivate later, but might not be worth it
        OpenDevControls.performed -= ActivateDevControls;
    }

    private void FindDevTimeMapActions()
    {
        skipToNight = devTimeMap.FindAction("SkipToNight");
        quickEndDay = devTimeMap.FindAction("QuickEndDay");
    }

    private void ActivateDevTimeMapControls()
    {
        devTimeMap.Enable();

        skipToNight.performed += SkipToNight;
        quickEndDay.performed += QuickEndDay;
    }

    /// <summary>
    /// This will be added once we have a reason to use other maps
    /// </summary>
    private void DeactivateDevTimeMapControls()
    {
        devTimeMap.Disable();

        skipToNight.performed -= SkipToNight;
        quickEndDay.performed -= QuickEndDay;
    }

    

    private void SkipToNight(InputAction.CallbackContext context)
    {
        TimeCycleManager.Instance.SkipToNight();
    }

    private void QuickEndDay(InputAction.CallbackContext context)
    {
        TimeCycleManager.Instance.QuickEndDay();
    }
    #endif

}
