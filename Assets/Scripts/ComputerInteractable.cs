using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInteractable : MonoBehaviour
{
    public GameObject computerPanel;
    private int gameNum = -1;


    public GameObject game1;
    [SerializeField] private TextMeshProUGUI[] quizAnswers;
    [SerializeField] private TextMeshProUGUI quizQuestionTextUI;
    private int questionNum = 0;
    public TextAsset jsonFile;
    private QuizData quizData;

    public GameObject game2;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Texture2D[] dataDecryptImages;
    [SerializeField] private GameObject[] dropZones;
    bool imagesLoaded = false;
    bool canClick = true;

    private System.Random rng = new System.Random();
    public bool completed = false;
    public GameObject exitButton;
    public int numGames;

    private T[] Shuffle<T>(T[] originalArray)
    {
        T[] array = (T[])originalArray.Clone();
        int n = originalArray.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }

        return array;
    }

    private bool inBounds;

    public GameObject intro;

    public void OnInteract()
    {
        inBounds = true;
    }
    public void OnInteractExit()
    {
        inBounds = false;
    }

    private void Awake()
    {
        game1.SetActive(false);
        game2.SetActive(false);
        intro.SetActive(false);

        computerPanel.SetActive(false);
        quizData = JsonUtility.FromJson<QuizData>(jsonFile.text);
    }

    private void Update()
    {
        //Enter the computer
        if (inBounds && Input.GetKeyDown(KeyCode.E) && !completed)
        {
            gameNum = -1;
            computerPanel.SetActive(true);
            LoadGame();
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().canWalk = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameNum++;
            game1.SetActive(false);
            game2.SetActive(false);
            completed = true;
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().completedText.SetActive(true);
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().interactText.SetActive(true);
            LoadGame();
        }
    }

    void LoadGame()
    {
        switch (gameNum)
        {
            case -1:
                exitButton.SetActive(true);
                intro.SetActive(true);
                LoadIntro();
                break;
            case 0:
                game1.SetActive(true);
                LoadQuizGame();
                break;
            case 1:
                if (numGames == 1)
                {
                    ExitComputer();
                    completed = true;
                }
                else
                {
                    game2.SetActive(true);
                    LoadDataDecryptGame();
                }
                break;
            default:
                ExitComputer();
                break;
        }
    }

    public void HandleExitButton()
    {
        ExitComputer();
    }

    void ExitComputer()
    {
        computerPanel.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().canWalk = true;
        Time.timeScale = 1.0f;
    }

    void LoadIntro()
    {
        Time.timeScale = 0.0f;
    }

    public void StartGame()
    {
        Time.timeScale = 1.0f;
        gameNum=0;
        intro.SetActive(false);
        exitButton.SetActive(false);
        LoadGame();
    }

    void LoadQuizGame()
    {
        canClick = true;
        if (questionNum >= quizData.questions.Length)
        {
            Debug.Log(questionNum);
            gameNum++;
            game1.SetActive(false);
            LoadGame();
            return;
        }
        quizQuestionTextUI.text = quizData.questions[questionNum].question;
        for (int i = 0; i < quizAnswers.Length; i++)
        {
            quizAnswers[i].text = quizData.questions[questionNum].answers[i];
            quizAnswers[i].gameObject.GetComponentInParent<Image>().color = Color.white;
        }
    }

    void LoadDataDecryptGame()
    {
        if (!imagesLoaded)
        {
            SpawnImages();
            imagesLoaded = true;
        }
    }

    void SpawnImages()
    {
        Texture2D[] shuffledImages = Shuffle(dataDecryptImages);
        int numImages = 9;
        Vector2 startPosition = new Vector2(-256, -122);
        Vector2 endPosition = new Vector2(256, -122);
        float distance = (endPosition.x - startPosition.x) / (numImages - 1);

        for (int i = 0; i < numImages; i++)
        {
            // Calculate position for each image
            Vector2 position = new Vector2(startPosition.x + i * distance, startPosition.y);

            // Instanstiate the prefab
            GameObject imageObj = Instantiate(imagePrefab, Vector3.zero, Quaternion.identity, game2.transform);
            RectTransform rt = imageObj.GetComponent<RectTransform>();
            rt.anchoredPosition = position; // Set the anchored position, not the transform position
            rt.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            rt.sizeDelta = new Vector2(shuffledImages[i].width, shuffledImages[i].height); // Adjust the size by the scale factor

            // Set the name
            int row = i / 3;
            int col = i % 3;
            imageObj.name = $"{row}{col}";

            // Set the image source
            Sprite newSprite = LoadSprite(shuffledImages[i]);
            imageObj.GetComponent<Image>().sprite = newSprite;
        }
    }

    Sprite LoadSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public bool CheckImagePosition(int imgIdx, int zoneIdx)
    {
        return imgIdx == zoneIdx;
    }

    public void CheckAnswer(int answerNum)
    {
        if (canClick)
        {
            if (answerNum == quizData.questions[questionNum].correct)
            {
                quizAnswers[answerNum].gameObject.GetComponentInParent<Image>().color = Color.green;
                questionNum++;
                Invoke("LoadQuizGame", 0.0f);
            }
            else
            {
                quizAnswers[answerNum].gameObject.GetComponentInParent<Image>().color = Color.red;

                canClick = false;
                // cooldown for 1 second
                Invoke("LoadQuizGame", 1.0f);
            }
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
