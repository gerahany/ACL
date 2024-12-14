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

        // foreach (Transform child in transform[0])
        // {
        //     Debug.Log("child: " + child);
        //     dashes.Add(child.gameObject); // Add each child (dash) to the list
        // }

        Transform healthBarParent = transform; // Or reference to the correct parent

        // for (int i = 0; i < healthBarParent.childCount; i++)
        // {
        //     childTransform = healthBarParent.GetChild(i););
        //     // Use childTransform to access the child's components or properties
        // }

        // for (int i = 0; i < childTransform.childCount; i++)
        // {
        //     dashes.Add(childTransform.GetChild(i).gameObject);
        //     Debug.Log("child: " + childTransform.GetChild(i).gameObject);
        // }


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

    public void TakeDamage(int damage)
    {
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