using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _sound;
    [SerializeField] private AudioClip _questionSound;
    [SerializeField] private AudioClip _positiveSound;
    [SerializeField] private AudioClip _negativeSound;

    private void Start()
    {
        Pawn.onPawnReachedFinalTileWithGameObject += playSound;
    }

    private void OnDestroy()
    {
        Pawn.onPawnReachedFinalTileWithGameObject -= playSound;
    }

    private void playSound(GameObject pTile)
    {
        if (pTile.CompareTag("Tile"))
            FindObjectOfType<AudioManager>().Play(_sound);
        else if (pTile.CompareTag("QuestionTile"))
            FindObjectOfType<AudioManager>().Play(_questionSound);
        else if (pTile.CompareTag("PositiveTile"))
            FindObjectOfType<AudioManager>().Play(_positiveSound);
        else if (pTile.CompareTag("NegativeTile"))
            FindObjectOfType<AudioManager>().Play(_negativeSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Tile")
        //{
        //    //print("Something gone wrong!");
        //    FindObjectOfType<AudioManager>().Play(_sound);
        //}
        //else if (other.tag == "QuestionTile")
        //{
        //    //print("Something gone wrong!");
        //    FindObjectOfType<AudioManager>().Play(_questionSound);
        //}
        //else if (other.tag == "PositiveTile")
        //{
        //    //print("Something gone wrong!");
        //    FindObjectOfType<AudioManager>().Play(_positiveSound);
        //}
        //else if (other.tag == "NegativeTile")
        //{
        //    //print("Something gone wrong!");
        //    FindObjectOfType<AudioManager>().Play(_negativeSound);
        //}
    }
}
