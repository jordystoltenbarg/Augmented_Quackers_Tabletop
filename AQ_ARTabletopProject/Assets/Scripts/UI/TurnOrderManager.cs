using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderManager : MonoBehaviour
{
    private static VasilPlayer _currentPlayerTurn = null;
    public static VasilPlayer CurrentPlayerTurn { get { return _currentPlayerTurn; } }

    private static int _round = 1;
    public static int Round { get { return _round; } }

    private List<GameObject> _positions = new List<GameObject>();
    private List<GameObject> _playerAvatars = new List<GameObject>();

    private List<VasilPlayer> _players = new List<VasilPlayer>();
    private List<VasilPlayer> _playersByTurnOrder = new List<VasilPlayer>();
    private List<GameObject> _playersByTurnOrderPositionPrefabs = new List<GameObject>();

    private bool _hasCreatingSkippedTurnFinished = false;

    void Start()
    {
        fillLists();
        shufflePlayerTurnOrder();
        setPlayerTurn(_playersByTurnOrder[0]);
        GameObject.Find("RoundCount").GetComponent<TextMeshProUGUI>().text = string.Format("Round: {0}", _round);
    }

    void Update()
    {
        //End Turn
        if (Input.GetKeyDown(KeyCode.D))
            if (!_currentPlayerTurn.IsOutOfActions)
                GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("Waiting for {0}...", _currentPlayerTurn.PlayerName);
            else
                nextPlayer();

        //Skip P1
        if (Input.GetKeyDown(KeyCode.Keypad1))
            playerSkipTurn(_playersByTurnOrder[0]);

        //Skip P2
        if (Input.GetKeyDown(KeyCode.Keypad2))
            playerSkipTurn(_playersByTurnOrder[1]);

        //Skip P3
        if (Input.GetKeyDown(KeyCode.Keypad3))
            playerSkipTurn(_playersByTurnOrder[2]);

        //Skip P4
        if (Input.GetKeyDown(KeyCode.Keypad4))
            playerSkipTurn(_playersByTurnOrder[3]);
    }

    public void EndTurnInput()
    {
        if (!_currentPlayerTurn.IsOutOfActions)
            GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("Waiting for {0}...", _currentPlayerTurn.PlayerName);
        else
            nextPlayer();
    }

    void fillLists()
    {
        _players.Clear();
        _players = GameObject.FindObjectsOfType<VasilPlayer>().ToList();

        _positions.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Border") continue;

            _positions.Add(transform.GetChild(i).gameObject);
        }

        _playerAvatars.Clear();
        for (int i = 0; i < _positions.Count; i++)
        {
            _playerAvatars.Add(_positions[i].transform.GetChild(0).gameObject);
        }
    }

    void shufflePlayerTurnOrder()
    {
        Dictionary<VasilPlayer, int> order = new Dictionary<VasilPlayer, int>();
        foreach (VasilPlayer p in _players)
            order.Add(p, p.RullDie());

        List<KeyValuePair<VasilPlayer, int>> orderList = order.ToList();
        orderList.Sort((x, y) => x.Value.CompareTo(y.Value));
        orderList.Reverse();

        _playersByTurnOrder.Clear();
        _playersByTurnOrderPositionPrefabs.Clear();
        for (int i = 0; i < _players.Count; i++)
        {
            if (orderList[i].Key.TurnOrderPositionPrefab.GetComponentInChildren<TextMeshProUGUI>())
                orderList[i].Key.TurnOrderPositionPrefab.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            _playerAvatars[i].GetComponent<TurnOrderPlayerAvatar>().SetPlayer(orderList[i].Key);

            _playersByTurnOrder.Add(orderList[i].Key);
            _playersByTurnOrderPositionPrefabs.Add(_playersByTurnOrder[i].TurnOrderPositionPrefab);
        }

        updatePlayerTurnOrder();
    }

    void updatePlayerTurnOrder()
    {
        for (int i = 0; i < _playerAvatars.Count; i++)
        {
            _playerAvatars[i].transform.SetParent(_positions[i].transform);

            if (i == 0)
                _playerAvatars[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = true;
            else
                _playerAvatars[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = false;
        }
    }

    void nextPlayer()
    {
        VasilPlayer currentPlayer = _playerAvatars[0].GetComponent<TurnOrderPlayerAvatar>().Player;
        if (!currentPlayer.pawn.hasReachedDestination) return;

        currentPlayer.hasActedThisTurn = true;
        currentPlayer.EndTurn();

        if (hasFinalPlayerActed())
        {
            _round++;
            GameObject.Find("RoundCount").GetComponent<TextMeshProUGUI>().text = string.Format("Round: {0}", _round);

            foreach (VasilPlayer p in _players)
            {
                if (p.turnsToSkip == 0)
                    p.hasActedThisTurn = false;

                if (p.turnsToSkip > 0)
                    p.turnsToSkip--;
            }
        }

        GameObject firstPlayer = _playerAvatars[0];
        _playerAvatars.RemoveAt(0);
        _playerAvatars.Add(firstPlayer);

        for (int i = 0; i < _positions.Count; i++)
        {
            _playerAvatars[i].transform.SetParent(_positions[i].transform);

            if (i == 0)
                _playerAvatars[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = true;
            else
                _playerAvatars[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = false;
        }

        if (_playerAvatars.Count > _players.Count)
        {
            _playerAvatars[_playerAvatars.Count - 1].GetComponent<LerpToZero>().enabled = false;
            _playerAvatars[_playerAvatars.Count - 1].GetComponent<Image>().enabled = false;
            _playerAvatars[_playerAvatars.Count - 1].GetComponent<RectTransform>().position = new Vector2(transform.parent.parent.position.x, _playerAvatars[_playerAvatars.Count - 1].GetComponent<RectTransform>().position.y);
            _playerAvatars[_playerAvatars.Count - 1].GetComponent<TurnOrderPlayerAvatar>().SelfDestroy();
            _positions.RemoveAt(_positions.Count - 1);
            _playerAvatars.RemoveAt(_playerAvatars.Count - 1);
        }

        setPlayerTurn(_playerAvatars[0].GetComponent<TurnOrderPlayerAvatar>().Player);
    }

    bool hasFinalPlayerActed()
    {
        for (int i = _playersByTurnOrder.Count - 1; i >= 0; i--)
        {
            if (_playersByTurnOrder[i].turnsToSkip == 0)
            {
                if (_playersByTurnOrder[i].hasActedThisTurn)
                    return true;
                else
                    return false;
            }
        }

        return false;
    }

    void playerSkipTurn(VasilPlayer pPlayer)
    {
        pPlayer.turnsToSkip++;
        for (int i = 0; i < _playerAvatars.Count; i++)
        {
            if (_playerAvatars[i].GetComponent<TurnOrderPlayerAvatar>().Player == pPlayer)
            {
                if (_currentPlayerTurn == pPlayer && i == 0)
                {
                    if (_playerAvatars.Count == _players.Count)
                    {
                        StartCoroutine(createSkippedTurn(pPlayer));
                        StartCoroutine(createNextPlayableTurn());
                        return;
                    }

                    continue;
                }

                _playerAvatars[i].GetComponent<TurnOrderPlayerAvatar>().SelfDestroy();
                _positions.RemoveAt(i);
                _playerAvatars.RemoveAt(i);
                break;
            }
        }

        if (_playerAvatars.Count >= _players.Count)
        {
            for (int i = _players.Count - 1; i >= 0; i--)
            {
                if (_playerAvatars[_playerAvatars.Count - 1 - i].GetComponent<TurnOrderPlayerAvatar>().Player != _playersByTurnOrder[_players.Count - 1 - i])
                    break;

                if (i == 0 && _playerAvatars.Count != _players.Count)
                    return;
            }
        }

        if (_playerAvatars.Count < _players.Count ||
           (_playerAvatars.Count <= _players.Count && !pPlayer.hasActedThisTurn))
        {
            //Show turns in which pPlayer is skipped
            StartCoroutine(createSkippedTurn(pPlayer));
        }
        else
        {
            _hasCreatingSkippedTurnFinished = true;
        }

        //Show the next turn that the pPlayer can play
        StartCoroutine(createNextPlayableTurn());
    }

    private IEnumerator createSkippedTurn(VasilPlayer pPlayer)
    {
        //Waiting for other turns to be drawn
        while (_hasCreatingSkippedTurnFinished)
            yield return null;

        for (int i = 0; i < _playersByTurnOrderPositionPrefabs.Count; i++)
        {
            if (_playersByTurnOrderPositionPrefabs[i].GetComponentInChildren<TurnOrderPlayerAvatar>().Player == pPlayer ||
                _playersByTurnOrderPositionPrefabs[i].GetComponentInChildren<TurnOrderPlayerAvatar>().Player.hasActedThisTurn)
                continue;

            createNewPosition(i);
            yield return new WaitForSeconds(0.05f);
        }

        _hasCreatingSkippedTurnFinished = true;
    }

    private IEnumerator createNextPlayableTurn()
    {
        //Waiting for other turns to be drawn
        while (!_hasCreatingSkippedTurnFinished)
            yield return null;

        for (int i = 0; i < _playersByTurnOrderPositionPrefabs.Count; i++)
        {
            createNewPosition(i);
            yield return new WaitForSeconds(0.05f);
        }

        _hasCreatingSkippedTurnFinished = false;
    }

    void createNewPosition(int i)
    {
        GameObject newPosition = Instantiate(_playersByTurnOrderPositionPrefabs[i], _positions[0].transform.parent);
        newPosition.GetComponentInChildren<TurnOrderPlayerAvatar>().SetPlayer(_playersByTurnOrderPositionPrefabs[i].transform.GetChild(0).GetComponentInChildren<TurnOrderPlayerAvatar>().Player);
        newPosition.GetComponent<SetSizeToChildSize>().StartUpdatingSize(newPosition.transform.GetChild(0).gameObject);
        _positions.Add(newPosition);
        _playerAvatars.Add(newPosition.transform.GetChild(0).gameObject);
    }

    void setPlayerTurn(VasilPlayer pPlayer)
    {
        _currentPlayerTurn = pPlayer;
        foreach (VasilPlayer p in _players)
            if (p == _currentPlayerTurn)
                p.StartTurn();
    }
}