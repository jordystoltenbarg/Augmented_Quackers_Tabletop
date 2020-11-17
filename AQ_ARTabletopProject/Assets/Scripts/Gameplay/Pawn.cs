using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public static event Action onPawnReadecFinalTile;

    [SerializeField]
    private float _speed = 5;

    private Tile _currentTile = null;

    private Tile _targetTile = null;
    private Vector3 _currentWaypoint = Vector3.zero;
    private Rigidbody _rb = null;

    private VasilPlayer _player;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _rb.velocity = Vector3.zero;
    }

    void OnEnable()
    {
        RollDie.onDieRolled += (int pRoll) => setNextTargetTile(TilesManager.ListOfTiles, pRoll);
    }

    void OnDisable()
    {
        RollDie.onDieRolled -= (int pRoll) => setNextTargetTile(TilesManager.ListOfTiles, pRoll);
    }

    public void SetPlayer(VasilPlayer pPlayer)
    {
        _player = pPlayer;
    }

    void setNextTargetTile(List<Tile> pTilesList, int pDieRoll)
    {
        if (!_player.HasCurrentTurn) return;

        int currTileIndex = 0;
        if (_currentTile)
            for (int i = 0; i < pTilesList.Count; i++)
                if (pTilesList[i] == _currentTile)
                    currTileIndex = i;
        _currentTile = pTilesList[currTileIndex];

        if (currTileIndex + pDieRoll <= pTilesList.Count - 1)
            _targetTile = pTilesList[currTileIndex + pDieRoll];
        else
            _targetTile = pTilesList[pTilesList.Count - 1];

        List<Tile> tiles = new List<Tile>();
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = currTileIndex; i < pTilesList.Count; i++)
        {
            tiles.Add(pTilesList[i]);
            waypoints.Add(pTilesList[i].Waypoint);
            if (pTilesList[i] == _targetTile) break;
        }

        if (_currentTile == pTilesList[pTilesList.Count - 1])
        {
            _currentTile = null;
            setNextTargetTile(pTilesList, pDieRoll);
            return;
        }

        StartCoroutine(moveTowardsTargetTile(tiles, waypoints));
    }

    IEnumerator moveTowardsTargetTile(List<Tile> pTileList, List<Vector3> pWaypointList)
    {
        int reachedWPIndex = 0;
        int targetWPIndex = 1;
        Vector3 direction = (pWaypointList[targetWPIndex] - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
        transform.LookAt(new Vector3(pWaypointList[targetWPIndex].x, transform.position.y, pWaypointList[targetWPIndex].z));

        _rb.constraints = RigidbodyConstraints.FreezeAll;

        while (true)
        {
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), pWaypointList[targetWPIndex]) <= Time.deltaTime * _speed * 1.5f)
            {
                reachedWPIndex = Mathf.Clamp(reachedWPIndex + 1, 0, pWaypointList.Count - 1);
                if (reachedWPIndex >= pWaypointList.Count - 1)
                {
                    _currentTile = pTileList[reachedWPIndex];
                    _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    onPawnReadecFinalTile?.Invoke();
                    yield break;
                }
                else
                {
                    targetWPIndex = Mathf.Clamp(targetWPIndex + 1, 0, pWaypointList.Count - 1);
                    _currentTile = pTileList[targetWPIndex];
                    direction = (pWaypointList[targetWPIndex] - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                    transform.LookAt(new Vector3(pWaypointList[targetWPIndex].x, transform.position.y, pWaypointList[targetWPIndex].z));
                }
            }

            transform.Translate(direction * Time.deltaTime * _speed, Space.World);
            if (!_player.HasCurrentTurn) break;
            yield return null;
        }
    }
}
