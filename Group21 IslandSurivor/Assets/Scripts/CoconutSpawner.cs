using UnityEngine;
using System.Collections.Generic;

public class CoconutSpawner : MonoBehaviour
{
    [Header("Coconut Settings")]
    public GameObject coconutPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int maxCoconuts = 5; 
    public float spawnRadius = 30f; 
    public float minDistanceFromPlayer = 15f; 
    public float spawnInterval = 60f; 

    [Header("Ground Detection")]
    public LayerMask groundLayer = 1;
    public float groundCheckDistance = 20f;
    public float coconutHeightAboveGround = 0.5f;

    [Header("Day/Night Settings")]
    public bool spawnOnlyDuringDay = true; // Coconuts spawn during day, stay until eaten

    private DayNightCycle dayNightCycle;
    private float spawnTimer = 0f;
    private List<GameObject> spawnedCoconuts = new List<GameObject>();
    private bool wasNight = false;

    void Start()
    {
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("CoconutSpawner: No player found!");
        }

        if (coconutPrefab == null)
        {
            Debug.LogError("CoconutSpawner: No coconut prefab assigned!");
        }
    }

    void Update()
    {
        if (player == null || coconutPrefab == null) return;

        // Clean up null references (coconuts that were picked up)
        CleanupNullCoconuts();

        // Check day/night transitions
        if (dayNightCycle != null)
        {
            float cycleProgress = dayNightCycle.GetNormalizedTime();
            bool isNight = cycleProgress >= 0.5f;

            // Detect night to day transition
            if (wasNight && !isNight)
            {
                OnNewDay();
            }

            wasNight = isNight;
        }

        // Check if we should spawn coconuts
        bool shouldSpawn = true;

        if (spawnOnlyDuringDay && dayNightCycle != null)
        {
            float cycleProgress = dayNightCycle.GetNormalizedTime();
            bool isDay = cycleProgress < 0.5f; // Day is first half of cycle
            shouldSpawn = isDay;
        }

        // Handle spawning
        if (shouldSpawn)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval && spawnedCoconuts.Count < maxCoconuts)
            {
                TrySpawnCoconut();
                spawnTimer = 0f;
            }
        }
    }

    void OnNewDay()
    {
        // Clear old coconuts at dawn to prevent overlap
        Debug.Log("New day! Clearing old coconuts...");
        ClearAllCoconuts();
    }

    void TrySpawnCoconut()
    {
        Vector3 spawnPosition;
        if (FindValidSpawnPosition(out spawnPosition))
        {
            GameObject coconut = Instantiate(coconutPrefab, spawnPosition, Quaternion.identity);
            spawnedCoconuts.Add(coconut);

            Debug.Log($"Spawned coconut! Total: {spawnedCoconuts.Count}/{maxCoconuts}");
        }
        else
        {
            Debug.LogWarning("Could not find valid spawn position for coconut");
        }
    }

    bool FindValidSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;

        // Try multiple times to find a valid position
        for (int attempts = 0; attempts < 30; attempts++)
        {
            // Generate random position around player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistanceFromPlayer, spawnRadius);
            Vector3 testPosition = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (IsValidSpawnPosition(testPosition, out spawnPosition))
            {
                return true;
            }
        }

        return false;
    }

    bool IsValidSpawnPosition(Vector3 position, out Vector3 groundPosition)
    {
        groundPosition = position;

        // Raycast from above to find ground
        Vector3 rayStart = position + Vector3.up * groundCheckDistance;
        RaycastHit hit;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance * 2f, groundLayer))
        {
            groundPosition = hit.point + Vector3.up * coconutHeightAboveGround;

            // Check if there's enough space above ground (no obstacles)
            Vector3 spaceCheck = groundPosition + Vector3.up * 1f;

            if (!Physics.CheckSphere(spaceCheck, 0.5f))
            {
                // Make sure we're not too close to existing coconuts
                foreach (GameObject coconut in spawnedCoconuts)
                {
                    if (coconut != null && Vector3.Distance(coconut.transform.position, groundPosition) < 5f)
                    {
                        return false; // Too close to another coconut
                    }
                }

                return true;
            }
        }

        return false;
    }

    void CleanupNullCoconuts()
    {
        // Remove coconuts that have been picked up (destroyed)
        for (int i = spawnedCoconuts.Count - 1; i >= 0; i--)
        {
            if (spawnedCoconuts[i] == null)
            {
                spawnedCoconuts.RemoveAt(i);
            }
        }
    }

    // Call this to spawn a coconut immediately
    public void SpawnCoconutNow()
    {
        if (spawnedCoconuts.Count < maxCoconuts)
        {
            TrySpawnCoconut();
        }
    }

    // I will clear all coconuts here
    public void ClearAllCoconuts()
    {
        foreach (GameObject coconut in spawnedCoconuts)
        {
            if (coconut != null)
                Destroy(coconut);
        }
        spawnedCoconuts.Clear();
    }

    
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

       
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, spawnRadius);

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);

        
        Gizmos.color = Color.cyan;
        foreach (GameObject coconut in spawnedCoconuts)
        {
            if (coconut != null)
            {
                Gizmos.DrawWireSphere(coconut.transform.position, 0.5f);
            }
        }
    }
}