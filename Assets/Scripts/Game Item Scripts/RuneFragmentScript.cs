using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneFragmentScript : MonoBehaviour
{
    public GameObject uiIndicator; // UI indicator above the Rune Fragment

    void Start()
    {
        // Ensure the UI indicator is disabled initially
        if (uiIndicator != null)
        {
            uiIndicator.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UI Indicator is not assigned for Rune Fragment!");
        }

        // Initially hide the Rune Fragment (disable visuals and interactions)
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a valid player
        if (other.CompareTag("Barbarian") || other.CompareTag("Rogue") || other.CompareTag("Sorcerer"))
        {
            // Attempt to get the PlayerInventory component from the player
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                // Add Rune Fragment to the player's inventory
                playerInventory.CollectRune();
                Debug.Log($"{other.tag} collected a Rune Fragment!");
                Destroy(gameObject); // Remove the Rune Fragment after collection
            }
            else
            {
                Debug.LogError($"{other.tag} does not have a PlayerInventory component!");
            }
        }
    }

    public void ActivateFragment()
    {
        // Make the Rune Fragment visible and enable interactions
        Debug.Log("Activating Rune Fragment...");
        GetComponent<Renderer>().enabled = true; // Show visuals
        GetComponent<Collider>().enabled = true; // Enable collisions

        if (uiIndicator != null)
        {
            uiIndicator.SetActive(true); // Show the UI indicator
        }
    }
}
