using TMPro;
using UnityEngine;

public class VasilPlayer : MonoBehaviour
{
    [SerializeField]
    private Sprite _characterImage = null;
    public Sprite CharacterImage { get { return _characterImage; } }
    [SerializeField]
    private GameObject _turnOrderPositionPrefab = null;
    public GameObject TurnOrderPositionPrefab { get { return _turnOrderPositionPrefab; } }
    [SerializeField]
    private string _name = "";
    public string PlayerName { get { return _name; } }

    [HideInInspector]
    public int turnsToSkip = 0;
    [HideInInspector]
    public int extraTurns = 0;
    [HideInInspector]
    public bool hasActedThisTurn = false;

    private bool _hasCurrentTurn = false;
    public bool HasCurrentTurn { get { return _hasCurrentTurn; } }
    private bool _isOutOfActions = false;
    public bool IsOutOfActions { get { return _isOutOfActions; } }

    private int _dieRolls = 1;
    private bool _canRollAgain = true;

    private Pawn _pawn;

    private void OnEnable()
    {
        RollDie.OnDieRolled += onDieRoll;
        _turnOrderPositionPrefab.GetComponentInChildren<TurnOrderPlayerAvatar>().SetPlayer(this);

        Pawn[] pawns = FindObjectsOfType(typeof(Pawn)) as Pawn[];
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].name == _name)
            {
                _pawn = pawns[i];
                _pawn.SetPlayer(this);
                break;
            }
        }
    }

    private void OnDisable()
    {
        RollDie.OnDieRolled -= onDieRoll;
    }

    private void Update()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Roll();
            _dieRolls--;
            if (_dieRolls <= 0)
                _canRollAgain = false;
        }
    }

    public int RullDie(int pSides = 6)
    {
        int rand = Random.Range(1, pSides + 1);
        return rand;
    }

    public void Roll()
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

        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("{0} press 'R' roll your die", _name);
        GameObject.Find("BackgoundEmmision").GetComponent<Renderer>().material.color = _pawn.GetComponent<Renderer>().material.color;
        GameObject.Find("BackgoundEmmision").GetComponent<Renderer>().material.SetColor("_EmissionColor", _pawn.GetComponent<Renderer>().material.color);

        RollDie die = FindObjectOfType(typeof(RollDie)) as RollDie;
        die.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = _pawn.GetComponent<Renderer>().material.color;
        die.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", _pawn.GetComponent<Renderer>().material.color);
    }

    public void EndTurn()
    {
        _hasCurrentTurn = false;
        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = "";
    }

    void onDieRoll(int pInt)
    {
        _isOutOfActions = true;
    }
}
