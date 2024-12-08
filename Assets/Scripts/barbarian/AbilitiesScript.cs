using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
public class Barbarian : BasePlayer
{
    public Animator animator;
    public ParticleSystem chargeEffect;
    public NavMeshAgent agent;
    public GameObject shieldBall; 
    private Dictionary<string, float> abilityCooldowns = new Dictionary<string, float>();
    private float bashCooldown = 1f;
    private float shieldCooldown = 10f;
    private float ironMaelstromCooldown = 5f;
    private float chargeCooldown = 10f;
    public bool shieldActive = false;
    private float shieldDuration = 3f;
    public ParticleSystem bloodEffect;

    private bool isAbilityInProgress = false; // Flag to track if an ability is in progress
    private bool isIronMaelstromActive = false; // Flag for Iron Maelstrom ability
    private bool isChargeActive = false; // Flag for Charge ability


    void Update()
    {
        HandleAbilityInput();
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        bool isMoving = agent.remainingDistance > agent.stoppingDistance && agent.velocity.sqrMagnitude > 0.1f;
    
        // Ensure that "isWalking" is set correctly after Charge or Bash
        if (!animator.GetBool("isWalking") && isMoving)
        {
            animator.SetBool("isWalking", true);
        }

        if (animator.GetBool("isWalking") && !isMoving)
        {
            animator.SetBool("isWalking", false);
        }
    }
    public bool IsShieldActive()
    {
        return shieldActive;
    }


    void HandleAbilityInput()
{
    if (Input.GetKeyDown(KeyCode.W)) UseShield();

    if (!isAbilityInProgress)
    {
        // Check for ability activation first
        if (Input.GetKeyDown(KeyCode.Q) && !isAbilityInProgress)  // Wild Card
        {
            isIronMaelstromActive = true;  // Mark the ability as active
            isAbilityInProgress = true;  // Lock other abilities
            Debug.Log("Wild Card activated. Right-click to specify position.");
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAbilityInProgress)  // Ultimate
        {
            isChargeActive = true;  // Mark the ability as active
            isAbilityInProgress = true;  // Lock other abilities
            Debug.Log("Ultimate activated. Right-click to specify position.");
        }
    }

    // Handle right-click once abilities are activated
    if (Input.GetMouseButtonDown(1) && isAbilityInProgress)
    {
        Vector3 targetPosition = GetMouseWorldPosition();
        
        // Use the ability based on the state
        if (isIronMaelstromActive)  // If Wild Card (Iron Maelstrom) is active
        {
            //Debug.Log("Qpress");
            UseIronMaelstrom(targetPosition);  // Use ability at the specified position
            isIronMaelstromActive = false;  // Reset flag after use
        }
        else if (isChargeActive)  // If Ultimate (Charge) is active
        {
            //Debug.Log("Epress");
            UseCharge(targetPosition);  // Use ability at the specified position
            isChargeActive = false;  // Reset flag after use
        }

        // Unlock ability usage after handling right-click
        isAbilityInProgress = false;
    }
}

Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;  // Return the world position of the mouse click
        }
        return Vector3.zero;  // Return zero if nothing is hit
    }
    // 1. Bash Ability (Single Target Attack)
   void UseBash()
{
    if (!IsAbilityOnCooldown("Bash") && !isAbilityInProgress)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the hit object is a Minion or Demon
            if (hit.collider.CompareTag("Minion") || hit.collider.CompareTag("Demon"))
            {
                Debug.Log($"Bash targeting: {hit.collider.name}");

                // Calculate distance to ensure the enemy is within range
                float distanceToEnemy = Vector3.Distance(transform.position, hit.point);
                if (distanceToEnemy <= 3f) // Ensure the enemy is within attack range
                {
                    // Set the destination for the NavMeshAgent to move towards the enemy
                    agent.SetDestination(hit.point);
                    animator.SetBool("isWalking", true);
                    StartCoroutine(MoveAndAttack(hit.collider.gameObject));

                    // Apply damage immediately after triggering Bash
                    ApplyBashDamage(hit.collider.gameObject);  // Apply damage to the enemy
                }
                else
                {
                    Debug.Log("Enemy is out of range!");
                }
            }
            else
            {
                Debug.Log("Invalid target for Bash: Not a Minion or Demon.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object.");
        }
    }
}


