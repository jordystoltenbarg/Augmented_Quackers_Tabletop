using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    public string question;
    public string[] answers = new string[4];
    public string correctAnswer;
    public string quizTheme;

    //The icon should be part of the object displaying the question
    public Sprite QuizIcon;

    //Question
    [SerializeField] private LocalizedString _localizedQuestion;
    public LocalizedString LocalizedQuestion => _localizedQuestion;
    //Wrong Answers
    [SerializeField] private LocalizedString[] _localizedWrongAnswersArray;
    public LocalizedString[] LocalizedWrongAnswersArray => _localizedWrongAnswersArray;
    //Correct Asnwer
    [SerializeField] private LocalizedString _localizedCorrectAnswer;
    public LocalizedString LocalizedCorrectAnswer => _localizedCorrectAnswer;
    //Type of Question
    public enum Type
    {
        Nature,
        History,
        WaterTreatment,
        Bats
    }
    [SerializeField] private Type _questionType;
    public Type QuestionType => _questionType;
}
