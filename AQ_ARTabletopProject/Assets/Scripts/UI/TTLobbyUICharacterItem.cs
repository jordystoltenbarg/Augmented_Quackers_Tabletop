using System.Collections;
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
    [SerializeField] private bool _autoUpdate = true;
    [SerializeField] private float _autoUpdateInterval = 0.2f;
    [HideInInspector] public int characterIndex = -1;

    private Color _transparentColor = new Color(1, 1, 1, 0);

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = _original;
    }

    private void OnEnable()
    {
        StartCoroutine(autoUpdate());
    }

    private IEnumerator autoUpdate()
    {
        while (_autoUpdate)
        {
            yield return new WaitForSeconds(_autoUpdateInterval);
            updateInfo();
        }
    }

    private void updateInfo()
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
            updateInfo();

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
            updateInfo();

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
}
