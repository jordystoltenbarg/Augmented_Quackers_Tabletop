using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSizeToChildSize : MonoBehaviour
{
    private void OnValidate()
    {
        RectTransform childRect = transform.GetChild(0).GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2(childRect.rect.width, childRect.rect.height);
    }
}
