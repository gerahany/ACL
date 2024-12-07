using System.Collections.Generic;
using UnityEngine;

public class demonhealthbar : MonoBehaviour
{
    public Transform healthBarParent; // Assign dynamically during spawning
    public GameObject dashPrefab; // Assign in the Inspector
    private List<GameObject> dashes = new List<GameObject>();

    private int maxHealth = 40;
    private int currentHealth;

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

    // Reduce health
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Heal health
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }
}
