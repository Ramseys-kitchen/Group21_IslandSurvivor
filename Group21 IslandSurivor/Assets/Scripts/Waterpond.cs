using UnityEngine;

public class WaterPond : MonoBehaviour
{
    [Header("Pond Settings")]
    public float drinkRange = 2f; // How close player needs to be
    public KeyCode drinkKey = KeyCode.E; // Key to drink water

    [Header("UI")]
    public GameObject drinkPrompt; // "Press E to drink" text

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        // Find the player (assumes player has "Player" tag)
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Hide drink prompt at start
        if (drinkPrompt != null)
        {
            drinkPrompt.SetActive(false);
        }
    }

    void Update()
    {
        CheckPlayerDistance();
        HandleDrinking();
    }

    void CheckPlayerDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            playerInRange = distance <= drinkRange;

            // Show/hide drink prompt
            if (drinkPrompt != null)
            {
                drinkPrompt.SetActive(playerInRange);
            }
        }
    }

    void HandleDrinking()
    {
        if (playerInRange && Input.GetKeyDown(drinkKey))
        {
            // Find the thirst bar and restore thirst
            ThirstBar thirstBar = FindObjectOfType<ThirstBar>();
            if (thirstBar != null)
            {
                thirstBar.DrinkWater();
                Debug.Log("Player drank water!");
            }
        }
    }

    // Visual helper in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, drinkRange);
    }
}