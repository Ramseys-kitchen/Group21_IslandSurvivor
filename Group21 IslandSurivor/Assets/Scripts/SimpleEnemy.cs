using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float detectionRange = 15f;
    public float stopDistance = 1.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 5f;
    public LayerMask groundLayer = 1;

    [Header("Combat System")]
    public float maxHealth = 100f;
    public float attackDamage = 20f;
    public float attackCooldown = 2f;
    public GameObject healthBarPrefab;

    [Header("Visual Feedback")]
    public GameObject attackIndicator;
    public Color targetingColor = Color.yellow;
    public Color attackingColor = Color.red;

    // Combat variables
    private float currentHealth;
    private float lastAttackTime;
    private bool isDead = false;
    private GameObject healthBarInstance;
    private Slider healthBarSlider;

    // Visual feedback
    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    private Transform player;
    private Rigidbody rb;
    private bool isChasing = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize health
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("SimpleEnemy: No GameObject with 'Player' tag found!");
        }


        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        SnapToGround();

        // Get renderer for damage feedback
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;

        // Create health bar
        CreateHealthBar();

        // Initialize attack indicator
        if (attackIndicator != null)
            attackIndicator.SetActive(false);
    }

    void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            // Find existing canvas
            Canvas existingCanvas = FindObjectOfType<Canvas>();

            if (existingCanvas != null)
            {
                // Create health bar as child of existing canvas
                healthBarInstance = Instantiate(healthBarPrefab, existingCanvas.transform);

                // For Screen Space canvas, we need to handle positioning differently
                if (existingCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                    existingCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    // Screen space positioning - we'll update position in Update()
                    RectTransform rectTransform = healthBarInstance.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.one * 0.5f; // Scale down the health bar
                    }
                }
            }

            healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = maxHealth;
                healthBarSlider.value = currentHealth;
            }

            // Initially hide health bar
            healthBarInstance.SetActive(false);
        }
    }

    void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * 0.5f;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Show/hide health bar based on distance or damage
        if (healthBarInstance != null)
        {
            bool shouldShowHealthBar = distanceToPlayer <= detectionRange || currentHealth < maxHealth;
            healthBarInstance.SetActive(shouldShowHealthBar);

            // Update health bar position for screen space canvas
            if (shouldShowHealthBar)
            {
                UpdateHealthBarPosition();
            }
        }

        // Update attack indicator
        UpdateAttackIndicator(distanceToPlayer);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }


        if (isChasing && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
            isAttacking = false;
        }
        else if (isChasing && distanceToPlayer <= stopDistance)
        {
            LookAtPlayer();
            isAttacking = true;

            // Attack player if close enough
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            isAttacking = false;
        }


        if (distanceToPlayer > detectionRange * 2f)
        {
            isChasing = false;
            isAttacking = false;
        }
    }

    void UpdateAttackIndicator(float distanceToPlayer)
    {
        if (attackIndicator == null) return;

        if (distanceToPlayer <= detectionRange && distanceToPlayer > stopDistance)
        {
            // Targeting but not attacking yet
            attackIndicator.SetActive(true);
            if (enemyRenderer != null && !isFlashing)
                enemyRenderer.material.color = targetingColor;
        }
        else if (distanceToPlayer <= stopDistance)
        {
            // In attack range
            attackIndicator.SetActive(true);
            if (enemyRenderer != null && !isFlashing)
                enemyRenderer.material.color = attackingColor;
        }
        else
        {
            // Not targeting
            attackIndicator.SetActive(false);
            if (enemyRenderer != null && !isFlashing)
                enemyRenderer.material.color = originalColor;
        }
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarInstance == null || Camera.main == null) return;

        // Check if we're using screen space canvas
        Canvas parentCanvas = healthBarInstance.GetComponentInParent<Canvas>();
        if (parentCanvas != null &&
            (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
             parentCanvas.renderMode == RenderMode.ScreenSpaceCamera))
        {
            // Convert world position to screen position
            Vector3 worldPosition = transform.position + Vector3.up * 2f; // Lowered the height
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // Only show if enemy is in front of camera
            if (screenPosition.z > 0)
            {
                RectTransform rectTransform = healthBarInstance.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.position = screenPosition;
                }
                healthBarInstance.SetActive(true);
            }
            else
            {
                // Hide if behind camera
                healthBarInstance.SetActive(false);
            }
        }
    }

    void AttackPlayer()
    {
        // Find player health component and damage it
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked player for {attackDamage} damage!");

            // Visual feedback for attack
            StartCoroutine(AttackFlash());
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Update health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
            Debug.Log($"Health bar updated to: {healthBarSlider.value}");
        }

        // Damage feedback
        StartCoroutine(DamageFlash());

        Debug.Log($"Enemy took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log($"TakeDamage called with: {damage}");
    }

    IEnumerator DamageFlash()
    {
        if (enemyRenderer != null && !isFlashing)
        {
            isFlashing = true;
            Color original = enemyRenderer.material.color;
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = original;
            isFlashing = false;
        }
    }

    IEnumerator AttackFlash()
    {
        if (attackIndicator != null)
        {
            attackIndicator.transform.localScale = Vector3.one * 1.5f;
            yield return new WaitForSeconds(0.2f);
            attackIndicator.transform.localScale = Vector3.one;
        }
    }

    void Die()
    {
        isDead = true;

        // Hide health bar
        if (healthBarInstance != null)
            healthBarInstance.SetActive(false);

        // Hide attack indicator
        if (attackIndicator != null)
            attackIndicator.SetActive(false);

        Debug.Log("Enemy died!");

        // Notify spawner that this enemy is dead
        NightEnemySpawner spawner = FindObjectOfType<NightEnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDied(gameObject);
        }

        // Disable collider and renderer
        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        if (enemyRenderer != null) enemyRenderer.enabled = false;

        // Remove after a delay
        Destroy(gameObject, 2f);
    }

    void ChasePlayer()
    {

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep enemy on ground
        directionToPlayer = directionToPlayer.normalized;


        Vector3 movement = directionToPlayer * moveSpeed * Time.deltaTime;


        Vector3 newPosition = transform.position + movement;
        if (HasGroundBelow(newPosition))
        {
            transform.Translate(movement, Space.World);


            KeepOnGround();
        }


        LookAtPlayer();
    }

    void KeepOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, groundLayer))
        {

            float targetY = hit.point.y + 1f;
            if (Mathf.Abs(transform.position.y - targetY) > 0.1f)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
            }
        }
    }

    void LookAtPlayer()
    {

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    bool HasGroundBelow(Vector3 position)
    {

        RaycastHit hit;
        Vector3 rayStart = position + Vector3.up * 0.5f;

        return Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);


        if (isChasing && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }


        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * groundCheckDistance);

        // Draw attack range indicator
        if (isAttacking)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player!");
        }
    }

    void OnDestroy()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }
}