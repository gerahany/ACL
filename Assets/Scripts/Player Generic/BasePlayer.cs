using UnityEngine;
using UnityEngine.UI; // For UI components like Slider and Text
using TMPro;
using System.Collections;
public class BasePlayer : MonoBehaviour
{
    private Image xpBarFill; // To access the XP bar's fill color
    public Text xpText; // XP text for displaying XP progress
    public Slider xpBar; // XP bar UI Slider
    public string playerName;
    public int currentLevel = 1;
    public int currentXP = 0;
    public static bool coolZero=false;
    public Button basicButton;     // Button for Basic ability
    public Button defensiveButton; // Button for Defensive ability
    public Button wildButton;      // Button for Wild ability
    public Button ultimateButton;  // Button for Ultimate ability
    private int runeCount = 0; // To track the number of collected rune fragments

    // Tracks the state of abilities
    private bool basicUnlocked = true;
    private bool defensiveUnlocked = false;
    private bool wildUnlocked = false;
    private bool ultimateUnlocked = false;

    public bool IsBasicUnlocked => basicUnlocked;
    public bool IsDefensiveUnlocked => defensiveUnlocked;
    public bool IsWildUnlocked => wildUnlocked;
    public bool IsUltimateUnlocked => ultimateUnlocked;

    // Button text components for color changes
    public TMP_Text basicButtonText;
    public TMP_Text defensiveButtonText;
    public TMP_Text wildButtonText;
    public TMP_Text ultimateButtonText;

    public int maxXP = 100;
    public int abilityPoints = 0;
    private Button[] abilityButtons;
    public Animator animator; // Reference to the Animator component
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

    private Image healthBarFill;
    private bool isInvincible = false; // Tracks whether the player is invincible
    private bool isSlowMotion = false;
    private bool ToggleCooldown = false;
    public SoundEffectHandler soundEffectHandler;
    public GameObject gameOverPanel;
    public GameObject panels;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

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
        UpdateXPUI();

