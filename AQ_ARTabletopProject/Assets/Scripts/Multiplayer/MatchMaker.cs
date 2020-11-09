using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class Match
{
    public string matchID;

    public bool publicMatch;

    public bool inMatch;

    public bool matchFull;

    public SynclistGameObject players = new SynclistGameObject();

    public Match(string matchID, GameObject player)
    {
        this.matchID = matchID;
        players.Add(player);
    }

    public Match() { }
}

[System.Serializable]
public class SynclistGameObject : SyncList<GameObject> { }

[System.Serializable]
public class SynclistMatch : SyncList<Match> { }

public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;

    public SynclistMatch matches = new SynclistMatch();
    public SyncListString matchIDs = new SyncListString();

    [SerializeField] GameObject turnManagerPrefab;

    void Start()
    {
        instance = this;
    }

    public bool HostGame(string _matchID, GameObject _player, bool publicMatch, out int playerIndex)
    {
        playerIndex = -1;
        if (!matchIDs.Contains(_matchID))
        {
            matchIDs.Add(_matchID);
            Match match = new Match(_matchID, _player);
            match.publicMatch = publicMatch;
            matches.Add(match);
            Debug.Log($"Match generated");
            //_player.GetComponent<Player>().currentMatch = match;
            playerIndex = 1;
            return true;
        }
        else
        {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }

    public bool JoinGame(string _matchID, GameObject _player, out int playerIndex)
    {
        playerIndex = -1;
        if (matchIDs.Contains(_matchID))
        {

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    matches[i].players.Add(_player);
                    //_player.GetComponent<Player>().currentMatch = matches[i];
                    playerIndex = matches[i].players.Count;
                    break;
                }
            }

            Debug.Log($"Match joined");
            return true;
        }
        else
        {
            Debug.Log($"Match ID does not exists");
            return false;
        }
    }

    public bool SearchGame(GameObject _player, out int playerIndex, out string matchID)
    {
        playerIndex = -1;
        matchID = string.Empty;

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].publicMatch && !matches[i].matchFull && !matches[i].inMatch)
            {
                matchID = matches[i].matchID;
                if (JoinGame(matchID, _player, out playerIndex))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void StartGame(string _matchID)
    {
        GameObject newTurnManager = Instantiate(turnManagerPrefab);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                foreach (var player in matches[i].players)
                {
                    Player _player = player.GetComponent<Player>();
                    turnManager.AddPlayer(_player);
                    _player.BeginGame();
                }
                break;
            }
        }
    }

    public static string GetRandomMatchID()
    {
        string _id = string.Empty;
        for (int i = 0; i < 6; i++)
        {
            int random = UnityEngine.Random.Range(0, 26);
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
        }

        Debug.Log($"Random Match ID: {_id}");
        return _id;
    }

    public void PlayerDisconnected(Player player, string _matchID)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                int playerIndex = matches[i].players.IndexOf(player.gameObject);
                matches[i].players.RemoveAt(playerIndex);
                Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                if (matches[i].players.Count == 0)
                {
                    Debug.Log($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);
                }
                break;
            }
        }
    }
}

public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
