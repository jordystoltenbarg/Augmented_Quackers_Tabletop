using System;
using UnityEngine;

public class TTSettingsManager : MonoBehaviour
{
    public static event Action<string> onPlayerNameChanged;

    public string[] bannedWords;

    public static TTSettingsManager Singleton;

    private string _playerName = "";
    public string PlayerName => _playerName;
    private int _playerIndex = 0;
    public int PlayerIndex => _playerIndex;

    private void Awake()
    {
        TTSettingsManager[] ttSMs = FindObjectsOfType<TTSettingsManager>();
        if (ttSMs.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);

        ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
    }

    public void ChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
        onPlayerNameChanged?.Invoke(pNewName);
    }

    public void SetPlayerIndex(int pIndex)
    {
        _playerIndex = pIndex;
    }
}
