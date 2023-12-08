using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class PickupController : MonoBehaviour
{
    public int caseID;

    private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject;
        player.GetComponent<InventoryManager>().hoverPanel.SetActive(true);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.caseNum = caseID;
                SceneManager.LoadScene(1);
            }
            StartCoroutine(FetchCaseDataAndSetDescription());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player.GetComponent<InventoryManager>().hoverPanel.SetActive(false);
    }


    IEnumerator FetchCaseDataAndSetDescription()
    {
        Debug.Log("Running");
        // Fetch case data from the API using the caseId
        UnityWebRequest webRequest = UnityWebRequest.Get($"https://civicsquestapi.azurewebsites.net/case?id={caseID}");
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

        // Set the TextMeshProUGUI text
        player.GetComponent<InventoryManager>().hoverTitleText.GetComponent<TextMeshProUGUI>().text = caseData.title;
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
}
