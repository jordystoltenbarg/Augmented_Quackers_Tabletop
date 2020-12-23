using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    public TMP_Text questionText;
    public GameObject BubbleIcon;
    public TMP_Text themeText;

    void OnEnable()
    {
        Question[] questions = QuizManager.singleton.questions;
        int random = Random.Range(0, questions.Length);
        questionText.text = questions[random].question.ToString();

        QuizManager.singleton.SetQuestion(random);
        BubbleIcon.GetComponent<Image>().sprite = QuizManager.singleton.QuizBubbleIcon;
        themeText.text = QuizManager.singleton.QuizBubbleTheme;
    }
}
