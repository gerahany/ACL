using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Non-static reference for Inspector assignment
    [SerializeField]
    private BasePlayer activePlayerReference;

    // Static property to access the active player globally
    public static BasePlayer ActivePlayer { get; private set; }

    private void Awake()
    {
        // Assign the static reference to the Inspector-assigned player
        ActivePlayer = activePlayerReference;

        if (ActivePlayer != null)
        {
            Debug.Log($"Active player set to: {ActivePlayer.playerName}");
        }
        else
        {
            Debug.LogError("No active player assigned in the Inspector!");
        }
    }
}
