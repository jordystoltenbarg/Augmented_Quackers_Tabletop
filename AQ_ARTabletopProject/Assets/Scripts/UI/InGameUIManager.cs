using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    #region Inspector exposed fields
    [Header("Navigation Buttons")]
    [SerializeField] private Button _settingsButton = null;
    [SerializeField] private Button _backButton = null;
    [SerializeField] private Button _homeButton = null;

    [Header("Menu Screens")]
    [SerializeField] private GameObject _play = null;
    [SerializeField] private GameObject _settings = null;
    [SerializeField] private GameObject _credits = null;
    [SerializeField] private float _menuUpdateCallDelay = 0.2f;
    #endregion

    #region Private fields
    private GameObject _navButtons = null;
    #endregion

    #region Enum fields
    public enum InGameUIState
    {
        Play,
        Settings,
        Credits
    }
    private InGameUIState _currentInGameUIState;
    #endregion

    private void Start()
    {
        _navButtons = _settingsButton.transform.parent.gameObject;
        updateMenuState();
    }

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(onSettingsButtonClick);
        _backButton.onClick.AddListener(onBackButtonClick);
        _homeButton.onClick.AddListener(onHomeButtonClick);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _homeButton.onClick.RemoveAllListeners();
    }

    private void onSettingsButtonClick()
    {
        switch (_currentInGameUIState)
        {
            case InGameUIState.Play:
                GoToSettings();
                break;
        }
    }

    private void onBackButtonClick()
    {
        switch (_currentInGameUIState)
        {
            case InGameUIState.Settings:
                GoToPlay();
                break;
            case InGameUIState.Credits:
                GoToSettings();
                break;
        }
    }

    private void onHomeButtonClick()
    {
        //Go home you're drunk
    }

    private void checkMenuState()
    {
        if (_play.activeSelf)
            _currentInGameUIState = InGameUIState.Play;
        else if (_settings.activeSelf)
            _currentInGameUIState = InGameUIState.Settings;
        else if (_credits.activeSelf)
            _currentInGameUIState = InGameUIState.Credits;
    }

    private void updateMenuState()
    {
        checkMenuState();

        switch (_currentInGameUIState)
        {
            case InGameUIState.Play:
                navButtons(0);
                break;
            case InGameUIState.Settings:
                navButtons(2);
                break;
            case InGameUIState.Credits:
                navButtons(1);
                break;
        }
    }

    /// <summary>
    /// Enables nav buttons buttons
    /// </summary>
    /// <remarks>
    /// 0 = _settingsButton <para>1 = _backButton</para> <para>2 = _backButton and _homeButton</para>
    /// </remarks>
    private void navButtons(int pState)
    {
        switch (pState)
        {
            case 0:
                _settingsButton.gameObject.SetActive(true);
                _settingsButton.interactable = true;

                _backButton.gameObject.SetActive(false);
                _backButton.interactable = false;

                _homeButton.gameObject.SetActive(false);
                _homeButton.interactable = false;
                break;
            case 1:
                _settingsButton.gameObject.SetActive(false);
                _settingsButton.interactable = false;

                _backButton.gameObject.SetActive(true);
                _backButton.interactable = true;

                _homeButton.gameObject.SetActive(false);
                _homeButton.interactable = false;
                break;
            case 2:
                _settingsButton.gameObject.SetActive(false);
                _settingsButton.interactable = false;

                _backButton.gameObject.SetActive(true);
                _backButton.interactable = true;

                _homeButton.gameObject.SetActive(true);
                _homeButton.interactable = true;
                break;
        }
    }

    public void GoToPlay()
    {
        switch (_currentInGameUIState)
        {
            case InGameUIState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showPlay, _settings, false));

                _navButtons.GetComponent<Animator>().SetTrigger("CollapseHomeButton");

                disableNavButtons();
                break;
        }
    }

    private void showPlay()
    {
        _play.SetActive(true);
        Invoke(nameof(updateMenuState), _menuUpdateCallDelay);
    }

    public void GoToSettings()
    {
        switch (_currentInGameUIState)
        {
            case InGameUIState.Play:
                _play.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_play.GetComponent<Animator>(), showSettings, _play, false));

                _navButtons.GetComponent<Animator>().SetTrigger("ExpandHomeButton");

                disableNavButtons();
                break;
            case InGameUIState.Credits:
                _credits.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_credits.GetComponent<Animator>(), showSettings, _credits, false));

                _navButtons.GetComponent<Animator>().SetTrigger("ExpandHomeButton");

                disableNavButtons();
                break;
        }
    }

    private void showSettings()
    {
        _settings.SetActive(true);
        Invoke(nameof(updateMenuState), _menuUpdateCallDelay);
    }

    public void GoToCredits()
    {
        switch (_currentInGameUIState)
        {
            case InGameUIState.Settings:
                _settings.GetComponent<Animator>().SetTrigger("FadeOut");
                StartCoroutine(disableGOAfterAnimation(_settings.GetComponent<Animator>(), showCredits, _settings, false));

                _navButtons.GetComponent<Animator>().SetTrigger("CollapseHomeButton");

                disableNavButtons();
                break;
        }
    }

    private void showCredits()
    {
        _credits.SetActive(true);
        Invoke(nameof(updateMenuState), _menuUpdateCallDelay);
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
        _homeButton.interactable = false;
    }
}
