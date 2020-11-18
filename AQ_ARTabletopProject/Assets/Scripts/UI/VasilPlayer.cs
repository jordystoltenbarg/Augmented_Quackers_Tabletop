using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Pawn pawn => _pawn;

    private void Start()
    {
        FindObjectOfType<TurnOrderManager>().GetComponent<TurnOrderManager>().Init();

        GameObject.Find("RollButton").GetComponent<Button>().onClick.AddListener(() => RollDieInput());
    }

    private void OnEnable()
    {
        RollDie.onDieRolled += onDieRoll;
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
        RollDie.onDieRolled -= onDieRoll;
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

    public void RollDieInput()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        Roll();
        _dieRolls--;
        if (_dieRolls <= 0)
            _canRollAgain = false;
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
        StartCoroutine(lerpToColor(_pawn.GetComponent<Renderer>().material.color));
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

    IEnumerator lerpToColor(Color pColor)
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
