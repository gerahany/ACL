
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab; // Health bar prefab
    public GameObject ShieldhealthPrefab; 
    private GameObject healthBar; // Instance of the health bar
    private Transform[] healthSegments; // Individual health bar segments
    private GameObject shieldHealthBar; // Instance of the shield health bar
    private Transform[] shieldSegments; 
    [Header("Health Settings")]
    public float phase1Health = 50f; // Phase 1 health
    public float phase2Health = 100f; // Phase 2 health
    private float currentHealth;
    private bool isPhase2 = false;
    private bool isDead = false;
    public Text healthText;  // Legacy Text for health display

    [Header("Minion Summoning")]
    public GameObject minionPrefab; // Prefab for minions
    public Transform[] spawnPoints; // Spawn points for minions
    public int maxMinions = 3; // Maximum number of minions
    private int activeMinions = 0; // Track active minions
    public GameObject SummonEffect; // Particle effect for summoning minions

    [Header("Divebomb Attack")]
    public int divebombDamage = 20; // Damage dealt by Divebomb
    public float divebombRadius = 5f; // Radius for Divebomb
    public GameObject divebombEffect; // Effect for Divebomb impact

    [Header("Attack Cooldown")]
    public float attackCooldown = 5f; // Cooldown time between actions
    private bool canAttack = true; // Control for cooldown

    [Header("Phase 2 Abilities")]

    public int reflectiveAuraDamage = 15; // Damage dealt by Reflective Aura
    public int bloodSpikesDamage = 30; // Damage dealt by Blood Spikes
    public float bloodSpikesRange = 10f; // Range of Blood Spikes attack
    public GameObject bloodSpikesEffect; // Effect for Blood Spikes attack
    public GameObject reflectiveAuraEffect; // Visual representation of Reflective Aura
    [Header("Defensive Shield Settings")]
    public float maxShieldHealth = 50f; // Shield max health
    private float shieldHealth;
    private bool shieldActive = false; 

    private bool isReflectiveAuraActive = false; // Tracks if Reflective Aura is active
    private bool canUseBloodSpikes = false;
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
    private Animator animator;

    void Start()
    {
        IdentifyActiveCharacter();
        currentHealth = phase1Health;
        animator = GetComponent<Animator>();
        shieldHealth = maxShieldHealth;

        CreateShieldHealthBar();
        UpdateShieldHealthBar();
        CreateHealthBar();
    }

    private void CreateShieldHealthBar()
{
    if (ShieldhealthPrefab != null && shieldHealthPosition != null)
    {
        shieldHealthBar = Instantiate(
            ShieldhealthPrefab,
            transform.position + new Vector3(0, 2.4f, 0),
            Quaternion.Euler(0, 90, 0)
        );
        

        shieldHealthBar.transform.SetParent(shieldHealthPosition);
        InitializeShieldSegments();
    }
}

// Initialize the shield health bar segments
private void InitializeShieldSegments()
{
    if (shieldHealthBar != null)
    {
        shieldSegments = new Transform[shieldHealthBar.transform.childCount];
        for (int i = 0; i < shieldHealthBar.transform.childCount; i++)
        {
            shieldSegments[i] = shieldHealthBar.transform.GetChild(i);
            shieldSegments[i].gameObject.SetActive(true);
        }
    }
}

