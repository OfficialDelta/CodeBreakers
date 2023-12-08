using TMPro;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Spell")]
public class Spell : ScriptableObject
{
    public string _name;
    public int damage;
    public string description;
}