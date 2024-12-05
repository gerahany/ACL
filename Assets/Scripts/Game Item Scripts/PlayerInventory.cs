using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int runeCount = 0; // Tracks the number of collected Rune Fragments

    public void CollectRune()
    {
        runeCount++;
        Debug.Log($"{gameObject.name} now has {runeCount} Rune Fragments!");
    }
}
