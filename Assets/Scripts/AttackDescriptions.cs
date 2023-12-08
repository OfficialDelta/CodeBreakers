using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackDescriptions : MonoBehaviour
{
    public string AttackName;
    public string AttackDescription;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public int ID;

    public void descriptionPanelClick()
    {
        description.text = AttackDescription;
        title.text = AttackName;
        if (SceneManager.GetActiveScene().name == "Map")
            FindObjectOfType<PlayerController>().gameObject.GetComponent<InventoryManager>().descriptionPanelClick();
        else
        {
            FindObjectOfType<BattleManager>().gameObject.GetComponent<BattleManager>().descriptionPanelClick();

            FindObjectOfType<BattleManager>().gameObject.GetComponent<BattleManager>()._attackId = ID;
        }
    }
}
