using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerCheck : MonoBehaviour
{
    public string buttonAnswer;
    private TMP_Text answer1Text;
    private readonly List<string> rightAnswerList = new List<string>();
    void Start()
    {
        Question[] questions = Resources.LoadAll<Question>("QuizQuestions");

        //rightAnswer = questions[].correctAnswer.ToString();

        foreach (var question in questions)
        {
            rightAnswerList.Add(question.correctAnswer);
        }
    }
    public void AnswerButtonClicked(bool isCorrect) { 
        //check if correctanswer string is same as pressed button string
        if (isCorrect)
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }

        if (isCorrect)
        {
            //display green correct button, then disable after a few seconds
        }

        if (!isCorrect)
        {
            //display red wrong button, then reset to default answer state
        }

    }
}
