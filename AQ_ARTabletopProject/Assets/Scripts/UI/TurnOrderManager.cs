using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnOrderManager : MonoBehaviour
{
    private static GameObject _currentPlayerTurn = null;
    public static GameObject CurrentPlayerTurn { get { return _currentPlayerTurn; } }

    private static int _turn = 1;
    public static int Turn { get { return _turn; } }

    private List<GameObject> _positions = new List<GameObject>();
    private List<GameObject> _players = new List<GameObject>();

    void Start()
    {
        fillPositionsList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            nextPlayer();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            currentPlayerSkipsTurn();
        }
    }

    void updateTurnOrder()
    {
    }

    void fillPositionsList()
    {
        _positions.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Border") continue;

            _positions.Add(transform.GetChild(i).gameObject);
        }

        _players.Clear();
        for (int i = 0; i < _positions.Count; i++)
        {
            _players.Add(_positions[i].transform.GetChild(0).gameObject);

            if (i == 0)
                _players[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = true;
            else
                _players[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = false;
        }
    }

    void nextPlayer()
    {
        GameObject firstPlayer = _players[0];
        _players.RemoveAt(0);
        _players.Add(firstPlayer);

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].transform.SetParent(_positions[i].transform);

            if (i == 0)
                _players[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = true;
            else
                _players[i].GetComponent<ExtendTurnOrderAvatar>().IsFirst = false;
        }
    }

    void currentPlayerSkipsTurn(int pNumberOfTurns = 1)
    {

    }
}
