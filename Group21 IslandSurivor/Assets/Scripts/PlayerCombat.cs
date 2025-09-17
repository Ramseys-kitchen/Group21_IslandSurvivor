using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackDamage = 50f;
    public float attackRange = 3f;
    public float attackCooldown = 1f;

    [Header("Input")]
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Visual Feedback")]
    public GameObject crosshair;
    public Color defaultCrosshairColor = Color.white;
    public Color targetCrosshairColor = Color.red;
    public GameObject hitEffect;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 100f;

    private float lastAttackTime;
    private Camera playerCamera;
    private Image crosshairImage;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        // Setup crosshair
        if (crosshair != null)
        {
            crosshairImage = crosshair.GetComponent<Image>();
            if (crosshairImage != null)
            {
                crosshairImage.color = defaultCrosshairColor;
            }
        }
    }

    void Update()
    {
        HandleCombatInput();
        UpdateTargeting();
    }

    void UpdateTargeting()
    {
        if (crosshairImage == null) return;

        // Raycast from camera center to see if we're targeting an enemy
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            SimpleEnemy enemy = hit.collider.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                crosshairImage.color = targetCrosshairColor;
                return;
            }
        }

        crosshairImage.color = defaultCrosshairColor;
    }

    void HandleCombatInput()
    {
        if (Input.GetKeyDown(attackKey) && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void PerformAttack()
    {
        // Create visible bullet - FIXED VERSION
        if (bulletPrefab != null && playerCamera != null)
        {
            // Get camera's forward direction
            Vector3 shootDirection = playerCamera.transform.forward;

            // Create bullet with PROPER rotation
            GameObject bullet = Instantiate(bulletPrefab,
                                          playerCamera.transform.position,
                                          Quaternion.LookRotation(shootDirection));

            // Make bullet move straight
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = shootDirection * bulletSpeed;

                // Add Bullet component if not present
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript == null)
                {
                    bulletScript = bullet.AddComponent<Bullet>();
                }
                bulletScript.damage = attackDamage;
                bulletScript.owner = gameObject;
            }

            // Destroy bullet after short time
            Destroy(bullet, 0.5f);
        }

        // Raycast from camera center for immediate hit detection
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        bool hitEnemy = false;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            SimpleEnemy enemy = hit.collider.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log($"Hit enemy for {attackDamage} damage!");
                hitEnemy = true;

                // Show hit effect
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hit.point, Quaternion.identity);
                }
            }
        }

        Debug.Log("Player attacked!" + (hitEnemy ? " And hit an enemy!" : " But missed!"));
    }

    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rayStart = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)).origin;
            Vector3 rayEnd = rayStart + playerCamera.transform.forward * attackRange;
            Gizmos.DrawLine(rayStart, rayEnd);
        }
    }
}