// Update the shield health bar dynamically
private void UpdateShieldHealthBar()
{
    if (shieldHealthBar != null && shieldSegments != null)
    {
        int activeSegments = Mathf.CeilToInt((shieldHealth / maxShieldHealth) * shieldSegments.Length);

        for (int i = 0; i < shieldSegments.Length; i++)
        {
            shieldSegments[i].gameObject.SetActive(i < activeSegments); // Enable/disable segments
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

private bool IsActiveCharacter()
{
    return wanderer == this.transform; // Use 'this.transform' to refer to the current Transform
}


    void Update()
    {
        UpdateHealthBar();

        if (isDead) return;

        FaceWanderer();

        if (!isPhase2 && canAttack)
        {
            PerformPhase1Attack();
        }
        else if (isPhase2 && canAttack)
        {
            PerformPhase2Attack();
        }
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
    if (healthBar != null && healthSegments != null)
    {
        // Ensure proper type casting for Mathf.CeilToInt
        int activeSegments = Mathf.CeilToInt((float)(currentHealth / (isPhase2 ? phase2Health : phase1Health)) * healthSegments.Length);

        for (int i = 0; i < healthSegments.Length; i++)
        {
            healthSegments[i].gameObject.SetActive(i < activeSegments); // Enable/disable segments
        }

    }
}


    private void PerformPhase1Attack()
    {
        if (!canAttack) return;

        if (activeMinions == 0)
        {
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
        Debug.Log("Lilith is summoning minions!");
        animator.SetTrigger("Summon");

        for (int i = 0; i < maxMinions; i++)
        {
            Vector3 spawnPosition = transform.position + 
                new Vector3(Mathf.Cos(2 * Mathf.PI * i / maxMinions) * 5f, 0, Mathf.Sin(2 * Mathf.PI * i / maxMinions) * 5f);

            Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            activeMinions++;
            if (SummonEffect != null)
            {
                animator.SetTrigger("Summon");
                GameObject effectInstance = Instantiate(SummonEffect, transform.position, Quaternion.identity);
                Destroy(effectInstance, 10f);
            }
        }

        
    }

    public void OnMinionDefeated()
    {
        activeMinions--;
        Debug.Log($"A minion was defeated! Remaining: {activeMinions}");
    }

    private void Divebomb()
    {
        if (!canAttack) return;

        animator.SetTrigger("Divebomb");
        Debug.Log("Lilith is performing a Divebomb attack!");

        StartCoroutine(ExecuteDivebomb());
    }

    private IEnumerator ExecuteDivebomb()
    {
        yield return new WaitForSeconds(1.5f); // Sync with animation timing

        if (wanderer != null)
        {
            Vector3 stompPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(stompPosition, divebombRadius);

            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    Debug.Log("Wanderer hit by Divebomb!");
                    BasePlayer player = collider.GetComponent<BasePlayer>();
                    if (player != null)
                    {
                        player.TakeDamage(divebombDamage);
                    }
                }
            }

            if (divebombEffect != null)
            {
                animator.SetTrigger("Divebomb");
                GameObject effectInstance = Instantiate(divebombEffect, transform.position, Quaternion.identity);
                Destroy(effectInstance, 10f);
            }
        }

        // animator.ResetTrigger("Divebomb"); 
        yield return StartCoroutine(AttackCooldown());
    }

    private void PerformPhase2Attack()
    {
        if (isReflectiveAuraActive) return;

        if (!canUseBloodSpikes)
        {
            ActivateReflectiveAura();
        }
        else
        {
            UseBloodSpikes();
        }

        StartCoroutine(AttackCooldown());
    }
private void ActivateReflectiveAura()
{   
    
    if (isReflectiveAuraActive) return;

    animator.SetTrigger("CastSpell");
    Debug.Log("Lilith activated Reflective Aura!");

    isReflectiveAuraActive = true;

    // Instantiate Reflective Aura at predefined points
    if (reflectiveAuraPoints != null && reflectiveAuraPoints.Length > 0)
    {
        foreach (Transform point in reflectiveAuraPoints)
        {
            if (reflectiveAuraEffect != null)
            {
                // GameObject aura = Instantiate(reflectiveAuraEffect, point.position, point.rotation);
                // Destroy(aura, 5f); // Destroy after 5 seconds
                reflectiveAuraEffect.SetActive(true); // Activate the aura
                Destroy(reflectiveAuraEffect, 20f); 
            }

            if (auraParticleEffect != null)
            {
                GameObject particle = Instantiate(auraParticleEffect, point.position, Quaternion.identity);
                Destroy(particle, 5f); // Destroy after 5 seconds
            }
        }
    }
    if (reflectiveAuraEffect == null){
        isReflectiveAuraActive = false;
    }

    if (wanderer != null)
    {
        float distanceToWanderer = Vector3.Distance(transform.position, wanderer.position);
        if (distanceToWanderer <= 5f) // Adjust range as needed
        {
            BasePlayer player = wanderer.GetComponent<BasePlayer>();
            if (player != null)
            {
                Debug.Log("Wanderer takes damage from Reflective Aura!");
                player.TakeDamage(reflectiveAuraDamage);
            }
        }
    }

    // StartCoroutine(DeactivateReflectiveAura());
}

    private void DeactivateReflectiveAura()
    {
        if (!shieldActive) return;
        // yield return new WaitForSeconds(30f); // Duration of the Reflective Aura

        isReflectiveAuraActive = false;
        canUseBloodSpikes = true; // Allow Blood Spikes after aura deactivates

        // Disable the Reflective Aura effect
        if (reflectiveAuraEffect != null)
        {
            reflectiveAuraEffect.SetActive(false); // Deactivate the aura
        }
        shieldActive = false;
        UpdateShieldHealthBar();

        

        Debug.Log("Shield deactivated!");
        StartCoroutine(RegenerateShield());


    }


    private void ActivateShield()
{
    shieldActive = true;
    shieldHealth = maxShieldHealth; // Reset shield health
    UpdateShieldHealthBar();

    // Enable shield visuals
    if (reflectiveAuraEffect != null)
    {
        reflectiveAuraEffect.SetActive(true);
    }

    Debug.Log("Shield reactivated with full health!");
}


private void UseBloodSpikes()
{
    if (!canUseBloodSpikes) return;

    animator.SetTrigger("SwingHands");
    Debug.Log("Lilith is using Blood Spikes!");

    // Instantiate Blood Spikes at predefined points
    if (bloodSpikesPoints != null && bloodSpikesPoints.Length > 0)
    {
        foreach (Transform point in bloodSpikesPoints)
        {
            if (bloodSpikesEffect != null)
            {
                GameObject spikes = Instantiate(bloodSpikesEffect, point.position, point.rotation);
                Destroy(spikes, 10f);
            }
        }
    }

    // Damage the Wanderer if within range
    if (wanderer != null)
    {
        float distanceToWanderer = Vector3.Distance(transform.position, wanderer.position);
        if (distanceToWanderer <= bloodSpikesRange)
        {
            BasePlayer player = wanderer.GetComponent<BasePlayer>();
            if (player != null)
            {
                Debug.Log("Wanderer takes damage from Blood Spikes!");
                player.TakeDamage(bloodSpikesDamage);
            }
        }
    }

    canUseBloodSpikes = false; // Reset for Reflective Aura next
}

    




    private IEnumerator RegenerateShield()
{
    Debug.Log("Shield regeneration started. Waiting for cooldown...");

    // Wait for the cooldown time (adjust as needed)
    yield return new WaitForSeconds(Random.Range(30f, 60f));

    // Reactivate the shield
    ActivateReflectiveAura();
}


    public void TakeDamage(float damage)
{
    if (isDead) return;

    if (shieldActive)
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
        // Apply damage to the boss health if the shield is inactive
        ApplyHealthDamage(damage);
    }
}


    private void ApplyHealthDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Damaged");
        UpdateHealthBar();

        Debug.Log($"Lilith took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            if(isPhase2)
            {
                Die();
                animator.SetTrigger("Die");
            }
            else
            {
                animator.SetTrigger("Resurrect");
                isPhase2 = true;
            }
        }
    }

    
    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
        Debug.Log("Lilith has been defeated!");
    }
}



