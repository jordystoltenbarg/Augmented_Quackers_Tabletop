using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLastChild : MonoBehaviour
{
    [SerializeField] private Transform _otherAfter = null;

    void LateUpdate()
    {
        transform.SetAsLastSibling();
        if (_otherAfter)
            _otherAfter.SetAsLastSibling();
    }

    private void OnValidate()
    {
        transform.SetAsLastSibling();
        if (_otherAfter)
            _otherAfter.SetAsLastSibling();
    }
}
