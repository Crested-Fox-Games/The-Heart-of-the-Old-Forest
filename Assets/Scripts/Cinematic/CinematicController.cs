using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CinematicController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cameraPositions;

    private int cameraPosCount = -1;

    private PlayerRef player;

    private Camera mainCam;

    private Vector3 playerCamLocalPos;
    private Quaternion playerCamLocalRot;

    private Coroutine cameraCoroutine;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        player = GameManager.Instance.LocalPlayer;
        mainCam = player.GetComponentInChildren<Camera>();

        playerCamLocalPos = mainCam.transform.localPosition;
        playerCamLocalRot = mainCam.transform.localRotation;

        Debug.Log($"Player cam pos: {playerCamLocalPos} || Player cam rot: {playerCamLocalRot}");
    }

    public void NextCameraPosition()
    {
        Debug.Log($"Trying to move camera {mainCam.name} to next position");
        if(cameraPosCount == -1)
        {
            cameraPosCount = 0;
            EnableCinematicMode();

            //Make the position the first in the list
            mainCam.transform.parent = cameraPositions[cameraPosCount].transform;
            mainCam.transform.SetPositionAndRotation(cameraPositions[cameraPosCount].transform.position, 
                cameraPositions[cameraPosCount].transform.rotation);
        }
        else if(cameraPosCount == cameraPositions.Count -1)
        {
            cameraPosCount = -1;
            DisableCinematicMode();

            //Make the position the player
            mainCam.transform.parent = player.playerStatus.CameraPivot;
            mainCam.transform.localPosition = playerCamLocalPos;
            mainCam.transform.localRotation = playerCamLocalRot;
        }
        else
        {
            cameraPosCount++;

            //Make the position the next in the list
            mainCam.transform.parent = cameraPositions[cameraPosCount].transform;
            mainCam.transform.SetPositionAndRotation(cameraPositions[cameraPosCount].transform.position, 
                cameraPositions[cameraPosCount].transform.rotation);
        }
    }

    public void PreviousCameraPosition()
    {
        if (cameraPosCount == -1)
        {
            cameraPosCount = cameraPositions.Count - 1;
            EnableCinematicMode();

            //Make the position the first in the list
            mainCam.transform.parent = cameraPositions[cameraPosCount].transform;
            mainCam.transform.SetPositionAndRotation(cameraPositions[cameraPosCount].transform.position, 
                cameraPositions[cameraPosCount].transform.rotation);
        }
        else if (cameraPosCount == 0)
        {
            cameraPosCount = -1;
            DisableCinematicMode();

            //Make the position the player
            mainCam.transform.parent = player.playerStatus.CameraPivot;
            mainCam.transform.localPosition = playerCamLocalPos;
            mainCam.transform.localRotation = playerCamLocalRot;
        }
        else
        {
            cameraPosCount--;

            //Make the position the next in the list
            mainCam.transform.parent = cameraPositions[cameraPosCount].transform;
            mainCam.transform.SetPositionAndRotation(cameraPositions[cameraPosCount].transform.position, 
                cameraPositions[cameraPosCount].transform.rotation);
        }
    }

    public void ActivatePositionAnimation()
    {
        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        switch (cameraPosCount)
        {
            case -1: break;
            case 0: cameraCoroutine = StartCoroutine(ZoomOut(5f, 3f)); break;
            case 1: cameraCoroutine = StartCoroutine(ZoomIn(5f, 3f)); break;
        }
    }

    private void EnableCinematicMode()
    {
        //Disable Ui
        UiManager.Instance.gameObject.SetActive(false);

        //Disable Player Controls
        player.playerInput.DisablePlayerMap();
    }

    private void DisableCinematicMode()
    {
        //Enable Ui
        UiManager.Instance.gameObject.SetActive(true);

        //Enable Player Controls
        player.playerInput.EnablePlayerMap();
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

        cameraCoroutine = null;
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

        cameraCoroutine = null;
    }
}
