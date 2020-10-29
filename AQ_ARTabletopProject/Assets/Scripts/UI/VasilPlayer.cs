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
    private bool _isOutOfActions = false;
    public bool IsOutOfActions { get { return _isOutOfActions; } }

    private Pawn _pawn;

    private void OnEnable()
    {
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

    private void Update()
    {
        if (!_hasCurrentTurn || _isOutOfActions) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            int roll = RollDie();
            _pawn.SetNextTargetTile(TilesManager.ListOfTiles, roll);
            _isOutOfActions = true;

            GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("{0} rulled a {1}", _name, roll);
            GameObject.Find("Help").GetComponent<TextMeshProUGUI>().text = string.Format("{0} you're out of AP press 'D' to end you turn", _name);
        }
    }

    public int RollDie(int pSides = 6)
    {
        int rand = Random.Range(1, pSides + 1);
        return rand;
    }

    public void StartTurn()
    {
        _hasCurrentTurn = true;
        _isOutOfActions = false;

        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("{0} press 'R' roll your die", _name);
        GameObject.Find("BackgoundEmmision").GetComponent<Renderer>().material.color = _pawn.GetComponent<Renderer>().material.color;
        GameObject.Find("BackgoundEmmision").GetComponent<Renderer>().material.SetColor("_EmissionColor", _pawn.GetComponent<Renderer>().material.color);
    }

    public void EndTurn()
    {
        _hasCurrentTurn = false;
        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = "";
    }
}
