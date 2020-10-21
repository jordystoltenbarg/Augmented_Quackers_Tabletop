using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelect : MonoBehaviour
{
    public static GameObject SelectedServer = null;

    [SerializeField]
    private Color _defultColor;
    [SerializeField]
    private Color _highlightColor;

    private List<GameObject> _listOfServers = new List<GameObject>();

    void Start()
    {
    }

    void updateListOfServers()
    {
        _listOfServers.Clear();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            _listOfServers.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateListOfServers();
        SelectedServer = null;

        foreach (GameObject go in _listOfServers)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            go.GetComponent<Image>().color = _defultColor;
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
            go.GetComponent<Image>().color = _defultColor;

        pGO.GetComponent<Image>().color = _highlightColor;
        SelectedServer = pGO;
    }
}