// using UnityEngine;
// using System.Collections;

// public class BossBehavior : MonoBehaviour
// {
//     [Header("Health Settings")]
//     public float phase1Health = 50f; // Phase 1 health
//     public float phase2Health = 100f; // Phase 2 health
//     private float currentHealth;
//     private bool isPhase2 = false;
//     private bool isDead = false;

//     [Header("Defensive Shield Settings")]
//     public float shieldHealth = 50f; // Shield health
//     public float shieldRegenDelay = 10f; // Time before shield regenerates
//     private float currentShieldHealth;
//     private bool isShieldActive = true;
//     public GameObject shieldEffect; // Visual representation of the shield

//     [Header("Health Bar Settings")]
//     public GameObject healthBarPrefab; // Health bar prefab
//     private GameObject healthBar; // Instance of the health bar
//     private Transform[] healthSegments; // Individual health bar segments

//     [Header("Minion Summoning")]
//     public GameObject minionPrefab; // Prefab for minions
//     public Transform[] spawnPoints; // Spawn points for minions
//     public int maxMinions = 3; // Maximum number of minions
//     private int activeMinions = 0; // Track active minions
//     public GameObject summonParticleEffect; // Particle effect for summoning minions

//     [Header("Divebomb Attack")]
//     public int divebombDamage = 20; // Damage dealt by Divebomb
//     public float divebombRadius = 5f; // Radius for Divebomb
//     public GameObject divebombEffect; // Effect for Divebomb impact

