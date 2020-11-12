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

    private readonly List<TTServerListUIItem> _items = new List<TTServerListUIItem>();

    private void OnDisable()
    {
        deleteOldItems();
    }

    public void UpdateList(ServerCollectionJson pServerCollection)
    {
        deleteOldItems();
        createNewItems(pServerCollection.servers);
    }

    private void createNewItems(ServerJson[] pServers)
    {
        Array.Sort(pServers, delegate (ServerJson x, ServerJson y) { return x.playerCount.CompareTo(y.playerCount); });
        foreach (ServerJson server in pServers)
        {
            TTServerListUIItem clone = Instantiate(_listItemPrefab, _listItemParent);
            clone.GetComponent<Button>().onClick.AddListener(() => highlightItem(clone));
            clone.Setup(server);
            _items.Add(clone);
        }
    }

    private void deleteOldItems()
    {
        foreach (TTServerListUIItem item in _items)
            Destroy(item.gameObject);

        _items.Clear();
    }

    private void highlightItem(TTServerListUIItem pItem)
    {
        GameObject targetGO = pItem.gameObject;
        foreach (TTServerListUIItem item in _items)
            item.HighlightServer(targetGO);
    }
}