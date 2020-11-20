using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TTLobbyUIPlayerItem : MonoBehaviour
{
    private TTPlayer _player = null;
    public TTPlayer Player => _player;

    private Image _playerAvatarColor = null;

    [SerializeField] private Sprite[] _playerColorVariations = null;
    [SerializeField] private Image _avatar = null;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _ready;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _autoUpdate = true;
    [SerializeField] private float _autoUpdateInterval = 0.2f;

    private static readonly List<Sprite> _playerColorVariationsInUse = new List<Sprite>();
    private int _chosenColorVariation = -1;
    private int _chosenCharacterIndex = -1;

    private void OnEnable()
    {
        _playerAvatarColor = GetComponent<Image>();
        StartCoroutine(autoUpdateContent());
    }

    public void Setup(TTPlayer pPLayer)
    {
        _player = pPLayer;
        StartCoroutine(setPlayerColorVariation());
        _avatar.enabled = false;
        _name.text = _player.PlayerName;
        //_ready.text = (pPLayer.isReady) ? "Ready" : "Not Ready";
        _ready.text = _player.LobbyIndex.ToString();

        if (pPLayer == TTPlayer.LocalPlayer)
            _highlight.SetActive(true);
        else
            _highlight.SetActive(false);

        TTSettingsManager.onPlayerNameChanged += updateContent;
    }

    private void OnDestroy()
    {
        if (_playerColorVariationsInUse.Contains(_playerColorVariations[_chosenColorVariation]))
            _playerColorVariationsInUse.Remove(_playerColorVariations[_chosenColorVariation]);

        if (_chosenCharacterIndex != -1)
            CharacterSelect.ListOfCharacters[_chosenCharacterIndex].GetComponent<TTLobbyUICharacterItem>().DeSelectCharacter(_player);

        TTSettingsManager.onPlayerNameChanged -= updateContent;
    }

    private IEnumerator autoUpdateContent()
    {
        while (_autoUpdate)
        {
            yield return new WaitForSeconds(_autoUpdateInterval);
            updateContent();
        }
    }

    private void updateContent()
    {
        if (_player.SelectedCharacterIndex > -1)
        {
            for (int i = 0; i < CharacterSelect.ListOfCharacters.Count; i++)
            {
                TTLobbyUICharacterItem listItem = CharacterSelect.ListOfCharacters[i].GetComponent<TTLobbyUICharacterItem>();
                if (i == _player.SelectedCharacterIndex)
                {
                    _avatar.enabled = true;
                    _avatar.sprite = listItem.GetPlayerSprite(_player);
                    _chosenCharacterIndex = _player.SelectedCharacterIndex;
                    listItem.SelectCharacter(_player);
                }
                else
                {
                    listItem.DeSelectCharacter(_player);
                }
            }
        }
        _name.text = _player.PlayerName;
        _ready.text = _player.LobbyIndex.ToString();
    }

    private void updateContent(string pNewName)
    {
        updateContent();
    }

    private IEnumerator setPlayerColorVariation()
    {
        yield return new WaitWhile(() => _player.ColorVariation == -1);
        _playerAvatarColor.sprite = getPlayerAvatarColorVariation();
    }

    private Sprite getPlayerAvatarColorVariation()
    {
        Sprite sprite = _playerColorVariations[_player.ColorVariation];

        if (_playerColorVariationsInUse.Contains(sprite))
        {
            for (int i = 0; i < _playerColorVariations.Length; i++)
            {
                if (!_playerColorVariationsInUse.Contains(_playerColorVariations[i]))
                {
                    _chosenColorVariation = i;
                    _player.UpdateColorVariation(_chosenColorVariation);
                    _playerColorVariationsInUse.Add(_playerColorVariations[_chosenColorVariation]);
                    return _playerColorVariations[_chosenColorVariation];
                }
            }

            throw new System.Exception($"This _playerColorVariations could not be found.");
        }
        else
        {
            _chosenColorVariation = _player.ColorVariation;
            _playerColorVariationsInUse.Add(sprite);
            return sprite;
        }
    }
}
