using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelect : MonoBehaviour
{
    private static GameObject _selectedLanguage = null;
    public static GameObject SelectedLanguage { get { return _selectedLanguage; } }

    private List<GameObject> _listOfLanguages = new List<GameObject>();
    private Color _transparent = new Color(1, 1, 1, 0);

    private void Start()
    {
        updateList();
        transform.Find("English").Find("Highlight").GetComponent<Image>().color = Color.white;
        _selectedLanguage = transform.Find("English").gameObject;
    }

    void updateList()
    {
        _listOfLanguages.Clear();

        for (int i = 0; i < transform.childCount; i++)
            _listOfLanguages.Add(transform.GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateList();

        foreach (GameObject go in _listOfLanguages)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            if (go == SelectedLanguage) continue;
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in _listOfLanguages)
            go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in _listOfLanguages)
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;

        pGO.transform.Find("Highlight").GetComponent<Image>().color = Color.white;
        _selectedLanguage = pGO;
    }
}
