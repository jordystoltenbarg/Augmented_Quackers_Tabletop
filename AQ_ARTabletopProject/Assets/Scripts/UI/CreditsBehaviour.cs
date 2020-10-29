using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform _textContainer = null;
    [SerializeField]
    private float _textScrollSpeed;

    private float _heightModifier;

    void OnEnable()
    {
        _textContainer.anchoredPosition = new Vector2(_textContainer.anchoredPosition.x, 0);
        _heightModifier = _textContainer.parent.GetComponent<RectTransform>().rect.height / GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution.y;
    }

    void Update()
    {
        _textContainer.anchoredPosition = new Vector2(_textContainer.anchoredPosition.x, _textContainer.anchoredPosition.y + (_textScrollSpeed * Time.deltaTime * _heightModifier * 100));
    }

    void OnValidate()
    {
        _textScrollSpeed = Mathf.Clamp(_textScrollSpeed, 1, 20);
    }
}
