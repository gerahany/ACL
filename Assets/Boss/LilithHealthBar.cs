using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithHealthBar : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health represented by the number of dashes
    private int currentHealth; // Current health
    public Animator animator; // Animator for the health bar
    private List<GameObject> dashes = new List<GameObject>();
    private bool isDead = false;

    void Start()
    {
        // Initialize health and get all dash images
        currentHealth = maxHealth;

        foreach (Transform child in transform)
        {
            dashes.Add(child.gameObject); // Add each child (dash) to the list
        }
    }


    public void TakeDamage(int damage)
    {
        if (currentHealth > 0 && !isDead) // Avoid taking damage if already dead
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();

            if (animator != null)
            {
                animator.SetTrigger("Damage"); // Trigger the damage animation
            }
        }
    }



    private void UpdateHealthBar()
    {
        for (int i = 0; i < dashes.Count; i++)
        {
            if (i < currentHealth)
                dashes[i].SetActive(true); // Show the dash if within health range
            else
                dashes[i].SetActive(false); // Hide the dash if outside health range
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }
}