IEnumerator TriggerBloodEffect()
    {
        // Position the blood effect where the weapon hits
        bloodEffect.transform.position = transform.position + transform.forward * 2f;  // Adjust position near the weapon

        // Play the blood effect (you can adjust timing if needed)
        bloodEffect.Play();

        yield return new WaitForSeconds(bloodEffect.main.duration);  // Wait for the blood effect duration to finish
        bloodEffect.Stop();  // Stop the blood effect after it finishes
    }
    IEnumerator MoveAndAttack(GameObject enemy)
    {
        isAbilityInProgress = true;  // Lock ability usage until this is done

        // Wait until the player reaches the target position
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

StartCoroutine(TriggerBloodEffect());
        // Once the player reaches the enemy, trigger the Bash animation
        animator.SetTrigger("Bash");

        // Apply the Bash logic (e.g., deal 5 damage to the enemy)
       

        // After attack, stop the walking animation and reset NavMeshAgent
        animator.SetBool("isWalking", false);
        agent.isStopped = true;  // Stop the agent after attack
        agent.ResetPath();  // Reset the agent's path to avoid unwanted movement

        // Transition to Idle after the attack
        yield return new WaitForSeconds(0.5f);  // Wait a bit for the animation to play
        animator.SetTrigger("isIdle");  // Transition to Idle animation

        isAbilityInProgress = false;  // Unlock ability usage
    }

    // Apply the Bash damage (for example, you could hit the enemy here)
void ApplyBashDamage(GameObject enemy)
{
        // Check if the enemy has a MinionHealth component
        minionhealthbar minionHealth = enemy.GetComponent<minionhealthbar>();
        demonhealthbar demonHealth = enemy.GetComponent<demonhealthbar>();

        if (minionHealth != null)
        {
            Debug.Log("Bash hit the minion!");

            minionHealth.TakeDamage(5);  // Apply 5 damage to the minion
            Debug.Log($"Bash hit {enemy.name} for 5 damage!");
        }
        else if (demonHealth != null)
        {
            Debug.Log("Bash hit the minion!");

            demonHealth.TakeDamage(5);  // Apply 5 damage to the minion
            Debug.Log($"Bash hit {enemy.name} for 5 damage!");
        }


        else
    {
        Debug.LogWarning($"No MinionHealth script found on {enemy.name}!");
    }
}



