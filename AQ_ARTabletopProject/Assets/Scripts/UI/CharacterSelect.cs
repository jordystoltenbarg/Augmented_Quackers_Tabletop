using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static GameObject SelectedCharacter = null;

    [SerializeField]
    private Color _defultColor;
    [SerializeField]
    private Color _highlightColor;

    private List<GameObject> _listOfCharacters = new List<GameObject>();

    void Start()
    {
    }

    void updateListOfServers()
    {
        _listOfCharacters.Clear();

        for (int i = 0; i < transform.childCount; i++)
            _listOfCharacters.Add(transform.GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateListOfServers();
        SelectedCharacter = null;

        foreach (GameObject go in _listOfCharacters)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            go.GetComponent<Image>().color = _defultColor;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in _listOfCharacters)
            go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in _listOfCharacters)
            go.GetComponent<Image>().color = _defultColor;

        pGO.GetComponent<Image>().color = _highlightColor;
        SelectedCharacter = pGO;
    }
}
