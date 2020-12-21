using Mirror;
using System;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer LocalPlayer;

    public Action<int> onCharacterSelectApproved;
    public Action<int> onCharacterSelectDenied;

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
    [SyncVar] private bool _hostedServerIsPrivate = false;
    public bool HostedServerIsPrivate => _hostedServerIsPrivate;
    [HideInInspector] public bool hasBeenKicked = false;

    private TTNetworkManagerListServer _manager = null;

    private void Start()
    {
        print($"<color=green> Hello there!</color>");

        Invoke(nameof(lobbyUIAddPlayer), 0.5f);
        DontDestroyOnLoad(gameObject);
    }

    private void lobbyUIAddPlayer()
    {
        TTSettingsManager.Singleton.AddPlayer(this);
    }

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = this;
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;

        TTSettingsManager.onPlayerNameChanged += changePlayerName;
        TTSettingsManager.onServerPrivacyChanged += changeServerPrivacy;

        FindObjectOfType<MenuManager>().GoToLobby();

        Invoke(nameof(cmdSetPlayerIndex), 0.2f);
        Invoke(nameof(cmdSetPlayerColorVariation), 0.2f);
        cmdChangePlayerName(TTSettingsManager.Singleton.PlayerName);
    }

    private void OnDestroy()
    {
        TTSettingsManager.Singleton.RemovePlayer(this);

        if (!isLocalPlayer) return;

        if (hasBeenKicked)
        {
            TTMessagePopup.Singleton.DisplayPopup(TTMessagePopup.PopupTitle.Notification, TTMessagePopup.PopupMessage.Kick, TTMessagePopup.PopupResponse.Ok);
        }

        ReadyUp.IsLocalPlayerReady = false;
        ReadyUp.CanStartGame = false;
        TTSettingsManager.onPlayerNameChanged -= changePlayerName;
        TTSettingsManager.onServerPrivacyChanged -= changeServerPrivacy;

        LocalPlayer = null;

        FindObjectOfType<MenuManager>().GoToPreLobby();
    }

    /// <summary>
    /// Called when a client joins a Host or when a Hosted game is created
    /// </summary>
    public override void OnStartClient()
    {
    }

    public override void OnStopClient()
    {
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
    }

    private void changePlayerName(string pNewName)
    {
        cmdChangePlayerName(pNewName);
    }

    [Command]
    private void cmdChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
    }

    private void changeServerPrivacy(bool pIsPrivate)
    {
        cmdChangeServerPrivacy(pIsPrivate);
    }

    [Command]
    private void cmdChangeServerPrivacy(bool pIsPrivate)
    {
        _hostedServerIsPrivate = pIsPrivate;
    }

    [Command]
    private void cmdSetPlayerIndex()
    {
        _lobbyIndex = TTApiUpdater.Singleton.GetServerPlayerCount() - 1;
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
        _colorVariation = TTApiUpdater.Singleton.GetServerPlayerCount() - 1;
    }

    public void ReadyToggle()
    {
        cmdReadyToggle();
    }

    [Command]
    private void cmdReadyToggle()
    {
        _isReady = !_isReady;
    }

    public void SelectCharacter(int pCharacterIndex)
    {
        cmdSelectCharacter(pCharacterIndex);
    }

    [Command]
    private void cmdSelectCharacter(int pCharacterIndex)
    {
        _selectedCharacterIndex = pCharacterIndex;
        targetCharacterSelectApproved(pCharacterIndex);
    }

    [TargetRpc]
    private void targetCharacterSelectApproved(int pCharacterIndex)
    {
        onCharacterSelectApproved?.Invoke(pCharacterIndex);
    }

    [TargetRpc]
    private void targetCharacterSelectDenied(int pCharacterIndex)
    {
        onCharacterSelectDenied?.Invoke(pCharacterIndex);
    }

    public void KickClient(int pLocalPlayerIndex)
    {
        cmdKickClient(pLocalPlayerIndex);
    }

    [Command]
    private void cmdKickClient(int pLocalPlayerIndex)
    {
        clientKick(pLocalPlayerIndex);
    }

    [ClientRpc]
    private void clientKick(int pLocalPlayerIndex)
    {
        if (LocalPlayer.LobbyIndex != pLocalPlayerIndex || LocalPlayer.hasBeenKicked) return;

        LocalPlayer.hasBeenKicked = true;
        NetworkManager.singleton.StopClient();
    }
}
