using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health
    public int currentHealth; // Current health

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Heal without exceeding max health
        Debug.Log($"Healed for {amount}. Current Health: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0); // Reduce health without dropping below 0
        Debug.Log($"Took {amount} damage. Current Health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Wanderer has died!");
        // Add respawn or game-over logic here
    }
}