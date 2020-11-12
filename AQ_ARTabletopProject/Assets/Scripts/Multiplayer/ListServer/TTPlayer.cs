using Mirror;
using TMPro;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer localPlayer;

    [SyncVar] private string _playerName = "";
    public string playerName => _playerName;
    [SyncVar] private bool _isReady = false;
    public bool isReady => _isReady;

    private TTNetworkManagerListServer _manager;

    private void Start()
    {
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
    }

    private void OnDestroy()
    {
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
            localPlayer = this;
    }

    public void ChangePlayerName(string pNewName)
    {
        CmdChangePlayerName(pNewName);
    }

    [Command]
    private void CmdChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
        print(_playerName);
    }
}
