using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField]
    private Button _settingsButton;
    [SerializeField]
    private Button _backButton;

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


    public enum MenuState
    {
        Play,
        Collection,
        PreLobby,
        Lobby,
        Settings,
        Credits
    }
    public static MenuState CurrentMenuState;

    void Start()
    {
        checkMenuState();
    }

    void Update()
    {

    }

    void checkMenuState()
    {
        if (_play.activeSelf)
            CurrentMenuState = MenuState.Play;
        else if (_collection.activeSelf)
            CurrentMenuState = MenuState.Collection;
        else if (_lobby.activeSelf)
            CurrentMenuState = MenuState.PreLobby;
        else if (_lobby.activeSelf)
            CurrentMenuState = MenuState.Lobby;
        else if (_settings.activeSelf)
            CurrentMenuState = MenuState.Settings;
        else if (_credits.activeSelf)
            CurrentMenuState = MenuState.Credits;
    }

    public void OpenSlider()
    {

    }
}
