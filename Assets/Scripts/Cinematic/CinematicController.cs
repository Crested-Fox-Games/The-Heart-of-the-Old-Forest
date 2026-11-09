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
    private Coroutine cameraSlideCoroutine;
    private Coroutine cameraPanCoroutine;

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

    [Header("Pan controls")]
    [SerializeField]
    private float panCenterDistance = 10f;
    [SerializeField]
    private float panAmount = 5f;
    [SerializeField]
    private float panTime = 2f;

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
        if (cameraZoomCoroutine != null)
            StopCoroutine(cameraZoomCoroutine);

        cameraZoomCoroutine = StartCoroutine(ZoomIn(zoomAmount, zoomTime));
    }

    public void ActivateCameraZoomOut()
    {
        if (cameraZoomCoroutine != null)
            StopCoroutine(cameraZoomCoroutine);

        cameraZoomCoroutine = StartCoroutine(ZoomOut(zoomAmount, zoomTime));
    }

    public void ActivateSlideLeft()
    {
        if (cameraSlideCoroutine != null)
            StopCoroutine(cameraSlideCoroutine);

        cameraSlideCoroutine = StartCoroutine(SlideCamera(-mainCam.transform.right, slideAmount, slideTime));
    }

    public void ActivateSlideRight()
    {
        if (cameraSlideCoroutine != null)
            StopCoroutine(cameraSlideCoroutine);

        cameraSlideCoroutine = StartCoroutine(SlideCamera(mainCam.transform.right, slideAmount, slideTime));
    }

    public void ActivateSlideUp()
    {
        if (cameraSlideCoroutine != null)
            StopCoroutine(cameraSlideCoroutine);

        cameraSlideCoroutine = StartCoroutine(SlideCamera(mainCam.transform.up, slideAmount, slideTime));
    }

    public void ActivateSlideDown()
    {
        if (cameraSlideCoroutine != null)
            StopCoroutine(cameraSlideCoroutine);

        cameraSlideCoroutine = StartCoroutine(SlideCamera(-mainCam.transform.up, slideAmount, slideTime));
    }

    public void ActivatePanLeft()
    {
        if (cameraPanCoroutine != null)
            StopCoroutine(cameraPanCoroutine);

        cameraPanCoroutine = StartCoroutine(PanCamera(mainCam.transform.right, panAmount, panTime));
    }

    public void ActivatePanRight()
    {
        if (cameraPanCoroutine != null)
            StopCoroutine(cameraPanCoroutine);

        cameraPanCoroutine = StartCoroutine(PanCamera(-mainCam.transform.right, panAmount, panTime));
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


        cameraZoomCoroutine = null;
    }

    private IEnumerator SlideCamera(Vector3 direction, float amountToSlide, float timeForSlide)
    {
        float timer = 0f;
        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = startPos + direction * amountToSlide;
        while (timer < timeForSlide)
        {
            timer += Time.deltaTime;
            float progress = timer / timeForSlide;
            mainCam.transform.position = Vector3.Lerp(startPos, endPos, progress);
            yield return null;
        }
        mainCam.transform.position = endPos;
        yield return new WaitForSeconds(3f);

        cameraSlideCoroutine = null;
    }

    private IEnumerator PanCamera(Vector3 direction, float amountToPan, float timeForPan)
    {
        float timer = 0f;
        
        Vector3 orbitPoint = mainCam.transform.position + mainCam.transform.forward * panCenterDistance;

        Vector3 startPos = mainCam.transform.position;

        Vector3 rotationAxis = Vector3.Cross(mainCam.transform.forward, direction).normalized;
        
        Vector3 offset = startPos - orbitPoint;

        Quaternion endRot = mainCam.transform.rotation;

        while (timer < timeForPan)
        {
            timer += Time.deltaTime;
            float progress = timer / timeForPan;

            Quaternion currentRot = Quaternion.AngleAxis(progress * amountToPan, rotationAxis);

            mainCam.transform.position = orbitPoint + currentRot * offset;

            mainCam.transform.rotation = Quaternion.LookRotation(orbitPoint - mainCam.transform.position);
            endRot = mainCam.transform.rotation;
            yield return null;
        }

        mainCam.transform.rotation = endRot;

        Debug.Log($"Pan complete. Target end rotation {endRot}, New camera rotation: {mainCam.transform.rotation}");
        yield return new WaitForSeconds(3f);

        cameraPanCoroutine = null;
    }
}
