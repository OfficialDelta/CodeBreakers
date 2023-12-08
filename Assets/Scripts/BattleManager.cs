using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Animator judgeAnimator;

    [SerializeField] private GameObject[] attackList;
    [SerializeField] private TextMeshProUGUI[] listTitle;

    [SerializeField] private GameObject prompt;
    [SerializeField] private GameObject result;
    [SerializeField] private GameObject attacksButton;
    private List<Spell> inventory;

    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider enemyHealthBar;

    [SerializeField] private float playerHealth = 100f;
    [SerializeField] private float enemyHealth = 100f;
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private Sprite[] head;
    [SerializeField] private Sprite[] body;
    [SerializeField] private Sprite[] arm;
    [SerializeField] private Sprite[] leg;

    [SerializeField] private Image headImg;
    [SerializeField] private Image bodyImg;
    [SerializeField] private Image armImg1;
    [SerializeField] private Image armImg2;
    [SerializeField] private Image legImg1;
    [SerializeField] private Image legImg2;

    [SerializeField] private PlayerAnimationController playerAnimationController;

    [SerializeField] private GameObject attackUI;
    [SerializeField] private GameObject parentObject;

    [SerializeField] private TextMeshProUGUI titleUI;
    [SerializeField] private TextMeshProUGUI descriptionUI;

    [SerializeField] private GameObject promptText;

    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;
    [SerializeField] private GameObject button;

    public int _caseId = 0;
    public int _attackId;

    void Start()
    {
        //DontDestroyOnLoad(this);
        playerHealth = 100f;
        enemyHealth = 100f;

        home();

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        // Check if the InventoryManager was found
        if (inventoryManager != null)
        {
            // Access the inventory list
            inventory = inventoryManager.inventory;

            // Do something with the inventory...
        }

        prompt.GetComponent<Animator>().SetTrigger("Enter");
        prompt.GetComponentInChildren<TextMeshProUGUI>().text = "Test";

        result.SetActive(false);

        int randomIndex = UnityEngine.Random.Range(0, 6);

        // Assign sprites based on the random index
        headImg.sprite = head[randomIndex];
        bodyImg.sprite = body[randomIndex];
        armImg1.sprite = arm[randomIndex];
        legImg1.sprite = leg[randomIndex];
        armImg2.sprite = arm[randomIndex];
        legImg2.sprite = leg[randomIndex];

        StartCoroutine(FetchAttackData());
        StartCoroutine(FetchCaseDataAndSetDescription());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.M) && Input.GetKeyDown(KeyCode.E))
        {
            ShowWinScreen();
        }
    }
    IEnumerator FetchAttackData()
    {
        // Fetch attack data from the API
        UnityWebRequest webRequest = UnityWebRequest.Get("https://civicsquestapi.azurewebsites.net/attack");
        yield return webRequest.SendWebRequest();

        // Check for errors in the API request
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch attack data: " + webRequest.error);
            yield break;
        }

        // Parse the JSON response as an array of objects
        string jsonResponse = webRequest.downloadHandler.text;

        // Wrap the array in a helper class
        AttackDataArrayWrapper wrapper = JsonUtility.FromJson<AttackDataArrayWrapper>("{\"items\":" + jsonResponse + "}");

        // Get the array from the wrapper
        AttackData[] attackDataArray = wrapper.items;

        // Log the number of attacks retrieved
        Debug.Log("Number of attacks retrieved: " + attackDataArray.Length);

        // Loop through the attack data and generate UI elements
        for (int i = 0; i < attackDataArray.Length; i++)
        {
            // Log attack information
            Debug.Log("Generating UI for attack: " + attackDataArray[i].title);

            // Instantiate the attackUI prefab
            GameObject _attackUI = Instantiate(attackUI, parentObject.transform);

            // Set RectTransform properties
            RectTransform rt = _attackUI.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(15f + (750 * i), -30f, 0f);

            // Set the TextMeshProUGUI text based on the attack title
            TextMeshProUGUI textMeshPro = _attackUI.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = attackDataArray[i].title;
            }

            // Find the AttackDescriptions script on the attackUI object
            AttackDescriptions attackDescriptions = _attackUI.GetComponent<AttackDescriptions>();

            // Set the public variables using values from JSON
            if (attackDescriptions != null)
            {
                string senario = attackDataArray[i].description;
                string descriptionPart = GetDescriptionPart(senario);

                // Function to extract the part after "Description:"
                string GetDescriptionPart(string senario)
                {
                    int descriptionIndex = senario.IndexOf("Description:");
                    if (descriptionIndex != -1)
                    {
                        string descriptionPart = senario.Substring(descriptionIndex + "Description:".Length).Trim();
                        return descriptionPart;
                    }
                    return string.Empty;
                }

                attackDescriptions.AttackName = attackDataArray[i].title;
                attackDescriptions.AttackDescription = attackDataArray[i].description;

                // Set the TextMeshProUGUI variables
                attackDescriptions.title = titleUI; // Assuming the title is the TextMeshProUGUI you want to set
                attackDescriptions.description = descriptionUI; // Assuming the description is also the TextMeshProUGUI you want to set
            }

            // Optionally, set a unique name for each attackUI
            _attackUI.name = "AttackUI_" + i.ToString();
        }
    }

    // ... (existing code)


    // Helper class to wrap the array
    [System.Serializable]
    public class AttackDataArrayWrapper
    {
        public AttackData[] items;
    }


    [System.Serializable]
    public class AttackData
    {
        public int id;
        public string title;
        public string description;
        public string descriptionExtended;
        // Add other fields as needed
    }

    IEnumerator FetchCaseDataAndSetDescription()
    {
        // Fetch case data from the API using the caseId
        UnityWebRequest webRequest = UnityWebRequest.Get($"https://civicsquestapi.azurewebsites.net/case?id={_caseId}");
        yield return webRequest.SendWebRequest();

        // Check for errors in the API request
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch case data: " + webRequest.error);
            yield break;
        }

        // Parse the JSON response as an array of objects
        string jsonResponse = webRequest.downloadHandler.text;

        // Wrap the array in a helper class (not needed for a single object)
        CaseData caseData = JsonUtility.FromJson<CaseData>(jsonResponse);

        // Extract the description part
        string descriptionPart = GetDescriptionPart(caseData.senario);

        // Function to extract the part between "Description" and "Players' Objective"
        string GetDescriptionPart(string senario)
        {
            const string descriptionTag = "Description:";
            const string objectiveTag = "Players' Objective:";

            int descriptionIndex = senario.IndexOf(descriptionTag, StringComparison.OrdinalIgnoreCase);
            int objectiveIndex = senario.IndexOf(objectiveTag, StringComparison.OrdinalIgnoreCase);

            if (descriptionIndex != -1 && objectiveIndex != -1)
            {
                string descriptionPart = senario.Substring(descriptionIndex + descriptionTag.Length, objectiveIndex - descriptionIndex - descriptionTag.Length).Trim();
                return descriptionPart;
            }
            return string.Empty;
        }

        // Set the TextMeshProUGUI text
        promptText.GetComponent<TextMeshProUGUI>().text = descriptionPart;
    }

    // ... (existing code)

    [System.Serializable]
    public class CaseData
    {
        public int id;
        public string name;
        public int year;
        public string title;
        public string pdfLink;
        public string senario;
        public string isLaw;

        // Add other fields as needed

        public override string ToString()
        {
            return $"\nCase: {name}\nTitle: {title}\nScenario: {senario}\nIs Law: {isLaw}";
        }
    }



    public void attackPanelClick()
    {
        attackPanel.SetActive(true);
        descriptionPanel.SetActive(false);
    }
    public void descriptionPanelClick()
    {
        attackPanel.SetActive(false);
        descriptionPanel.SetActive(true);
    }
    public void home()
    {
        attackPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }

    public void Attack()
    {
        //TODO: API Querying

        // Start the coroutine to introduce a wait
        StartCoroutine(AttackCoroutine());

    }

    IEnumerator AttackCoroutine()
    {

        home();
        attacksButton.SetActive(false);
        playerAnimationController.Attack();
        // Wait for 1 second
        //yield return new WaitForSeconds(1f);

        // Send HTTP request and wait for response
        yield return StartCoroutine(SendHttpRequest("https://civicsquestapi.azurewebsites.net/interaction", _caseId, _attackId));

        // Trigger the "Exit" animation on the prompt GameObject
        prompt.GetComponent<Animator>().SetTrigger("Exit");

        // Wait for another 1 second
        yield return new WaitForSeconds(1f);

        // Trigger the "gavel" animation on the judge Animator
        judgeAnimator.SetTrigger("gavel");
        result.SetActive(true);
        home();
        attacksButton.SetActive(false);

        int dmgDealt = UnityEngine.Random.Range(20, 70);
        playerHealth -= dmgDealt;

        if (playerHealth <= 0) ShowLoseScreen();
        if (enemyHealth <= 0) ShowWinScreen();

        playerHealthBar.value = playerHealth;
        enemyHealthBar.value = enemyHealth;

        promptText.GetComponent<TextMeshProUGUI>().text += "\n\n\nEnemy Dealt " + dmgDealt + " Damage";
    }

    IEnumerator SendHttpRequest(string baseUrl, int caseId, int attackId)
    {
        // Construct the URL with caseId and attackId parameters
        string url = $"{baseUrl}?caseId={caseId}&attackId={attackId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("HTTP request error: " + webRequest.error);
            }
            else
            {
                // Parse and format the JSON response
                string jsonResponse = webRequest.downloadHandler.text;

                // Use a custom class to parse the response
                InteractionData interactionData = JsonUtility.FromJson<InteractionData>(jsonResponse);

                enemyHealth -= interactionData.damage;
                // Log the parsed data (replace with UI Text update or other processing)
                Debug.Log("Parsed Interaction Data: " + interactionData.ToString());

                // Update the UI Text element (replace with your actual UI Text reference)
                if (resultText != null)
                {
                    resultText.text = interactionData.ToString();
                }
            }
        }
    }

    [System.Serializable]
    public class InteractionData
    {
        public int caseId;
        public int attackId;
        public int damage;
        public string response;

        public override string ToString()
        {
            return $"\nDamage: {damage}\nResponse: {response}";
        }
    }

    // Call this method when the player wins
    public void ShowWinScreen()
    {
        // Activate the end screen panel
        if (endScreen != null)
        {
            endScreen.SetActive(true);
            button.SetActive(true);
        }

        // Activate the win text
        if (winText != null)
        {
            winText.SetActive(true);
            loseText.SetActive(false);
            button.SetActive(true);
        }

        // Optionally, you can perform other actions related to winning here
    }

    // Call this method when the player loses
    public void ShowLoseScreen()
    {
        // Activate the end screen panel
        if (endScreen != null)
        {
            endScreen.SetActive(true);
            button.SetActive(true);
        }

        // Activate the lose text
        if (loseText != null)
        {
            loseText.SetActive(true);
            winText.SetActive(false);
            button.SetActive(true);
        }

        // Optionally, you can perform other actions related to losing here
    }

    public void SwitchScreen()
    {
        SceneManager.LoadScene(0);
        Destroy(FindObjectOfType<PlayerController>());
    }
}

