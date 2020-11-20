using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer LocalPlayer;

    [SyncVar] private int _lobbyIndex = -1;
    public int LobbyIndex => _lobbyIndex;
    [SyncVar] private string _playerName = "";
    public string PlayerName => _playerName;
    [SyncVar] private bool _isReady = false;
    public bool IsReady => _isReady;
    [SyncVar] private int _selectedCharacterIndex = -1;
    public int SelectedCharacterIndex => _selectedCharacterIndex;
    [SyncVar] private int _colorVariation = -1;
    public int ColorVariation => _colorVariation;
    [SyncVar] private List<GameObject> _playersInCurrentServer = null;
    public List<GameObject> PlayersInCurrentServer => _playersInCurrentServer;

    [SerializeField] private bool _autoUpdatePlayerListInCurrentServer = true;
    [SerializeField] private float _playerListInCurrentServerUpdateInterval = 1.0f;

    private TTNetworkManagerListServer _manager = null;

    private void Start()
    {
        Invoke(nameof(lobbyUIAddPlayer), 0.5f);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer) return;

        TTSettingsManager.onPlayerNameChanged -= ChangePlayerName;

        LocalPlayer = null;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
    }

    private void lobbyUIAddPlayer()
    {
        TTLobbyUI.Singleton.AddPlayer(this);
    }

    /// <summary>
    /// Called when a client joins a Host or when a Hosted game is created
    /// </summary>
    public override void OnStartClient()
    {
        if (!isLocalPlayer) return;

        LocalPlayer = this;
        FindObjectOfType<MenuManager>().GoToLobby();

        initLocalPlayer();
    }

    public override void OnStopClient()
    {
        if (this == LocalPlayer) return;
        TTLobbyUI.Singleton.RemovePlayer(this);
    }

    private void initLocalPlayer()
    {
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
        TTSettingsManager.onPlayerNameChanged += ChangePlayerName;

        Invoke(nameof(cmdSetPlayerIndex), 0.5f);
        Invoke(nameof(cmdSetPlayerColorVariation), 0.5f);
        cmdChangePlayerName(TTSettingsManager.Singleton.PlayerName);
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

    public void UpdateColorVariation(int pNewColorVariation)
    {
        cmdUpdatePlayerColorVariation(pNewColorVariation);
    }

    [Command]
    private void cmdUpdatePlayerColorVariation(int pNewColorVariation)
    {
        _colorVariation = pNewColorVariation;
    }

    [Command]
    private void cmdSetPlayerColorVariation()
    {
        _colorVariation = TTApiUpdater.apiUpdater.GetServerPlayerCount() - 1;
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
