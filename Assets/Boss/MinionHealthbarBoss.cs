using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHealthbarBoss : MonoBehaviour
{
    public Transform healthBarParent; // Assign dynamically during spawning
    public GameObject dashPrefab; // Assign in the Inspector
    private List<GameObject> dashes = new List<GameObject>();
    public Animator animator;
    private int maxHealth = 20;
    private bool isDead = false;
    private int currentHealth;
    public BossBehavior boss;
    void Start()
    {
        InitializeHealthBar();

    }

    // Initializes the health bar
    void InitializeHealthBar()
    {
        if (healthBarParent == null)
        {
            Debug.LogError($"Health bar parent is not assigned for {gameObject.name}.");
            return;
        }

        currentHealth = maxHealth;

        // Clear any existing dashes
        foreach (Transform child in healthBarParent)
        {
            Destroy(child.gameObject);
        }
        dashes.Clear();

        // Create dashes
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject dash = Instantiate(dashPrefab, healthBarParent);
            dash.SetActive(true);
            dashes.Add(dash);
        }
    }

    // Updates the health bar
    public void UpdateHealthBar()
    {
        for (int i = 0; i < dashes.Count; i++)
        {

            dashes[i].SetActive(i < currentHealth);
        }
    }

    // Reduce heal
    public void TakeDamage(int damage)
    {
        Debug.Log("hidamage");
        if (currentHealth > 0 && !isDead) // Avoid taking damage if already dead
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();

            if (animator != null && currentHealth > 0)
            {
                animator.SetTrigger("DamageMinion"); // Trigger the damage animation
            }
        }

        // Check if health reaches 0 and trigger DieMinion animation
        if (currentHealth <= 0 && !isDead)
        {
            BasePlayer activePlayer = GameManager.ActivePlayer;

            if (activePlayer != null)
            {
                activePlayer.GainXP(10); // Award 10 XP to the active player
            }
            else
            {
                Debug.LogError("No active player found!");
            }

            isDead = true; // Mark demon as dead
            RandMinionBoss randMinion = FindObjectOfType<RandMinionBoss>();

            if (animator != null && currentHealth <= 0 && randMinion != null)
            {
                randMinion.RemoveAggressiveMinion(gameObject);
                animator.SetTrigger("DieMinion"); // Trigger the death animation
                boss.OnMinionDefeated();

            }

            // Optionally, wait for the death animation to finish before destroying
            Destroy(gameObject, 3f); // Adjust time based on animation length
        }
    }


    // Heal health
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }
}
