using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{
    public Animator animator;
    public GameObject currentReflectiveAuraEffect;
    public GameObject currentAuraEffect;

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // Health bar prefab
    public GameObject ShieldhealthPrefab;
    private GameObject healthBar; // Instance of the health bar
    private Transform[] healthSegments; // Individual health bar segments
    private GameObject shieldHealthBar; // Instance of the shield health bar
    private Transform[] shieldSegments;
    public SoundEffectHandler soundEffectHandler;

    [Header("Health Settings")]
    public float phase1Health = 50f; // Phase 1 health
    public float phase2Health = 100f; // Phase 2 health
    private float currentHealth;
    private bool isPhase2 = false;
    private bool isFirst = true;
    private bool isDead = false;

    public Text healthText; // Legacy Text for health display

    [Header("Minion Summoning")]
    public GameObject minionPrefab; // Prefab for minions
    public Transform[] spawnPoints; // Spawn points for minions
    public int maxMinions = 3; // Maximum number of minions
    private int activeMinions = 0; // Track active minions
    public GameObject SummonEffect; // Particle effect for summoning minions

    [Header("Divebomb Attack")]
    public int divebombDamage = 20; // Damage dealt by Divebomb
    public float divebombRadius = 10f; // Radius for Divebomb
    public GameObject divebombEffect; // Effect for Divebomb impact

    [Header("Attack Cooldown")]
    public float attackCooldown = 10f; // Cooldown time between actions
    private bool canAttack = true; // Control for cooldown

    [Header("Phase 2 Abilities")]
    public int reflectiveAuraDamage = 15; // Damage dealt by Reflective Aura
    public int bloodSpikesDamage = 30; // Damage dealt by Blood Spikes
    public float bloodSpikesRange = 10f; // Range of Blood Spikes attack
    public GameObject bloodSpikesEffect; // Effect for Blood Spikes attack
    public GameObject reflectiveAuraEffect; // Visual representation of Reflective Aura
    public GameObject AuraEffect;

    [Header("Defensive Shield Settings")]
    public float maxShieldHealth = 50f; // Shield max health
    private float shieldHealth;
    private bool shieldActive = false;

    [Header("Effect Positions")]
    public Transform[] reflectiveAuraPoints; // Points for Reflective Aura
    public Transform[] bloodSpikesPoints; // Points for Blood Spikes
    public GameObject auraParticleEffect; // Visual representation of Reflective Aura

    [Header("References")]
    public Transform character1;
    public Transform character2;
    public Transform character3;
    private Transform wanderer;
    public Transform shieldHealthPosition;
    public Animator minionAnimator;
    public RandMinionBoss randMinionBoss; // Assign RandMinionBoss in the Inspector
    public int minionsToSpawn = 3; // Number of minions to spawn each time

    private bool auraactive=false;

    void Start()
    {
        IdentifyActiveCharacter();
        currentHealth = phase1Health;
        shieldHealth = maxShieldHealth;

        CreateShieldHealthBar();
        UpdateShieldHealthBar();
        CreateHealthBar();

    }

        private void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBar = Instantiate(
                healthBarPrefab,
                transform.position + new Vector3(0, 2.8f, 0),
                Quaternion.Euler(0, 90, 0)
            );

            healthBar.transform.SetParent(transform);
            InitializeHealthSegments();
        }
    }

    private void InitializeHealthSegments()
    {
        if (healthBar != null)
        {
            healthSegments = new Transform[healthBar.transform.childCount];
            for (int i = 0; i < healthBar.transform.childCount; i++)
            {
                healthSegments[i] = healthBar.transform.GetChild(i);
                healthSegments[i].gameObject.SetActive(true);
            }
        }
    }

     private void UpdateHealthBar()
        {
            if (healthBar == null || healthSegments == null || healthSegments.Length == 0)
            {
                Debug.LogWarning("Health bar or segments are not properly initialized.");
                return;
            }


            // Calculate active segments based on current health
            int activeSegments = Mathf.CeilToInt(
                Mathf.Clamp01(currentHealth / (isPhase2 ? phase2Health : phase1Health)) * healthSegments.Length
            );

            // Enable/disable segments dynamically
            for (int i = 0; i < healthSegments.Length; i++)
            {
                bool shouldBeActive = i < activeSegments;

                // Avoid redundant updates
                if (healthSegments[i].gameObject.activeSelf != shouldBeActive)
                {
                    healthSegments[i].gameObject.SetActive(shouldBeActive);
                }
            }

        }


    private void CreateShieldHealthBar()
    {
        if (currentReflectiveAuraEffect != null && shieldHealthPosition != null)
        {
            // Instantiate the shield health bar above the shield
            shieldHealthBar = Instantiate(
                currentReflectiveAuraEffect,
                shieldHealthPosition.position + new Vector3(0, 1f, 0), // Adjust Y-offset for position above the shield
                Quaternion.identity
            );

            // Set the parent of the shield health bar to follow the boss
            shieldHealthBar.transform.SetParent(transform);

            // Correct the scale of the shield health bar
            shieldHealthBar.transform.localScale = new Vector3(
                Mathf.Abs(shieldHealthBar.transform.localScale.x),
                Mathf.Abs(shieldHealthBar.transform.localScale.y),
                Mathf.Abs(shieldHealthBar.transform.localScale.z)
            );

            Debug.Log("Shield Health Bar created above the shield.");

            // Initialize health segments or assign max health
            InitializeShieldSegments();
        }
        else
        {
            Debug.LogWarning("Shield Health Prefab or Position is not assigned.");
        }
    }


    private void InitializeShieldSegments()
    {
        if (shieldHealthBar != null)
        {
            ShieldHealthbar shieldHealthScript = shieldHealthBar.GetComponent<ShieldHealthbar>();
            if (shieldHealthScript != null)
            {
                shieldHealthScript.maxHealth = (int)maxShieldHealth;
            }
        }
    }

    private void UpdateShieldHealthBar()
    {
        if (shieldHealthBar != null)
        {
            ShieldHealthbar shieldHealthScript = shieldHealthBar.GetComponent<ShieldHealthbar>();
            if (shieldHealthScript != null)
            {
                shieldHealthScript.TakeDamage(0); // Update the health bar visuals
            }
            
            else
            {
                Debug.Log("scriptttt");
            }
        }
    }


    private void IdentifyActiveCharacter()
    {
        if (character1 != null && character1.gameObject.activeSelf)
        {
            wanderer = character1; // Assign the Transform directly
        }
        else if (character2 != null && character2.gameObject.activeSelf)
        {
            wanderer = character2; // Assign the Transform directly
        }
        else if (character3 != null && character3.gameObject.activeSelf)
        {
            wanderer = character3; // Assign the Transform directly
        }
        else
        {
            wanderer = null; // No character is active
        }
    }

    private void Update()
    {
        //UpdateHealthBar();
        // UpdateShieldHealthBar();
        if (isDead) return;

        FaceWanderer();

        // Phase 1 behavior
        // if (!isPhase2 && canAttack)
         //{
          //   PerformPhase1Attack();
         //}

        // Phase 2 behavior
        if (isPhase2 && canAttack)
        {
            PerformPhase2Attack();
        }
        else{
            PerformPhase1Attack();
        }

        if (currentReflectiveAuraEffect != null)
        {
            UpdateShieldHealthBar();
        }

        // Transition to Phase 2 if health reaches 0 in Phase 1
        if (currentHealth <= 0 && !isPhase2)
        {
            StartCoroutine(Resurrect());
        }
    }

    private void PerformPhase1Attack()
    {
        if (!canAttack) return;

        if (activeMinions == 0)
        {
            Debug.Log("Summoning minions woweee!");
             SummonMinions();
        }
        else
        {
            Divebomb();
        }

        StartCoroutine(AttackCooldown());
    }
    private void SummonMinions()
    {
        soundEffectHandler.PlayBossSummonSound();
        activeMinions = 3;

        if (randMinionBoss != null && SummonEffect != null)
        {
            activeMinions = 3;
            animator.SetTrigger("Summon");

            randMinionBoss.minionCount = minionsToSpawn; // Set the number of minions to spawn

            randMinionBoss.SpawnMinions(); // Call the SpawnMinions method
           
            

        }
    }

    private void Divebomb()
    {
        Debug.Log("Performing Divebomb!");
        animator.SetTrigger("Divebomb");
       
        soundEffectHandler.PlayBossStompSound();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        Debug.Log("Performing Divebomb!1");
        foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                Debug.Log("Performing Divebomb!2");
                BasePlayer player = collider.GetComponent<BasePlayer>();
                    if (player != null)
                    {
                        player.TakeDamage(20);
                    }
                }
            }
        

        // Optional visual effect
        if (divebombEffect != null)
        {
            GameObject effectInstance = Instantiate(divebombEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 5f);
        }
    }

    private void PerformPhase2Attack()
    {
        if (!isPhase2)
        {
            Debug.LogWarning("PerformPhase2Attack called in phase 1. Ignoring.");
            return;
        }

        if (!canAttack) return;
        if (!auraactive)
        {
            ActivateAura();
        }

        if (!shieldActive)
        {
            ActivateReflectiveAura();
        }
        else
        {
            UseBloodSpikes();
        }

        StartCoroutine(AttackCooldown());
    }


    private void UseBloodSpikes()
    {
        Debug.Log("Using Blood Spikes!");
        animator.SetTrigger("SwingHands");
        soundEffectHandler.PlayBossSwingSound();

        foreach (Transform point in bloodSpikesPoints)
        {
            if (bloodSpikesEffect != null)
            {
                GameObject effectInstance = Instantiate(bloodSpikesEffect, point.position, point.rotation);
                Destroy(effectInstance, 5f);
            }
            
            
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        Debug.Log("Performing Divebomb!1");
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Performing Blood");
                BasePlayer player = collider.GetComponent<BasePlayer>();
                if (player != null)
                {
                    player.TakeDamage(30);
                }
            }
        }   
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown); // Cooldown before next action
        canAttack = true;
    }

    private bool IsPositionValid(Vector3 position, List<Vector3> usedPositions)
    {
            foreach (Vector3 usedPosition in usedPositions)
            {
                if (Vector3.Distance(position, usedPosition) < 5f)
                {
                    return false;
                }
            }
            return true;
        }

    public void OnMinionDefeated()
    {
        activeMinions--;
        Debug.Log($"A minion was defeated! Remaining: {activeMinions}");
    }

    private bool hitaura =false;
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (activeMinions == 0 && !auraactive)
        {

            if (isPhase2 && shieldActive)
            {
                // Reduce shield health
                shieldHealth -= damage;
                Debug.Log($"Shield took {damage} damage. Remaining shield health: {shieldHealth}");
                UpdateShieldHealthBar();

                // Check if shield health is depleted
                if (shieldHealth <= 0)
                {
                    Debug.Log("Shield depleted!");
                    DeactivateReflectiveAura();
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetTrigger("Damaged");
                    soundEffectHandler.PlayBossDamageSound();
                }
                else
                {
                    Debug.LogWarning("Animator is not assigned!");
                }
                ApplyHealthDamage(damage);
                UpdateHealthBar();
            }
        }
      
    }
    public void TakeAuraDamage(int damage)
    {
        if (isPhase2 && auraactive)
        {
            DeactivateAura();
            GameObject player = GameObject.FindWithTag("Player");
            BasePlayer p = player.GetComponent<BasePlayer>();
            p.TakeDamage(damage);
        }
    }


    private IEnumerator Resurrect()
    {
        animator.SetTrigger("Resurrect");
        yield return new WaitForSeconds(5f);
        isPhase2 = true;
        canAttack = true;
        currentHealth = phase2Health;
        UpdateHealthBar();
    }

    public void DeactivateReflectiveAura()
    {
        shieldActive = false;

        if (currentReflectiveAuraEffect != null)
        {
            Destroy(currentReflectiveAuraEffect);
            currentReflectiveAuraEffect = null;
        }

        if (shieldHealthBar != null)
        {
            Destroy(shieldHealthBar);
            shieldHealthBar = null;
        }

        Debug.Log("Shield deactivated.");
        StartCoroutine(RegenerateShield());
    }
    private IEnumerator RegenerateShield()
    {
        yield return new WaitForSeconds(30f);
        ActivateReflectiveAura();
    }
    private void ActivateReflectiveAura()
    {
        Debug.Log("Activating Reflective Aura...");

        if (!isPhase2)
        {
            Debug.LogWarning("Cannot activate Reflective Aura in phase 1!");
            return;
        }

        if (shieldActive)
        {
            Debug.Log("Shield is already active. Exiting.");
            return;
        }

        float reflectiveAuraRange = 7f;
        float safeDistance = 7f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, reflectiveAuraRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log($"Player {collider.name} detected inside the aura range. Moving them outside.");
                Vector3 directionToPlayer = (collider.transform.position - transform.position).normalized;
                Vector3 newPosition = transform.position + directionToPlayer * safeDistance;
                collider.transform.position = newPosition;
                Debug.Log($"Player {collider.name} moved to {newPosition}.");
            }
        }

        shieldActive = true;
        shieldHealth = maxShieldHealth;
        UpdateShieldHealthBar();

        if (reflectiveAuraEffect != null)
        {
            if (currentReflectiveAuraEffect != null)
            {
                Debug.Log("Destroying existing Reflective Aura effect...");
                Destroy(currentReflectiveAuraEffect);
            }

            currentReflectiveAuraEffect = Instantiate(
                reflectiveAuraEffect,
                transform.position,
                Quaternion.identity
            );

            currentReflectiveAuraEffect.transform.SetParent(transform);

            // Enable collider for shield
            Collider shieldCollider = currentReflectiveAuraEffect.GetComponent<Collider>();
            if (shieldCollider != null)
            {
                shieldCollider.enabled = true;
            }
        }
        else
        {
            Debug.LogError("Reflective Aura Effect prefab is not assigned!");
        }

        Debug.Log("Shield activated successfully.");
    }
    public void DeactivateAura()
    {

        Destroy(currentAuraEffect);
        Debug.Log("Shield deactivated.");
        auraactive = false;

        StartCoroutine(RegenerateAura());
    }
    private IEnumerator RegenerateAura()
    {
        yield return new WaitForSeconds(50f);
        ActivateAura();
    }
    private void ActivateAura()
    {
        
        Debug.Log("Activating Reflective Aura...");

        if (!isPhase2)
        {
            Debug.LogWarning("Cannot activate Reflective Aura in phase 1!");
            return;
        }

        if (auraactive)
        {
            Debug.Log("Shield is already active. Exiting.");
            return;
        }
        soundEffectHandler.PlayBossSpellSound();

        float reflectiveAuraRange = 10f;
        float safeDistance = 10f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, reflectiveAuraRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log($"Player {collider.name} detected inside the aura range. Moving them outside.");
                Vector3 directionToPlayer = (collider.transform.position - transform.position).normalized;
                Vector3 newPosition = transform.position + directionToPlayer * safeDistance;
                collider.transform.position = newPosition;
                Debug.Log($"Player {collider.name} moved to {newPosition}.");
            }
        }

        auraactive = true;

        if (AuraEffect != null)
        {
            if (currentAuraEffect != null)
            {
                Debug.Log("Destroying existing Reflective Aura effect...");
                Destroy(currentAuraEffect);
            }

            currentAuraEffect = Instantiate(
                AuraEffect,
                transform.position,
                Quaternion.identity
            );

            currentAuraEffect.transform.SetParent(transform);

            // Enable collider for shield
            Collider shieldCollider = currentAuraEffect.GetComponent<Collider>();
            if (shieldCollider != null)
            {
                shieldCollider.enabled = true;
            }
        }
        else
        {
            Debug.LogError("Reflective Aura Effect prefab is not assigned!");
        }

        Debug.Log("Shield activated successfully.");
    }

    private void ApplyHealthDamage(float damage)
    {
        currentHealth -= damage;
        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }
        else
        {
            Debug.LogWarning("Animator is not assigned!");
        }
        UpdateHealthBar();


        if (currentHealth <= 0)
        {
            if(isPhase2)
            {
                Die();
                animator.SetTrigger("Die");
                soundEffectHandler.PlayBossDeathSound();
                
            }
            else
            {
                animator.SetTrigger("Resurrect");
                // wait for 5 seconds
                StartCoroutine(Resurrect());
                isPhase2 = true;
            }
        }
    }

    private void FaceWanderer()
    {
        if (wanderer != null)
        {
            Vector3 direction = (wanderer.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        soundEffectHandler.PlayBossDeathSound();
        Debug.Log("Lilith has been defeated!");
    }
}