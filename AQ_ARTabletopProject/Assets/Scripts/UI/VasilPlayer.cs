using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VasilPlayer : MonoBehaviour
{
    [Header("Turn Related")]
    [SerializeField] private GameObject[] _turnOrderPositionPrefabs = null;
    [SerializeField] private Material[] _emissionMaterials = null;
    private Sprite _characterImage = null;
    public Sprite CharacterImage => _characterImage;
    private GameObject _turnOrderPositionPrefab = null;
    public GameObject TurnOrderPositionPrefab => _turnOrderPositionPrefab;
    [Header("Pawns")]
    [SerializeField] private GameObject[] _pawnPrefabs = null;
    private string _playerName = "";
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
    public AudioClip TurnSound;

    public void Init(int pLobbyIndex, int pSelectedCharacterIndex, int pColorVariation)
    {
        GameObject pawn = Instantiate(_pawnPrefabs[pSelectedCharacterIndex],
                                      GameObject.Find("Tile").transform.GetChild(pLobbyIndex).transform.position,
                                      GameObject.Find("Tile").transform.GetChild(pLobbyIndex).transform.rotation,
                                      //GameObject.Find("In-GameBoard").transform);
                                      GameObject.Find("Finalgameboard(Clone)").transform);
        _pawn = pawn.GetComponent<Pawn>();
        _pawn.SetPlayer(this);
        _turnOrderPositionPrefab = _turnOrderPositionPrefabs[pColorVariation];
        _characterImage = _turnOrderPositionPrefab.GetComponentInChildren<Image>().sprite;
        _turnOrderPositionPrefab.GetComponentInChildren<TurnOrderPlayerAvatar>().SetPlayer(this);

        //RollDie.onDieRolled += onDieRolled;
        Pawn.onPawnReachedTileEvent += onPawnReachedTileEvent;
        Pawn.onPawnReachedRegularTile += onPawnReachedRegularTile;
        GameObject.Find("RollButton").GetComponent<Button>().onClick.AddListener(() => RollDieInput());

        Invoke(nameof(initTOM), 0.25f);
    }

    private void initTOM()
    {
        FindObjectOfType<TurnOrderManager>().Init();
    }

    private void OnDestroy()
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

    public void SimulateDieThrow()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        _dieRolls--;
        if (_dieRolls <= 0)
            _canRollAgain = false;
    }

    public void RollDieInput()
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        RollTheDie();
        _dieRolls--;
        if (_dieRolls <= 0)
            _canRollAgain = false;
    }

    public void RollDieInput(Vector2 pDieTossValues)
    {
        if (!_hasCurrentTurn || !_canRollAgain) return;

        RollTheDie(pDieTossValues);
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
        GetComponent<TTPlayer>().RequestDieRoll();
        //RollDie die = FindObjectOfType<RollDie>();
        //die.StartCoroutine(die.RollRandom());
    }

    public void RollTheDie(Vector2 pDieTossValues)
    {
        RollDie die = FindObjectOfType<RollDie>();
        die.StartCoroutine(die.RollWithValues(pDieTossValues));
    }

    public void StartTurn()
    {
        if (GetComponent<TTPlayer>().isLocalPlayer)
        {
            FindObjectOfType<AudioManager>().Play(TurnSound);
            GameObject.Find("RollButton").GetComponent<Image>().enabled = true;
        }
        else
        {
            GameObject.Find("RollButton").GetComponent<Image>().enabled = false;
        }

        _hasCurrentTurn = true;
        _isOutOfActions = false;

        _dieRolls = 1;
        _canRollAgain = true;

        GameObject.Find("DieRoll").GetComponent<TextMeshProUGUI>().text = string.Format("{0} press 'R' roll your die", _playerName);
        StartCoroutine(lerpToColor(_emissionMaterials[GetComponent<TTPlayer>().ColorVariation].color));
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
