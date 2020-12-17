using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ReadyUp : MonoBehaviour
{
    public static bool IsLocalPlayerReady = false;

    [SerializeField] private TMP_FontAsset _greenFont;
    [SerializeField] private TMP_FontAsset _redFont;
    [SerializeField] private LocalizeStringEvent _text;
    [SerializeField] private LocalizedString _hostStartString;
    [SerializeField] private LocalizedString _hostWaitingString;
    [SerializeField] private LocalizedString _clientReadyString;
    [SerializeField] private LocalizedString _clientUnreadyString;
    [SerializeField] private LocalizedString _chooseCharacterString;
    [SerializeField] private LocalizedString _chooseDifferentCharacterString;

    [SerializeField] private Sprite _available;
    [SerializeField] private Sprite _unavailable;
    [SerializeField] private Sprite _unready;

    private TTLobbyUIPlayerItem _localPlayerUILobbyItem = null;

    private Image _image;
    private bool _canStartGame = false;
    private bool _isButtonInteractable = true;
    private Mirror.NetworkManager _manager;

    private void Start()
    {
        _manager = Mirror.NetworkManager.singleton;
        _image = GetComponent<Image>();
        _image.sprite = _unavailable;
        _text.StringReference = _chooseCharacterString;
        _isButtonInteractable = false;
        StartCoroutine(waitForCharacterSelect());

        GetComponent<Button>().onClick.AddListener(onReadyPressed);
        TTSettingsManager.onUpdateCall += updateContent;
    }

    private void OnEnable()
    {
        if (_image == null) return;
        _image.sprite = _unavailable;
        _text.StringReference = _chooseCharacterString;
        _isButtonInteractable = false;
        StartCoroutine(waitForCharacterSelect());
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        TTSettingsManager.onUpdateCall -= updateContent;
    }

    private void onReadyPressed()
    {
        if (!_isButtonInteractable) return;

        if (_localPlayerUILobbyItem.characterAlreadySelected)
        {
            _image.sprite = _unavailable;
            _text.StringReference = _chooseDifferentCharacterString;
            if (_isButtonInteractable)
            {
                StartCoroutine(setInteracableWhenCharacterIsAvailable());
                _isButtonInteractable = false;
            }
            return;
        }

        switch (_manager.mode)
        {
            case Mirror.NetworkManagerMode.ClientOnly:
                //Client gets ready
                TTPlayer.LocalPlayer.ReadyToggle();
                IsLocalPlayerReady = !IsLocalPlayerReady;
                if (IsLocalPlayerReady)
                {
                    _image.sprite = _unready;
                    _text.StringReference = _clientUnreadyString;
                }
                else
                {
                    _image.sprite = _available;
                    _text.StringReference = _clientReadyString;
                }
                break;
            case Mirror.NetworkManagerMode.Host:
                if (!_canStartGame)
                {
                    //Host gets ready
                    TTPlayer.LocalPlayer.ReadyToggle();
                    IsLocalPlayerReady = !IsLocalPlayerReady;

                    _image.sprite = _unavailable;
                    _text.StringReference = _hostWaitingString;

                    _isButtonInteractable = false;
                    _canStartGame = true;
                }
                else
                {
                    if (TTSettingsManager.Singleton.players.Count == 1)
                    {
                        TTMessagePopup.Singleton.DisplayPopup(TTMessagePopup.PopupTitle.Warning, TTMessagePopup.PopupMessage.StartAlone, TTMessagePopup.PopupResponse.YesNo);
                        StartCoroutine(waitForPopupResponse()); //If Yes game should start
                    }
                    else
                    {
                        //Game should start
                        Debug.Log("Starting...lol");
                    }
                }
                break;
        }
    }

    private void updateContent()
    {
        if (_localPlayerUILobbyItem == null || !_localPlayerUILobbyItem.Avatar.enabled) return;
        if (_localPlayerUILobbyItem.characterAlreadySelected)
        {
            _image.sprite = _unavailable;
            _text.StringReference = _chooseDifferentCharacterString;
            if (_isButtonInteractable)
            {
                StartCoroutine(setInteracableWhenCharacterIsAvailable());
                _isButtonInteractable = false;
            }
            return;
        }

        switch (_manager.mode)
        {
            case Mirror.NetworkManagerMode.ClientOnly:
                if (IsLocalPlayerReady)
                {
                    _image.sprite = _unready;
                    _text.StringReference = _clientUnreadyString;
                }
                else
                {
                    _image.sprite = _available;
                    _text.StringReference = _clientReadyString;
                }
                break;
            case Mirror.NetworkManagerMode.Host:
                //Check if everyone is ready
                bool everyoneIsReady = true;
                foreach (TTPlayer player in TTSettingsManager.Singleton.players)
                    if (!player.IsReady) everyoneIsReady = false;

                if (_canStartGame)
                {
                    if (everyoneIsReady)
                    {
                        _image.sprite = _available;
                        _text.StringReference = _hostStartString;
                        _isButtonInteractable = true;
                    }
                    else
                    {
                        _image.sprite = _unavailable;
                        _text.StringReference = _hostWaitingString;
                        _isButtonInteractable = false;
                    }
                }
                else if (!IsLocalPlayerReady)
                {
                    _image.sprite = _available;
                    _text.StringReference = _clientReadyString;
                }
                else
                {
                    _image.sprite = _unavailable;
                    _text.StringReference = _hostWaitingString;
                }
                break;
        }
    }

    private IEnumerator setInteracableWhenCharacterIsAvailable()
    {
        yield return new WaitWhile(() => _localPlayerUILobbyItem.characterAlreadySelected);
        _isButtonInteractable = true;
    }

    private IEnumerator waitForCharacterSelect()
    {
        while (_localPlayerUILobbyItem == null)
        {
            foreach (TTLobbyUIPlayerItem item in FindObjectsOfType<TTLobbyUIPlayerItem>())
                if (item.Player == TTPlayer.LocalPlayer)
                    _localPlayerUILobbyItem = item;
            yield return null;
        }

        yield return new WaitUntil(() => _localPlayerUILobbyItem.Avatar.enabled);

        if (_localPlayerUILobbyItem.characterAlreadySelected)
        {
            _image.sprite = _unavailable;
            _text.StringReference = _chooseDifferentCharacterString;
            StartCoroutine(setInteracableWhenCharacterIsAvailable());
            _isButtonInteractable = false;
        }
        else
        {
            _image.sprite = _available;
            _text.StringReference = _clientReadyString;
            _isButtonInteractable = true;
        }
    }

    private IEnumerator waitForPopupResponse()
    {
        TTMessagePopup.OnYesButtonPressed += yes;
        TTMessagePopup.OnNoButtonPressed += no;
        bool hasReponded = false;

        yield return new WaitWhile(() => !hasReponded);

        TTMessagePopup.OnYesButtonPressed -= yes;
        TTMessagePopup.OnNoButtonPressed -= no;

        void yes()
        {
            //Game should start
            hasReponded = true;
        }

        void no()
        {
            hasReponded = true;
        }
    }
}
