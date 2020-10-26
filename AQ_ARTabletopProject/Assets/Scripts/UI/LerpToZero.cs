using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToZero : MonoBehaviour
{
    [SerializeField]
    private int _speed = 10;
    private RectTransform _rect;

    private enum LerpAxis
    {
        Both,
        X,
        Y
    }
    [SerializeField]
    private LerpAxis _axisToLerpOn;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        switch (_axisToLerpOn)
        {
            case LerpAxis.Both:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, Vector2.zero, Time.deltaTime * _speed);
                break;
            case LerpAxis.X:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, new Vector2(0, _rect.anchoredPosition.y), Time.deltaTime * _speed);
                break;
            case LerpAxis.Y:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, new Vector2(_rect.anchoredPosition.x, 0), Time.deltaTime * _speed);
                break;
        }
    }
}
