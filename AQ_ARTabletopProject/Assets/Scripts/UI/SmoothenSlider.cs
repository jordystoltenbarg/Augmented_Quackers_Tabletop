using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmoothenSlider : MonoBehaviour, IBeginDragHandler, IDropHandler
{
    private Slider _slider;

    private void OnEnable()
    {
        _slider = GetComponent<Slider>();
        _slider.wholeNumbers = false;
    }

    private void OnDisable()
    {
        _slider.value = Mathf.FloorToInt(_slider.value);
        _slider.wholeNumbers = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            _slider.wholeNumbers = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            _slider.value = Mathf.FloorToInt(_slider.value);
            _slider.wholeNumbers = true;
        }
    }
}
