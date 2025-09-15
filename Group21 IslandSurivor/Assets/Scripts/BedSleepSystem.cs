using UnityEngine;

public class BedSleepSystem : MonoBehaviour
{
    [Header("Sleep Settings")]
    public float sleepRange = 2f;
    public float enemyCheckRadius = 10f;

    [Header("Sleep Effects")]
    public bool restoreEnergyOnSleep = true;
    public bool restoreThirstOnSleep = false;
    public float energyRestoreAmount = 100f;

    [Header("UI Reference")]
    public BedUIManager uiManager;

    [Header("Bar References")]
    public EnergyBar energyBar;  // Drag your EnergyBar here
    public ThirstBar thirstBar;  // Drag your ThirstBar here

    private Transform player;
    private bool playerInRange = false;
    private bool enemiesNearby = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Find them once at start if not assigned in inspector
        if (energyBar == null)
            energyBar = FindObjectOfType<EnergyBar>();
        if (thirstBar == null)
            thirstBar = FindObjectOfType<ThirstBar>();
    }

    void Update()
    {
        CheckPlayerDistance();
        CheckForEnemies();
        UpdateUI();
    }

    void CheckPlayerDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            playerInRange = distance <= sleepRange;
        }
    }

    void CheckForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesNearby = false;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy <= enemyCheckRadius)
                {
                    enemiesNearby = true;
                    break;
                }
            }
        }
    }

    public void TrySleep()
    {
        if (!playerInRange)
            return;

        if (enemiesNearby)
        {
            Debug.Log("Cannot sleep - enemies are nearby!");
            return;
        }

        Sleep();
    }

    void Sleep()
    {
        Debug.Log("Player is sleeping...");

        if (restoreEnergyOnSleep && energyBar != null)
        {
            energyBar.currentEnergy = energyRestoreAmount;
            Debug.Log("Energy restored from sleeping!");
        }

        if (restoreThirstOnSleep && thirstBar != null)
        {
            thirstBar.currentThirst = thirstBar.maxThirst;
            Debug.Log("Thirst restored from sleeping!");
        }
    }

    void UpdateUI()
    {
        if (uiManager == null)
            return;

        if (playerInRange)
        {
            if (enemiesNearby)
            {
                uiManager.ShowDangerPrompt();
            }
            else
            {
                uiManager.ShowSleepPrompt();
            }
        }
        else
        {
            uiManager.HideAllPrompts();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sleepRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);
    }
}