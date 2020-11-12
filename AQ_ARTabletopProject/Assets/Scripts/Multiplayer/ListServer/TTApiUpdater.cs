using System;
using Mirror;
using Mirror.Cloud.ListServerService;
using UnityEngine;

/// <summary>
/// This component should be put on the NetworkManager object
/// </summary>
public class TTApiUpdater : MonoBehaviour
{
    private static readonly ILogger _logger = LogFactory.GetLogger<TTApiUpdater>();

    private TTNetworkManagerListServer _manager;
    private TTApiConnector _apiConnector;
    public string gameName = "Game";

    private void Start()
    {
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
        _apiConnector = _manager.GetComponent<TTApiConnector>();

        _manager.onPlayerListChanged += onPlayerListChanged;
        _manager.onServerStarted += serverStartedHandler;
        _manager.onServerStopped += serverStoppedHandler;
    }

    private void OnDestroy()
    {
        _manager.onPlayerListChanged -= onPlayerListChanged;
        _manager.onServerStarted -= serverStartedHandler;
        _manager.onServerStopped -= serverStoppedHandler;
    }

    private void onPlayerListChanged(int pPlayerCount)
    {
        if (_apiConnector.ListServer.ServerApi.ServerInList)
        {
            //Update player count so that other players can see
            if (pPlayerCount < _manager.maxConnections)
            {
                if (_logger.LogEnabled()) _logger.Log($"Updating Server, player count: {pPlayerCount} ");
                _apiConnector.ListServer.ServerApi.UpdateServer(pPlayerCount);
            }
            //Remove server when there is max players
            else
            {
                if (_logger.LogEnabled()) _logger.Log($"Removing Server, player count: {pPlayerCount}");
                _apiConnector.ListServer.ServerApi.RemoveServer();
            }
        }
        else
        {
            //If not in list, and player counts drops below 2, add server to list
            if (pPlayerCount < _manager.maxConnections - 1)
            {
                if (_logger.LogEnabled()) _logger.Log($"Adding Server, player count: {pPlayerCount}");
                addServer(pPlayerCount);
            }
        }
    }

    private void serverStartedHandler()
    {
        addServer(0);
    }

    private void addServer(int pPlayerCount)
    {
        Transport transport = Transport.activeTransport;

        Uri uri = transport.ServerUri();
        int port = uri.Port;
        string protocol = uri.Scheme;

        _apiConnector.ListServer.ServerApi.AddServer(new ServerJson
        {
            displayName = $"{TTSettingsManager.singleton.playerName}",
            protocol = protocol,
            port = port,
            maxPlayerCount = NetworkManager.singleton.maxConnections,
            playerCount = pPlayerCount
        });
    }

    private void serverStoppedHandler()
    {
        _apiConnector.ListServer.ServerApi.RemoveServer();
    }
}