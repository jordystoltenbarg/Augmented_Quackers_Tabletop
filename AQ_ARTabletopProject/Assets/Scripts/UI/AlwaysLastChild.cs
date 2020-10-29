using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLastChild : MonoBehaviour
{
    void LateUpdate()
    {
        transform.SetAsLastSibling();
    }

    private void OnValidate()
    {
        transform.SetAsLastSibling();
    }
}
