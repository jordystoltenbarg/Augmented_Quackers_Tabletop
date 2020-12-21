using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Cloud.ListServerService;
using UnityEngine;
using TMPro;
/// <summary>
/// This component should be put on the NetworkManager object
/// </summary>
public class TTApiUpdater : MonoBehaviour
{
    public static TTApiUpdater Singleton = null;
    private static readonly ILogger _logger = LogFactory.GetLogger<TTApiUpdater>();

    private TTNetworkManagerListServer _manager;
    private TTApiConnector _apiConnector;
    private string _gameName = "";
    public string GameName => _gameName;
    private bool _isPrivateServer = false;
    public bool IsPrivateServer => _isPrivateServer;

    public readonly List<ServerJson> serverList = new List<ServerJson>();

    private void Awake()
    {
        TTApiUpdater[] ttApiUs = FindObjectsOfType<TTApiUpdater>();
        if (ttApiUs.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        _manager = NetworkManager.singleton as TTNetworkManagerListServer;
        _apiConnector = _manager.GetComponent<TTApiConnector>();

        _manager.onPlayerListChanged += onPlayerListChanged;
        _manager.onServerStarted += serverStartedHandler;
        _manager.onServerStopped += serverStoppedHandler;
        _apiConnector.ListServer.ClientApi.onServerListUpdated += updateListOfServers;
        TTSettingsManager.onPlayerNameChanged += updateGameName;
        TTSettingsManager.onServerPrivacyChanged += updatePrivacy;
    }

    private void OnDestroy()
    {
        _manager.onPlayerListChanged -= onPlayerListChanged;
        _manager.onServerStarted -= serverStartedHandler;
        _manager.onServerStopped -= serverStoppedHandler;
        _apiConnector.ListServer.ClientApi.onServerListUpdated -= updateListOfServers;
        TTSettingsManager.onPlayerNameChanged -= updateGameName;
        TTSettingsManager.onServerPrivacyChanged -= updatePrivacy;
    }

    private void updateListOfServers(ServerCollectionJson pServerCollection)
    {
        serverList.Clear();
        foreach (ServerJson server in pServerCollection.servers)
            serverList.Add(server);
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
                _apiConnector.ListServer.ServerApi.UpdateServer(pPlayerCount);
                _apiConnector.ListServer.ServerApi.RemoveServer();
            }
        }
        else
        {
            //If not in list, and player counts drops below 2, add server to list
            if (pPlayerCount < _manager.maxConnections)
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
        DarkReflectiveMirrorTransport transport = Transport.activeTransport as DarkReflectiveMirrorTransport;

        Uri uri = transport.ServerUri();
        int port = uri.Port;
        string protocol = uri.Scheme;

        ServerJson server = new ServerJson
        {
            displayName = $"{_gameName}",
            protocol = protocol,
            port = port,
            maxPlayerCount = NetworkManager.singleton.maxConnections,
            playerCount = pPlayerCount,
        };
        Dictionary<string, string> customDataDict = new Dictionary<string, string>
        {
            { "serverID", transport.serverID.ToString() },
            { "private", (_isPrivateServer) ? true.ToString() : false.ToString() },
            { "serverCode", getRandomServerCode() }
        };
        server.SetCustomData(customDataDict);

        _apiConnector.ListServer.ServerApi.AddServer(server);
        TTSettingsManager.Singleton.SetServerCode(customDataDict["serverCode"]);
    }

    private void serverStoppedHandler()
    {
        _apiConnector.ListServer.ServerApi.RemoveServer();
    }

    private void updateGameName(string pNewName)
    {
        _gameName = pNewName;
        _apiConnector.ListServer.ServerApi.UpdateServer(_gameName);
    }

    private void updatePrivacy(bool pPrivacy)
    {
        _isPrivateServer = pPrivacy;
        _apiConnector.ListServer.ServerApi.UpdateServerCustomDataValue("private", pPrivacy.ToString());
    }

    public int GetServerPlayerCount()
    {
        return _apiConnector.ListServer.ServerApi.GetServerPlayerCount();
    }

    public List<GameObject> GetPlayersInServer()
    {
        return _apiConnector.ListServer.ServerApi.GetPlayersInServer();
    }

    private string getRandomServerCode()
    {
        string code = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, 26);
            if (random < 26)
                code += (char)(random + 65);
        }

        //Make sure the code is unique
        foreach (ServerJson server in serverList)
        {
            for (int i = 0; i < server.customData.Length; i++)
            {
                if (server.customData[i].key == "serverCode" &&
                    server.customData[i].value.ToUpper() == code.ToUpper())
                {
                    code = getRandomServerCode();
                    break;
                }
            }
        }

        return code;
    }
}