//     [Header("Phase 2 Abilities")]
//     public int reflectiveAuraDamage = 15; // Damage dealt by Reflective Aura
//     public int bloodSpikesDamage = 30; // Damage dealt by Blood Spikes
//     public float bloodSpikesRange = 10f; // Range of Blood Spikes attack
//     public GameObject reflectiveAuraEffect; // Prefab for Reflective Aura
//     public GameObject auraParticleEffect; // Particle effect for Reflective Aura
//     public GameObject bloodSpikesEffect; // Effect for Blood Spikes
//     public Transform[] reflectiveAuraPoints; // Points for Reflective Aura
//     public Transform[] bloodSpikesPoints; // Points for Blood Spikes

//     [Header("Attack Cooldown")]
//     public float attackCooldown = 5f; // Cooldown between actions
//     private bool canAttack = true;

//     [Header("References")]
//     public Transform wanderer; // Reference to the Wanderer (Player)
//     private Animator animator;

//     private bool isReflectiveAuraActive = false; // Tracks Reflective Aura state
//     private bool canUseBloodSpikes = false; // Determines if Blood Spikes can be used

//     void Start()
//     {
//         currentHealth = phase1Health;
//         currentShieldHealth = shieldHealth; // Initialize shield health
//         animator = GetComponent<Animator>();

//         CreateHealthBar();
//         UpdateShieldVisual();
//     }

//     void Update()
//     {
//         if (isDead) return;

//         FaceWanderer();

//         if (!isPhase2 && canAttack)
//         {
//             PerformPhase1Attack();
//         }
//         else if (isPhase2 && canAttack)
//         {
//             PerformPhase2Attack();
//         }
//     }

//     // -------------------- Core Mechanics --------------------

//     private void PerformPhase1Attack()
//     {
//         if (activeMinions == 0)
//         {
//             SummonMinions();
//         }
//         else
//         {
//             Divebomb();
//         }

//         StartCoroutine(AttackCooldown());
//     }

//     private void PerformPhase2Attack()
//     {
//         if (isReflectiveAuraActive) return;

//         if (!canUseBloodSpikes)
//         {
//             ActivateReflectiveAura();
//         }
//         else
//         {
//             UseBloodSpikes();
//         }

//         StartCoroutine(AttackCooldown());
//     }

//     private void SummonMinions()
//     {
//         Debug.Log("Lilith is summoning minions!");
//         animator.SetTrigger("Summon");

//         foreach (Transform spawnPoint in spawnPoints)
//         {
//             GameObject minion = Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
//             activeMinions++;

//             // Instantiate summon particle effect at the spawn point
//             if (summonParticleEffect != null)
//             {
//                 GameObject summonEffect = Instantiate(summonParticleEffect, spawnPoint.position, Quaternion.identity);
//                 Destroy(summonEffect, 2f); // Destroy particle after 2 seconds
//             }
//         }
//     }
//     public void OnMinionDefeated()
//     {
//         activeMinions--;
//         Debug.Log($"A minion was defeated! Remaining: {activeMinions}");
//     }

//     private void Divebomb()
//     {
//         Debug.Log("Lilith is performing a Divebomb attack!");
//         animator.SetTrigger("Divebomb");

//         StartCoroutine(ExecuteDivebomb());
//     }

//     private IEnumerator ExecuteDivebomb()
//     {
//         yield return new WaitForSeconds(1.5f); // Sync with animation timing

//         Vector3 stompPosition = transform.position;
//         Collider[] hitColliders = Physics.OverlapSphere(stompPosition, divebombRadius);

//         foreach (Collider collider in hitColliders)
//         {
//             if (collider.CompareTag("Player"))
//             {
//                 Debug.Log("Wanderer hit by Divebomb!");
//                 BasePlayer player = collider.GetComponent<BasePlayer>();
//                 if (player != null)
//                 {
//                     player.TakeDamage(divebombDamage);
//                 }
//             }
//         }

//         if (divebombEffect != null)
//         {
//             GameObject effectInstance = Instantiate(divebombEffect, stompPosition, Quaternion.identity);
//             Destroy(effectInstance, 2f);
//         }
//     }

//     private void ActivateReflectiveAura()
//     {
//         Debug.Log("Lilith activated Reflective Aura!");
//         animator.SetTrigger("CastSpell");

//         isReflectiveAuraActive = true;

//         foreach (Transform point in reflectiveAuraPoints)
//         {
//             GameObject aura = Instantiate(reflectiveAuraEffect, point.position, point.rotation);
//             Destroy(aura, 5f); // Aura duration

//             // Instantiate particle effect for Reflective Aura
//             if (auraParticleEffect != null)
//             {
//                 GameObject particle = Instantiate(auraParticleEffect, point.position, Quaternion.identity);
//                 Destroy(particle, 5f); // Particle duration
//             }
//         }

