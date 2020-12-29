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
        TTSettingsManager.onTTPlayerAdded += onTTPlayerAdded;
        TTSettingsManager.onTTPlayerRemoved += onTTPlayerRemoved;
    }

    private void OnDestroy()
    {
        TTSettingsManager.onUpdateCall -= updateContent;
        TTSettingsManager.onTTPlayerAdded -= onTTPlayerAdded;
        TTSettingsManager.onTTPlayerRemoved -= onTTPlayerRemoved;
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
            if (pPlayer == TTPlayer.LocalPlayer)
            {
                pPlayer.SelectCharacter(characterIndex);
                //_highlight.color = Color.white;
            }
            else
            {
                _playersThatSelected.Add(pPlayer);
                updateContent();
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

    private void onTTPlayerAdded(TTPlayer pPlayer)
    {
        if (!pPlayer.isLocalPlayer) return;

        pPlayer.onCharacterSelectApproved += onCharacterSelectApproved;
        pPlayer.onCharacterSelectDenied += onCharacterSelectDenied;
    }

    private void onTTPlayerRemoved(TTPlayer pPlayer)
    {
        if (!pPlayer.isLocalPlayer) return;

        pPlayer.onCharacterSelectApproved -= onCharacterSelectApproved;
        pPlayer.onCharacterSelectDenied -= onCharacterSelectDenied;
    }

    private void onCharacterSelectApproved(int pCharacterIndex)
    {
        if (pCharacterIndex != characterIndex) return;

        _playersThatSelected.Add(TTPlayer.LocalPlayer);
        updateContent();
        _highlight.color = Color.white;
    }

    private void onCharacterSelectDenied(int pCharacterIndex)
    {
        if (pCharacterIndex != characterIndex) return;

        _playersThatSelected.Add(TTPlayer.LocalPlayer);
        updateContent();
        _highlight.color = Color.white;
    }
}
