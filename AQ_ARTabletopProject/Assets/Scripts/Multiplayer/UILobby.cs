using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    public static UILobby instance;

    [Header("Host Join")]

    [SerializeField] TMP_InputField joinMatchInput;
    [SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas searchCanvas;
    [SerializeField] GameObject Lobby_UI;


    [Header("Lobby")]

    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] TMP_Text matchIDText;
    [SerializeField] GameObject startGameButton;

    GameObject playerLobbyUI;

    bool searching = false;

    void Start()
    {
        instance = this;
    }

    public void HostPrivate()
    {
        joinMatchInput.interactable = false;

        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(false);
    }
    public void HostPublic()
    {
        joinMatchInput.interactable = false;

        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(true);
    }

    public void HostSucces(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;

            if (playerLobbyUI != null) Destroy(playerLobbyUI);
            playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
            startGameButton.SetActive(true);
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public void Join()
    {
        joinMatchInput.interactable = false;
        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
    }

    public void JoinSucces(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            startGameButton.SetActive(false);

            playerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
        }
        else
        {
            joinMatchInput.interactable = true;
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public GameObject SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
        return newUIPlayer;
    }

    public void StartGame()
    {
        Lobby_UI.SetActive(false);
        Player.localPlayer.StartGame();
    }

    public void SearchGame()
    {
        Debug.Log($"Searching for game");
        searchCanvas.enabled = true;
        StartCoroutine(SearchingForGame());
    }

    IEnumerator SearchingForGame()
    {
        searching = true;

        float currentTime = 1;
        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 1;
                Player.localPlayer.SearchGame();
            }
            yield return null;
        }
    }
    public void SearchSucces(bool success, string matchID)
    {
        if (success)
        {
            searchCanvas.enabled = false;
            JoinSucces(success, matchID);
            searching = false;
        }
    }

    public void SearchCancel()
    {
        searchCanvas.enabled = false;
        searching = false;
        lobbySelectables.ForEach(x => x.interactable = true);
    }

    public void DisconnectLobby()
    {
        if (playerLobbyUI != null) Destroy(playerLobbyUI);
        Player.localPlayer.DisconnectGame();

        lobbyCanvas.enabled = false;
        lobbySelectables.ForEach(x => x.interactable = true);
        startGameButton.SetActive(false);
    }

}
