using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public BasePlayer[] players; 

    public void SelectPlayer(int index)
    {
        if (index >= 0 && index < players.Length)
        {
            //GameManager.SetActivePlayer(players[index]);
            Debug.Log($"Player {players[index].playerName} selected.");
        }
        else
        {
            Debug.LogError("Invalid player index.");
        }
    }
}
