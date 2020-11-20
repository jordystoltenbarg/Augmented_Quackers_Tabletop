using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Uses the ApiConnector on NetworkManager to update the Server list
/// </summary>
public class TTServerListManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TTServerListUI _serverListUI = null;

    [Header("Buttons")]
    public Button joinButton = null;
    [SerializeField] private Button _refreshButton = null;
    [SerializeField] private Button _startHostButton = null;

    [Header("Auto Refresh")]
    [SerializeField] private bool _autoRefreshServerlist = false;
    [SerializeField] private int _refreshinterval = 20;

    private TTApiConnector _apiConnector;

    void Start()
    {
        NetworkManager manager = NetworkManager.singleton;
        _apiConnector = manager.GetComponent<TTApiConnector>();
        _apiConnector.ListServer.ClientApi.onServerListUpdated += _serverListUI.UpdateList;

        if (_autoRefreshServerlist)
            _apiConnector.ListServer.ClientApi.StartGetServerListRepeat(_refreshinterval);

        AddButtonHandlers();
    }

    void AddButtonHandlers()
    {
        //_refreshButton.onClick.AddListener(refreshButtonHandler);
        _startHostButton.onClick.AddListener(startHostButtonHandler);
    }

    void OnDestroy()
    {
        if (_apiConnector == null)
            return;

        if (_autoRefreshServerlist)
            _apiConnector.ListServer.ClientApi.StopGetServerListRepeat();

        _apiConnector.ListServer.ClientApi.onServerListUpdated -= _serverListUI.UpdateList;
    }

    private void refreshButtonHandler()
    {
        _serverListUI.allowDraw = true;
        _apiConnector.ListServer.ClientApi.GetServerList();
    }

    public void Refresh()
    {
        _serverListUI.allowDraw = true;
        _apiConnector.ListServer.ClientApi.GetServerList();
    }

    private void startHostButtonHandler()
    {
        NetworkManager.singleton.StartHost();
    }
}