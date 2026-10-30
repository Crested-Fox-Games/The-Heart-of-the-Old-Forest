using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevController : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset inputActions;

    private InputActionMap devMap;

    private InputActionMap devTimeMap;

    private InputActionMap devCinematicMap;

    private int currentDevMapIndex = 0;

    private List<InputActionMap> actionMapsList = new List<InputActionMap>();

    /// <summary>
    /// WARNING: This needs to be in the same order as the dev action maps otherwise it wont work properly
    /// </summary>
    [SerializeField]
    private List<GameObject> actionUiPanelList;

    [SerializeField]
    private GameObject devControlPanel;

    private InputAction OpenDevControls;

    private InputAction NextDevMap;

    private InputAction PreviousDevMap;

    /// <summary>
    /// Skips to night time, for testing night time mechanics
    /// </summary>
    private InputAction skipToNight;

    /// <summary>
    /// Goes to the end of the day night cycle and starts a new day
    /// </summary>
    private InputAction quickEndDay;

    /// <summary>
    /// Goes to the next camera position for the camera
    /// </summary>
    private InputAction nextCameraPosition;

    /// <summary>
    /// Goes to the previous camera position
    /// </summary>
    private InputAction previousCameraPosition;

    /// <summary>
    /// Starts the camera animation for the position
    /// </summary>
    private InputAction startCameraAnimation;

    private void Awake()
    {
        GetActionMaps();

        //Checks if the game is being played in editor or dev build
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

        devCinematicMap = inputActions.FindActionMap("DevCinematic");
        actionMapsList.Add(devCinematicMap);
    }

    /// <summary>
    /// This #if will only run if the game is being played in the editor or dev build
    /// </summary>
    #if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void InitializeDevControls()
    {
        devMap.Enable();

        OpenDevControls = devMap.FindAction("OpenDevControls");
        OpenDevControls.performed += ActivateDevControls;

        NextDevMap = devMap.FindAction("NextPage");
        NextDevMap.performed += NextPage;

        PreviousDevMap = devMap.FindAction("PreviousPage");
        PreviousDevMap.performed += PreviousPage;

        //Find actions for all the action maps(add more functions below)
        FindDevTimeMapActions();
        FindDevCinematicMapControls();
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

    private void NextPage(InputAction.CallbackContext context)
    {
        for(int i = 0; i < actionMapsList.Count; i++)
        {
            InputActionMap map = actionMapsList[i];

            if(map.enabled)
            {
                actionUiPanelList[i].gameObject.SetActive(false);
                map.Disable();

                if(i == actionMapsList.Count -1)
                {
                    actionMapsList[0].Enable();
                    actionUiPanelList[0].gameObject.SetActive(true);
                }
                else
                {
                    actionMapsList[i + 1].Enable();
                    actionUiPanelList[i + 1].gameObject.SetActive(true);
                }

                break;
            }
        }
    }

    private void PreviousPage(InputAction.CallbackContext context)
    {
        for (int i = 0; i < actionMapsList.Count; i++)
        {
            InputActionMap map = actionMapsList[i];

            if (map.enabled)
            {
                actionUiPanelList[i].gameObject.SetActive(false);
                map.Disable();

                if (i == 0)
                {
                    actionMapsList[actionMapsList.Count - 1].Enable();
                    actionUiPanelList[actionMapsList.Count - 1].gameObject.SetActive(true);
                }
                else
                {
                    actionMapsList[i - 1].Enable();
                    actionUiPanelList[i - 1].gameObject.SetActive(true);
                }

                break;
            }
        }
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

    private void FindDevCinematicMapControls()
    {

    }

    private void ActivateCinematicMapControls()
    {

    }

    private void DeactivateCinematicMapControls()
    {

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
