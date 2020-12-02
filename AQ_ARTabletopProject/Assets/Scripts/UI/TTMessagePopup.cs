using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TTMessagePopup : MonoBehaviour
{
    public static TTMessagePopup Singleton = null;

    public static Action OnYesButtonPressed;
    public static Action OnNoButtonPressed;
    public static Action OnOkButtonPressed;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private TextMeshProUGUI _secondary;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Button _okButton;

    public enum PopupType
    {
        Warning,
        Error
    }

    public enum PopupResponse
    {
        YesNo,
        Ok
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
    }

    private void OnDestroy()
    {
        _yesButton.onClick.RemoveAllListeners();
        _noButton.onClick.RemoveAllListeners();
        _okButton.onClick.RemoveAllListeners();
    }

    public void DisplayPopup(PopupType pTypeOfMessage, string pMessage, string pSecondary, PopupResponse pMessageResponse)
    {
        switch (pTypeOfMessage)
        {
            case PopupType.Warning:
                break;
            case PopupType.Error:
                break;
        }

        _message.text = pMessage;
        _secondary.text = pSecondary;

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
    }

    private void yesButtonHandler()
    {
        OnYesButtonPressed?.Invoke();
    }

    private void noButtonHandler()
    {
        OnNoButtonPressed?.Invoke();
    }

    private void okButtonHandler()
    {
        OnOkButtonPressed?.Invoke();
    }
}