void ApplyIronMaelstromDamage(Vector3 position)
{
    // Get all colliders in a small radius (AoE around the player's position)
    Collider[] hitColliders = Physics.OverlapSphere(position, 3f); // Adjust radius as needed
    foreach (Collider collider in hitColliders)
    {
        minionhealthbar minionhealthbar = collider.GetComponent<minionhealthbar>();
        demonhealthbar demonHealth = collider.GetComponent<demonhealthbar>();

        if (minionhealthbar != null)
        {
            minionhealthbar.TakeDamage(10);  // Apply 10 damage to each enemy in the AoEs
            Debug.Log($"Iron Maelstrom hit {collider.name} for 10 damage!");
        }
        if (demonHealth != null)
        {
            demonHealth.TakeDamage(10);  // Apply 10 damage to each enemy in the AoEs
            Debug.Log($"Iron Maelstrom hit {collider.name} for 10 damage!");
        }
    }
}


    // 2. Shield Ability (Defensive)
    void UseShield()
    {
        if (!IsAbilityOnCooldown("Shield"))
        {
             // Shield can be activated even if other abilities are in progress
            shieldActive = true;
            shieldBall.SetActive(true);  // Activate the shield visually
            animator.SetTrigger("Shield");  // Trigger the shield animation
            Debug.Log($"{playerName} activated Shield!");
            StartCoroutine(ShieldCoroutine());
            
        }
    }

    IEnumerator ShieldCoroutine()
    {
        // Shield remains active for 3 seconds
        yield return new WaitForSeconds(shieldDuration);

        // Deactivate the shield after the duration
        shieldActive = false;
        shieldBall.SetActive(false);
        Debug.Log("Shield expired!");

        isAbilityInProgress = false; 
        StartAbilityCooldown("Shield", shieldCooldown); // Unlock ability usage after shield is done
    }

    // 3. Iron Maelstrom Ability (AoE Attack)
    void UseIronMaelstrom(Vector3 targetPosition)
    {
        Debug.Log("Iron");
        Debug.Log(isAbilityInProgress);
        if (!IsAbilityOnCooldown("IronMaelstrom") )
        {
            isAbilityInProgress = true;
            ApplyIronMaelstromDamage(transform.position);
            animator.SetTrigger("IronMaelstrom");

            Debug.Log($"{playerName} used Iron Maelstrom!");
            
            // Apply AoE damage logic here
            StartCoroutine(TriggerBloodEffect());
            StartCoroutine(WaitForAbilityCompletion());
            StartAbilityCooldown("IronMaelstrom", ironMaelstromCooldown);
        }
    }
    

    void UseCharge(Vector3 targetPosition)
    {
        if (!IsAbilityOnCooldown("Charge") )
        {
            isAbilityInProgress = true;
            // Stop the NavMeshAgent to prevent pathfinding interference
            agent.isStopped = true;
            agent.updateRotation = false;  // Prevent rotation adjustments by NavMeshAgent

            // Start the charge animation and movement
            animator.SetBool("isCharging", true);
            StartCoroutine(ChargeForward(targetPosition));

            StartAbilityCooldown("Charge", chargeCooldown);
        }
    }
    
    void ApplyChargeDamage()
{
    // Get all colliders in a small radius (for collision detection during the charge)
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // Adjust radius as needed
    
    foreach (Collider collider in hitColliders)
    {
        minionhealthbar minionhealthbar = collider.GetComponent<minionhealthbar>();
         demonhealthbar demonHealth = collider.GetComponent<demonhealthbar>();
        if (minionhealthbar != null)
        {
            minionhealthbar.TakeDamage(20);  // Apply 20 damage to minions/demons
            Debug.Log($"Charge hit {collider.name} for 20 damage!");
        }
        if (demonHealth != null)
        {
            demonHealth.TakeDamage(20);  // Apply 20 damage to minions/demons
            Debug.Log($"Charge hit {collider.name} for 20 damage!");
        }


        // Destroy breakable objects
        if (collider.CompareTag("Breakable"))
        {
            Destroy(collider.gameObject);  // Destroy breakable object
            Debug.Log($"{collider.name} was destroyed by the charge!");
        }
    }
}
   IEnumerator ChargeForward(Vector3 targetPosition)
{
    float chargeSpeed = 15f;          // Charge speed
    float chargeDistance = Vector3.Distance(transform.position, targetPosition);       // Distance to charge
    float traveledDistance = 0f;
    Vector3 startPosition = transform.position;

    // Timeout limit in seconds (e.g., 5 seconds for timeout)
    float timeoutDuration = 5f;
    float elapsedTime = 0f;

    // Cache the initial forward direction for the entire charge duration
    Vector3 chargeDirection = transform.forward.normalized;

    while (traveledDistance < chargeDistance)
    {
        // Timeout check
        if (elapsedTime >= timeoutDuration)
        {
            Debug.Log("Charge timeout reached!");
            break; // Stop the charge after timeout
        }

        // Move the player forward
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, chargeSpeed * Time.deltaTime);
        traveledDistance = Vector3.Distance(transform.position, targetPosition);
        
        // Apply damage to enemies in the charge path
        ApplyChargeDamage();

        // Update elapsed time
        elapsedTime += Time.deltaTime;

        yield return null;
    }

    // Stop the charge and reset NavMeshAgent
    agent.isStopped = false;
    agent.ResetPath();  // Clear the NavMeshAgent’s path
    agent.updateRotation = true;  // Allow normal rotation behavior to resume

    // Stop charge animation and transition to idle
    animator.SetBool("isCharging", false);
    animator.SetTrigger("isIdle");

    Debug.Log("Charge complete, or timeout reached, staying at final position.");
    isAbilityInProgress = false;  // Unlock ability usage after charge is complete
}

    IEnumerator WaitForAbilityCompletion()
    {
        // Wait until the current animation completes (adjust this time based on your animation)
        yield return new WaitForSeconds(1f);

        isAbilityInProgress = false;  // Unlock ability usage after the ability completes
    }

    // Cooldown handling logic
    private bool IsAbilityOnCooldown(string abilityName)
    {
        if (abilityCooldowns.ContainsKey(abilityName))
        {
            return Time.time < abilityCooldowns[abilityName];
        }
        return false;
    }

    private void StartAbilityCooldown(string abilityName, float cooldownDuration)
    {
        abilityCooldowns[abilityName] = Time.time + cooldownDuration;
    }
}