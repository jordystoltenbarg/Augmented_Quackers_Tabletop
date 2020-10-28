using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendTurnOrderAvatar : MonoBehaviour
{
    private bool _isFirst;
    public bool IsFirst { get { return _isFirst; } set { _isFirst = value; } }

    private RectTransform _rect;
    private int _extendedY;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _extendedY = Mathf.FloorToInt(99 * 0.5f);
    }

    void Update()
    {
        if (_isFirst)
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, new Vector2(_rect.anchoredPosition.x, -_extendedY), Time.deltaTime * 5);
        else
            _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, new Vector2(_rect.anchoredPosition.x, 0), Time.deltaTime * 20);
    }
}
