using System;
using System.Collections;
using UnityEngine;

public class TTSettingsManager : MonoBehaviour
{
    public static event Action<string> onPlayerNameChanged;
    public static event Action<bool> onServerPrivacyChanged;

    public string[] bannedWords;

    public static TTSettingsManager Singleton;

    private string _playerName = "";
    public string PlayerName => _playerName;
    private int _playerIndex = 0;
    public int PlayerIndex => _playerIndex;
    private string _serverCode = "";
    public string ServerCode => _serverCode;

    private void Awake()
    {
        TTSettingsManager[] ttSMs = FindObjectsOfType<TTSettingsManager>();
        if (ttSMs.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(delayedChangeNameForAPIToUpdate());
    }

    public void ChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
        onPlayerNameChanged?.Invoke(pNewName);
    }

    private IEnumerator delayedChangeNameForAPIToUpdate()
    {
        yield return new WaitWhile(() => TTApiUpdater.apiUpdater == null);
        ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
    }

    public void SetServerCode(string pCode)
    {
        _serverCode = pCode;
    }

    public void ToggleServerPrivacySetting(bool pIsPrivate)
    {
        onServerPrivacyChanged?.Invoke(pIsPrivate);
    }
}
