using System;
using Mirror;
using Mirror.Cloud.ListServerService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a server created by TTServerListUI
/// </summary>
public class TTServerListUIItem : MonoBehaviour
{
    private static int _currentBackGroundVariation = 0;

    [SerializeField] private TextMeshProUGUI _hostName = null;
    [SerializeField] private TextMeshProUGUI _playerCount = null;
    [SerializeField] private string _playerCountFormat = "{0} / {1}";
    [SerializeField] private Sprite[] _backgroundVariations = null;

    private Image _imageComponent = null;
    private Image _highlightImage = null;
    private Color _transparentColor = new Color(1, 1, 1, 0);
    private bool _isHighlited = false;

    private Button _joinButton = null;

    private ServerJson _server;

    public void Setup(ServerJson pServer)
    {
        _server = pServer;
        _hostName.text = pServer.displayName;
        _playerCount.text = string.Format(_playerCountFormat, pServer.playerCount, pServer.maxPlayerCount);

        _imageComponent = GetComponent<Image>();
        _imageComponent.sprite = getBackgroundVariation();

        _highlightImage = transform.Find("Highlight").GetComponent<Image>();
        _highlightImage.color = _transparentColor;

        _joinButton = FindObjectOfType<TTServerListManager>().joinButton;
        _joinButton.onClick.AddListener(onJoinClicked);
    }

    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        _joinButton.onClick.RemoveListener(onJoinClicked);
    }

    public void HighlightServer(GameObject pGO)
    {
        if (gameObject == pGO)
        {
            _highlightImage.color = Color.white;
            _isHighlited = true;
        }
        else
        {
            _highlightImage.color = _transparentColor;
            _isHighlited = false;
        }
    }

    private void onJoinClicked()
    {
        if (!_isHighlited) return;
        //NetworkManager.singleton.StartClient(new Uri(_server.address));
        TTSettingsManager.singleton.SetPlayerIndex(_server.playerCount);
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();
    }

    private Sprite getBackgroundVariation()
    {
        _currentBackGroundVariation = _currentBackGroundVariation + 1 < _backgroundVariations.Length ?
                                     _currentBackGroundVariation + 1 : 0;
        return _backgroundVariations[_currentBackGroundVariation];
    }
}