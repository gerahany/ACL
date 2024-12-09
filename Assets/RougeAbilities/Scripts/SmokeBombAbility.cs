using System.Collections;
using UnityEngine;

public class SmokeBombAbility : MonoBehaviour
{
    public GameObject smokeBombPrefab; // Smoke bomb visual effect prefab
    public float stunDuration = 5f; // Duration of the stun effect
    public float smokeBombCooldown = 10f; // Cooldown for the ability
    public float stunRange = 5f; // Range of the smoke bomb's effect

    private bool canUseSmokeBomb = true; // Can the Rogue use the smoke bomb?

    private Animator animator; // Reference to the Animator component

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        // Activate smoke bomb ability when pressing "W"
        if (Input.GetKeyDown(KeyCode.W) && canUseSmokeBomb)
        {
            ActivateSmokeBomb();
        }
    }

    void ActivateSmokeBomb()
    {
        // Instantiate the smoke bomb at the Rogue's position
        Vector3 bombPosition = transform.position;
        GameObject smokeBomb = Instantiate(smokeBombPrefab, bombPosition, Quaternion.identity);

        // Start the stun effect
        //StartCoroutine(StunEnemies(bombPosition));

        // Trigger animation
        animator.SetTrigger("SmokeBomb");

        // Start cooldown
        StartCoroutine(SmokeBombCooldown());
    }

    // IEnumerator StunEnemies(Vector3 center)
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(center, stunRange);

    //     foreach (var collider in hitColliders)
    //     {
    //         if (collider.CompareTag("Minion") || collider.CompareTag("Demon"))
    //         {
    //             EnemyAI enemy = collider.GetComponent<EnemyAI>();
    //             if (enemy != null)
    //             {
    //                 enemy.Stun(stunDuration);
    //             }
    //         }
    //     }

    //     yield return new WaitForSeconds(stunDuration);

    //     Debug.Log("Enemies are no longer stunned.");
    // }

    IEnumerator SmokeBombCooldown()
    {
        canUseSmokeBomb = false;
        yield return new WaitForSeconds(smokeBombCooldown);
        canUseSmokeBomb = true;
        Debug.Log("Smoke bomb ability is ready.");
    }

    private void OnDrawGizmos()
    {
        // Visualize the stun range in the editor
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, stunRange);
    }
}
