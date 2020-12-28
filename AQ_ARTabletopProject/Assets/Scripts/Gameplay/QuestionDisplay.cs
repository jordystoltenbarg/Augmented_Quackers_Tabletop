using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class QuestionDisplay : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _localizedQuestionTextEvent;
    [SerializeField] private Image _bubbleIcon;

    void OnEnable()
    {
        setupQuestionDisplay();
    }

    private void setupQuestionDisplay()
    {
        Question[] qs = QuizManager.Singleton.Questions;
        int random = Random.Range(0, qs.Length);
        _localizedQuestionTextEvent.StringReference = qs[random].LocalizedQuestion;

        QuizManager.Singleton.SetQuestion(random);
        _bubbleIcon.sprite = QuizManager.Singleton.QuizBubbleIcon.sprite;
    }
}
