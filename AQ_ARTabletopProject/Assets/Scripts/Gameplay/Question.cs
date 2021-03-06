﻿using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
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
