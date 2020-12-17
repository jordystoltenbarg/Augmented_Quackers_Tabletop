using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
