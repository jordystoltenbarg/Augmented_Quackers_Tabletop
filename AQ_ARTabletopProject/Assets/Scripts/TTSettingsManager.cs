using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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

    public enum ApplicationLanguage
    {
        Dutch,
        English
    }
    [HideInInspector] public ApplicationLanguage applicationLanguage;

    private void Awake()
    {
        TTSettingsManager[] ttSMs = FindObjectsOfType<TTSettingsManager>();
        if (ttSMs.Length > 1)
            Destroy(gameObject);

        Singleton = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(delayedChangeNameForAPIToUpdate());
        StartCoroutine(delayedSelectLanguage());
    }

    public void ChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
        onPlayerNameChanged?.Invoke(pNewName);
        PlayerPrefs.SetString("PlayerName", pNewName);
    }

    public void SetServerCode(string pCode)
    {
        _serverCode = pCode;
    }

    public void ToggleServerPrivacySetting(bool pIsPrivate)
    {
        onServerPrivacyChanged?.Invoke(pIsPrivate);
    }

    private IEnumerator delayedChangeNameForAPIToUpdate()
    {
        yield return new WaitWhile(() => TTApiUpdater.apiUpdater == null);

        if (PlayerNameInputfieldIdentifier.ValidateName(PlayerPrefs.GetString("PlayerName")))
            ChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        else
            ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
    }

    private IEnumerator delayedSelectLanguage()
    {
        yield return new WaitWhile(() => LocalizationSettings.AvailableLocales.Locales.Count == 0);
        string language = PlayerPrefs.GetString("Langauge");
        if (language == ApplicationLanguage.Dutch.ToString())
            SelectLanguage(ApplicationLanguage.Dutch);
        else if (language == ApplicationLanguage.English.ToString())
            SelectLanguage(ApplicationLanguage.English);
    }

    public void SelectLanguage(ApplicationLanguage pLanguage)
    {
        switch (pLanguage)
        {
            case ApplicationLanguage.Dutch:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
            case ApplicationLanguage.English:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                break;
        }

        applicationLanguage = pLanguage;
        PlayerPrefs.SetString("Langauge", pLanguage.ToString());
    }
}
