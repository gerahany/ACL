using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class DashAbility : MonoBehaviour
{
    public float dashSpeedMultiplier = 2f;
    public float dashCooldown = 5f;
    public GameObject dashEffectPrefab;
    public TMP_Text cooldownText;
    public float maxDashDistance = 10f;

    private bool canDash = true;
    private bool isSelectingPosition = false;
    private bool isDashing = false;
    private bool isCooldown = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 dashTargetPosition;
    private float currentCooldownTime = 0f;
    private GameObject dashEffectInstance;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        cooldownText.text = "OK";
    }

    void Update()
    {
        // Block all input during dash
        if (isDashing) return;

        // Only activate ability if no other ability is active
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !AbilityManager.IsAbilityActive())
        {
            StartSelectingPosition();
        }

        if (isSelectingPosition && Input.GetMouseButtonDown(1))
        {
            TryDash();
        }

        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime;

            float timeLeft = dashCooldown - currentCooldownTime;

            // Update cooldown text
            cooldownText.text = Mathf.Ceil(timeLeft) + "s";

            if (currentCooldownTime >= dashCooldown)
            {
                currentCooldownTime = 0f;
                cooldownText.text = "OK";
                Debug.Log("Dash ability is ready.");
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
        // Prevent setting a new target while dashing
        if (isDashing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            if (IsWalkable(targetPosition))
            {
                // Calculate direction and clamp distance
                Vector3 direction = targetPosition - transform.position;
                float distance = direction.magnitude;

                if (distance > maxDashDistance)
                {
                    direction = direction.normalized;
                    targetPosition = transform.position + direction * maxDashDistance;
                }

                dashTargetPosition = targetPosition;
                StartCoroutine(Dash());
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
        return NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas);
    }

IEnumerator Dash()
{
    AbilityManager.SetAbilityActive(true);
    isDashing = true; // Block input
    canDash = false;

    float originalSpeed = agent.speed;
    agent.speed *= dashSpeedMultiplier;

    // Disable NavMeshAgent control
    agent.isStopped = true;

    // Dash effect
    dashEffectInstance = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
    dashEffectInstance.transform.parent = transform;

    // Manually move and rotate the agent toward the target position
    while (Vector3.Distance(transform.position, dashTargetPosition) > agent.stoppingDistance)
    {
        Vector3 dashDirection = (dashTargetPosition - transform.position).normalized;

        // Rotate the agent to face the dash direction
        Quaternion targetRotation = Quaternion.LookRotation(dashDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // Move the agent toward the target
        agent.Move(dashDirection * originalSpeed * dashSpeedMultiplier * Time.deltaTime);
        yield return null;
    }

    // Ensure the agent is exactly at the target position
    transform.position = dashTargetPosition;

    // Restore NavMeshAgent control
    agent.isStopped = false;
    agent.speed = originalSpeed;

    // Set the destination to the final dash position to avoid "snapping back"
    agent.SetDestination(dashTargetPosition);

    isDashing = false;

    if (dashEffectInstance != null)
    {
        Destroy(dashEffectInstance);
    }

    StartCoroutine(DashCooldown());
}



    IEnumerator DashCooldown()
    {
        AbilityManager.SetAbilityActive(false);
        isCooldown = true;

        yield return new WaitForSeconds(dashCooldown);

        isCooldown = false;
        canDash = true;
        Debug.Log("Dash cooldown ended.");
    }
}
