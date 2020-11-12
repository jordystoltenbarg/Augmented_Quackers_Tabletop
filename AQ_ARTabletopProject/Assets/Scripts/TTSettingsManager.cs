using UnityEngine;

public class TTSettingsManager : MonoBehaviour
{
    public string[] bannedWords;

    public static TTSettingsManager singleton;

    private string _playerName = "";
    public string playerName => _playerName;

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
        if (TTPlayer.localPlayer)
            TTPlayer.localPlayer.ChangePlayerName(pNewName);
    }
}
