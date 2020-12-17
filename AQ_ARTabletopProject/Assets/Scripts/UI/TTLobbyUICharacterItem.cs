using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTLobbyUICharacterItem : MonoBehaviour
{
    private readonly List<TTPlayer> _playersThatSelected = new List<TTPlayer>();

    private Image _image;

    [SerializeField] private Image _highlight;
    [SerializeField] private Sprite _original;
    [SerializeField] private Sprite _selected;
    [HideInInspector] public int characterIndex = -1;

    private Color _transparentColor = new Color(1, 1, 1, 0);

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = _original;
        TTSettingsManager.onUpdateCall += updateContent;
    }

    private void OnDestroy()
    {
        TTSettingsManager.onUpdateCall -= updateContent;
    }

    private void updateContent()
    {
        if (_playersThatSelected.Count != 0)
        {
            _image.sprite = _selected;
            return;
        }

        _image.sprite = _original;
    }

    public void SelectCharacter(TTPlayer pPlayer)
    {
        if (!_playersThatSelected.Contains(pPlayer))
        {
            _playersThatSelected.Add(pPlayer);
            updateContent();

            if (pPlayer == TTPlayer.LocalPlayer)
            {
                pPlayer.SelectCharacter(characterIndex);
                _highlight.color = Color.white;
            }
        }
    }

    public void DeSelectCharacter(TTPlayer pPlayer)
    {
        if (_playersThatSelected.Contains(pPlayer))
        {
            _playersThatSelected.Remove(pPlayer);
            updateContent();

            if (pPlayer == TTPlayer.LocalPlayer)
                _highlight.color = _transparentColor;
        }
    }

    public Sprite GetPlayerSprite(TTPlayer pPlayer)
    {
        if (_playersThatSelected.Count == 0) return _original;
        if (pPlayer == _playersThatSelected[0]) return _original;
        else return _selected;
    }

    public bool GetPlayerIsAlreadySelected(TTPlayer pPlayer)
    {
        if (_playersThatSelected.Count == 0) return false;
        if (pPlayer == _playersThatSelected[0]) return false;
        else return true;
    }
}
