using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static readonly List<GameObject> ListOfCharacters = new List<GameObject>();

    private Color _transparent = new Color(1, 1, 1, 0);

    void updateList()
    {
        ListOfCharacters.Clear();

        for (int i = 0; i < transform.childCount; i++)
            ListOfCharacters.Add(transform.GetChild(i).gameObject);
    }

    private void OnEnable()
    {
        updateList();

        for (int i = 0; i < ListOfCharacters.Count; i++)
        {
            GameObject go = ListOfCharacters[i];
            go.GetComponent<TTLobbyUICharacterItem>().characterIndex = i;
            go.GetComponent<Button>().onClick.AddListener(() => highlightServer(go));
            go.transform.Find("Highlight").GetComponent<Image>().color = _transparent;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in ListOfCharacters)
            go.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void highlightServer(GameObject pGO)
    {
        foreach (GameObject go in ListOfCharacters)
            if (go != pGO)
                go.GetComponent<TTLobbyUICharacterItem>().DeSelectCharacter(TTPlayer.localPlayer);

        pGO.GetComponent<TTLobbyUICharacterItem>().SelectCharacter(TTPlayer.localPlayer);
    }
}
