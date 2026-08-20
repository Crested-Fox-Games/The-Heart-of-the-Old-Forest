using UnityEngine;
using UnityEngine.InputSystem;

public class DevController : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset inputActions;

    private InputActionMap devMap;

    /// <summary>
    /// NUMPAD 1 - Skips to night time, for testing night time mechanics
    /// </summary>
    private InputAction skipToNight;

    private void Awake()
    {
        devMap = inputActions.FindActionMap("Dev");

        //Checks if the game is being played in editor
        //NOTE: Might want to add a check for dev build so we can also test in build
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        devMap.Enable();

        skipToNight = devMap.FindAction("SkipToNight");
        skipToNight.performed += ctx => SkipToNight();
    #endif
    }

    /// <summary>
    /// This #if will only run if the game is being played in the editor
    /// </summary>
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField]
        TimeCycleManager timeManager;

        private void SkipToNight()
        {
            timeManager.SkipToNight();
        }
    #endif

}
