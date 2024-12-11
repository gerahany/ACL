using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float originalSpeed = 5f;
    private float currentSpeed;
    public float health = 100f;

    private bool isSlowed = false;
    private float slowDuration;

    void Start()
    {
        currentSpeed = originalSpeed;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        currentSpeed = originalSpeed;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (isSlowed) return; // Prevent overlapping slow effects

        isSlowed = true;
        SetSpeedMultiplier(multiplier);
        slowDuration = duration;
        Invoke(nameof(RemoveSlow), duration);
    }

    private void RemoveSlow()
    {
        ResetSpeed();
        isSlowed = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Move forward in the Z direction; adjust movement logic as needed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
