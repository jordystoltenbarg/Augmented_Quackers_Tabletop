using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Singleton = null;

    [SerializeField] private Sprite[] _questionTypeVariations;
    [SerializeField] private GameObject _answerParent;

    private Question[] _questions;
    public Question[] Questions => _questions;
    private Question _currentQuestion;
    public Question CurrentQuestion => _currentQuestion;
    private Image _quizBubbleIcon;
    public Image QuizBubbleIcon => _quizBubbleIcon;

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
                _quizBubbleIcon.sprite = _questionTypeVariations[0];
                break;
            case Question.Type.History:
                _quizBubbleIcon.sprite = _questionTypeVariations[1];
                break;
            case Question.Type.WaterTreatment:
                _quizBubbleIcon.sprite = _questionTypeVariations[2];
                break;
            case Question.Type.Bats:
                _quizBubbleIcon.sprite = _questionTypeVariations[3];
                break;
        }
    }
}
