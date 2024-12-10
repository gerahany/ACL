using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DashAbility : MonoBehaviour
{
    public float dashSpeedMultiplier = 2f;
    public float dashCooldown = 5f;
    public GameObject dashEffectPrefab;

    private bool canDash = true;
    private bool isSelectingPosition = false;
    private bool isDashing = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 dashTargetPosition;
    private GameObject dashEffectInstance;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Only activate ability if no other ability is active
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !AbilityManager.IsAbilityActive())
        {
            StartSelectingPosition();
        }

        if (isSelectingPosition && Input.GetMouseButtonDown(1))
        {
            TryDash();
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
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    IEnumerator Dash()
    {
        AbilityManager.SetAbilityActive(true); // Mark ability as active
        isDashing = true;
        canDash = false;
        float originalSpeed = agent.speed;
        agent.speed *= dashSpeedMultiplier;

        dashEffectInstance = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        dashEffectInstance.transform.parent = transform;

        agent.SetDestination(dashTargetPosition);

        while (Vector3.Distance(transform.position, dashTargetPosition) > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.speed = originalSpeed;
        isDashing = false;

        if (dashEffectInstance != null)
        {
            Destroy(dashEffectInstance);
        }

        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        AbilityManager.SetAbilityActive(false); // Mark ability as inactive
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("Dash cooldown ended.");
    }
}