using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBarSlider;
    public Text healthText;

    [Header("Game Over")]
    public GameObject gameOverPanel; // Assign a UI panel for game over

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Hide game over panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
        }
    }

    void Die()
    {
        Debug.Log("Player died!");

        // Show game over screen instead of pausing the game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Optional: disable player controls
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component != this)
                    component.enabled = false;
            }

            // Disable Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
        else
        {
            // Fallback: reload scene instead of pausing time
            Debug.LogWarning("Game Over panel not assigned! Would reload scene here.");
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    // Public method to restart game
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Time.timeScale = 1f; // Ensure time is running
    }
}