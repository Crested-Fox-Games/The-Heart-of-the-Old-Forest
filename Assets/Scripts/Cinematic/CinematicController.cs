using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicController : MonoBehaviour
{

    private bool cameraLocked = true;

    private PlayerRef player;

    private Camera mainCam;

    private Vector3 playerCamLocalPos;
    private Quaternion playerCamLocalRot;

    private Coroutine cameraZoomCoroutine;

    //Inputs
    [SerializeField]
    private InputActionAsset inputActions;

    private InputActionMap cinematicMovementMap;

    [Header("Zoom controls")]
    [SerializeField]
    private float zoomAmount = 5f;
    [SerializeField]
    private float zoomTime = 2f;

    [Header("Slide controls")]
    [SerializeField]
    private float slideAmount = 5f;
    [SerializeField]
    private float slideTime = 2f;


    private void Start()
    {
        Initialize();

        cinematicMovementMap = inputActions.FindActionMap("CinematicMovement");

        cinematicMovementMap.Disable();
    }

    private void Initialize()
    {
        player = GameManager.Instance.LocalPlayer;
        mainCam = player.GetComponentInChildren<Camera>();

        playerCamLocalPos = mainCam.transform.localPosition;
        playerCamLocalRot = mainCam.transform.localRotation;
    }

    public void ChangeCameraMode()
    {
        Debug.Log($"Trying to move camera {mainCam.name} to next position");
        if (cameraLocked)
        {
            cameraLocked = false;
            EnableCinematicMode();

            //Make the position the first in the list
            mainCam.transform.parent = gameObject.transform;
        }
        else if (!cameraLocked)
        {
            cameraLocked = true;
            DisableCinematicMode();

            //Make the position the player
            mainCam.transform.parent = player.playerStatus.CameraPivot;
            mainCam.transform.localPosition = playerCamLocalPos;
            mainCam.transform.localRotation = playerCamLocalRot;
        }
    }

    public void ActivateCameraZoomIn()
    {
        //Disable dev ui

        if (cameraZoomCoroutine != null)
            StopCoroutine(cameraZoomCoroutine);

        cameraZoomCoroutine = StartCoroutine(ZoomIn(zoomAmount, zoomTime));
    }

    public void ActivateCameraZoomOut()
    {
        //Disable dev ui

        if (cameraZoomCoroutine != null)
            StopCoroutine(cameraZoomCoroutine);

        cameraZoomCoroutine = StartCoroutine(ZoomOut(zoomAmount, zoomTime));
    }

    private void EnableCinematicMode()
    {
        //Disable Ui
        UiManager.Instance.gameObject.SetActive(false);

        //Disable Player Controls
        player.playerInput.DisablePlayerMap();
        cinematicMovementMap.Enable();
    }

    private void DisableCinematicMode()
    {
        //Enable Ui
        UiManager.Instance.gameObject.SetActive(true);

        //Enable Player Controls
        player.playerInput.EnablePlayerMap();
        cinematicMovementMap.Disable();
    }

    private IEnumerator ZoomOut(float amountToZoomOut, float timeForZoom)
    {
        float timer = 0f;

        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = startPos - mainCam.transform.forward * amountToZoomOut;

        while(timer < timeForZoom)
        {
            timer += Time.deltaTime;

            float progress = timer / timeForZoom;

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, progress);

            yield return null;
        }

        mainCam.transform.position = endPos;

        yield return new WaitForSeconds(3f);

        //Enable devcontrols ui again

        cameraZoomCoroutine = null;
    }

    private IEnumerator ZoomIn(float amountToZoomOut, float timeForZoom)
    {
        float timer = 0f;

        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = startPos + mainCam.transform.forward * amountToZoomOut;

        while (timer < timeForZoom)
        {
            timer += Time.deltaTime;

            float progress = timer / timeForZoom;

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, progress);

            yield return null;
        }

        mainCam.transform.position = endPos;

        yield return new WaitForSeconds(3f);

        //Enable devcontrols ui again

        cameraZoomCoroutine = null;
    }
}
