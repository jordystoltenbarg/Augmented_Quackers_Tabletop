using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{

    public static QuizManager singleton = null;
    [HideInInspector]public Question[] questions;
    private Question currentQuestion;
    public Question CurrentQuestion => currentQuestion;
    public Sprite QuizBubbleIcon;
    public string QuizBubbleTheme;


    [SerializeField]
    private GameObject answerParent;

    private void Awake()
    {
        QuizManager[] QM = FindObjectsOfType<QuizManager>();

        if (QM.Length > 1)
        {
            Debug.Log("Destroyed GameManager");
            Destroy(gameObject);
            return;
            
        }

        singleton = this;

        LoadQuestions();

        

    }

    private void LoadQuestions ()
    {
        questions = Resources.LoadAll<Question>("QuizQuestions");
    }

    public void SetQuestion(int index)
    {
        currentQuestion = questions[index];
        QuizBubbleIcon = currentQuestion.QuizIcon;
        QuizBubbleTheme = currentQuestion.quizTheme;
    }


    


}
