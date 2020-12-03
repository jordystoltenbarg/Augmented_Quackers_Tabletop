using Mirror;
using Mirror.Cloud.ListServerService;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Button _refreshButton = null;
    [SerializeField] private Button _startHostButton = null;
    public Button joinButton = null;
    [SerializeField] private Button _joinWithCodeButton = null;

    private TTApiConnector _apiConnector;
    private readonly float _refreshTimer = 3.0f;
    private bool _canRefreshServerList = true;
    private bool _isDirty = false;

    void Start()
    {
        NetworkManager manager = NetworkManager.singleton;
        _apiConnector = manager.GetComponent<TTApiConnector>();
        _apiConnector.ListServer.ClientApi.onServerListUpdated += _serverListUI.UpdateList;
        _apiConnector.ListServer.ClientApi.onServerListUpdated += clean;

        AddButtonHandlers();
    }

    void AddButtonHandlers()
    {
        _refreshButton.onClick.AddListener(refreshButtonHandler);
        _startHostButton.onClick.AddListener(startHostButtonHandler);
        _joinWithCodeButton.onClick.AddListener(joinWithCodeButtonHandler);
    }

    void OnDestroy()
    {
        if (_apiConnector == null)
            return;

        _apiConnector.ListServer.ClientApi.onServerListUpdated -= _serverListUI.UpdateList;
        _apiConnector.ListServer.ClientApi.onServerListUpdated -= clean;
    }

    private void refreshButtonHandler()
    {
        if (!_canRefreshServerList) return;

        _apiConnector.ListServer.ClientApi.GetServerList();

        _canRefreshServerList = false;
        Invoke(nameof(enableReflesh), _refreshTimer);
    }

    private void enableReflesh()
    {
        _canRefreshServerList = true;
    }

    public void Refresh()
    {
        _apiConnector.ListServer.ClientApi.GetServerList();
        _isDirty = true;
    }

    private void startHostButtonHandler()
    {
        NetworkManager.singleton.StartHost();
    }

    private void joinWithCodeButtonHandler()
    {
        StartCoroutine(joinServerWithCode());
        _joinWithCodeButton.interactable = false;
    }

    private IEnumerator joinServerWithCode()
    {
        Refresh();
        yield return new WaitUntil(() => !_isDirty);

        if (!joinWithCode())
            TTMessagePopup.Singleton.DisplayPopup(TTMessagePopup.PopupTitle.Error, TTMessagePopup.PopupMessage.Code, TTMessagePopup.PopupResponse.Ok);

        _joinWithCodeButton.interactable = true;
    }

    private bool joinWithCode()
    {
        string code = FindObjectOfType<ServerCodeInputfieldIdentifier>().inputField.text;

        if (string.IsNullOrEmpty(code)) return false;

        foreach (ServerJson server in TTApiUpdater.apiUpdater.serverList)
        {
            for (int i = 0; i < server.customData.Length; i++)
            {
                if (server.customData[i].key == "serverCode")
                {
                    if (server.customData[i].value.ToUpper() == code.ToUpper())
                    {
                        TTSettingsManager.Singleton.SetServerCode(code);
                        for (int j = 0; j < server.customData.Length; j++)
                        {
                            if (server.customData[j].key == "serverID")
                            {
                                NetworkManager.singleton.networkAddress = server.customData[j].value;
                                break;
                            }
                        }
                        NetworkManager.singleton.StartClient();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void clean(ServerCollectionJson pServers)
    {
        _isDirty = false;
    }
}