using System;
using UnityEngine;

public class TTSettingsManager : MonoBehaviour
{
    public static event Action<string> onPlayerNameChanged;

    public string[] bannedWords;

    public static TTSettingsManager singleton;

    private string _playerName = "";
    public string playerName => _playerName;
    private int _playerIndex = 0;
    public int playerIndex => _playerIndex;

    private void Awake()
    {
        TTSettingsManager[] ttSMs = FindObjectsOfType<TTSettingsManager>();
        if (ttSMs.Length > 1)
            Destroy(gameObject);

        singleton = this;
        DontDestroyOnLoad(gameObject);
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
