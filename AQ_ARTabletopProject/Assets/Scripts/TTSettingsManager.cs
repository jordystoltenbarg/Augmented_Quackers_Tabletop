using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class TTSettingsManager : MonoBehaviour
{
    public static event Action<TTPlayer> onTTPlayerAdded;
    public static event Action<TTPlayer> onTTPlayerRemoved;
    public static event Action<string> onPlayerNameChanged;
    public static event Action<bool> onServerPrivacyChanged;
    public static event Action onUpdateCall;

    public static TTSettingsManager Singleton;

    [Header("Profanity")]
    public string[] bannedWords;
    [Header("Program Related")]
    [SerializeField] private float _updateInterval = 0.2f;
    [Header("Audio Related")]
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField] [Range(20, 80)] private int _audioVolumeModifier = 60;
    public int AudioVolumeModifier => _audioVolumeModifier;

    public readonly List<TTPlayer> players = new List<TTPlayer>();
    private string _playerName = "";
    public string PlayerName => _playerName;
    private int _playerIndex = 0;
    public int PlayerIndex => _playerIndex;
    private string _serverCode = "";
    public string ServerCode => _serverCode;
    private Camera _lobbyCamera = null;
    public Camera LobbyCamera => _lobbyCamera;
    private GameObject _inGameCamera = null;
    public GameObject InGameCamera => _inGameCamera;

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
        Invoke(nameof(adjustSoundVolume), 0.2f);
        StartCoroutine(updateCall());

        _lobbyCamera = GameObject.Find("LobbyCamera").GetComponent<Camera>();
        _inGameCamera = GameObject.Find("AR Session Origin");
        _inGameCamera.gameObject.SetActive(false);
    }

    public void AddPlayer(TTPlayer pPlayer)
    {
        if (!players.Contains(pPlayer))
        {
            players.Add(pPlayer);
            onTTPlayerAdded?.Invoke(pPlayer);
        }
    }

    public void RemovePlayer(TTPlayer pPlayer)
    {
        if (players.Contains(pPlayer))
        {
            players.Remove(pPlayer);
            onTTPlayerRemoved?.Invoke(pPlayer);
        }
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
        yield return new WaitWhile(() => TTApiUpdater.Singleton == null);

        if (PlayerNameInputfieldIdentifier.ValidateName(PlayerPrefs.GetString("PlayerName")))
            ChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        else
            ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
    }

    private IEnumerator delayedSelectLanguage()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => LocalizationSettings.AvailableLocales.Locales.Count > 0);
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

    private void adjustSoundVolume()
    {
        float volume = 0;
        volume = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetInt("SFX") * 0.1f, 0.0001f, 1f)) * _audioVolumeModifier;
        _audioMixer.SetFloat("sfxVolume", volume);

        volume = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetInt("Music") * 0.1f, 0.0001f, 1f)) * _audioVolumeModifier;
        _audioMixer.SetFloat("musicVolume", volume);

        volume = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetInt("Dialogue") * 0.1f, 0.0001f, 1f)) * _audioVolumeModifier;
        _audioMixer.SetFloat("dialogueVolume", volume);
    }

    private IEnumerator updateCall()
    {
        while (true)
        {
            yield return new WaitForSeconds(_updateInterval);
            onUpdateCall?.Invoke();
        }
    }
}
