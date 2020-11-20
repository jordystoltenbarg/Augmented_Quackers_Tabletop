using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Cloud.ListServerService;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the list of servers
/// </summary>
public class TTServerListUI : MonoBehaviour
{
    [SerializeField] private TTServerListUIItem _listItemPrefab = null;
    [SerializeField] private Transform _listItemParent = null;

    [HideInInspector] public bool allowDraw = true;

    private readonly List<TTServerListUIItem> _items = new List<TTServerListUIItem>();

    private void OnDisable()
    {
        deleteOldItems();
    }

    public void UpdateList(ServerCollectionJson pServerCollection)
    {
        if (pServerCollection.servers.Length == 0) return;

        if (_items.Count == 0)
            allowDraw = true;

        if (allowDraw)
        {
            deleteOldItems();
            createNewItems(pServerCollection.servers);
        }
        else
        {
            updateListContent(pServerCollection.servers);
        }
    }

    private void createNewItems(ServerJson[] pServers)
    {
        Array.Sort(pServers, delegate (ServerJson pX, ServerJson pY) { return pX.playerCount.CompareTo(pY.playerCount); });
        Array.Reverse(pServers);
        foreach (ServerJson server in pServers)
        {
            TTServerListUIItem clone = Instantiate(_listItemPrefab, _listItemParent);
            clone.GetComponent<Button>().onClick.AddListener(() => highlightItem(clone));
            clone.Setup(server);
            _items.Add(clone);
        }
        allowDraw = false;
    }

    private void deleteOldItems()
    {
        foreach (TTServerListUIItem item in _items)
            Destroy(item.gameObject);

        _items.Clear();
    }

    private void updateListContent(ServerJson[] pServers)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (pServers.Length < i) return;
            _items[i].UpdateContent(pServers[i]);
        }
    }

    private void highlightItem(TTServerListUIItem pItem)
    {
        GameObject targetGO = pItem.gameObject;
        foreach (TTServerListUIItem item in _items)
            item.HighlightServer(targetGO);
    }
}