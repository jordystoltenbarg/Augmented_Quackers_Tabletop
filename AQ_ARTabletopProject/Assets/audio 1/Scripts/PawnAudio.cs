using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAudio : MonoBehaviour

{    //public GameObject moving;
     public AudioClip Sound;
     public AudioClip QuestionSound;
     public AudioClip PositiveSound;
     public AudioClip NegativeSound;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            //print("Something gone wrong!");
            FindObjectOfType<AudioManager>().Play(Sound);
        }
        else if (other.tag == "QuestionTile")
        {
            //print("Something gone wrong!");
            FindObjectOfType<AudioManager>().Play(QuestionSound);
        }
        else if (other.tag == "PositiveTile")
        {
            //print("Something gone wrong!");
            FindObjectOfType<AudioManager>().Play(PositiveSound);
        }
        else if (other.tag == "NegativeTile")
        {
            //print("Something gone wrong!");
            FindObjectOfType<AudioManager>().Play(NegativeSound);
        }
    }
}
