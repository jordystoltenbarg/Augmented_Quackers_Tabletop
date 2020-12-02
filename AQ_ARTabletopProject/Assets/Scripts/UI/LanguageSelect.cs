using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelect : MonoBehaviour
{
    private static GameObject _selectedLanguage = null;

    private List<GameObject> _listOfLanguages = new List<GameObject>();
    private Color _transparent = new Color(1, 1, 1, 0);

    private void Start()
    {
        updateList();

        foreach (GameObject go in _listOfLanguages)
        {
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            if (go == _selectedLanguage) continue;
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
        }

        string language = PlayerPrefs.GetString("Langauge");
        transform.Find(language).Find("Highlight").GetComponent<Image>().color = Color.white;
        _selectedLanguage = transform.Find(language).gameObject;
    }

    private void updateList()
    {
        _listOfLanguages.Clear();

        for (int i = 0; i < transform.childCount; i++)
            _listOfLanguages.Add(transform.GetChild(i).gameObject);
    }

    private void OnDestroy()
    {
        foreach (GameObject go in _listOfLanguages)
            go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in _listOfLanguages)
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;

        pGO.transform.Find("Highlight").GetComponent<Image>().color = Color.white;
        _selectedLanguage = pGO;
        selectLanguage();
    }

    private void selectLanguage()
    {
        switch (_selectedLanguage.name)
        {
            case "Dutch":
                TTSettingsManager.Singleton.SelectLanguage(TTSettingsManager.ApplicationLanguage.Dutch);
                break;
            case "English":
                TTSettingsManager.Singleton.SelectLanguage(TTSettingsManager.ApplicationLanguage.English);
                break;
        }
    }
}
