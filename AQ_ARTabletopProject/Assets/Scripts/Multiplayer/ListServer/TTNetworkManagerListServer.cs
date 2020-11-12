using System;
using Mirror;

/// <summary>
/// Network Manager with events that are used by the list server
/// </summary>
public class TTNetworkManagerListServer : NetworkManager
{
    /// <summary>
    /// Called when Server Starts
    /// </summary>
    public event Action onServerStarted;

    /// <summary>
    /// Called when Server Stops
    /// </summary>
    public event Action onServerStopped;

    /// <summary>
    /// Called when Client Starts
    /// </summary>
    public event Action onClientStarted;

    /// <summary>
    /// Called when players leaves or joins the room
    /// </summary>
    public event OnPlayerListChanged onPlayerListChanged;
    public delegate void OnPlayerListChanged(int pPlayerCount);

    private int _connectionCount => NetworkServer.connections.Count;

    public override void OnServerConnect(NetworkConnection pConnnection)
    {
        int count = _connectionCount;
        if (count > maxConnections)
        {
            pConnnection.Disconnect();
            return;
        }

        onPlayerListChanged?.Invoke(count);
    }

    public override void OnServerDisconnect(NetworkConnection pConnection)
    {
        base.OnServerDisconnect(pConnection);
        onPlayerListChanged?.Invoke(_connectionCount);
    }

    public override void OnStartServer()
    {
        onServerStarted?.Invoke();
    }

    public override void OnStopServer()
    {
        onServerStopped?.Invoke();
    }

    public override void OnStartClient()
    {
        onClientStarted?.Invoke();
    }
}