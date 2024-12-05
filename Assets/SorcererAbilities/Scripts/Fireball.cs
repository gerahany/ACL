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
    if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion"))
    {
        Debug.Log("Enemy hit!");
        // Damage logic here
    }

    // Destroy only if it's a valid collision
    if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion"))
    {
        Destroy(gameObject);
    }
}

}
