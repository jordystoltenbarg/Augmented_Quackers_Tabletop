﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer LocalPlayer;

    public Action<int> onCharacterSelectApproved;
    public Action<int> onCharacterSelectDenied;

    [SyncVar] private int _lobbyIndex = -1;
    public int LobbyIndex => _lobbyIndex;
    [SyncVar] private string _playerName = "";
    public string PlayerName => _playerName;
    [SyncVar] private bool _isReady = false;
    public bool IsReady => _isReady;
    [SyncVar] private int _selectedCharacterIndex = -1;
    public int SelectedCharacterIndex => _selectedCharacterIndex;
    [SyncVar] private int _colorVariation = -1;
    public int ColorVariation => _colorVariation;
    [SyncVar] private bool _hostedServerIsPrivate = false;
    public bool HostedServerIsPrivate => _hostedServerIsPrivate;
    [SyncVar] private bool _hasLoaded = false;
    public bool HasLoaded => _hasLoaded;

    [HideInInspector] public bool hasBeenKicked = false;

    private TTNetworkManagerListServer _manager = null;
    [HideInInspector]
    public Rigidbody dieRB = null;

    private void Start()
    {
        print($"<color=green> Hello there!</color>");



        Invoke(nameof(lobbyUIAddPlayer), 0.5f);
        DontDestroyOnLoad(gameObject);

        Invoke(nameof(setGOName), 2f);
    }

    private void lobbyUIAddPlayer()
    {
        TTSettingsManager.Singleton.AddPlayer(this);
    }

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = this;
        _manager = NetworkManager.singleton as TTNetworkManagerListServer;

        TTSettingsManager.onPlayerNameChanged += changePlayerName;
        TTSettingsManager.onServerPrivacyChanged += changeServerPrivacy;

        FindObjectOfType<MenuManager>().GoToLobby();

        Invoke(nameof(cmdSetPlayerIndex), 0.2f);
        Invoke(nameof(cmdSetPlayerColorVariation), 0.2f);
        cmdChangePlayerName(TTSettingsManager.Singleton.PlayerName);
    }

    private void OnDestroy()
    {
        TTSettingsManager.Singleton.RemovePlayer(this);

        if (!isLocalPlayer) return;

        if (hasBeenKicked)
        {
            TTMessagePopup.Singleton.DisplayPopup(TTMessagePopup.PopupTitle.Notification, TTMessagePopup.PopupMessage.Kick, TTMessagePopup.PopupResponse.Ok);
        }

        ReadyUp.IsLocalPlayerReady = false;
        ReadyUp.CanStartGame = false;
        TTSettingsManager.onPlayerNameChanged -= changePlayerName;
        TTSettingsManager.onServerPrivacyChanged -= changeServerPrivacy;

        LocalPlayer = null;

        FindObjectOfType<MenuManager>().GoToPreLobby();
    }

    /// <summary>
    /// Called when a client joins a Host or when a Hosted game is created
    /// </summary>
    public override void OnStartClient()
    {
    }

    public override void OnStopClient()
    {
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
    }

    private void setGOName()
    {
        if (_lobbyIndex == 0)
            gameObject.name = (isLocalPlayer) ? $"Host/LocalPlayer ({_playerName})" : $"Host ({_playerName})";
        else
            gameObject.name = (isLocalPlayer) ? $"LocalPlayer ({_playerName})" : $"Client:{_lobbyIndex} ({_playerName})";
    }

    private void changePlayerName(string pNewName)
    {
        cmdChangePlayerName(pNewName);
    }

    [Command]
    private void cmdChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
    }

    private void changeServerPrivacy(bool pIsPrivate)
    {
        cmdChangeServerPrivacy(pIsPrivate);
    }

    [Command]
    private void cmdChangeServerPrivacy(bool pIsPrivate)
    {
        _hostedServerIsPrivate = pIsPrivate;
    }

    [Command]
    private void cmdSetPlayerIndex()
    {
        _lobbyIndex = TTApiUpdater.Singleton.GetServerPlayerCount() - 1;
    }

    public void UpdateHigherLobbyIndex(int pLobbyIndex)
    {
        if (_lobbyIndex > pLobbyIndex)
            cmdUpdateHigherLobbyIndex();
    }

    [Command]
    private void cmdUpdateHigherLobbyIndex()
    {
        _lobbyIndex--;
    }

    public void UpdateColorVariation(int pNewColorVariation)
    {
        cmdUpdatePlayerColorVariation(pNewColorVariation);
    }

    [Command]
    private void cmdUpdatePlayerColorVariation(int pNewColorVariation)
    {
        _colorVariation = pNewColorVariation;
    }

    [Command]
    private void cmdSetPlayerColorVariation()
    {
        _colorVariation = TTApiUpdater.Singleton.GetServerPlayerCount() - 1;
    }

    public void ReadyToggle()
    {
        cmdReadyToggle();
    }

    [Command]
    private void cmdReadyToggle()
    {
        _isReady = !_isReady;
    }

    public void SelectCharacter(int pCharacterIndex)
    {
        cmdSelectCharacter(pCharacterIndex);
    }

    [Command]
    private void cmdSelectCharacter(int pCharacterIndex)
    {
        _selectedCharacterIndex = pCharacterIndex;
        targetCharacterSelectApproved(pCharacterIndex);
    }

    [TargetRpc]
    private void targetCharacterSelectApproved(int pCharacterIndex)
    {
        onCharacterSelectApproved?.Invoke(pCharacterIndex);
    }

    [TargetRpc]
    private void targetCharacterSelectDenied(int pCharacterIndex)
    {
        onCharacterSelectDenied?.Invoke(pCharacterIndex);
    }

    public void KickClient(int pLocalPlayerIndex)
    {
        cmdKickClient(pLocalPlayerIndex);
    }

    [Command]
    private void cmdKickClient(int pLocalPlayerIndex)
    {
        clientKick(pLocalPlayerIndex);
    }

    [ClientRpc]
    private void clientKick(int pLocalPlayerIndex)
    {
        if (LocalPlayer.LobbyIndex != pLocalPlayerIndex || LocalPlayer.hasBeenKicked) return;

        LocalPlayer.hasBeenKicked = true;
        NetworkManager.singleton.StopClient();
    }

    public void StartGame()
    {
        cmdStartGame();
    }

    [Command]
    private void cmdStartGame()
    {
        clientStartGame();
    }

    [ClientRpc]
    private void clientStartGame()
    {
        TTSettingsManager.Singleton.LobbyCamera.gameObject.SetActive(false);
        GameObject canvas = GameObject.Find("MainUICanvas");
        canvas.transform.Find("LobbyUI").gameObject.SetActive(false);
        TTSettingsManager.Singleton.InGameCamera.gameObject.SetActive(true);
        if (canvas.transform.Find("In-GameUI") != null)
            canvas.transform.Find("In-GameUI").gameObject.SetActive(true);
        else
            GameObject.Find("AR Session Origin").transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

        foreach (TTPlayer player in TTSettingsManager.Singleton.players)
        {
            player.dieRB = FindObjectOfType<RollDie>().GetComponentInChildren<Rigidbody>();
            player.InitializeBoard();
        }
    }

    public void InitializeBoard()
    {
        GetComponent<VasilPlayer>().Init(_lobbyIndex, _selectedCharacterIndex, _colorVariation);
    }

    private IEnumerator loading()
    {
        yield return null;
        cmdFinshedLoading();
    }

    [Command]
    private void cmdFinshedLoading()
    {
        _hasLoaded = true;
    }

    public void RequestDieRoll()
    {
        cmdRequestDieRoll();
    }

    [Command]
    private void cmdRequestDieRoll()
    {
        RollDie die = FindObjectOfType<RollDie>();
        die.StartCoroutine(die.RollRandom());
        StartCoroutine(updateDie(die.transform.GetChild(0).gameObject));
    }

    private IEnumerator updateDie(GameObject pDie)
    {
        Rigidbody rb = pDie.GetComponentInChildren<Rigidbody>();
        while (true)
        {
            clientUpdateDie(pDie.transform.localPosition, pDie.transform.rotation.eulerAngles);

            yield return new WaitForSeconds(0.025f);

            if (rb.velocity.magnitude <= 0)
            {
                clientDieStopped(pDie.transform.parent.GetComponent<RollDie>().GetNumberRolled());
                yield break;
            }
        }
    }

    [ClientRpc]
    private void clientUpdateDie(Vector3 pPos, Vector3 pRot)
    {
        if (LocalPlayer.LobbyIndex == 0) return;

        dieRB.isKinematic = true;
        dieRB.transform.localPosition = pPos;
        dieRB.transform.rotation = Quaternion.Euler(pRot);
    }

    [ClientRpc]
    private void clientDieStopped(int pRoll)
    {
        if (LocalPlayer.LobbyIndex == 0) return;
        print(pRoll);
        foreach (TTPlayer player in TTSettingsManager.Singleton.players)
        {
            if (!player.GetComponent<VasilPlayer>().HasCurrentTurn) continue;
            player.GetComponent<VasilPlayer>().SimulateDieThrow();
            print("simulate die throw");
        }

        dieRB.transform.parent.GetComponent<RollDie>().OnDieRolled(pRoll);
        dieRB.isKinematic = false;
    }

    public void RequestShowQuiz(int pQuestionIndex)
    {
        cmdShowQuiz(pQuestionIndex);
    }

    [Command]
    private void cmdShowQuiz(int pQuestionIndex)
    {
        clientShowQuiz(pQuestionIndex);
    }

    [ClientRpc]
    private void clientShowQuiz(int pQuestionIndex)
    {
        StartCoroutine(displayQuestionWithDelay(pQuestionIndex));
        //QuestionDisplay qd = FindObjectOfType<QuestionDisplay>();
        //if (qd != null)
        //    qd.DisplayQuestion(pQuestionIndex);
    }

    private IEnumerator displayQuestionWithDelay(int pQuestionIndex)
    {
        yield return new WaitUntil(() => FindObjectOfType<QuestionDisplay>() != null);
        QuestionDisplay qd = FindObjectOfType<QuestionDisplay>();
        if (qd != null)
            qd.DisplayQuestion(pQuestionIndex);
    }

    public void RequestHideQuiz()
    {
        cmdHideQuiz();
    }

    [Command]
    private void cmdHideQuiz()
    {
        clientHideQuiz();
    }

    [ClientRpc]
    private void clientHideQuiz()
    {
        if (this == LocalPlayer) return;
        AnswerDisplay ad = FindObjectOfType<AnswerDisplay>();
        if (ad != null)
            ad.HideQuiz();
    }
}
