using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TTMessagePopup : MonoBehaviour
{
    public static TTMessagePopup Singleton = null;

    public static Action OnYesButtonPressed;
    public static Action OnNoButtonPressed;
    public static Action OnOkButtonPressed;

    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Button _okButton;
    [Header("Title")]
    [SerializeField] private LocalizeStringEvent _title;
    [SerializeField] private LocalizedString _tWarning;
    [SerializeField] private LocalizedString _tError;
    [Header("Message")]
    [SerializeField] private LocalizeStringEvent _message;
    [SerializeField] private LocalizedString _mCode;
    [SerializeField] private LocalizedString _mLeaveLobby;
    [Header("Secondary")]
    [SerializeField] private LocalizeStringEvent _secondary;
    [SerializeField] private LocalizedString _sCodeExample;
    [SerializeField] private LocalizedString _sLeaveLobbyHost;
    [SerializeField] private LocalizedString _sLeaveLobbyClient;

    public enum PopupResponse
    {
        YesNo,
        Ok
    }

    public enum PopupTitle
    {
        Warning,
        Error
    }

    public enum PopupMessage
    {
        Code,
        LeaveLobby
    }

    private void Awake()
    {
        TTMessagePopup[] ttPops = FindObjectsOfType<TTMessagePopup>();
        if (ttPops.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);

        _yesButton.onClick.AddListener(yesButtonHandler);
        _noButton.onClick.AddListener(noButtonHandler);
        _okButton.onClick.AddListener(okButtonHandler);

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _yesButton.onClick.RemoveAllListeners();
        _noButton.onClick.RemoveAllListeners();
        _okButton.onClick.RemoveAllListeners();
    }

    public void DisplayPopup(PopupTitle pTitle, PopupMessage pMessage, PopupResponse pMessageResponse)
    {
        switch (pTitle)
        {
            case PopupTitle.Warning:
                //Warning
                _title.StringReference = _tWarning;
                break;
            case PopupTitle.Error:
                //Title
                _title.StringReference = _tError;
                break;
        }

        switch (pMessage)
        {
            case PopupMessage.Code:
                //Message
                _message.StringReference = _mCode;
                //Secondary
                _secondary.StringReference = _sCodeExample;
                break;
            case PopupMessage.LeaveLobby:
                //Message
                _message.StringReference = _mLeaveLobby;
                //Secondary
                Mirror.NetworkManager manager = Mirror.NetworkManager.singleton;
                if (manager == null) break;
                if (manager.mode == Mirror.NetworkManagerMode.ClientOnly)
                    _secondary.StringReference = _sLeaveLobbyClient;
                else if (manager.mode == Mirror.NetworkManagerMode.Host)
                    _secondary.StringReference = _sLeaveLobbyHost;
                break;
        }

        switch (pMessageResponse)
        {
            case PopupResponse.YesNo:
                _yesButton.gameObject.SetActive(true);
                _noButton.gameObject.SetActive(true);
                _okButton.gameObject.SetActive(false);
                break;
            case PopupResponse.Ok:
                _yesButton.gameObject.SetActive(false);
                _noButton.gameObject.SetActive(false);
                _okButton.gameObject.SetActive(true);
                break;
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void hideMessage()
    {
        Animator anim = transform.GetChild(0).GetComponent<Animator>();
        anim.SetTrigger("FadeOut");
        StartCoroutine(disableGOWhenAnimationFinishes(anim));
    }

    private IEnumerator disableGOWhenAnimationFinishes(Animator pAnimator)
    {
        yield return new WaitWhile(() => pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.1f);
        yield return new WaitUntil(() => pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !pAnimator.IsInTransition(0));

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void yesButtonHandler()
    {
        hideMessage();
        OnYesButtonPressed?.Invoke();
    }

    private void noButtonHandler()
    {
        hideMessage();
        OnNoButtonPressed?.Invoke();
    }

    private void okButtonHandler()
    {
        hideMessage();
        OnOkButtonPressed?.Invoke();
    }
}
