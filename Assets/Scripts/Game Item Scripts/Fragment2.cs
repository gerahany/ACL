using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with the fragment
        if (other.CompareTag("Player")) // Ensure the tag is "Player"
        {
            Debug.Log($"gera");
            BasePlayer player = other.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.addrune();
                Destroy(gameObject); // Optional: Destroy the fragment after collection
            }
        }
    }
}
