using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public static event Action onPawnReadecFinalTile;
    public static event Action<Tile> onPawnReachedTileEvent;
    public static event Action<Tile> onPawnReachedRegularTile;
    public static event Action onPawnReachedFinalTile;
    public static event Action<GameObject> onPawnReachedFinalTileWithGameObject;

    [SerializeField] private float _speed = 5;
    [Header("Verticality")]
    [SerializeField] private bool _moveVertically = false;
    [SerializeField] private float _yOffset = 0;
    private float _totalYOffset = 0;

    private Tile _currentTile = null;

    private Tile _targetTile = null;
    private Vector3 _currentWaypoint = Vector3.zero;
    private Rigidbody _rb = null;

    private VasilPlayer _player;
    public VasilPlayer Player => _player;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _totalYOffset = GetComponent<Collider>().bounds.extents.y + _yOffset;
    }

    private void Update()
    {
        _rb.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        RollDie.onDieRolled += (int pRoll) => setNextTargetTile(TilesManager.ListOfTiles, pRoll);
    }

    private void OnDisable()
    {
        RollDie.onDieRolled -= (int pRoll) => setNextTargetTile(TilesManager.ListOfTiles, pRoll);
    }

    public void SetPlayer(VasilPlayer pPlayer)
    {
        _player = pPlayer;
    }

    public void MoveByValue(int pValue, float pSpeed = -1)
    {
        if (pValue > 0)
            setNextTargetTile(TilesManager.ListOfTiles, pValue, false, (pSpeed > 0) ? pSpeed : -1);
        else
            setNextTargetTile(TilesManager.ListOfTiles, pValue, true, (pSpeed > 0) ? pSpeed : -1);
    }

    public void MoveAlongCustomPath(List<Tile> pTilesList, float pSpeed = -1)
    {
        setNextTargetTile(pTilesList, pTilesList.Count - 1, false, (pSpeed > 0) ? pSpeed : -1);
    }

    private void setNextTargetTile(List<Tile> pTilesList, int pDieRoll, bool pGoingBackwards = false, float pSpeed = -1)
    {
        if (!_player.HasCurrentTurn) return;
        if (_currentTile == pTilesList[pTilesList.Count - 1])
        {
            //_currentTile = null;
            //setNextTargetTile(pTilesList, pDieRoll);
            Debug.Log("Player already finished");
            return;
        }

        int currTileIndex = 0;
        if (_currentTile)
        {
            for (int i = 0; i < pTilesList.Count; i++)
            {
                if (pTilesList[i] == _currentTile)
                {
                    currTileIndex = i;
                    break;
                }
            }
        }
        _currentTile = pTilesList[currTileIndex];

        if (currTileIndex + pDieRoll <= pTilesList.Count - 1)
            _targetTile = pTilesList[currTileIndex + pDieRoll];
        else
            _targetTile = pTilesList[pTilesList.Count - 1];

        List<Tile> tiles = new List<Tile>();
        List<Vector3> waypoints = new List<Vector3>();
        if (!pGoingBackwards)
        {
            for (int i = currTileIndex; i < pTilesList.Count; i++)
            {
                tiles.Add(pTilesList[i]);
                waypoints.Add(pTilesList[i].Waypoint);
                if (pTilesList[i] == _targetTile) break;
            }
        }
        else
        {
            for (int i = currTileIndex; i >= 0; i--)
            {
                tiles.Add(pTilesList[i]);
                waypoints.Add(pTilesList[i].Waypoint);
                if (pTilesList[i] == _targetTile) break;
            }
        }

        StartCoroutine(moveTowardsTargetTile(tiles, waypoints, (pSpeed > 0) ? pSpeed : _speed));
    }

    private IEnumerator moveTowardsTargetTile(List<Tile> pTileList, List<Vector3> pWaypointList, float pSpeed)
    {
        int reachedWPIndex = 0;
        int targetWPIndex = 1;
        Vector3 direction = (new Vector3(pWaypointList[targetWPIndex].x, (_moveVertically) ? pWaypointList[targetWPIndex].y : 0, pWaypointList[targetWPIndex].z) -
                             new Vector3(transform.position.x, (_moveVertically) ? transform.position.y - _totalYOffset : 0, transform.position.z)).normalized;
        transform.LookAt(new Vector3(pWaypointList[targetWPIndex].x, transform.position.y, pWaypointList[targetWPIndex].z));

        _rb.constraints = RigidbodyConstraints.FreezeAll;

        while (true)
        {
            if (Vector3.Distance(new Vector3(transform.position.x, (_moveVertically) ? transform.position.y - _totalYOffset : 0, transform.position.z),
                new Vector3(pWaypointList[targetWPIndex].x, (_moveVertically) ? pWaypointList[targetWPIndex].y : 0, pWaypointList[targetWPIndex].z)) <= Time.deltaTime * _speed * 1.5f)
            {
                reachedWPIndex = Mathf.Clamp(reachedWPIndex + 1, 0, pWaypointList.Count - 1);
                if (reachedWPIndex >= pWaypointList.Count - 1)
                {
                    _currentTile = pTileList[reachedWPIndex];
                    _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

                    if (_currentTile.GetComponent<TileEvent>() != null && _currentTile.GetComponent<TileEvent>().enabled ||
                        _currentTile.GetComponent<QuizTile>() != null && _currentTile.GetComponent<QuizTile>().enabled)
                    {
                        onPawnReachedTileEvent?.Invoke(_currentTile);
                        AnswerDisplay.OnQuizHide += onHideQuiz;
                    }
                    else
                    {
                        onPawnReachedRegularTile?.Invoke(_currentTile);
                    }

                    onPawnReachedFinalTile?.Invoke();
                    onPawnReachedFinalTileWithGameObject?.Invoke(_currentTile.gameObject);
                    yield break;
                }
                else
                {
                    targetWPIndex = Mathf.Clamp(targetWPIndex + 1, 0, pWaypointList.Count - 1);
                    _currentTile = pTileList[targetWPIndex];
                    direction = (new Vector3(pWaypointList[targetWPIndex].x, (_moveVertically) ? pWaypointList[targetWPIndex].y : 0, pWaypointList[targetWPIndex].z) -
                                 new Vector3(transform.position.x, (_moveVertically) ? transform.position.y - _totalYOffset : 0, transform.position.z)).normalized;
                    transform.LookAt(new Vector3(pWaypointList[targetWPIndex].x, transform.position.y, pWaypointList[targetWPIndex].z));
                }
            }

            transform.Translate(direction * Time.deltaTime * pSpeed, Space.World);
            if (!_player.HasCurrentTurn) break;
            yield return null;
        }
    }

    private void onHideQuiz()
    {
        if (TTPlayer.LocalPlayer.GetComponent<VasilPlayer>().HasCurrentTurn)
            TTPlayer.LocalPlayer.RequestHideQuiz();
        onPawnReachedRegularTile?.Invoke(_currentTile);
        onPawnReachedFinalTile?.Invoke();
        AnswerDisplay.OnQuizHide -= onHideQuiz;
    }
}
