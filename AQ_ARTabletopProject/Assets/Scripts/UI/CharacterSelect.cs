using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static GameObject SelectedCharacter = null;

    private List<GameObject> _listOfCharacters = new List<GameObject>();
    private Color _transparent = new Color(1, 1, 1, 0);

    void updateList()
    {
        _listOfCharacters.Clear();

        for (int i = 0; i < transform.childCount; i++)
            _listOfCharacters.Add(transform.GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateList();
        SelectedCharacter = null;

        foreach (GameObject go in _listOfCharacters)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
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
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;

        pGO.transform.Find("Highlight").GetComponent<Image>().color = Color.white;
        SelectedCharacter = pGO;
    }
}
