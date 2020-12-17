using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionDisplay : MonoBehaviour
{
    public TMP_Text questionText;


    void OnEnable()
    {
        Question[] questions = QuizManager.singleton.questions;
        int random = Random.Range(0, questions.Length);
        questionText.text = questions[random].question.ToString();

        QuizManager.singleton.SetQuestion(random);
    }

}
