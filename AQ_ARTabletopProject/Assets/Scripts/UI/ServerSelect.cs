using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject _serverPrefab = null;
    [SerializeField]
    private Sprite[] _spriteVariations = null;

    public static MatchMaker HostMatchMaker = null;
    private static Match _selectedServer = null;
    public static Match SelectedServer { get { return _selectedServer; } }

    private List<Match> _servers = new List<Match>();
    private List<GameObject> _listOfServers = new List<GameObject>();
    private Color _transparent = new Color(1, 1, 1, 0);
    private int _currServerSpriteVar = 0;

    void updateList()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(transform.GetChild(0).GetChild(i).gameObject);
        }

        getServers();
        for (int i = 0; i < _servers.Count; i++)
        {
            GameObject go = Instantiate(_serverPrefab, transform.GetChild(0));
            go.GetComponent<Image>().sprite = getSpriteVariation();
            go.transform.Find("HostName").GetComponent<TextMeshProUGUI>().text = _servers[i].players[0].name;
            go.transform.Find("PlayerCount").GetComponent<TextMeshProUGUI>().text = string.Format("{0}/4", _servers[i].players.Count);
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
        }

        StartCoroutine(fillListofServers());
    }

    private IEnumerator fillListofServers()
    {
        yield return new WaitForSeconds(0.05f);
        _listOfServers.Clear();
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            _listOfServers.Add(transform.GetChild(0).GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateList();
        _selectedServer = null;

        //foreach (GameObject go in _listOfServers)
        //{
        //    go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
        //    go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
        //}
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(transform.GetChild(0).GetChild(i).gameObject);
        }

        //foreach (GameObject go in _listOfServers)
        //    go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in _listOfServers)
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;

        pGO.transform.Find("Highlight").GetComponent<Image>().color = Color.white;
        for (int i = 0; i < _listOfServers.Count; i++)
            if (pGO == _listOfServers[i])
                _selectedServer = _servers[i];
    }

    private void getServers()
    {
        _servers = MatchMaker.instance.matches.ToList();
    }

    private Sprite getSpriteVariation()
    {
        _currServerSpriteVar++;
        if (_currServerSpriteVar >= _spriteVariations.Length)
            _currServerSpriteVar = 0;
        return _spriteVariations[_currServerSpriteVar];
    }
}
