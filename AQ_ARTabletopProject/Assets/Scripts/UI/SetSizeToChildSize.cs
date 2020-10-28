using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SetSizeToChildSize : MonoBehaviour
{
    private RectTransform _rect = null;
    private ContentSizeFitter _contentSizeFitter = null;
    private RectTransform _childRect = null;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _contentSizeFitter = transform.parent.GetComponent<ContentSizeFitter>();
    }

    private void OnValidate()
    {
        _childRect = transform.GetChild(0).GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2(_childRect.rect.width, _childRect.rect.height);
    }

    public void StartUpdatingSize(GameObject pChild)
    {
        _childRect = pChild.GetComponent<RectTransform>();
        _rect = GetComponent<RectTransform>();
        _contentSizeFitter = transform.parent.GetComponent<ContentSizeFitter>();
        StartCoroutine(updateSizeWhileAnimationIsRunning(pChild));
    }

    private IEnumerator updateSizeWhileAnimationIsRunning(GameObject pGO)
    {
        Animator anim = pGO.GetComponent<Animator>();

        float timeout = 0;
        while (true)
        {
            timeout += Time.deltaTime;
            if (timeout >= 5) yield break;

            _rect.sizeDelta = new Vector2(_childRect.rect.width, _childRect.rect.height);
            _contentSizeFitter.enabled = false;
            _contentSizeFitter.enabled = true;

            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.1f) yield return null;
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !anim.IsInTransition(0))
                yield break;
            else
                yield return null;
        }
    }
}
