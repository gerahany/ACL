using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehavior : MonoBehaviour
{
    private RandMinion campManager;
    private Vector3 initialPosition;
    private bool isAggressive = false;
    private GameObject targetPlayer;
    private NavMeshAgent agent;
    private Animator animator;
    private static GameObject globalTarget;
    private float punchDistance = 2f; // Distance to start punching
     private float punchCooldown = 7f; // Time between punches
    private bool isPunching = false;

    void Start()
    {
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(RandMinion manager)
    {
        campManager = manager;
    }

    public void SetAggressive(GameObject player)
    {
        isAggressive = true;
        targetPlayer = player;
        agent.isStopped = false;
        UpdateAnimation("IsWalking", true);
    }

    public void ReturnToCamp()
    {
        isAggressive = false;
        targetPlayer = null;
        agent.SetDestination(initialPosition);
        agent.isStopped = false;
        UpdateAnimation("IsWalking", true);
    }

    void Update()
    {
        if (MinionBehavior.globalTarget != null)
        {
            targetPlayer = MinionBehavior.globalTarget; // Switch to the global target
        }
        else
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player"); // Default to the player
        }

        if (isAggressive && targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            Vector3 directionToPlayer = targetPlayer.transform.position - transform.position;
            directionToPlayer.y = 0;  // Keep rotation only on the horizontal plane (y-axis)
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            if (distanceToPlayer <= punchDistance)
            {
                agent.isStopped = true; // Stop moving
                UpdateAnimation("IsWalking", false);

                if (!isPunching)
                {
                    StartCoroutine(PunchPlayer());
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(targetPlayer.transform.position);
                UpdateAnimation("IsWalking", true);
            }
        }
        else if (!isAggressive && Vector3.Distance(transform.position, initialPosition) < 1f)
        {
            UpdateAnimation("IsWalking", false);
        }
    }

    private IEnumerator PunchPlayer()
    {
        isPunching = true;

        // Trigger the punch animation
        animator.SetTrigger("Punch");

        // Apply damage immediately when the punch starts
        if (targetPlayer != null)
        {
            BasePlayer playerHealth = targetPlayer.GetComponent<BasePlayer>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(5); // Inflict damage
            }
        }

        // Wait for punch animation to finish
        yield return new WaitForSeconds(GetAnimationClipLength("Punch"));

        // Transition to idle
        UpdateAnimation("IsIdle", true);

        // Wait for cooldown
        yield return new WaitForSeconds(punchCooldown);

        // Reset to allow the next punch
        isPunching = false;
    }

   

    private float GetAnimationClipLength(string clipName)
    {
        // Find the animation clip duration by name
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0f; // Default to 0 if not found
    }

    public void Die()
    {
        
        Destroy(gameObject);
    }

    private void UpdateAnimation(string animationName, bool state)
    {
        if (animator != null)
        {
            animator.SetBool(animationName, state);
        }
    }

    public static void SetGlobalTarget(GameObject target)
    {
        globalTarget = target;
    }

    public static void RevertGlobalTarget()
    {
        globalTarget = null;
    }
}
