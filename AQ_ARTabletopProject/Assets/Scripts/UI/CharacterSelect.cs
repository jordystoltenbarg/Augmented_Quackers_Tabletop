using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static Action OnSelectedCharacterChanged;
    public static readonly List<GameObject> ListOfCharacters = new List<GameObject>();

    private Color _transparent = new Color(1, 1, 1, 0);

    private GameObject _lastButtonClicked = null;

    private void Start()
    {
        updateList();

        for (int i = 0; i < ListOfCharacters.Count; i++)
        {
            GameObject go = ListOfCharacters[i];
            go.GetComponent<TTLobbyUICharacterItem>().characterIndex = i;
            go.GetComponent<Button>().onClick.AddListener(() => highlightCharacter(go));
        }

        TTSettingsManager.onTTPlayerAdded += onTTPlayerAdded;
        TTSettingsManager.onTTPlayerRemoved += onTTPlayerRemoved;
    }

    private void updateList()
    {
        ListOfCharacters.Clear();

        for (int i = 0; i < transform.childCount; i++)
            ListOfCharacters.Add(transform.GetChild(i).gameObject);
    }

    private void highlightCharacter(GameObject pGO)
    {
        if (ReadyUp.IsLocalPlayerReady) return;

        //foreach (GameObject go in ListOfCharacters)
        //    if (go != pGO)
        //        go.GetComponent<TTLobbyUICharacterItem>().DeSelectCharacter(TTPlayer.LocalPlayer);

        pGO.GetComponent<TTLobbyUICharacterItem>().SelectCharacter(TTPlayer.LocalPlayer);
        _lastButtonClicked = pGO;
        //OnSelectedCharacterChanged?.Invoke();
    }

    private void onTTPlayerAdded(TTPlayer pPlayer)
    {
        if (!pPlayer.isLocalPlayer) return;

        pPlayer.onCharacterSelectApproved += onCharacterSelectApproved;
        pPlayer.onCharacterSelectDenied += onCharacterSelectDenied;
    }

    private void onTTPlayerRemoved(TTPlayer pPlayer)
    {
        if (!pPlayer.isLocalPlayer) return;

        pPlayer.onCharacterSelectApproved -= onCharacterSelectApproved;
        pPlayer.onCharacterSelectDenied -= onCharacterSelectDenied;
    }

    private void onCharacterSelectApproved(int pCharacterIndex)
    {
        foreach (GameObject go in ListOfCharacters)
            if (go != _lastButtonClicked)
                go.GetComponent<TTLobbyUICharacterItem>().DeSelectCharacter(TTPlayer.LocalPlayer);

        OnSelectedCharacterChanged?.Invoke();
    }

    private void onCharacterSelectDenied(int pCharacterIndex)
    {
        foreach (GameObject go in ListOfCharacters)
            if (go != _lastButtonClicked)
                go.GetComponent<TTLobbyUICharacterItem>().DeSelectCharacter(TTPlayer.LocalPlayer);

        OnSelectedCharacterChanged?.Invoke();
    }
}
