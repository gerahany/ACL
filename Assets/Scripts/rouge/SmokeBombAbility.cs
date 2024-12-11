using System.Collections;
using UnityEngine;
using TMPro;

public class SmokeBombAbility : MonoBehaviour
{
    public GameObject smokeBombPrefab; // Smoke bomb visual effect prefab
    public float stunDuration = 5f; // Duration of the stun effect
    public float smokeBombCooldown = 10f; // Cooldown for the ability
    public float stunRange = 5f; // Range of the smoke bomb's effect
    public TMP_Text cooldownText;
    public BasePlayer basePlayer;
    public LayerMask targetLayer;
    private bool canUseSmokeBomb = true; // Can the Rogue use the smoke bomb?
    private bool isCooldown = false;
    private Animator animator; // Reference to the Animator component
    private float currentCooldownTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        cooldownText.text = "OK";
    }

    void Update()
    {
        // Activate smoke bomb ability when pressing "W"
        if (Input.GetKeyDown(KeyCode.W) && canUseSmokeBomb && basePlayer.IsDefensiveUnlocked)
        {
            ActivateSmokeBomb();
        }
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime; // Increase cooldown time

            float timeLeft = 10 - currentCooldownTime; // Calculate remaining time

            // Update the TMP text to show the remaining cooldown time
            cooldownText.text = Mathf.Ceil(timeLeft) + "s"; // Round and show seconds

            if (currentCooldownTime >= 10)
            {
                //canUseAbility = true; // Reset ability to be ready
                currentCooldownTime = 0f; // Reset cooldown time
                cooldownText.text = "OK"; // Show "OK" when cooldown is complete
                Debug.Log("Shower of Arrows ability is ready.");
            }
        }
    }

    void ActivateSmokeBomb()
    {
        // Instantiate the smoke bomb at the Rogue's position
        Vector3 bombPosition = transform.position;
        GameObject smokeBomb = Instantiate(smokeBombPrefab, bombPosition, Quaternion.identity);

        // Start the stun effect
        StartCoroutine(StunEnemies(bombPosition));

        // Trigger animation
        animator.SetTrigger("SmokeBomb");

        // Start cooldown
        StartCoroutine(SmokeBombCooldown());
    }

    IEnumerator StunEnemies(Vector3 center)
{
    // Check if enemies are being detected
    Collider[] hitColliders = Physics.OverlapSphere(center, stunRange, targetLayer);
    Debug.Log($"Stun range checked at {center}. {hitColliders.Length} enemies detected.");

    foreach (Collider collider in hitColliders)
    {
        // Debug log to see if the right enemy is being checked
        Debug.Log($"Checking collider: {collider.name}, Tag: {collider.tag}");

        if (collider.CompareTag("Demon") || collider.CompareTag("Minion"))
        {
            // Get the NavMeshAgent and Animator of the enemy
            UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>();
            Animator enemyAnimator = collider.GetComponent<Animator>();

            // Ensure the components exist
            if (agent != null && enemyAnimator != null)
            {
                // Log that the stun is being applied
                Debug.Log($"Stunning enemy: {collider.name}");

                // Stun the enemy by disabling movement
                agent.isStopped = true;
                agent.velocity = Vector3.zero;  // Make sure the agent stops moving instantly

                // Trigger the stun animation
               
                    enemyAnimator.SetTrigger("Stunned");
        
                // Wait for the stun duration
                yield return new WaitForSeconds(stunDuration);

                // After stun duration, restore enemy behavior
                agent.isStopped = false;
                Debug.Log($"{collider.name} has recovered from the stun.");
            }
            else
            {
                // Log if either NavMeshAgent or Animator are missing
                if (agent == null)
                    Debug.LogWarning($"NavMeshAgent missing on enemy: {collider.name}");
                if (enemyAnimator == null)
                    Debug.LogWarning($"Animator missing on enemy: {collider.name}");
            }
        }
    }
}


    IEnumerator SmokeBombCooldown()
    {
        canUseSmokeBomb = false;
        yield return new WaitForSeconds(5f);
        isCooldown=true;
        yield return new WaitForSeconds(smokeBombCooldown);
        isCooldown=false;
        canUseSmokeBomb = true;
        Debug.Log("Smoke bomb ability is ready.");
    }


}
