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
    public SoundEffectHandler soundEffectHandler;

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

            float timeLeft = smokeBombCooldown - currentCooldownTime; // Calculate remaining time

            // Update the TMP text to show the remaining cooldown time
            cooldownText.text = Mathf.Ceil(timeLeft) + "s"; // Round and show seconds

            if (currentCooldownTime >= smokeBombCooldown)
            {
                //canUseAbility = true; // Reset ability to be ready
                currentCooldownTime = 0f; // Reset cooldown time
                cooldownText.text = "OK"; // Show "OK" when cooldown is complete
                Debug.Log("Shower of Arrows ability is ready.");
            }
        }
          if(basePlayer.isCoolZero()){
           smokeBombCooldown=0f;
            cooldownText.text = "OK";
            isCooldown=false;
        }else{
            smokeBombCooldown=10f;
        }
    }

    void ActivateSmokeBomb()
    {
        // Instantiate the smoke bomb at the Rogue's position
        
        Vector3 bombPosition = transform.position;
        GameObject smokeBomb = Instantiate(smokeBombPrefab, bombPosition, Quaternion.identity);
        soundEffectHandler.PlayExplosionSound();
        // Start the stun effect
        StartCoroutine(StunEnemies(bombPosition));

        // Trigger animation
        animator.SetTrigger("SmokeBomb");

        // Start cooldown
        StartCoroutine(SmokeBombCooldown());
    }

  IEnumerator StunEnemies(Vector3 center)
{
    // Find all colliders within the stun range
    Collider[] enemiesInRange = Physics.OverlapSphere(center, stunRange, targetLayer);

    // Loop through all enemies and apply stun
    foreach (Collider enemyCollider in enemiesInRange)
    {
        // Check if the enemy has a MinionBehavior or DemonBehavior component and stun them
        MinionBehavior minion = enemyCollider.GetComponent<MinionBehavior>();
        if (minion != null)
        {
            minion.Stun(stunDuration); // Stun the minion for the given duration
        }

        DemonBehavior demon = enemyCollider.GetComponent<DemonBehavior>();
        if (demon != null)
        {
            demon.Stun(stunDuration); // Stun the demon for the given duration
        }
        // BossBehavior boss = enemyCollider.GetComponent<BossBehavior>();
        // if (boss != null)
        // {
        //     boss.Stun(stunDuration); // Stun the demon for the given duration
        // }
    }

    yield return null; // Finish the method
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
