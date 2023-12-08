using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject computerPanel;

    [SerializeField] private TextMeshProUGUI[] answers;
    [SerializeField] private TextMeshProUGUI questionTextUI;

    public TextAsset jsonFile;

    private QuizData quizData;

    public int questionNum;

    private bool inBounds;
    public void OnInteract()
    {
        inBounds = true;
    }
    public void OnInteractExit()
    {
        inBounds = false;
    }

    private void Update()
    {
        //Enter the computer
        if (inBounds && Input.GetKeyDown(KeyCode.E))
        {
            computerPanel.SetActive(true);
            
            
        }
    }

    public void ExitComputer()
    {
        computerPanel.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().canWalk = true;
    }

    void LoadQuizData()
    {
        quizData = JsonUtility.FromJson<QuizData>(jsonFile.text);
        foreach (var _question in quizData.questions)
        {
            string questionText = _question.question;
            string[] answerOptions = _question.answers;

            for (int i = 0; i < 4; i++)
            {
                answers[i].text = answerOptions[i];
            }
            questionTextUI.text = questionText;
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
