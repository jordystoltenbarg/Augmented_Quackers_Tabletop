using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField]
    private Button _settingsButton;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Button _homeButton;

    [Header("Menu Screens")]
    [SerializeField]
    private GameObject _play;
    [SerializeField]
    private GameObject _settings;
    [SerializeField]
    private GameObject _credits;

    public enum InGameUIState
    {
        Play,
        Settings,
        Credits
    }
    public static InGameUIState CurrentInGameUIState;

    void Start()
    {

    }

    void OnEnable()
    {
        _settingsButton.onClick.AddListener(onSettingsButtonClick);
        _backButton.onClick.AddListener(onBackButtonClick);
        _homeButton.onClick.AddListener(onHomeButtonClick);
    }

    void OnDisable()
    {
        _settingsButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _homeButton.onClick.RemoveAllListeners();
    }

    private void onSettingsButtonClick()
    {
    }

    private void onBackButtonClick()
    {
    }

    private void onHomeButtonClick()
    {
    }

    void checkMenuState()
    {
        if (_play.activeSelf)
            CurrentInGameUIState = InGameUIState.Play;
        else if (_settings.activeSelf)
            CurrentInGameUIState = InGameUIState.Settings;
        else if (_credits.activeSelf)
            CurrentInGameUIState = InGameUIState.Credits;
    }

    void updateMenuState()
    {
        checkMenuState();

        switch (CurrentInGameUIState)
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
    void navButtons(int pState)
    {
        switch (pState)
        {
            case 0:
                _settingsButton.gameObject.SetActive(true);
                _backButton.gameObject.SetActive(false);
                _homeButton.gameObject.SetActive(false);
                break;
            case 1:
                _settingsButton.gameObject.SetActive(false);
                _backButton.gameObject.SetActive(true);
                _homeButton.gameObject.SetActive(false);
                break;
            case 2:
                _settingsButton.gameObject.SetActive(false);
                _backButton.gameObject.SetActive(true);
                _homeButton.gameObject.SetActive(true);
                break;
        }
    }
}
