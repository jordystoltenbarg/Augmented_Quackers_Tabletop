using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField]
    private int fadeTime;

    [Header("Navigation Buttons")]
    [SerializeField]
    private Button _settingsButton;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Button _secondBackButton;

    [Header("Slider")]
    [SerializeField]
    private GameObject _slider;
    [SerializeField]
    private GameObject _preLobbyContent;
    [SerializeField]
    private GameObject _lobbyContent;

    [Header("Menu Screens")]
    [SerializeField]
    private GameObject _play;
    [SerializeField]
    private GameObject _collection;
    [SerializeField]
    private GameObject _preLobby;
    [SerializeField]
    private GameObject _lobby;
    [SerializeField]
    private GameObject _settings;
    [SerializeField]
    private GameObject _credits;

    private GameObject _mainMenu;

    private bool _isSliderOpen;

    public enum MenuState
    {
        Play,
        Collection,
        PreLobby,
        Lobby,
        Settings,
        Credits
    }
    private MenuState _previousState;
    public static MenuState CurrentMenuState;

    void Start()
    {
        _mainMenu = _play.transform.parent.gameObject;
        updateMenuState();
    }

    void OnEnable()
    {
        _settingsButton.onClick.AddListener(onSettingsButtonClick);
        _backButton.onClick.AddListener(onBackButtonClick);
        _secondBackButton.onClick.AddListener(onSecondBackButtonClick);
    }

    void OnDisable()
    {
        _settingsButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _secondBackButton.onClick.RemoveAllListeners();
    }

    void checkMenuState()
    {
        if (_mainMenu.activeSelf)
        {
            if (_play.activeSelf)
                CurrentMenuState = MenuState.Play;
            else if (_collection.activeSelf)
                CurrentMenuState = MenuState.Collection;
            else if (_preLobby.activeSelf)
                CurrentMenuState = MenuState.PreLobby;
            else if (_lobby.activeSelf)
                CurrentMenuState = MenuState.Lobby;
        }
        else if (_settings.activeSelf)
            CurrentMenuState = MenuState.Settings;
        else if (_credits.activeSelf)
            CurrentMenuState = MenuState.Credits;
    }

    void updateMenuState()
    {
        checkMenuState();

        switch (CurrentMenuState)
        {
            case MenuState.Play:
                navButtons(0);
                break;
            case MenuState.Collection:
                navButtons(2);
                break;
            case MenuState.PreLobby:
                navButtons(2);
                break;
            case MenuState.Lobby:
                navButtons(2);
                break;
            case MenuState.Settings:
                navButtons(1);
                break;
            case MenuState.Credits:
                navButtons(1);
                break;
            default:
                break;
        }
    }

    void navButtons(int pState)
    {
        switch (pState)
        {
            case 0:
                _settingsButton.gameObject.SetActive(true);
                _backButton.gameObject.SetActive(false);
                _secondBackButton.gameObject.SetActive(false);
                break;
            case 1:
                _settingsButton.gameObject.SetActive(false);
                _backButton.gameObject.SetActive(true);
                _secondBackButton.gameObject.SetActive(false);
                break;
            case 2:
                _settingsButton.gameObject.SetActive(true);
                _backButton.gameObject.SetActive(false);
                _secondBackButton.gameObject.SetActive(true);
                break;
        }
    }

    void onSettingsButtonClick()
    {
        disableNavButtons();

        _mainMenu.GetComponent<Animator>().SetTrigger("FadeOut");
        StartCoroutine(disableGOAfterAnimation(_mainMenu.GetComponent<Animator>(), showSettings, _mainMenu, false));

        _previousState = CurrentMenuState;
    }

    void showSettings()
    {
        _settings.SetActive(true);

        updateMenuState();
    }

    void onBackButtonClick()
    {
        disableNavButtons();

        if (CurrentMenuState == MenuState.Credits)
        {
            _credits.GetComponent<Animator>().SetTrigger("FadeOut");
            StartCoroutine(disableGOAfterAnimation(_credits.GetComponent<Animator>(), showSettings, _credits, false));
            return;
        }

        switch (_previousState)
        {
            case MenuState.Play:
                GoToPlay();
                break;
            case MenuState.PreLobby:
                GoToPreLobby();
                break;
            case MenuState.Lobby:
                GoToLobby();
                break;
        }
    }

    void onSecondBackButtonClick()
    {
        disableNavButtons();

        switch (CurrentMenuState)
        {
            case MenuState.PreLobby:
                GoToPlay();
                break;
            case MenuState.Lobby:
                GoToPreLobby();
                break;
        }
    }

    public void GoToPlay()
    {
        closeSlider();

        switch (CurrentMenuState)
        {
            case MenuState.PreLobby:
                _preLobby.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_preLobby.GetComponent<Animator>(), showPlay));

                disableNavButtons();
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showPlay, _settings, false));

                disableNavButtons();
                break;
        }
    }

    void showPlay()
    {
        if (!_mainMenu.activeSelf)
            _mainMenu.SetActive(true);

        _play.SetActive(true);

        updateMenuState();
    }

    public void GoToPreLobby()
    {

        switch (CurrentMenuState)
        {
            case MenuState.Play:
                _play.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_play.GetComponent<Animator>(), showPreLobby));

                disableNavButtons();
                break;
            case MenuState.Lobby:
                _lobby.GetComponent<Animator>().SetTrigger("FadeOut");
                _lobbyContent.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_lobby.GetComponent<Animator>(), showPreLobby));

                disableNavButtons();
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showPreLobby, _settings, false));

                disableNavButtons();
                break;
        }
    }

    void showPreLobby()
    {
        if (!_mainMenu.activeSelf)
        {
            _mainMenu.SetActive(true);
            _isSliderOpen = false;
        }

        _preLobby.SetActive(true);

        switch (CurrentMenuState)
        {
            case MenuState.Lobby:
                for (int i = 0; i < _lobby.transform.GetChild(0).childCount; i++)
                    _lobby.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                break;
        }

        updateMenuState();
        openSlider();
    }

    IEnumerator preLobbySearchForServer()
    {
        for (int i = 0; i < 1; i++)
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 5.0f));

        _preLobbyContent.transform.Find("Searching").gameObject.SetActive(false);
        _preLobbyContent.transform.Find("ServerListContainer").gameObject.SetActive(true);
        yield break;
    }

    public void GoToLobby()
    {

        switch (CurrentMenuState)
        {
            case MenuState.PreLobby:
                if (ServerSelect.SelectedServer == null) return;
                _preLobby.GetComponent<Animator>().SetTrigger("FadeOut");
                _preLobbyContent.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_preLobby.GetComponent<Animator>(), showLobby));

                disableNavButtons();
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showLobby, _settings, false));

                disableNavButtons();
                break;
        }
    }

    void showLobby()
    {
        if (!_mainMenu.activeSelf)
        {
            _mainMenu.SetActive(true);
            _isSliderOpen = false;
        }

        _lobby.SetActive(true);
        StartCoroutine(lobbyLoadPlayers());

        updateMenuState();
        openSlider();
    }

    IEnumerator lobbyLoadPlayers()
    {
        for (int i = 0; i < _lobby.transform.GetChild(0).childCount; i++)
        {
            _lobby.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 2.0f));
        }
        yield break;
    }

    void openSlider()
    {
        if (!_isSliderOpen)
            _slider.GetComponent<Animator>().SetTrigger("OpenSlider");

        switch (CurrentMenuState)
        {
            case MenuState.PreLobby:
                _lobbyContent.SetActive(false);

                _preLobbyContent.SetActive(true);
                _preLobbyContent.transform.Find("Searching").gameObject.SetActive(true);
                _preLobbyContent.transform.Find("ServerListContainer").gameObject.SetActive(false);
                StartCoroutine(preLobbySearchForServer());
                break;
            case MenuState.Lobby:
                _preLobbyContent.SetActive(false);

                _lobbyContent.SetActive(true);
                break;
        }

        _isSliderOpen = true;
    }

    void closeSlider()
    {
        if (_isSliderOpen)
            _slider.GetComponent<Animator>().SetTrigger("CloseSlider");

        switch (CurrentMenuState)
        {
            case MenuState.PreLobby:
                _preLobbyContent.transform.Find("Searching").gameObject.SetActive(true);
                _preLobbyContent.transform.Find("ServerListContainer").gameObject.SetActive(false);
                _preLobbyContent.SetActive(false);
                break;
            case MenuState.Lobby:
                _lobbyContent.SetActive(false);
                break;
        }

        _isSliderOpen = false;
    }

    public void GoToCredits()
    {
        disableNavButtons();

        _settings.GetComponent<Animator>().SetTrigger("FadeOut");
        StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showCredits, _settings, false));
    }

    void showCredits()
    {
        _credits.SetActive(true);

        updateMenuState();
    }

    IEnumerator disableGOAfterAnimation(Animator pAnimator, System.Action pMethodToCall, GameObject overrideGO = null, bool disableBothGOs = true, bool pShouldCallMethod = true)
    {
        float timeout = 0;
        while (true)
        {
            timeout += Time.deltaTime;
            if (timeout >= 10) yield break;

            if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.1f) yield return null;

            if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !pAnimator.IsInTransition(0))
            {
                if (overrideGO != null)
                {
                    overrideGO.SetActive(false);
                    if (disableBothGOs)
                        pAnimator.gameObject.SetActive(false);
                }
                else
                {
                    pAnimator.gameObject.SetActive(false);
                }

                if (pShouldCallMethod)
                    pMethodToCall();

                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    void disableNavButtons()
    {
        _settingsButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
        _secondBackButton.gameObject.SetActive(false);
    }
}
