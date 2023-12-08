using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<Spell> inventory = new List<Spell>();

    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject descriptionPanel;
    /*[SerializeField] private GameObject[] attackList;
    [SerializeField] private TextMeshProUGUI[] listTitle;*/
    [SerializeField] private GameObject attackUI;
    [SerializeField] private GameObject parentObject;

    [SerializeField] private TextMeshProUGUI titleUI;
    [SerializeField] private TextMeshProUGUI descriptionUI;
    [SerializeField] private Scrollbar scrollBar;

    private string jsonResponse;
    //private CaseData[] caseDataArray;

    public int caseNum;
    public GameObject hoverTitleText;
    public GameObject hoverPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SceneManager.LoadScene(1);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Map")
        {
            CloseMenu();
            StartCoroutine(FetchAttackData());
            hoverPanel.SetActive(false);
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
            rt.localPosition = new Vector3(15f, -15f + (-70 * i), 0f);

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


    public void attackPanelClick()
    {
        attackPanel.SetActive(true);
        descriptionPanel.SetActive(false);
    }
    public void descriptionPanelClick()
    {
        attackPanel.SetActive(false);
        descriptionPanel.SetActive(true);
        scrollBar.value = 1;
    }

    public void CloseMenu()
    {
        attackPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }
}
