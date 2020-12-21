using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VasilPlayer : MonoBehaviour
{
    [SerializeField] private Sprite _characterImage = null;
    public Sprite CharacterImage => _characterImage;
    [SerializeField] private GameObject _turnOrderPositionPrefab = null;
    public GameObject TurnOrderPositionPrefab => _turnOrderPositionPrefab;
    [SerializeField] private string _playerName = "";
    public string PlayerName => _playerName;

    [HideInInspector] public int turnsToSkip = 0;
    [HideInInspector] public int extraTurns = 0;
    [HideInInspector] public bool hasActedThisTurn = false;

    private bool _hasCurrentTurn = false;
    public bool HasCurrentTurn => _hasCurrentTurn;
    private bool _isOutOfActions = false;
    public bool IsOutOfActions => _isOutOfActions;

    private int _dieRolls = 1;
    private bool _canRollAgain = true;

    private Pawn _pawn;
    public Pawn Pawn => _pawn;

    private void Start()
    {
        FindObjectOfType<TurnOrderManager>().GetComponent<TurnOrderManager>().Init();

        GameObject.Find("RollButton").GetComponent<Button>().onClick.AddListener(() => RollDieInput());
    }

    private void OnEnable()
    {
        //RollDie.onDieRolled += onDieRolled;
        Pawn.onPawnReachedTileEvent += onPawnReachedTileEvent;
        Pawn.onPawnReachedRegularTile += onPawnReachedRegularTile;
        _turnOrderPositionPrefab.GetComponentInChildren<TurnOrderPlayerAvatar>().SetPlayer(this);

        Pawn[] pawns = FindObjectsOfType(typeof(Pawn)) as Pawn[];
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].name == _playerName)
            {
                _pawn = pawns[i];
                _pawn.SetPlayer(this);
                break;
            }
        }
    }

    private void OnDisable()
    {
        //RollDie.onDieRolled -= onDieRolled;
        Pawn.onPawnReachedTileEvent -= onPawnReachedTileEvent;
        Pawn.onPawnReachedRegularTile -= onPawnReachedRegularTile;
    }

    private void Update()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RollTheDie();
            _dieRolls--;
            if (_dieRolls <= 0)
                _canRollAgain = false;
        }
    }

    public void RollDieInput()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        RollTheDie();
        _dieRolls--;
        if (_dieRolls <= 0)
            _canRollAgain = false;
    }

    public int RollForInitialive(int pSides = 6)
    {
        int rand = Random.Range(1, pSides + 1);
        return rand;
    }

    public void RollTheDie()
    {
        RollDie die = FindObjectOfType(typeof(RollDie)) as RollDie;
        die.StartCoroutine(die.RollRandom());
    }

    public void StartTurn()
    {
        _hasCurrentTurn = true;
        _isOutOfActions = false;

        _dieRolls = 1;
        _canRollAgain = true;

        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("{0} press 'R' roll your die", _playerName);
        StartCoroutine(lerpToColor(_pawn.GetComponent<Renderer>().material.color));
    }

    public void EndTurn()
    {
        _hasCurrentTurn = false;
        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = "";
    }

    private void onPawnReachedTileEvent(Tile pTile)
    {
        if (!_hasCurrentTurn) return;
        _isOutOfActions = false;
    }

    private void onPawnReachedRegularTile(Tile pTile)
    {
        if (!_hasCurrentTurn) return;
        _isOutOfActions = true;
    }

    public IEnumerator EndTurnAfterDelay(bool pIsOutOfActions, float pDelay)
    {
        while (pDelay > 0)
        {
            if (pDelay % 1 <= 0.05f)
                Debug.Log($"{_playerName}'s turn will end in {(int)pDelay}s.");
            yield return new WaitForSeconds(0.1f);
            pDelay -= 0.1f;
        }
        _isOutOfActions = pIsOutOfActions;
        TurnOrderManager.Singleton.EndCurrentPlayerTurn();
        Debug.Log($"{_playerName}'s turn had ended.");
    }

    private void onDieRolled(int pInt)
    {
        if (!_hasCurrentTurn) return;
        _isOutOfActions = true;
    }

    private IEnumerator lerpToColor(Color pColor)
    {
        RollDie die = FindObjectOfType(typeof(RollDie)) as RollDie;
        Renderer dieRend = die.gameObject.transform.GetChild(0).GetComponent<Renderer>();
        Renderer boardRend = GameObject.Find("BackgoundEmmision").GetComponent<Renderer>();
        boardRend.material.EnableKeyword("_EMISSION");
        Color newCol = dieRend.material.color;

        while (true)
        {
            newCol = Color.Lerp(newCol, pColor, Time.deltaTime * 5);

            dieRend.material.color = newCol;
            dieRend.material.SetColor("_EmissionColor", newCol);

            boardRend.material.color = newCol;
            boardRend.material.SetColor("_EmissionColor", newCol);

            dieRend.gameObject.transform.GetComponentInChildren<Light>().color = newCol;

            if (newCol == pColor) yield break;
            else yield return null;
        }
    }
}