//         StartCoroutine(DeactivateReflectiveAura());
//     }

//     private IEnumerator DeactivateReflectiveAura()
//     {
//         yield return new WaitForSeconds(5f);

//         isReflectiveAuraActive = false;
//         canUseBloodSpikes = true;
//         Debug.Log("Reflective Aura deactivated!");
//     }

//     private void UseBloodSpikes()
//     {
//         Debug.Log("Lilith is using Blood Spikes!");
//         animator.SetTrigger("SwingHands");

//         foreach (Transform point in bloodSpikesPoints)
//         {
//             GameObject spikes = Instantiate(bloodSpikesEffect, point.position, point.rotation);
//             Destroy(spikes, 2f);
//         }

//         canUseBloodSpikes = false;
//     }

//     // -------------------- Shield Mechanics --------------------

//     private void UpdateShieldVisual()
//     {
//         if (shieldEffect != null)
//         {
//             shieldEffect.SetActive(isShieldActive);
//         }
//     }

//     public void TakeDamage(float damage)
//     {
//         if (isDead) return;

//         if (isShieldActive)
//         {
//             currentShieldHealth -= damage;

//             if (currentShieldHealth <= 0)
//             {
//                 float excessDamage = -currentShieldHealth;
//                 currentShieldHealth = 0;
//                 isShieldActive = false;

//                 UpdateShieldVisual();
//                 Debug.Log("Shield destroyed! Excess damage applied to Lilith.");

//                 TakeDirectDamage(excessDamage);
//                 StartCoroutine(RegenerateShield());
//             }
//         }
//         else
//         {
//             TakeDirectDamage(damage);
//         }
//     }

//     private void TakeDirectDamage(float damage)
//     {
//         currentHealth -= damage;
//         UpdateHealthBar();

//         if (currentHealth <= 0)
//         {
//             if (!isPhase2)
//             {
//                 TransitionToPhase2();
//             }
//             else
//             {
//                 Die();
//             }
//         }
//     }

//     private IEnumerator RegenerateShield()
//     {
//         yield return new WaitForSeconds(shieldRegenDelay);

//         currentShieldHealth = shieldHealth;
//         isShieldActive = true;

//         UpdateShieldVisual();
//         Debug.Log("Shield fully regenerated!");
//     }

//     private void TransitionToPhase2()
//     {
//         Debug.Log("Transitioning to Phase 2!");
//         isPhase2 = true;
//         currentHealth = phase2Health;
//         currentShieldHealth = shieldHealth;
//         isShieldActive = true;

//         UpdateHealthBar();
//         UpdateShieldVisual();
//     }

//     private void Die()
//     {
//         isDead = true;
//         animator.SetTrigger("Die");
//         Debug.Log("Lilith has been defeated!");
//     }

//     // -------------------- Utility Functions --------------------

//     private void FaceWanderer()
//     {
//         if (wanderer != null)
//         {
//             Vector3 direction = (wanderer.position - transform.position).normalized;
//             direction.y = 0;
//             transform.rotation = Quaternion.LookRotation(direction);
//         }
//     }

//     private IEnumerator AttackCooldown()
//     {
//         canAttack = false;
//         yield return new WaitForSeconds(attackCooldown);
//         canAttack = true;
//     }

//     private void CreateHealthBar()
//     {
//         if (healthBarPrefab != null)
//         {
//             healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2.5f, 0), Quaternion.Euler(0, 90, 0));
//             healthBar.transform.SetParent(transform);
//             InitializeHealthSegments();
//         }
//     }

//     private void InitializeHealthSegments()
//     {
//         if (healthBar != null)
//         {
//             healthSegments = new Transform[healthBar.transform.childCount];
//             for (int i = 0; i < healthBar.transform.childCount; i++)
//             {
//                 healthSegments[i] = healthBar.transform.GetChild(i);
//                 healthSegments[i].gameObject.SetActive(true);
//             }
//         }
//     }

//     private void UpdateHealthBar()
//     {
//         if (healthBar != null && healthSegments != null)
//         {
//             int activeSegments = Mathf.CeilToInt((currentHealth / (isPhase2 ? phase2Health : phase1Health)) * healthSegments.Length);
//             for (int i = 0; i < healthSegments.Length; i++)
//             {
//                 healthSegments[i].gameObject.SetActive(i < activeSegments);
//             }
//         }
//     }
// }
