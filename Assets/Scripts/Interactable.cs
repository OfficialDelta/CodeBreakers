using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public GameObject computerPanel;

    [SerializeField] private TextMeshProUGUI[] answers;
    [SerializeField] private TextMeshProUGUI questionTextUI;

    [SerializeField] private GameObject continueButton;

    public TextAsset jsonFile;

    private QuizData quizData;

    private int questionNum = 0;

    private bool inBounds;
    public void OnInteract()
    {
        inBounds = true;
    }
    public void OnInteractExit()
    {
        inBounds = false;
    }

    private void Start()
    {
        computerPanel.SetActive(false);
        quizData = JsonUtility.FromJson<QuizData>(jsonFile.text);
    }

    private void Update()
    {
        //Enter the computer
        if (inBounds && Input.GetKeyDown(KeyCode.E))
        {
            computerPanel.SetActive(true);
            LoadQuizData();
        }
    }

    public void ExitComputer()
    {
        computerPanel.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().canWalk = true;
    }

    void LoadQuizData()
    {
        if (questionNum >= quizData.questions.Length)
        {
            ExitComputer();
            return;
        }
        questionTextUI.text = quizData.questions[questionNum].question;
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].text = quizData.questions[questionNum].answers[i];
            answers[i].gameObject.GetComponentInParent<Image>().color = Color.white;
        }
    }

    public void CheckAnswer(int answerNum)
    {
        if (answerNum == quizData.questions[questionNum].correct)
        {
            answers[answerNum].gameObject.GetComponentInParent<Image>().color = Color.green;
            questionNum++;
            // cooldown for 1 second
            Invoke("LoadQuizData", 1f);
        }
        else
        {
            answers[answerNum].gameObject.GetComponentInParent<Image>().color = Color.red;
        }
    }

    [Serializable]
    public class QuestionData
    {
        public string question;
        public string[] answers;
        public int correct;
    }

    [Serializable]
    public class QuizData
    {
        public QuestionData[] questions;
    }
}
