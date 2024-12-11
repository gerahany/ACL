using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class DashAbility : MonoBehaviour
{
    public float dashSpeedMultiplier = 2f;
    public float dashCooldown = 5f;
    public GameObject dashEffectPrefab;
    private float currentCooldownTime = 0f;
    private bool canDash = true;
    public BasePlayer basePlayer;
    private bool isSelectingPosition = false;
    private bool isDashing = false;
    public TMP_Text cooldownText;
    private NavMeshAgent agent;
    private bool isCooldown = false;
    private Animator animator;
    private Vector3 dashTargetPosition;
    private GameObject dashEffectInstance;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cooldownText.text = "OK";
    }

    void Update()
    {
        // Only activate ability if no other ability is active
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !AbilityManager.IsAbilityActive()  && basePlayer.IsWildUnlocked)
        {
            StartSelectingPosition();
        }

        if (isSelectingPosition && Input.GetMouseButtonDown(1))
        {
            TryDash();
        }
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime; // Increase cooldown time

            float timeLeft = dashCooldown - currentCooldownTime; // Calculate remaining time

            // Update the TMP text to show the remaining cooldown time
            cooldownText.text = Mathf.Ceil(timeLeft) + "s"; // Round and show seconds

            if (currentCooldownTime >= dashCooldown)
            {
                //canUseAbility = true; // Reset ability to be ready
                currentCooldownTime = 0f; // Reset cooldown time
                cooldownText.text = "OK"; // Show "OK" when cooldown is complete
                Debug.Log("Shower of Arrows ability is ready.");
            }
        }
        animator.SetBool("isDashing", isDashing);
    }

    void StartSelectingPosition()
    {
        isSelectingPosition = true;
        Debug.Log("Select a position to dash using right-click.");
    }

    void TryDash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            if (IsWalkable(targetPosition))
            {
                dashTargetPosition = targetPosition;
                StartCoroutine(Dash(dashTargetPosition));  // Pass dashTargetPosition to Dash coroutine
            }
            else
            {
                Debug.Log("Target position is not walkable.");
            }
        }

        isSelectingPosition = false;
    }


    bool IsWalkable(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    IEnumerator Dash(Vector3 dashTargetPosition)
{
    animator.SetBool("isDashing", true);
    float dashSpeed = 15f;           // Dash speed
    float maxDashDistance = 10f;     // Maximum dash distance
    float dashDistance = Vector3.Distance(transform.position, dashTargetPosition);  // Distance to dash
    
    // Limit the dash distance to the maximum allowed distance
    dashDistance = Mathf.Min(dashDistance, maxDashDistance);

    float traveledDistance = 0f;
    Vector3 startPosition = transform.position;

    // Cache the initial forward direction for the entire dash duration
    Vector3 dashDirection = (dashTargetPosition - transform.position).normalized;

    // Calculate the clamped target position based on the maximum dash distance
    Vector3 clampedTargetPosition = transform.position + dashDirection * dashDistance;

    // Disable NavMeshAgent updates during dash
    agent.isStopped = true;
    agent.updateRotation = false;  // Lock rotation during the dash

    // Start the dash effect
    dashEffectInstance = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
    dashEffectInstance.transform.parent = transform;

    // Start dashing
    while (traveledDistance < dashDistance)
    {
        // Move the player forward along the cached dash direction
        transform.position = Vector3.MoveTowards(transform.position, clampedTargetPosition, dashSpeed * Time.deltaTime);

        // Update the traveled distance
        traveledDistance = Vector3.Distance(transform.position, startPosition);

        // Update the dash effect position to follow the player
        if (dashEffectInstance != null)
        {
            dashEffectInstance.transform.position = transform.position;
        }

        yield return null;
    }

    // Stop the dash and reset NavMeshAgent
    if (dashEffectInstance != null)
    {
        Destroy(dashEffectInstance);
    }

    agent.isStopped = false;        // Re-enable NavMeshAgent movement
    agent.updateRotation = true;   // Allow normal rotation
    agent.ResetPath();             // Clear the path
animator.SetBool("isDashing", false);
    // Dash cooldown or other follow-up logic
    StartCoroutine(DashCooldown());
}
public void SetTargetPosition(Vector3 targetPosition)
{
    if (!isDashing) // Block new inputs while dashing
    {
        dashTargetPosition = targetPosition;
    }
}


    IEnumerator DashCooldown()
    {
        AbilityManager.SetAbilityActive(false); // Mark ability as inactive
        isCooldown=true;
        yield return new WaitForSeconds(dashCooldown);
        isCooldown=false;
        canDash = true;
        Debug.Log("Dash cooldown ended.");
    }
}