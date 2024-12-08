using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 8f; // Speed of the arrow
    public float damage = 15f; // Damage dealt to the enemy upon impact
    public float lifetime = 10f; // Lifetime of the arrow

    private Vector3 direction;
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Destroy arrow after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 targetDirection)
    {
        direction = targetDirection.normalized; // Normalize the direction vector for consistent speed
    }

    void Update()
    {
        // If we have a Rigidbody, set the velocity
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            // In case there's no Rigidbody (fallback to position update)
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    Debug.Log("Collision Detected with: " + collision.gameObject.name); 
    Debug.Log($"Collision Tag: {collision.gameObject.tag}");
    if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion"))
    {
        Debug.Log("Enemy hit!");
        // Check if the enemy has a MinionHealth component
        minionhealthbar minionHealth = collision.gameObject.GetComponent<minionhealthbar>();
        demonhealthbar demonHealth = collision.gameObject.GetComponent<demonhealthbar>();

        if (minionHealth != null)
        {
            Debug.Log("fire hit the minion!");

            minionHealth.TakeDamage(5);  // Apply 5 damage to the minion
            Debug.Log($"fire hit {collision.gameObject.name} for 5 damage!");
        }
        else if (demonHealth != null)
        {
            Debug.Log("fire hit the minion!");

            demonHealth.TakeDamage(5);  // Apply 5 damage to the minion
            Debug.Log($"Bash hit {collision.gameObject.name} for 5 damage!");
        }
    }

    // Destroy only if it's a valid collision
    if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion") || collision.gameObject.CompareTag("Breakable") || collision.gameObject.CompareTag("Flooring"))
    {
        Destroy(gameObject);
    }
}
}
