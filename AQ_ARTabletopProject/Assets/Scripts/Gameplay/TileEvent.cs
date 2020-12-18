using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEvent : MonoBehaviour
{
    public enum EventType
    {
        StepForwardByX,
        StepBackwardsByX,
        SkipYourNextTurn,
        GoToLinkedTile,
        GoBackToStart
    }
    [SerializeField] private EventType _eventType;
    [Header("Pawn")]
    [Tooltip("If <0 will not affect Pawn moveSpeed")]
    [SerializeField] private float _pawnSpeedOverride = -1;
    [Header("MoveByX")]
    [SerializeField] private int _x = 2;
    [Header("Skip Turn")]
    [SerializeField] private float _endPlayerTurnAfter = 3;
    [Header("GoToLinkedTile")]
    [SerializeField] private Tile _linkedTile = null;

    private void Start()
    {
        Pawn.onPawnReachedTileEvent += onPawnReachedTileEvent;
    }

    private void onPawnReachedTileEvent(Tile pTile)
    {
        if (pTile.gameObject != gameObject) return;
        switch (_eventType)
        {
            case EventType.StepForwardByX:
                Debug.Log($"Step {_x} tiles forward");

                TurnOrderManager.CurrentPlayerTurn.Pawn.MoveByValue(_x, _pawnSpeedOverride);
                break;
            case EventType.StepBackwardsByX:
                Debug.Log($"Step {_x} tiles back");

                TurnOrderManager.CurrentPlayerTurn.Pawn.MoveByValue(-_x, _pawnSpeedOverride);
                break;
            case EventType.SkipYourNextTurn:
                Debug.Log("Skipped your next turn");

                TurnOrderManager.Singleton.SkipCurrentPlayerTurn();
                TurnOrderManager.CurrentPlayerTurn.StartCoroutine(TurnOrderManager.CurrentPlayerTurn.EndTurnAfterDelay(true, _endPlayerTurnAfter));
                break;
            case EventType.GoToLinkedTile:
                Debug.Log("Go to linked tile");

                List<Tile> tiles = new List<Tile>();
                tiles.Add(GetComponent<Tile>());
                for (int i = 0; i < transform.childCount; i++)
                    tiles.Add(transform.GetChild(i).GetComponent<Tile>());
                tiles.Add(_linkedTile);
                TurnOrderManager.CurrentPlayerTurn.Pawn.MoveAlongCustomPath(tiles, _pawnSpeedOverride);
                break;
            case EventType.GoBackToStart:
                Debug.Log("<color=red>!!!BACK TO THE PIT!!!</color>");
                break;
        }
    }
}
