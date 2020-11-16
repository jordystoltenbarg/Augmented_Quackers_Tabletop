using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class PlacementDisable : MonoBehaviour
{
    

    private ARPlaneManager planeManager;

    private void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }
    public void DisablePlanes ()
    {
        List<ARPlane> allPlanes = new List<ARPlane>();

        planeManager.enabled = false;
        foreach (ARPlane plane in allPlanes)
        {
            plane.gameObject.SetActive(false);           
        }
        foreach (var detectedPlanes in planeManager.trackables)
        {
            detectedPlanes.gameObject.SetActive(false);
        }
    }


    

}
