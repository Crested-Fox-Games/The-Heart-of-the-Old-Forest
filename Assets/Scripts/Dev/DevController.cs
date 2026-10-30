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

    [SerializeField]
    private CinematicController cinematicController;

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

    private void NextPage(InputAction.CallbackContext context)
    {
        for (int i = 0; i < actionMapsList.Count; i++)
        {
            InputActionMap map = actionMapsList[i];

            if (map.enabled)
            {
                actionUiPanelList[i].gameObject.SetActive(false);
                DeactivateMap(i);

                if (i == actionMapsList.Count - 1)
                {
                    ActivateMap(0);
                    actionUiPanelList[0].gameObject.SetActive(true);
                }
                else
                {
                    ActivateMap(i + 1);
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
                DeactivateMap(i);

                if (i == 0)
                {
                    ActivateMap(actionMapsList.Count - 1);
                    actionUiPanelList[actionMapsList.Count - 1].gameObject.SetActive(true);
                }
                else
                {
                    ActivateMap(i - 1);
                    actionUiPanelList[i - 1].gameObject.SetActive(true);
                }

                break;
            }
        }
    }

    private void ActivateMap(int index)
    {
        switch(index)
        {
            case 0: ActivateDevTimeMapControls(); break;
            case 1: ActivateCinematicMapControls(); break;
        }
    }

    private void DeactivateMap(int index)
    {
        switch (index)
        {
            case 0: DeactivateDevTimeMapControls(); break;
            case 1: DeactivateCinematicMapControls(); break;
        }
    }

    #region Time Map Functions
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

    #endregion

    #region Cinematic Map Functions
    private void FindDevCinematicMapControls()
    {
        nextCameraPosition = devCinematicMap.FindAction("NextPos");
        previousCameraPosition = devCinematicMap.FindAction("PrevPos");
        startCameraAnimation = devCinematicMap.FindAction("StartAnim");
    }

    private void ActivateCinematicMapControls()
    {
        cinematicController.gameObject.SetActive(true);
        devCinematicMap.Enable();

        nextCameraPosition.performed += NextCameraPosition;
        previousCameraPosition.performed += PreviousCameraPosition;
        startCameraAnimation.performed += StartCinematicAnimation;
    }

    private void DeactivateCinematicMapControls()
    {
        devCinematicMap.Disable();

        nextCameraPosition.performed -= NextCameraPosition;
        previousCameraPosition.performed -= PreviousCameraPosition;
        startCameraAnimation.performed -= StartCinematicAnimation;
    }

    

    private void NextCameraPosition(InputAction.CallbackContext context)
    {
        //Debug.Log("Trying to go next camera pos");
        cinematicController.NextCameraPosition();
    }

    private void PreviousCameraPosition(InputAction.CallbackContext context)
    {
        cinematicController.PreviousCameraPosition();
    }

    private void StartCinematicAnimation(InputAction.CallbackContext context)
    {
        cinematicController.ActivatePositionAnimation();
    }
    #endregion

#endif

}
