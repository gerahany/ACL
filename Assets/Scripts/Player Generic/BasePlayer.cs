using UnityEngine;
using UnityEngine.UI; // For UI components like Slider and Text

public class BasePlayer : MonoBehaviour
{
    public string playerName;
    public int currentLevel = 1;
    public int currentXP = 0;
    public int maxXP = 100;
    public int abilityPoints = 0;

    public int maxHealth = 100; // Maximum health
    public int currentHealth; // Current health
    public int healingPotions = 0; // Potion count
    public int maxHealingPotions = 3; // Maximum healing potions the player can carry
    public GameObject potionIconPrefab; // Prefab for potion icons
    public Transform potionUIParent; // UI parent for potion icons

    // UI Elements
    public Slider healthBar; // Health bar UI Slider
    public Text healthText;  // Legacy Text for health display
    public Text levelText;   // Legacy Text for level display

    // To access the fill color of the health bar
    private Image healthBarFill;

    void Start()
    {
        // Initialize health
        currentHealth = 40;

        // Get the fill image component of the health bar slider
        healthBarFill = healthBar.fillRect.GetComponent<Image>();

        // Initialize health bar
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Set the maximum value of the slider
            healthBar.value = currentHealth;    // Set the initial value to full
        }

        // Update the UI
        UpdateHealthUI();
        UpdateLevelUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0

        if (currentHealth == 0)
        {
            Die();
        }

        UpdateHealthUI();
    }

    public void Heal()
    {
        if (healingPotions > 0 && currentHealth < maxHealth)
        {
            healingPotions--;
            currentHealth += maxHealth / 2; // Heal by 50% of max health
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            RemovePotionIcon();
            UpdateHealthUI();
        }
        else
        {
            Debug.Log("Cannot heal: Either health is full or no potions available.");
        }
    }



    private void Die()
    {
        Debug.Log($"{playerName} has died. Game Over!");
        // Game over logic, e.g., restart scene or show game over UI
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update slider value based on current health
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}"; // Update health text
        }

        // Change the health bar color based on health percentage
        if (healthBarFill != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;

            if (healthPercentage <= 0.5f)
            {
                healthBarFill.color = Color.red; // If health is under 50%, make it red
            }
            else
            {
                healthBarFill.color = Color.green; // If health is above 50%, make it green
            }
        }
    }

    public void GainXP(int xp)
    {
        currentXP += xp;

        while (currentXP >= maxXP && currentLevel < 4)
        {
            currentXP -= maxXP;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        abilityPoints++;
        maxHealth += 100; // Increase max health by 100
        currentHealth = maxHealth; // Refill health to the new max
        maxXP = currentLevel * 100; // Update max XP for next level

        // Update UI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Update slider max value to reflect new health cap
            healthBar.value = maxHealth;    // Reset slider to full
        }

        UpdateHealthUI();
        UpdateLevelUI();

        Debug.Log($"{playerName} leveled up to Level {currentLevel}!");
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"L:{currentLevel}    S:{currentXP}xp ";
        }
    }

    // Method to add a potion to the player inventory
    public void AddPotion()
    {
        if (healingPotions < maxHealingPotions)
        {
            healingPotions++;
            AddPotionIcon();
        }
        else
        {
            Debug.Log("Potion inventory is full.");
        }
    }
    private void AddPotionIcon()
    {
        GameObject newPotionIcon = Instantiate(potionIconPrefab, potionUIParent);
        Button button = newPotionIcon.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => Heal());
        }
    }

    private void RemovePotionIcon()
    {
        if (potionUIParent.childCount > 0)
        {
            Destroy(potionUIParent.GetChild(0).gameObject);
        }
    }


}
