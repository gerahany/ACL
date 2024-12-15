using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealthbar : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health represented by the number of dashes
    private int currentHealth; // Current health

    private List<GameObject> dashes = new List<GameObject>();
    private List<GameObject> healthBars = new List<GameObject>();
    // private Transform childTransform;


    void Start()
    {
        // Initialize health and get all dash images
        currentHealth = maxHealth;
        Transform healthBarParent = transform; // Or reference to the correct parent


        Transform childTransform = healthBarParent.GetChild(0);
        Debug.Log("Child: " + childTransform.name);

        for (int i = 0; i < 50; i++)
    {
        // childTransform = childTransform.GetChild(i);
        dashes.Add(childTransform.GetChild(i).gameObject);
        Debug.Log("Child Dash: " + childTransform.GetChild(i).gameObject.name);
    }
        
    }

    // void Update()
    // {
    //     UpdateHealthBar();
    // }


    // public void TakeDamage(int damage)
    // {
    //     currentHealth -= damage;
    //     Debug.Log("Shield took damage: " + damage);
    //     Debug.Log("Shield health: " + currentHealth);
    //     currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    //     UpdateHealthBar();
    // }
    private int h;

    public void TakeDamage(int damage)
    {
        h = currentHealth;
        currentHealth -= damage;
        Debug.Log("Shield took damage: " + damage);
        Debug.Log("Shield health: " + currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            // Notify the boss that the shield is depleted
            BossBehavior boss = GetComponentInParent<BossBehavior>();
            if (boss != null)
            {
                boss.DeactivateReflectiveAura();
            }
            if (damage - h > 0)
                boss.TakeDamage( damage-h);
        }
    }


    private void UpdateHealthBar()
    // healthBars = dashes[0];
    {
        for (int i = 0; i < dashes.Count; i++)
        {
            Debug.Log(dashes.Count + "thats the count of dashes");
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