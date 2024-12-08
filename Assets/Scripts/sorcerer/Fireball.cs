using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 3f;  // Speed of the fireball
    public float damage = 50f;  // Damage dealt to the enemy upon impact
    public float lifetime = 10f;
    private Vector3 direction;
    private ParticleSystem fireballParticles;

    public void SetDirection(Vector3 targetDirection)
    {
        direction = targetDirection.normalized; // Normalize the direction vector for consistent speed
    }

    void Start()
    {
        // Get the ParticleSystem component from the fireball object
        fireballParticles = GetComponentInChildren<ParticleSystem>();
        
        // Destroy fireball after its lifetime expires
        Destroy(gameObject, lifetime);

        // Start the particle system when the fireball is spawned
        if (fireballParticles != null)
        {
            fireballParticles.Play();
        }
    }

    void Update()
    {
        // Move the fireball in the specified direction
        transform.position += direction * speed * Time.deltaTime;
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
    if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion") || collision.gameObject.CompareTag("Breakable") )
    {
        Destroy(gameObject);
    }
}

}
