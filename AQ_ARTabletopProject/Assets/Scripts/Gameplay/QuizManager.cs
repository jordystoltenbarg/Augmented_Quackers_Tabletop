using UnityEngine;
using UnityEngine.Localization;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Singleton = null;

    [SerializeField] private Sprite[] _questionTypeVariations;
    [SerializeField] private LocalizedString[] _localizedQuestionThemeStrings;

    private Question[] _questions = null;
    public Question[] Questions => _questions;
    private Question _currentQuestion = null;
    public Question CurrentQuestion => _currentQuestion;
    private Sprite _quizBubbleIcon = null;
    public Sprite QuizBubbleIcon => _quizBubbleIcon;
    private LocalizedString _localizedQuestionTheme = null;
    public LocalizedString LocalizedQuestionTheme => _localizedQuestionTheme;

    private void Awake()
    {
        QuizManager[] qm = FindObjectsOfType<QuizManager>();
        if (qm.Length > 1)
        {
            Debug.Log("QuizManager already exists: DESTROY!");
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        LoadQuestions();
    }

    private void LoadQuestions()
    {
        _questions = Resources.LoadAll<Question>("QuizQuestions");
    }

    public void SetQuestion(int pIndex)
    {
        _currentQuestion = _questions[pIndex];
        switch (_currentQuestion.QuestionType)
        {
            case Question.Type.Nature:
                _quizBubbleIcon = _questionTypeVariations[0];
                _localizedQuestionTheme = _localizedQuestionThemeStrings[0];
                break;
            case Question.Type.History:
                _quizBubbleIcon = _questionTypeVariations[1];
                _localizedQuestionTheme = _localizedQuestionThemeStrings[1];
                break;
            case Question.Type.WaterTreatment:
                _quizBubbleIcon = _questionTypeVariations[2];
                _localizedQuestionTheme = _localizedQuestionThemeStrings[2];
                break;
            case Question.Type.Bats:
                _quizBubbleIcon = _questionTypeVariations[3];
                _localizedQuestionTheme = _localizedQuestionThemeStrings[3];
                break;
        }
    }
}
