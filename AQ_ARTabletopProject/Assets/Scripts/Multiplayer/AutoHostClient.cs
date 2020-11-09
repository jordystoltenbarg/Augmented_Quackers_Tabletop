using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    void Start()
    {
        if (!Application.isBatchMode) //batchmode means headless build
        {
            Debug.Log("===== Client Build =====");
            networkManager.StartClient();
        }
        else
        {
            Debug.Log("===== Server Build =====");
        }
    }

    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    public void JoinKakei()
    {
        networkManager.networkAddress = "82.74.130.31";
        networkManager.StartClient();
    }

    public void JoinArnoud()
    {
        networkManager.networkAddress = "185.113.85.69";
        networkManager.StartClient();
    }
}
