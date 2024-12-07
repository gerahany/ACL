using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DemonBehavior : MonoBehaviour
{
    private DemonSpawner campManager;
    private Vector3 initialPosition;
    private bool isAggressive = false;
    private GameObject targetPlayer;
    private bool hasExploded = false;
    private NavMeshAgent agent;
    private static GameObject globalTarget2;
    private Animator animator; // Reference to Animator
    public float attackRange = 2f;  // Distance at which the demon will start attacking
    public float swingCooldown = 7f;  // Time between sword swings
    public float explodeRange = 2f;  // Distance at which the demon explodes
    private float swingCooldownTimer = 0f;
    private bool isAttacking = false;
    private bool isExploding = false;
    private Transform rightHand;  // Reference to the right hand bone
    private Transform sword;  
    public GameObject bombPrefab; // Bomb prefab reference
    public float throwRange = 10f; // Max distance to throw the bomb
    public float throwCooldown = 5f; // Cooldown between bomb throws
    private float throwCooldownTimer = 0f; // Timer to manage throw cooldown

public Transform explosionEffect;
    void Start()
    {
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get Animator component
        Debug.Log("Right Hand");
        if (animator != null && animator.isHuman)
    {
        HumanBodyBones[] bodyBones = (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones));
        
        foreach (HumanBodyBones bone in bodyBones)
        {
            Transform boneTransform = animator.GetBoneTransform(bone);
            if (boneTransform != null)
            {
                Debug.Log($"{bone.ToString()}: {boneTransform.name}");  // Log each bone's name
            }
        }
    }
        // Find the right hand bone using Animator's GetBoneTransform method
    rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand); // You can use LeftHand as well
    sword = transform.Find("Sword");  // Find the sword object (assuming the sword is named "Sword")
    
    // Optionally deactivate sword at the start
    if (sword != null)
    {
        sword.gameObject.SetActive(false); // Hide the sword initially
    }
    }

    public void Initialize(DemonSpawner manager)
    {
        campManager = manager;
    }

    public void SetAggressive(GameObject player)
    {
        isAggressive = true;
        targetPlayer = player;
    }

    public void ReturnToCamp()
    {
        isAggressive = false;
        targetPlayer = null;
        agent.SetDestination(initialPosition);
        agent.isStopped = false;
        UpdateAnimation(true);
    }

void Update()
{
    if (MinionBehavior.globalTarget2 != null)
        {
            targetPlayer = MinionBehavior.globalTarget2; // Switch to the global target
        }
        else
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player"); // Default to the player
        }

    // Track agent's current position and destination
    if (agent != null)
    {
        Debug.Log($"Agent Position: {agent.transform.position}, Destination: {agent.destination}");
    }

    if (isAggressive && targetPlayer != null)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("IsWalking", false);
            agent.isStopped = true;
            if (!isAttacking && !isExploding)
            {
                StartCoroutine(AttackPattern());
            }
        }
        else
        {
            animator.SetBool("IsWalking", true);
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.transform.position);
        }
    }
    else if (!isAggressive)
    {
        // Check for returning to camp
        float distanceToCamp = Vector3.Distance(transform.position, initialPosition);

        // Log distance to initial position
        Debug.Log($"Distance to camp: {distanceToCamp}");

        if (distanceToCamp <= 1f)
        {
            // Only stop if we are very close to the destination
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
            Debug.Log("Demon reached camp, stopping.");
        }
        else
        {
            // Ensure the agent continues to update towards the destination
            agent.isStopped = false;
            animator.SetBool("IsWalking", true);
            Debug.Log("Demon moving back to camp.");
        }
    }
}

