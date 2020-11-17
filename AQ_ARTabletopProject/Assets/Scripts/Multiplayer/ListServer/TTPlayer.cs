using System;
using Mirror;
using Mirror.Cloud.ListServerService;
using TMPro;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer localPlayer;

    [SyncVar] private int _lobbyIndex = 0;
    public int lobbyIndex => _lobbyIndex;
    [SyncVar] private string _playerName = "";
    public string playerName => _playerName;
    [SyncVar] private bool _isReady = false;
    public bool isReady => _isReady;

    private TTNetworkManagerListServer _manager;

    private void Awake()
    {
        if (TTSettingsManager.singleton)
            _playerName = TTSettingsManager.singleton.playerName;
    }

    private void Start()
    {
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
        _manager.onHostStarted += onHostStarted;
        _manager.onHostStopped += onHostStopped;
    }

    private void OnDestroy()
    {
        _manager.onHostStarted -= onHostStarted;
        _manager.onHostStopped -= onHostStopped;
    }

    /// <summary>
    /// Called when a client joins a Host or when a Hosted game is created
    /// </summary>
    public override void OnStartClient()
    {
        if (!isLocalPlayer) return;

        localPlayer = this;
        cmdSetPlayerIndex(TTSettingsManager.singleton.playerIndex);
    }

    private void onHostStarted()
    {
    }

    private void onHostStopped()
    {
    }

    [Command]
    private void cmdSetPlayerIndex(int pIndex)
    {
        _lobbyIndex = pIndex;
        TargetWhatisIndex();
    }

    [TargetRpc]
    private void TargetWhatisIndex()
    {
        print(lobbyIndex);
    }

    public void ChangePlayerName(string pNewName)
    {
        cmdChangePlayerName(pNewName);
    }

    [Command]
    private void cmdChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
        TargetNAme();
    }

    [TargetRpc]
    public void TargetNAme()
    {
        print(_playerName);
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
}
