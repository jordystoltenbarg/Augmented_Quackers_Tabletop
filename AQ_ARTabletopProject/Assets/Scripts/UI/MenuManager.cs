using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Inspector exposed fields
    [Header("Navigation Buttons")]
    [SerializeField] private Button _settingsButton = null;
    [SerializeField] private Button _backButton = null;
    [SerializeField] private Button _secondBackButton = null;

    [Header("Slider")]
    [SerializeField] private GameObject _slider = null;
    [SerializeField] private GameObject _preLobbyContent = null;
    [SerializeField] private GameObject _lobbyContent = null;

    [Header("Menu Screens")]
    [SerializeField] private GameObject _play = null;
    [SerializeField] private GameObject _collection = null;
    [SerializeField] private GameObject _preLobby = null;
    [SerializeField] private GameObject _lobby = null;
    [SerializeField] private GameObject _settings = null;
    [SerializeField] private GameObject _credits = null;

    [Header("Miscellaneous")]
    [SerializeField] private float _menuUpdateCallDelay = 0.2f;
    #endregion

    #region Private fields
    private GameObject _mainMenu = null;
    private bool _isSliderOpen = false;
    #endregion

    #region Enum fields
    public enum MenuState
    {
        Play,
        Collection,
        PreLobby,
        Lobby,
        Settings,
        Credits
    }
    private MenuState _currentMenuState;
    private MenuState _previousState;
    #endregion

    private void Start()
    {
        _mainMenu = _play.transform.parent.gameObject;
        init();
        updateMenuState();
    }

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(onSettingsButtonClick);
        _backButton.onClick.AddListener(onBackButtonClick);
        _secondBackButton.onClick.AddListener(onSecondBackButtonClick);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _secondBackButton.onClick.RemoveAllListeners();
    }

    private void init()
    {
        if (_play)
            _play.SetActive(true);
        if (_collection)
            _collection.SetActive(false);
        if (_preLobby)
            _preLobby.SetActive(false);
        if (_lobby)
            _lobby.SetActive(false);
        if (_settings)
            _settings.SetActive(false);
        if (_credits)
            _credits.SetActive(false);

        if (_preLobbyContent)
            _preLobbyContent.SetActive(false);
        if (_lobbyContent)
            _lobbyContent.SetActive(false);
    }

    private void checkMenuState()
    {
        if (_mainMenu.activeSelf)
        {
            if (_play.activeSelf)
                _currentMenuState = MenuState.Play;
            else if (_collection.activeSelf)
                _currentMenuState = MenuState.Collection;
            else if (_preLobby.activeSelf)
                _currentMenuState = MenuState.PreLobby;
            else if (_lobby.activeSelf)
                _currentMenuState = MenuState.Lobby;
        }
        else if (_settings.activeSelf)
            _currentMenuState = MenuState.Settings;
        else if (_credits.activeSelf)
            _currentMenuState = MenuState.Credits;
    }

    private void updateMenuState()
    {
        checkMenuState();
        Invoke(nameof(enableBackButtons), _menuUpdateCallDelay);
    }

    private void enableBackButtons()
    {
        switch (_currentMenuState)
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

    /// <summary>
    /// Enables nav buttons buttons
    /// </summary>
    /// <remarks>
    /// 0 = only _settingsButton <para>1 = only _backButton</para> <para>2 = _settingsButton and _secondBackButton</para>
    /// </remarks>
    private void navButtons(int pState)
    {
        switch (pState)
        {
            case 0:
                _settingsButton.interactable = true;
                _backButton.interactable = false;
                _secondBackButton.interactable = false;
                break;
            case 1:
                _settingsButton.interactable = false;
                _backButton.interactable = true;
                _secondBackButton.interactable = false;
                break;
            case 2:
                _settingsButton.interactable = true;
                _backButton.interactable = false;
                _secondBackButton.interactable = true;
                break;
        }
    }

    /// <summary>
    /// Transition to Settings Screen
    /// </summary>
    private void onSettingsButtonClick()
    {
        _mainMenu.GetComponent<Animator>().SetTrigger("FadeOut");
        StartCoroutine(disableGOAfterAnimation(_mainMenu.GetComponent<Animator>(), showSettings, _mainMenu, false));

        _previousState = _currentMenuState;

        _settingsButton.GetComponent<Animator>().SetTrigger("FadeOut");
        _backButton.GetComponent<Animator>().SetTrigger("FadeIn");
        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                _secondBackButton.GetComponent<Animator>().SetTrigger("Collapse");
                break;
            case MenuState.Lobby:
                _secondBackButton.GetComponent<Animator>().SetTrigger("Collapse");
                break;
        }

        disableNavButtons();
    }

    private void showSettings()
    {
        _settings.SetActive(true);

        updateMenuState();
    }

    private void onBackButtonClick()
    {
        disableNavButtons();

        if (_currentMenuState == MenuState.Credits)
        {
            _credits.GetComponent<Animator>().SetTrigger("FadeOut");
            StartCoroutine(disableGOAfterAnimation(_credits.GetComponent<Animator>(), showSettings, _credits, false));
            return;
        }

        _settingsButton.GetComponent<Animator>().SetTrigger("FadeIn");
        _backButton.GetComponent<Animator>().SetTrigger("FadeOut");

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

    private void onSecondBackButtonClick()
    {
        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                GoToPlay();
                disableNavButtons();
                break;
            case MenuState.Lobby:
                TTMessagePopup.Singleton.DisplayPopup(TTMessagePopup.PopupTitle.Warning, TTMessagePopup.PopupMessage.LeaveLobby, TTMessagePopup.PopupResponse.YesNo);
                StartCoroutine(waitForResponse());
                //GoToPreLobby(); Called from TTPlayer
                //Mirror.NetworkManager manager = Mirror.NetworkManager.singleton;
                //if (manager == null) break;
                //switch (manager.mode)
                //{
                //    case Mirror.NetworkManagerMode.ClientOnly:
                //        manager.StopClient();
                //        break;
                //    case Mirror.NetworkManagerMode.Host:
                //        manager.StopHost();
                //        break;
                //}
                break;
        }
    }

    private IEnumerator waitForResponse()
    {
        TTMessagePopup.OnYesButtonPressed += yes;
        TTMessagePopup.OnNoButtonPressed += no;
        bool hasReponded = false;

        yield return new WaitWhile(() => !hasReponded);

        TTMessagePopup.OnYesButtonPressed -= yes;
        TTMessagePopup.OnNoButtonPressed -= no;

        void yes()
        {
            disableNavButtons();
            Mirror.NetworkManager manager = Mirror.NetworkManager.singleton;
            switch (manager.mode)
            {
                case Mirror.NetworkManagerMode.ClientOnly:
                    manager.StopClient();
                    break;
                case Mirror.NetworkManagerMode.Host:
                    manager.StopHost();
                    break;
            }
            hasReponded = true;
        }

        void no()
        {
            hasReponded = true;
        }
    }

    /// <summary>
    /// Transition to Play Screen
    /// </summary>
    public void GoToPlay()
    {
        closeSlider();

        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                _preLobby.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_preLobby.GetComponent<Animator>(), showPlay));

                _secondBackButton.GetComponent<Animator>().SetTrigger("Collapse");
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showPlay, _settings, false));
                break;
        }

        disableNavButtons();
    }

    private void showPlay()
    {
        if (!_mainMenu.activeSelf)
            _mainMenu.SetActive(true);

        _play.SetActive(true);
        updateMenuState();
    }

    /// <summary>
    /// Transition to Pre-Lobby Screen
    /// </summary>
    public void GoToPreLobby()
    {
        switch (_currentMenuState)
        {
            case MenuState.Play:
                _play.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_play.GetComponent<Animator>(), showPreLobby));

                _secondBackButton.GetComponent<Animator>().SetTrigger("Expand");
                break;
            case MenuState.Lobby:
                _lobby.GetComponent<Animator>().SetTrigger("FadeOut");
                _lobbyContent.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_lobby.GetComponent<Animator>(), showPreLobby));
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showPreLobby, _settings, false));

                _secondBackButton.GetComponent<Animator>().SetTrigger("Expand");
                break;
        }

        disableNavButtons();
        FindObjectOfType<TTServerListManager>().Refresh();
    }

    private void showPreLobby()
    {
        if (!_mainMenu.activeSelf)
        {
            _mainMenu.SetActive(true);
            _isSliderOpen = false;
        }

        _preLobby.SetActive(true);

        switch (_currentMenuState)
        {
            case MenuState.Lobby:
                for (int i = 0; i < _lobby.transform.GetChild(0).childCount; i++)
                    Destroy(_lobby.transform.GetChild(0).GetChild(i).gameObject);
                break;
        }

        updateMenuState();
        openSlider();
    }

    /// <summary>
    /// Transition to Lobby Screen
    /// </summary>
    public void GoToLobby()
    {
        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                _preLobby.GetComponent<Animator>().SetTrigger("FadeOut");
                _preLobbyContent.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_preLobby.GetComponent<Animator>(), showLobby));
                break;
            case MenuState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showLobby, _settings, false));

                _secondBackButton.GetComponent<Animator>().SetTrigger("Expand");
                break;
        }

        disableNavButtons();
    }

    private void showLobby()
    {
        if (!_mainMenu.activeSelf)
        {
            _mainMenu.SetActive(true);
            _isSliderOpen = false;
        }

        _lobby.SetActive(true);

        updateMenuState();
        openSlider();
    }

    private void openSlider()
    {
        if (!_isSliderOpen)
            _slider.GetComponent<Animator>().SetTrigger("OpenSlider");

        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                _lobbyContent.SetActive(false);
                _preLobbyContent.SetActive(true);
                break;
            case MenuState.Lobby:
                _preLobbyContent.SetActive(false);
                _lobbyContent.SetActive(true);
                break;
        }

        _isSliderOpen = true;
    }

    private void closeSlider()
    {
        if (_isSliderOpen)
            _slider.GetComponent<Animator>().SetTrigger("CloseSlider");

        switch (_currentMenuState)
        {
            case MenuState.PreLobby:
                _preLobbyContent.SetActive(false);
                break;
            case MenuState.Lobby:
                _lobbyContent.SetActive(false);
                break;
        }

        _isSliderOpen = false;
    }

    /// <summary>
    /// Transition to Credits Screen
    /// </summary>
    public void GoToCredits()
    {
        _settings.GetComponent<Animator>().SetTrigger("FadeOut");
        StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showCredits, _settings, false));

        disableNavButtons();
    }

    private void showCredits()
    {
        _credits.SetActive(true);
        updateMenuState();
    }

    private IEnumerator disableGOAfterAnimation(Animator pAnimator, System.Action pMethodToCall, GameObject pOverrideGO = null, bool pDisableBothGOs = true, bool pShouldCallMethod = true)
    {
        float timeout = 0;
        while (true)
        {
            timeout += Time.deltaTime;
            if (timeout >= 10) yield break;

            if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.1f) yield return null;

            if (pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !pAnimator.IsInTransition(0))
            {
                if (pOverrideGO != null)
                {
                    pOverrideGO.SetActive(false);
                    if (pDisableBothGOs)
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

    private void disableNavButtons()
    {
        _settingsButton.interactable = false;
        _backButton.interactable = false;
        _secondBackButton.interactable = false;
    }
}