        if(currentHealth==0){
           animator.SetTrigger("Die");
           float fallDistance = 1.0f; // You can adjust this value to control how far down the player falls
            Vector3 newPosition = transform.position;
            newPosition.y -= fallDistance; // Move the player down
            transform.position = newPosition;
        }
        if (xpBar != null)
        {
            xpBarFill = xpBar.fillRect.GetComponent<Image>(); // Get the fill image component
            xpBar.maxValue = maxXP; // Set the maximum value of the slider
            xpBar.value = currentXP; // Initialize the slider's value
        }
            // Optional: Update XP Text
        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {maxXP} XP";
        }
        SetupButton(basicButton, basicButtonText, basicUnlocked);
        SetupButton(defensiveButton, defensiveButtonText, defensiveUnlocked);
        SetupButton(wildButton, wildButtonText, wildUnlocked);
        SetupButton(ultimateButton, ultimateButtonText, ultimateUnlocked);
    }

    //Test XPBar method
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Toggle invincibility
        {
            isInvincible = !isInvincible; // Toggle the invincibility state
            Debug.Log($"Invincibility toggled. Now invincible: {isInvincible}");
        }
        if (Input.GetKeyDown(KeyCode.C)) // Toggle invincibility
        {
            coolZero = !coolZero; // Toggle the invincibility state
            Debug.Log($"Cooldown toggled");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GainXP(100); 
             UpdateXPUI();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(20);
            UpdateHealthUI();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHealth += 20;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed the maximum
            UpdateHealthUI();
        }
        if (Input.GetKeyDown(KeyCode.M)) // Toggle slow motion
        {
            isSlowMotion = !isSlowMotion;
            Time.timeScale = isSlowMotion ? 0.5f : 1f; // 0.5x speed for slow motion, 1x for normal speed
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            defensiveUnlocked = true;
            defensiveButtonText.color = Color.black;
            wildUnlocked = true;
            wildButtonText.color = Color.black;
            ultimateUnlocked = true;
            ultimateButtonText.color = Color.black;
            UpdateAbilityButtons();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            abilityPoints++;
            UpdateLevelUI();
            UpdateAbilityButtons();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Heal(); // Use a potion when F is pressed
        }
        if (currentHealth == 0)
        {
            animator.SetTrigger("Die");
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
            if(currentXP>=400){
                currentXP=400;
            }

            UpdateXPUI();
        }
    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible! Damage ignored.");
            return;
        }
        Barbarian barbarian = GetComponent<Barbarian>();  // Get the Barbarian component (assuming BasePlayer and Barbarian are attached to the same GameObject)

        if (barbarian != null && barbarian.shieldActive)
        {
            // If the shield is active, do not apply damage
            Debug.Log("Damage blocked by shield!");
            return;
        }
       
        // Apply damage if no shield is active
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        
        if (currentHealth == 0)
        {
            soundEffectHandler.PlayPlayerDeathSound();
            animator.SetTrigger("Die");
            float fallDistance = 5.0f; // You can adjust this value to control how far down the player falls
            Vector3 newPosition = transform.position;
            newPosition.y -= fallDistance; // Move the player down
            transform.position = newPosition;
            StartCoroutine(ShowGameOverPanelAfterDelay(3f));
        }
        else
        {
            soundEffectHandler.PlayPlayerDamageSound();
            animator.SetTrigger("Hit");
        }

        UpdateHealthUI();
    }



    public void Heal()
    {
        if (healingPotions > 0 && currentHealth < maxHealth)
        {
            soundEffectHandler.PlayHealingSound();
            healingPotions--;
            currentHealth += maxHealth / 2; // Heal by 50% of max health
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            animator.SetTrigger("Drink");
            RemovePotionIcon();
            UpdateHealthUI();
        }
        else
        {
            Debug.Log("Cannot heal: Either health is full or no potions available.");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {   
            healthBar.value = currentHealth; // Update slider value based on current health
        }
        if(currentHealth==0){
            animator.SetTrigger("Die");
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
        }}
    
    private void LevelUp()
    {
        currentLevel++;
        abilityPoints++;
        //abilityPoints++;
        maxHealth += 100; // Increase max health by 100
        currentHealth = maxHealth; // Refill health to the new max
        maxXP = currentLevel * 100; // Update max XP for the next level

        // Update UI elements
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }

        if (xpBar != null)
        {
            xpBar.maxValue = maxXP;
            xpBar.value = currentXP; // Apply leftover XP
        }

        UpdateHealthUI();
        UpdateXPUI();
        UpdateLevelUI();
        UpdateAbilityButtons();

        Debug.Log($"{playerName} leveled up to Level {currentLevel}!");
    }
    private void UpdateXPUI()
    {
        if (xpBar != null)
        {
            xpBar.value = currentXP; // Update the slider's progress
        }

        if (xpBarFill != null)
        {
            float xpPercentage = (float)currentXP / maxXP;

            if (xpPercentage == 0f)
            {
                xpBarFill.color = Color.clear; // Less than 25%, transparent
            }

            // // Change color based on XP percentage (example: green to yellow to red)
            // else if (xpPercentage < 0.5f)
            // {
            //     xpBarFill.color = Color.red; // Less than 50%, red
            // }
            // else if (xpPercentage < 0.75f)
            // {
            //     xpBarFill.color = Color.yellow; // Between 50% and 75%, yellow
            // }
            // else
            // {
            //     xpBarFill.color = Color.green; // Above 75%, green
            // }
            xpBarFill.color = Color.blue;
        }

        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {maxXP} XP"; // Update the text display
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level:{currentLevel}   Ability Pts:{abilityPoints} Potions:{healingPotions} Rune Fragments:{runeCount}";
        }
    }

    // Method to add a potion to the player inventory
    public void AddPotion()
    {
        if (healingPotions < maxHealingPotions)
        {
            soundEffectHandler.PlayItemPickupSound();
            healingPotions++;
            AddPotionIcon();
            UpdateLevelUI();
        }
        else
        {
            Debug.Log("Potion inventory is full.");
        }
    }
    private void AddPotionIcon()
    {
        Instantiate(potionIconPrefab, potionUIParent);
    }

    private void RemovePotionIcon()
    {
        if (potionUIParent.childCount > 0)
        {
            animator.SetTrigger("Drink");
            Destroy(potionUIParent.GetChild(0).gameObject);
        }
    }
    private void Die()
    {
        Debug.Log($"{playerName} has died. Game Over!");

        if (animator != null)
        {
            animator.SetTrigger("Die"); // Assumes you have a "Die" trigger in the Animator
        }
        else
        {
            Debug.LogError("Animator not assigned for the player!");
        }

        // Additional game-over logic (e.g., disable controls, restart level, show UI, etc.)
    }