public static void SetGlobalTarget2(GameObject target)
    {
        globalTarget2 = target;
    }

    public static void RevertGlobalTarget2()
    {
        globalTarget2 = null;
    }
    private IEnumerator AttackPattern()
    {
        // Sword swing
        isAttacking = true;
        ActivateSword(); // Activate the sword at the start
        animator.SetTrigger("SwingSword");

        // Wait for the animation to finish before deactivating the sword
        yield return new WaitForSeconds(2f);
        DeactivateSword(); 
        // Apply damage when swing occurs
        ApplySwordDamage();
        animator.SetBool("IsIdle", true);
        // Wait for the swing cooldown before attacking again
        yield return new WaitForSeconds(swingCooldown);

        

        // Second sword swing
        ActivateSword(); // Activate the sword at the start
        animator.SetTrigger("SwingSword");

        // Wait for the animation to finish before deactivating the sword
        yield return new WaitForSeconds(2f);
        DeactivateSword(); 
        // Apply damage when swing occurs
        ApplySwordDamage();

        // Wait for the animation to finish before proceeding
        yield return new WaitForSeconds(GetAnimationDuration("SwingSword")); // Wait for sword swing animation to finish

        // Wait for the desired delay between the second sword swing and explosion
        yield return new WaitForSeconds(7f);  // Add delay here

        // Explode (when close to the player)
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= explodeRange)
        {
            animator.SetTrigger("Explode");
            // Apply explosion damage (you can use your own explosion logic here)
            // Always rotate to face the player
            Vector3 directionToPlayer = targetPlayer.transform.position - transform.position;
            directionToPlayer.y = 0;  // Keep rotation only on the horizontal plane (y-axis)
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);

            // Throw bomb if within range and cooldown is over
            if (distanceToPlayer <= throwRange)
            {
                throwCooldownTimer = throwCooldown; // Reset the cooldown timer
                StartCoroutine(ThrowBombAtPlayer());
            }
            
            throwCooldownTimer -= Time.deltaTime; // Update cooldown timer
           
            yield return new WaitForSeconds(GetAnimationDuration("Explode")); // Wait for explosion animation to finish
        }

        // Automatically return to idle after animations
        animator.SetBool("IsIdle", true);
        isAttacking = false;

        // Repeat the pattern
        yield return new WaitForSeconds(10f);  // Wait before starting the attack cycle again
    }

    // Helper method to get the duration of an animation by its name
    private float GetAnimationDuration(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);  // 0 is the layer index (usually 0)
        if (stateInfo.IsName(animationName))
        {
            return stateInfo.length;  // Duration of the current animation state
        }
        return 0f;  // Return 0 if the animation is not playing
    }


    // Apply sword damage
    private void ApplySwordDamage()
    {
        // Your sword damage logic (e.g., apply damage to the player)
        if (targetPlayer != null)
        {
            targetPlayer.GetComponent<BasePlayer>().TakeDamage(10); // Example of applying 10 damage
        }
    }

    // Apply explosion damage logic (Explosion logic here)
    private void ApplyExplosionDamage()
    {
        // Example: apply explosion damage to the player within explosion range
        if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.transform.position) <= explodeRange)
        {
            // Your explosion damage logic here, for example:
            targetPlayer.GetComponent<BasePlayer>().TakeDamage(15); // Example of applying 20 explosion damage
        }
    }

    // Animation update method for idle/walking
    private void UpdateAnimation(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking); // Update walking animation
        }
    }
    public void Die()
    {
        // Handle death logic (e.g., destroy minion, play animation)
        Destroy(gameObject);
    }
    // Sword control methods
private void ActivateSword()
{
    Transform sword = transform.Find("Sword");  // Ensure the sword is found
    if (sword != null)
    {
        sword.gameObject.SetActive(true);  // Activate the sword
        
        // Attach the sword to the right hand
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        
        if (rightHand != null)
        {
            sword.SetParent(rightHand);  // Attach sword to the right hand
            sword.localPosition = Vector3.zero;  // Adjust the position to align with the hand
            sword.localRotation = Quaternion.identity;  // Align rotation as needed
        }
        else
        {
            Debug.LogError("Right hand not found in animator.");
        }
    }
    else
    {
        Debug.LogError("Sword not found in hierarchy.");
    }
}


    private void DeactivateSword()
    {
        Transform sword = transform.Find("Sword"); // Replace "Sword" with the exact name of your sword object
      if (sword != null && sword.parent == rightHand)
    {
        sword.gameObject.SetActive(false); // Deactivate the sword
        sword.SetParent(null);             // Detach the sword from the right hand
    }
    }

       private IEnumerator ThrowBombAtPlayer()
{
    // Find the direction to the player
    Vector3 directionToPlayer = targetPlayer.transform.position - transform.position;

    // Instantiate the bomb at the demon's position
    GameObject thrownBomb = Instantiate(bombPrefab, transform.position + Vector3.up, Quaternion.identity);

    // Add force to the bomb to throw it towards the player
    Rigidbody bombRb = thrownBomb.GetComponent<Rigidbody>();
    if (bombRb != null)
    {
        bombRb.AddForce(directionToPlayer.normalized * 10f, ForceMode.VelocityChange); // Adjust the force as needed
    }

    // Wait for the bomb to reach the target (or for a collision)
    yield return new WaitForSeconds(2f); // Adjust the time as necessary to match the throwing distance

 hasExploded = false;
    // Trigger explosion only once if not already exploded
    if (!hasExploded && Vector3.Distance(thrownBomb.transform.position, targetPlayer.transform.position) <= explodeRange)
    {
       
        TriggerExplosion(thrownBomb.transform.position);
        hasExploded = true;
    }

    // Destroy the bomb after the explosion
    Destroy(thrownBomb);
}

    private void TriggerExplosion(Vector3 explosionPosition)
{
    // Check if explosionEffect is assigned in the Inspector
    if (explosionEffect != null)
    {
        // Instantiate the explosion effect at the bomb's position
        Transform explosion = Instantiate(explosionEffect, explosionPosition, Quaternion.identity);

        // Ensure the ParticleSystem exists
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            // Play the particle system for the explosion
            particleSystem.Stop();  // Stop any previous particle system
            particleSystem.Play();  // Play the explosion effect

            // Optionally, adjust destroy time based on the duration of the explosion effect
            float explosionDuration = 2f;
            Destroy(explosion.gameObject, explosionDuration);  // Destroy after the particle effect duration
            Debug.Log("Explosion effect played.");
        }
        else
        {
            Debug.LogError("Explosion effect does not have a ParticleSystem.");
        }

        // Apply damage after the explosion effect
        ApplyExplosionDamage();
    }
    else
    {
        Debug.LogError("Explosion effect is not assigned.");
    }
}

   
}