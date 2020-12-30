using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{

    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public GameObject LobbyUI;
    public GameObject LobbyCamera;
    public GameObject ARCamera;

    private ARSessionOrigin arOrigin;
    private PlacementDisable disablePlacement;
    private Pose placementPose;
    //private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid;
    private bool isLocked = false;
    public bool ObjectPlaced = false;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        
        //aRRaycastManager = arOrigin.GetComponent<ARRaycastManager>();
        disablePlacement = arOrigin.GetComponent<PlacementDisable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && ObjectPlaced == false)
        {
            PlaceObject();
        }

        if (ObjectPlaced == true)
        {
            placementIndicator.SetActive(false);
            ARCamera.GetComponent<MakeAppearOnPlane>().enabled = false;
        }
        else if(ObjectPlaced == false)
        {
            ShowPlacementIndicator();
            UpdatePlacementPose();
        }
    }

    private void Lock()
    {
        isLocked = !isLocked;
        if(ObjectPlaced == true)
        {

        }
    }

    private void DelaySetActive()
    {
        LobbyUI.SetActive(true);
        LobbyCamera.SetActive(true);
        ARCamera.SetActive(false);
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        ObjectPlaced = true;
        disablePlacement.DisablePlanes();
        Invoke(nameof(DelaySetActive), 3);
    }

    private void ShowPlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
