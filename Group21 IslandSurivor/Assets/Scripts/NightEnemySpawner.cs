using UnityEngine;
using System.Collections.Generic;

public class NightEnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnRadius = 30f;
    public float minDistanceFromPlayer = 10f;
    public int maxEnemiesPerNight = 5;
    public float spawnInterval = 8f;

    [Header("Despawn Settings")]
    public float despawnDistance = 50f;
    public bool despawnAtDawn = true;

    [Header("Spawn Areas")]
    public LayerMask groundLayer = 1;
    public float groundCheckDistance = 10f;

    private DayNightCycle dayNightCycle;
    private bool wasNight = false;
    private float spawnTimer = 0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int enemiesSpawnedThisNight = 0;

    void Start()
    {
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (dayNightCycle == null || player == null) return;

        float cycleProgress = dayNightCycle.GetNormalizedTime();
        bool isNight = cycleProgress >= 0.5f;

        if (isNight != wasNight)
        {
            if (isNight)
                OnNightStart();
            else
                OnDayStart();
            wasNight = isNight;
        }

        if (isNight)
            HandleNightSpawning();

        CleanupDistantEnemies();
    }

    void OnNightStart()
    {
        Debug.Log("Night started - enemies will spawn!");
        spawnTimer = 0f;
        enemiesSpawnedThisNight = 0;
    }

    void OnDayStart()
    {
        Debug.Log("Day started - enemies despawning!");
        if (despawnAtDawn)
            DespawnAllEnemies();
    }

    void HandleNightSpawning()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && enemiesSpawnedThisNight < maxEnemiesPerNight)
        {
            TrySpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void TrySpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        Vector3 spawnPosition;
        if (FindValidSpawnPosition(out spawnPosition))
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            spawnedEnemies.Add(enemy);
            enemiesSpawnedThisNight++;

            Debug.Log($"Spawned enemy! Total: {enemiesSpawnedThisNight}/{maxEnemiesPerNight}");
        }
    }

    bool FindValidSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;

        for (int attempts = 0; attempts < 20; attempts++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistanceFromPlayer, spawnRadius);
            Vector3 testPosition = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (IsValidSpawnPosition(testPosition, out spawnPosition))
                return true;
        }

        return false;
    }

    bool IsValidSpawnPosition(Vector3 position, out Vector3 groundPosition)
    {
        groundPosition = position;

        RaycastHit hit;
        Vector3 rayStart = position + Vector3.up * groundCheckDistance;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance * 2f, groundLayer))
        {
            groundPosition = hit.point;
            Vector3 spaceCheck = groundPosition + Vector3.up * 2f;

            if (!Physics.CheckSphere(spaceCheck, 1f))
                return true;
        }

        return false;
    }

    void CleanupDistantEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
            else if (Vector3.Distance(player.position, spawnedEnemies[i].transform.position) > despawnDistance)
            {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    void DespawnAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }

}