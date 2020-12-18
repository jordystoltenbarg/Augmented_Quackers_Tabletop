using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTile : MonoBehaviour
{
    public GameObject QuizBubble;

    void Start()
    {
        Pawn.onPawnReachedFinalTileWithGameObject += onPawnFinalTileReached;

        AnswerDisplay.OnQuizHide += HideQuiz;
    }

    private void onPawnFinalTileReached(GameObject pGameObject)
    {
        if (pGameObject == this.gameObject && QuizBubble != null)
        {
            QuizBubble.SetActive(true);
            AnswerDisplay aD = FindObjectOfType<AnswerDisplay>();
            aD.DisplayAnswers();
        }
    }

    private void HideQuiz()
    {
        QuizBubble.SetActive(false);
    }
}
