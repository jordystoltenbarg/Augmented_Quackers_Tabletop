using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTLobbyUICharacterItem : MonoBehaviour
{
    private List<TTPlayer> _playersThatSelected = new List<TTPlayer>();

    private Image _image;

    [SerializeField] private Color _selected;
    [SerializeField] private Color _notSelected;
    [SerializeField] private bool _autoUpdate = true;
    [SerializeField] private float _autoUpdateInterval = 0.5f;
    [HideInInspector] public int characterIndex = -1;

    private void Awake()
    {
        _image = GetComponent<Image>();
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
            _image.color = _selected;
            return;
        }

        _image.color = _notSelected;
    }

    public void SelectCharacter(TTPlayer pPlayer)
    {
        if (!_playersThatSelected.Contains(pPlayer))
        {
            _playersThatSelected.Add(pPlayer);
            pPlayer.SelectCharacter(characterIndex);
            updateInfo();
        }
    }

    public void DeSelectCharacter(TTPlayer pPlayer)
    {
        if (_playersThatSelected.Contains(pPlayer))
        {
            _playersThatSelected.Remove(pPlayer);
            updateInfo();
        }
    }
}
