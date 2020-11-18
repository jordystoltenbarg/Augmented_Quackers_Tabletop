using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TTLobbyUIPlayerItem : MonoBehaviour
{
    private TTPlayer _player = null;
    public TTPlayer player => _player;

    [SerializeField] private Image _avatar;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _ready;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _autoUpdate = true;
    [SerializeField] private float _autoUpdateInterval = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(autoUpdate());
    }

    public void Setup(TTPlayer pPLayer)
    {
        _player = pPLayer;
        _name.text = _player.playerName;
        //_ready.text = (pPLayer.isReady) ? "Ready" : "Not Ready";
        _ready.text = _player.lobbyIndex.ToString();
        if (pPLayer == TTPlayer.localPlayer)
            _highlight.SetActive(true);
        TTSettingsManager.onPlayerNameChanged += updateInfo;
    }

    private void OnDestroy()
    {
        TTSettingsManager.onPlayerNameChanged -= updateInfo;
    }

    private IEnumerator autoUpdate()
    {
        while (_autoUpdate)
        {
            yield return new WaitForSeconds(_autoUpdateInterval);
            updateInfo();
        }
    }

    private void updateInfo(string pNewName)
    {
        if (_player.selectedCharacterIndex > -1)
            _avatar.sprite = CharacterSelect.ListOfCharacters[_player.selectedCharacterIndex].GetComponent<Image>().sprite;
        _name.text = _player.playerName;
        _ready.text = _player.lobbyIndex.ToString();
    }

    private void updateInfo()
    {
        if (_player.selectedCharacterIndex > -1)
            _avatar.sprite = CharacterSelect.ListOfCharacters[_player.selectedCharacterIndex].GetComponent<Image>().sprite;
        _name.text = _player.playerName;
        _ready.text = _player.lobbyIndex.ToString();
    }
}
