using UnityEngine;
using UnityEngine.InputSystem;

public class DevController : MonoBehaviour
{
    [SerializeField] 
    private InputActionAsset inputactions;

    private InputActionMap devMap;

    /// <summary>
    /// NUMPAD 1 - Skips to night time, for testing night time mechanics
    /// </summary>
    private InputAction skipToNight;

    private void Awake()
    {
        devMap = inputactions.FindActionMap("Dev");

        //Checks if the game is being played in editor
        //NOTE: Might want to add a check for dev build so we can also test in build
        if(Application.isEditor)
        {
            devMap.Enable();

            skipToNight = devMap.FindAction("SkipToNight");
            skipToNight.performed += ctx => SkipToNight();
        }
    }

    /// <summary>
    /// This #if will only run if the game is being played in the editor
    /// </summary>
    #if UNITY_EDITOR
        [SerializeField]
        TimeManager timeManager;

        private void SkipToNight()
        {
            timeManager.SkipToNight();
        }
    #endif

}
