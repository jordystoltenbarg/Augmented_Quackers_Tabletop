using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ServerPrivacyToggle : MonoBehaviour
{
    [SerializeField] private Sprite _public;
    [SerializeField] private Sprite _private;
    [SerializeField] private Sprite _clientPublic;
    [SerializeField] private Sprite _clientPrivate;
    [SerializeField] private LocalizeStringEvent _text;
    [SerializeField] private LocalizedString _publicString;
    [SerializeField] private LocalizedString _privateString;

    private Image _image;
    private TTPlayer _host;

    private static bool _isPrivate = false;

    private void Start()
    {
        _image = GetComponent<Image>();
        _image.sprite = (_isPrivate) ? _private : _public;

        //Only in the lobby
        if (_text != null)
        {
            _text.StringReference = (_isPrivate) ? _privateString : _publicString;

            //Only for clients
            if (Mirror.NetworkManager.singleton.mode == Mirror.NetworkManagerMode.ClientOnly)
            {
                foreach (TTPlayer player in FindObjectsOfType<TTPlayer>())
                    if (player.LobbyIndex == 0) { _host = player; break; }

                TTSettingsManager.onUpdateCall += updateContent;
            }
            else
            {
                GetComponent<Button>().onClick.AddListener(toggle);
            }
        }

        TTSettingsManager.Singleton.ToggleServerPrivacySetting(_isPrivate);
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();

        //Only in lobby for clients
        if (_text != null && Mirror.NetworkManager.singleton.mode == Mirror.NetworkManagerMode.ClientOnly)
            TTSettingsManager.onUpdateCall -= updateContent;
    }

    private void toggle()
    {
        _isPrivate = !_isPrivate;
        _image.sprite = (_isPrivate) ? _private : _public;
        if (_text != null) _text.StringReference = (_isPrivate) ? _privateString : _publicString;
        TTSettingsManager.Singleton.ToggleServerPrivacySetting(_isPrivate);
    }

    private void updateContent()
    {
        bool serverIsPrivate = _host.HostedServerIsPrivate;
        _image.sprite = (serverIsPrivate) ? _clientPrivate : _clientPublic;
        _text.StringReference = (serverIsPrivate) ? _privateString : _publicString;
    }
}