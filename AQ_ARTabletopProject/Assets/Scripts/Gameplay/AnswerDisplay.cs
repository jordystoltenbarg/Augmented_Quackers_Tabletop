using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class AnswerDisplay : MonoBehaviour
{
    public static Action OnQuizHide;
    private readonly List<LocalizeStringEvent> _localizedAnswersList = new List<LocalizeStringEvent>();

    private void Start()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            //Get the localized string event from the text
            LocalizeStringEvent lse = transform.GetChild(0).GetChild(i).GetComponentInChildren<LocalizeStringEvent>();
            //Add it to the list
            _localizedAnswersList.Add(lse);
            //Add a listener to the Answer button
            lse.GetComponentInParent<Button>().onClick.AddListener(() => CheckAnswer(lse));
        }
    }

    public void DisplayAnswers()
    {
        Invoke(nameof(answerDisplay), 0.1f);
    }

    private void answerDisplay()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        Question q = QuizManager.Singleton.CurrentQuestion;
        for (int i = 0; i < _localizedAnswersList.Count; i++)
        {
            LocalizeStringEvent temp = _localizedAnswersList[i];
            int randomIndex = UnityEngine.Random.Range(i, _localizedAnswersList.Count);
            _localizedAnswersList[i] = _localizedAnswersList[randomIndex];
            _localizedAnswersList[randomIndex] = temp;
        }

        bool isLocalPlayer = TTPlayer.LocalPlayer.GetComponent<VasilPlayer>().HasCurrentTurn;
        for (int i = 0; i < _localizedAnswersList.Count; i++)
        {
            _localizedAnswersList[i].transform.parent.GetComponent<Button>().interactable = isLocalPlayer;

            _localizedAnswersList[i].transform.parent.parent.Find("Correct").gameObject.SetActive(false);
            _localizedAnswersList[i].transform.parent.parent.Find("Wrong").gameObject.SetActive(false);
            if (i == _localizedAnswersList.Count - 1)
                _localizedAnswersList[i].StringReference = q.LocalizedCorrectAnswer;
            else
                _localizedAnswersList[i].StringReference = q.LocalizedWrongAnswersArray[i];
        }
    }

    public void CheckAnswer(LocalizeStringEvent pLocalizedStringEvent)
    {
        if (pLocalizedStringEvent.StringReference == QuizManager.Singleton.CurrentQuestion.LocalizedCorrectAnswer)
        {
            Debug.Log("Correct!");
            pLocalizedStringEvent.transform.parent.parent.Find("Correct").gameObject.SetActive(true);

            for (int i = 0; i < _localizedAnswersList.Count; i++)
            {
                _localizedAnswersList[i].transform.parent.GetComponent<Button>().interactable = false;
            }

            Invoke(nameof(HideQuiz), 3);
        }
        else
        {
            Debug.Log("Sorry, but that's wrong");
            pLocalizedStringEvent.transform.parent.parent.Find("Wrong").gameObject.SetActive(true);
            StartCoroutine(ResetAnswers(pLocalizedStringEvent.transform.parent.parent.Find("Wrong").gameObject, 0.2f));
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
