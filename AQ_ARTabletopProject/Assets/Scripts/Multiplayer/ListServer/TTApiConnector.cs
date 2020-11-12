using Mirror.Cloud;
using Mirror.Cloud.ListServerService;
using UnityEngine;

/// <summary>
/// Used to requests and responses from the mirror api
/// </summary>
public interface ITTApiConnector
{
    ListServer ListServer { get; }
}

/// <summary>
/// Used to requests and responses from the mirror api
/// </summary>
[DisallowMultipleComponent]
public class TTApiConnector : MonoBehaviour, ITTApiConnector, ICoroutineRunner
{
    #region Inspector
    [Header("Settings")]
    [SerializeField] private string _apiAddress = "";
    [SerializeField] private string _apiKey = "";

    [Header("Events")]
    [SerializeField] ServerListEvent _onServerListUpdated = new ServerListEvent();
    #endregion

    private IRequestCreator _requestCreator;

    public ListServer ListServer { get; private set; }

    void Awake()
    {
        _requestCreator = new RequestCreator(_apiAddress, _apiKey, this);
        InitListServer();
    }

    void InitListServer()
    {
        IListServerServerApi serverApi = new ListServerServerApi(this, _requestCreator);
        IListServerClientApi clientApi = new ListServerClientApi(this, _requestCreator, _onServerListUpdated);
        ListServer = new ListServer(serverApi, clientApi);
    }

    public void OnDestroy()
    {
        ListServer.ServerApi.Shutdown();
        ListServer.ClientApi.Shutdown();
    }
}