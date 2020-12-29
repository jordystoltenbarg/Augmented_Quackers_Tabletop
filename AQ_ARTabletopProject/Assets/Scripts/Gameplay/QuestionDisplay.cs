using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    private LocalizeStringEvent _localizedQuestionTextEvent;
    private LocalizeStringEvent _localizedQuestionTheme;
    private Image _bubbleIcon;

    private void Awake()
    {
        _localizedQuestionTextEvent = transform.GetChild(0).GetComponent<LocalizeStringEvent>();
        _localizedQuestionTheme = transform.GetChild(1).GetChild(0).GetComponent<LocalizeStringEvent>();
        _bubbleIcon = transform.GetChild(1).GetChild(1).GetComponent<Image>();
    }

    private void OnEnable()
    {
        setupQuestionDisplay();
    }

    private void setupQuestionDisplay()
    {
        Question[] qs = QuizManager.Singleton.Questions;
        int random = Random.Range(0, qs.Length);
        _localizedQuestionTextEvent.StringReference = qs[random].LocalizedQuestion;

        QuizManager.Singleton.SetQuestion(random);
        _bubbleIcon.sprite = QuizManager.Singleton.QuizBubbleIcon;
        _localizedQuestionTheme.StringReference = QuizManager.Singleton.LocalizedQuestionTheme;
    }
}
