using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTLobbyUI : MonoBehaviour
{
    public static TTLobbyUI Singleton = null;

    [SerializeField] private GameObject _playerLobbyUIItemPrefab;
    [SerializeField] private Transform _playersContainer;

    private readonly List<TTLobbyUIPlayerItem> _items = new List<TTLobbyUIPlayerItem>();

    private void Awake()
    {
        TTLobbyUI[] ttLs = FindObjectsOfType<TTLobbyUI>();
        if (ttLs.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(TTPlayer pPlayer)
    {
        GameObject clone = Instantiate(_playerLobbyUIItemPrefab, _playersContainer);
        clone.GetComponent<TTLobbyUIPlayerItem>().Setup(pPlayer);
        _items.Add(clone.GetComponent<TTLobbyUIPlayerItem>());
    }

    public void RemovePlayer(TTPlayer pPlayer)
    {
        foreach (TTLobbyUIPlayerItem item in _items)
        {
            if (item.Player == pPlayer)
            {
                Destroy(item.gameObject);
                _items.Remove(item);
                TTPlayer.LocalPlayer.UpdateHigherLobbyIndex(pPlayer.LobbyIndex);
                break;
            }
        }
    }
}
