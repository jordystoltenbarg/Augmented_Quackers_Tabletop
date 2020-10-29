using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelect : MonoBehaviour
{
    private static GameObject _selectedServer = null;
    public static GameObject SelectedServer { get { return _selectedServer; } }

    private List<GameObject> _listOfServers = new List<GameObject>();
    private Color _transparent = new Color(1, 1, 1, 0);

    void updateList()
    {
        _listOfServers.Clear();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            _listOfServers.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateList();
        _selectedServer = null;

        foreach (GameObject go in _listOfServers)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in _listOfServers)
            go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in _listOfServers)
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;

        pGO.transform.Find("Highlight").GetComponent<Image>().color = Color.white;
        _selectedServer = pGO;
    }
}
