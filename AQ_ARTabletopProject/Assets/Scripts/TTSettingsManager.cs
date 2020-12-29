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

    [Header("FPS")]
    [SerializeField] private bool _vSYNC = true;
    [SerializeField] private int _FPSCap = 60;
    [Header("Profanity")]
    public string[] bannedWords;
    [Header("Program Related")]
    [SerializeField] private float _updateInterval = 0.2f;
    [Header("Audio Related")]
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField] [Range(20, 80)] private int _audioVolumeModifier = 60;
    public int AudioVolumeModifier => _audioVolumeModifier;
    [SerializeField] [Range(0, 10)] private int _defaultSFXVolume = 5;
    [SerializeField] [Range(0, 10)] private int _defaultMusicVolume = 5;
    [SerializeField] [Range(0, 10)] private int _defaultDialogueVolume = 5;

    public readonly List<TTPlayer> players = new List<TTPlayer>();
    private string _playerName = "";
    public string PlayerName => _playerName;
    private int _playerIndex = 0;
    public int PlayerIndex => _playerIndex;
    private string _serverCode = "";
    public string ServerCode => _serverCode;
    private Camera _lobbyCamera = null;
    public Camera LobbyCamera => _lobbyCamera;
    private Camera _inGameCamera = null;
    public Camera InGameCamera => _inGameCamera;

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

        StartCoroutine(delayedChangeName());
        StartCoroutine(delayedSelectLanguage());
        Invoke(nameof(adjustSoundVolume), 0.2f);
        StartCoroutine(updateCall());

        _lobbyCamera = GameObject.Find("LobbyCamera").GetComponent<Camera>();
        _inGameCamera = GameObject.Find("In-GameCamera").GetComponent<Camera>();
        _inGameCamera.gameObject.SetActive(false);

        Application.targetFrameRate = _FPSCap;
        QualitySettings.vSyncCount = (_vSYNC) ? 1 : 0;
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

    private IEnumerator delayedChangeName()
    {
        yield return new WaitWhile(() => TTApiUpdater.Singleton == null);

        if (!PlayerPrefs.HasKey("PlayerName"))
            ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
        else if (PlayerNameInputfieldIdentifier.ValidateName(PlayerPrefs.GetString("PlayerName")))
            ChangePlayerName(PlayerPrefs.GetString("PlayerName"));
        else
            ChangePlayerName($"Player {UnityEngine.Random.Range(10000000, 99999999)}");
    }

    private IEnumerator delayedSelectLanguage()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => LocalizationSettings.AvailableLocales.Locales.Count > 0);

        if (!PlayerPrefs.HasKey("Langauge"))
            PlayerPrefs.SetString("Langauge", ApplicationLanguage.Dutch.ToString());

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

        if (!PlayerPrefs.HasKey("SFX"))
            PlayerPrefs.SetInt("SFX", 5);
        volume = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetInt("SFX") * 0.1f, 0.0001f, 1f)) * _audioVolumeModifier;
        _audioMixer.SetFloat("sfxVolume", volume);

        if (!PlayerPrefs.HasKey("Music"))
            PlayerPrefs.SetInt("Music", 5);
        volume = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetInt("Music") * 0.1f, 0.0001f, 1f)) * _audioVolumeModifier;
        _audioMixer.SetFloat("musicVolume", volume);

        if (!PlayerPrefs.HasKey("Dialogue"))
            PlayerPrefs.SetInt("Dialogue", 5);
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
