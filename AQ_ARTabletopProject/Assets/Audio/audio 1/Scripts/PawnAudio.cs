using UnityEngine;

public class PawnAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _sound;
    [SerializeField] private AudioClip _questionSound;
    [SerializeField] private AudioClip _positiveSound;
    [SerializeField] private AudioClip _negativeSound;
    private VasilPlayer _player;

    private void Start()
    {
        _player = GetComponent<Pawn>().Player;
        Pawn.onPawnReachedFinalTileWithGameObject += playSound;
    }

    private void OnDestroy()
    {
        Pawn.onPawnReachedFinalTileWithGameObject -= playSound;
    }

    private void playSound(GameObject pTile)
    {
        if (!_player.HasCurrentTurn) return;

        if (pTile.CompareTag("Tile"))
            FindObjectOfType<AudioManager>().Play(_sound);
        else if (pTile.CompareTag("QuestionTile"))
            FindObjectOfType<AudioManager>().Play(_questionSound);
        else if (pTile.CompareTag("PositiveTile"))
            FindObjectOfType<AudioManager>().Play(_positiveSound);
        else if (pTile.CompareTag("NegativeTile"))
            FindObjectOfType<AudioManager>().Play(_negativeSound);
    }
}
