using UnityEngine;

public class CoconutPickup : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healAmount = 25f;

    [Header("Pickup Settings")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 1f;
    public float bobHeight = 0.3f;

    [Header("Effects")]
    public GameObject pickupEffect; // Optional: particle effect when picked up
    public AudioClip pickupSound; // Optional: sound effect

    private Vector3 startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Rotate the coconut
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player picked it up
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Only heal if player is not at full health
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log($"Player picked up coconut! Healed {healAmount} HP");

                    // Play pickup effect
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    // Play pickup sound
                    if (pickupSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(pickupSound);
                    }

                    // Destroy the coconut
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Player health is already full!");
                }
            }
        }
    }

    // Optional: Draw the pickup range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}