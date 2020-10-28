using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderPlayerAvatar : MonoBehaviour
{
    private VasilPlayer _player = null;
    public VasilPlayer Player { get { return _player; } }
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    }

    public void SetPlayer(VasilPlayer pPlayer)
    {
        _player = pPlayer;
        GetComponent<Image>().sprite = pPlayer.CharacterImage;
    }

    public void SelfDestroy()
    {
        StartCoroutine(destroySelf());
    }

    private IEnumerator destroySelf()
    {
        _animator.SetTrigger("Destroy");

        SetSizeToChildSize parentSizeSetter = transform.parent.gameObject.GetComponent<SetSizeToChildSize>();
        parentSizeSetter.StartUpdatingSize(gameObject);

        float timeout = 0;
        while (true)
        {
            timeout += Time.deltaTime;
            if (timeout >= 5) yield break;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.1f) yield return null;

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !_animator.IsInTransition(0))
            {
                Destroy(transform.parent.gameObject);
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }
}