private void SetupButton(Button button, TMP_Text buttonText, bool isUnlocked)
{
    if (button != null && buttonText != null)
    {
        button.interactable = isUnlocked;

        // Set text color based on unlocked state
        buttonText.color = isUnlocked ? Color.black : Color.grey;

        // Add click listener
        button.onClick.AddListener(() => OnAbilityButtonClicked(button));
    }
}

    private void OnAbilityButtonClicked(Button clickedButton)
    {
    if (clickedButton == defensiveButton && !defensiveUnlocked && abilityPoints > 0)
    {
        defensiveUnlocked = true;
        abilityPoints--;
        defensiveButtonText.color = Color.black;
    }
    else if (clickedButton == wildButton && !wildUnlocked && abilityPoints > 0)
    {
        wildUnlocked = true;
        abilityPoints--;
        wildButtonText.color = Color.black;
    }
    else if (clickedButton == ultimateButton && !ultimateUnlocked && abilityPoints > 0)
    {
        ultimateUnlocked = true;
        abilityPoints--;
        ultimateButtonText.color = Color.black;
    }

    // Update button states and ability points UI
    UpdateAbilityButtons();
    UpdateLevelUI(); // Update ability points display
    }

    private void UpdateAbilityButtons()
    {
        // Update text color for locked but highlightable buttons
        if (!defensiveUnlocked)
        {
            defensiveButtonText.color = abilityPoints > 0 ? Color.white : Color.grey;
            defensiveButton.interactable = abilityPoints > 0 ? true : false;
        }
        if (!wildUnlocked)
        {
            wildButtonText.color = abilityPoints > 0 ? Color.white : Color.grey;
            wildButton.interactable = abilityPoints > 0 ? true : false;
        }

        if (!ultimateUnlocked)
        {
            ultimateButtonText.color = abilityPoints > 0 ? Color.white : Color.grey;
            ultimateButton.interactable = abilityPoints > 0 ? true : false;
        }
    }
    public void addrune()
    {
        soundEffectHandler.PlayItemPickupSound();
        runeCount++;
        UpdateLevelUI();
        Debug.Log($" fragment {runeCount}");
    }
    public int getrune()
    {
        int r = runeCount;
       
        return r;
    }
    public void zerorune()
    {
        runeCount = 0;
        Debug.Log($" Zerofragment {runeCount}");
    }
    public  bool isCoolZero()
    {
        return coolZero;
    }

    public  void SetCool(bool isActive)
    {
        coolZero = isActive;
    }

    public bool IsBasicAbilityUnlocked() => basicUnlocked;
    public bool IsDefensiveAbilityUnlocked() => defensiveUnlocked;
    public bool IsWildAbilityUnlocked() => wildUnlocked;
    public bool IsUltimateAbilityUnlocked() => ultimateUnlocked;
    private IEnumerator ShowGameOverPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Show the Game Over panel after the delay
        if (gameOverPanel != null)
        {
            Time.timeScale = 0;
            panels.SetActive(true);
            gameOverPanel.SetActive(true);
        }
    }

    }