using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class AnswerDisplay : MonoBehaviour
{
    public static Action OnQuizHide;
    public TMP_Text[] answers;
    private List<TMP_Text> answerList = new List<TMP_Text>();


    void Start()
    {
        foreach (var a in answers)
        {
            answerList.Add(a);
        }
    }

    public void DisplayAnswers()
    {
        Invoke(nameof(answerDisplay), 0.1f);
    }

    private void answerDisplay()
    {      
        transform.GetChild(0).gameObject.SetActive(true);

        

        Question q = QuizManager.singleton.CurrentQuestion;

        for (int i = 0; i < answerList.Count; i++)
        {
            TMP_Text temp = answerList[i];
            int randomIndex = UnityEngine.Random.Range(i, answerList.Count);
            answerList[i] = answerList[randomIndex];
            answerList[randomIndex] = temp;
        }

        for (int i = 0; i < answerList.Count; i++)
        {
            answerList[i].transform.parent.GetComponent<Button>().interactable = true;

            answerList[i].transform.parent.parent.Find("Correct").gameObject.SetActive(false);
            answerList[i].transform.parent.parent.Find("Wrong").gameObject.SetActive(false);
            answerList[i].text = q.answers[i];
        }


    }

    public void CheckAnswer(TMP_Text pAnswer)
    {
        if (pAnswer.text == QuizManager.singleton.CurrentQuestion.correctAnswer)
        {
            Debug.Log("Correct!");
            pAnswer.transform.parent.parent.Find("Correct").gameObject.SetActive(true);

            for (int i = 0; i < answerList.Count; i++)
            {
                answerList[i].transform.parent.GetComponent<Button>().interactable = false;
            }

            Invoke(nameof(HideQuiz), 3);
        }
        else
        {
            Debug.Log("Sorry, but that's wrong");
            pAnswer.transform.parent.parent.Find("Wrong").gameObject.SetActive(true);
            StartCoroutine(ResetAnswers(pAnswer.transform.parent.parent.Find("Wrong").gameObject, 0.2f));

        }
    }

    private void HideQuiz()
    {
        OnQuizHide?.Invoke();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator ResetAnswers(GameObject wrong, float delay)
    {
        yield return new WaitForSeconds(delay);
        wrong.SetActive(false);
    }




}
