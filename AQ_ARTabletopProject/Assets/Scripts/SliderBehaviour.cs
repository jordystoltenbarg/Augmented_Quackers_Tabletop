using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour
{
    public static bool ShouldOpenSlider;
    public static bool ShouldCloseSlider;

    private bool _isSliderOpen;

    private RectTransform _rect;
    private float _width;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _width = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        _rect.localPosition = new Vector2(-_width * 0.5f, _rect.localPosition.y);
    }

    void Update()
    {
        if (ShouldOpenSlider)
        {
            _rect.localPosition = Vector2.Lerp(new Vector2(_rect.localPosition.x, 0), new Vector2(_width * 0.5f, 0), Time.deltaTime * 10);
        }
    }
}
