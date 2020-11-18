using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer localPlayer;

    [SyncVar] private int _lobbyIndex = -1;
    public int lobbyIndex => _lobbyIndex;
    [SyncVar] private string _playerName = "";
    public string playerName => _playerName;
    [SyncVar] private bool _isReady = false;
    public bool isReady => _isReady;
    [SyncVar] private int _selectedCharacterIndex = -1;
    public int selectedCharacterIndex => _selectedCharacterIndex;
    [SyncVar] private List<GameObject> _playersInCurrentServer = null;
    public List<GameObject> playersInCurrentServer => _playersInCurrentServer;

    [SerializeField] private bool _autoUpdatePlayerListInCurrentServer = true;
    [SerializeField] private float _playerListInCurrentServerUpdateInterval = 1.0f;

    private TTNetworkManagerListServer _manager = null;

    private void Start()
    {
        Invoke("lobbyUIAddPlayer", 0.5f);
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer) return;

        TTSettingsManager.onPlayerNameChanged -= ChangePlayerName;

        cmdRemovePlayerFromList(gameObject);

        FindObjectOfType<MenuManager>().GoToPreLobby();

        localPlayer = null;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
    }

    private void lobbyUIAddPlayer()
    {
        TTLobbyUI.singleton.AddPlayer(this);
    }

    /// <summary>
    /// Called when a client joins a Host or when a Hosted game is created
    /// </summary>
    public override void OnStartClient()
    {
        if (!isLocalPlayer) return;

        localPlayer = this;
        FindObjectOfType<MenuManager>().JoinLobby();

        initLocalPlayer();
    }

    public override void OnStopClient()
    {
        TTLobbyUI.singleton.RemovePlayer(this);
    }

    private void initLocalPlayer()
    {
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
        TTSettingsManager.onPlayerNameChanged += ChangePlayerName;

        Invoke("cmdSetPlayerIndex", 0.5f);
        cmdChangePlayerName(TTSettingsManager.singleton.playerName);
        cmdAddPToList(gameObject);

        //StartCoroutine(updatePlayerListInCurrentServer(_playerListInCurrentServerUpdateInterval));
    }

    [Command]
    private void cmdAddPToList(GameObject pGO)
    {
        TTApiUpdater.apiUpdater.GetPlayersInServer().Add(pGO);
    }

    [Command]
    private void cmdRemovePlayerFromList(GameObject pGO)
    {
        TTApiUpdater.apiUpdater.GetPlayersInServer().Remove(pGO);
    }

    public void GetPlayersInCurrentServer()
    {
        cmdGetPlayersInCurrentServer();
    }

    [Command]
    private void cmdGetPlayersInCurrentServer()
    {
        _playersInCurrentServer = new List<GameObject>(TTApiUpdater.apiUpdater.GetPlayersInServer());
    }

    private IEnumerator updatePlayerListInCurrentServer(float pInterval)
    {
        while (_autoUpdatePlayerListInCurrentServer)
        {
            yield return new WaitForSeconds(pInterval);
            GetPlayersInCurrentServer();
        }
    }

    public void ChangePlayerName(string pNewName)
    {
        cmdChangePlayerName(pNewName);
    }

    [Command]
    private void cmdChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
    }

    [Command]
    private void cmdSetPlayerIndex()
    {
        _lobbyIndex = TTApiUpdater.apiUpdater.GetServerPlayerCount() - 1;
    }

    public void UpdateHigherLobbyIndex(int pLobbyIndex)
    {
        if (_lobbyIndex > pLobbyIndex)
            cmdUpdateHigherLobbyIndex();
    }

    [Command]
    private void cmdUpdateHigherLobbyIndex()
    {
        _lobbyIndex--;
    }

    public void ReadyToggle()
    {
        cmdReadyToggle();
    }

    [Command]
    private void cmdReadyToggle()
    {
        _isReady = !_isReady;
        if (_isReady)
        {
            //When Ready
        }
        else
        {
            //When Un-Ready
        }
    }

    public void SelectCharacter(int pCharacterIndex)
    {
        cmdSelectCharacter(pCharacterIndex);
    }

    [Command]
    private void cmdSelectCharacter(int pCharacterIndex)
    {
        _selectedCharacterIndex = pCharacterIndex;
    }
}
