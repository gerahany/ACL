using UnityEngine;

public class EnemyInteractions : MonoBehaviour
{
    private bool isStunned = false;
    private bool isSlowed = false;

    private float originalSpeed = 5f;
    private float currentSpeed;

    // Timers for effects
    private float stunTimer = 0f;
    private float slowTimer = 0f;

    void Start()
    {
        currentSpeed = originalSpeed;
    }

    void Update()
    {
        HandleStunEffect();
        HandleSlowEffect();

        // Move enemy only if not stunned
        if (!isStunned)
        {
            MoveForward();
        }
    }

    private void HandleStunEffect()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
        }
    }

    private void HandleSlowEffect()
    {
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                currentSpeed = originalSpeed; // Reset speed
            }
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");
        Destroy(gameObject); // Simplified damage logic
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    public void Slow(float factor, float duration)
    {
        isSlowed = true;
        slowTimer = duration;
        currentSpeed = originalSpeed * factor; // Adjust speed
    }
}
