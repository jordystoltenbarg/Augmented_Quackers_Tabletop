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
    [SerializeField] private LocalizedString _tNotification;
    [Header("Message")]
    [SerializeField] private LocalizeStringEvent _message;
    [SerializeField] private LocalizedString _mCode;
    [SerializeField] private LocalizedString _mLeaveLobby;
    [SerializeField] private LocalizedString _mReconnect;
    [SerializeField] private LocalizedString _mKickHost;
    [SerializeField] private LocalizedString _mKickClient;
    [SerializeField] private LocalizedString _mStartAlone;
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
        Error,
        Notification
    }

    public enum PopupMessage
    {
        Code,
        LeaveLobby,
        Reconnect,
        Kick,
        StartAlone
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
        Mirror.NetworkManager manager = Mirror.NetworkManager.singleton;
        _secondary.gameObject.SetActive(false);

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
            case PopupTitle.Notification:
                //Title
                _title.StringReference = _tNotification;
                break;
        }

        switch (pMessage)
        {
            case PopupMessage.Code:
                //Message
                _message.StringReference = _mCode;
                //Secondary
                _secondary.gameObject.SetActive(true);
                _secondary.StringReference = _sCodeExample;
                break;
            case PopupMessage.LeaveLobby:
                //Message
                _message.StringReference = _mLeaveLobby;
                //Secondary
                _secondary.gameObject.SetActive(true);
                if (manager.mode == Mirror.NetworkManagerMode.Host)
                    _secondary.StringReference = _sLeaveLobbyHost;
                else if (manager.mode == Mirror.NetworkManagerMode.ClientOnly)
                    _secondary.StringReference = _sLeaveLobbyClient;
                break;
            case PopupMessage.Reconnect:
                //Message
                _message.StringReference = _mCode;
                break;
            case PopupMessage.Kick:
                //Message
                if (manager.mode == Mirror.NetworkManagerMode.Host)
                    _message.StringReference = _mKickHost;
                else
                    _message.StringReference = _mKickClient;
                break;
            case PopupMessage.StartAlone:
                //Message
                _message.StringReference = _mStartAlone;
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
