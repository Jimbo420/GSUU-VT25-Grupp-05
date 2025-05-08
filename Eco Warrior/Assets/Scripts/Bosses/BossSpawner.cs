using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("General Spawning Settings")]
    [Tooltip("Default prefab to spawn.")]
    public GameObject defaultPrefab; // Default prefab to spawn

    [Tooltip("Default spawn interval (in seconds).")]
    public float defaultSpawnInterval = 20f; // Default interval for spawning

    private bool isSpawning = false; // General flag for spawning

    [Header("Spawn Points")]
    [Tooltip("Default spawn points for general use.")]
    public List<Transform> defaultSpawnPoints = new();

    [Tooltip("Special spawn points for specific phases.")]
    public List<Transform> specialSpawnPoints = new();

    /// <summary>
    /// Spawns a specified prefab at the default spawn points.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="useAllPoints">Whether to use all spawn points or random ones.</param>
    /// <param name="quantity">The number of objects to spawn (if using random points).</param>
    public void SpawnAtDefaultPoints(GameObject prefab, bool useAllPoints = false, int quantity = 1)
    {
        SpawnObjects(prefab, defaultSpawnPoints, useAllPoints, quantity);
    }

    /// <summary>
    /// Spawns a specified prefab at the special spawn points.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="useAllPoints">Whether to use all spawn points or random ones.</param>
    /// <param name="quantity">The number of objects to spawn (if using random points).</param>
    public void SpawnAtSpecialPoints(GameObject prefab, bool useAllPoints = false, int quantity = 1)
    {
        SpawnObjects(prefab, specialSpawnPoints, useAllPoints, quantity);
    }

    /// <summary>
    /// Spawns a specified prefab at the given spawn points.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="spawnPoints">The spawn points to use.</param>
    /// <param name="useAllPoints">Whether to use all spawn points or random ones.</param>
    /// <param name="quantity">The number of objects to spawn (if using random points).</param>
    private void SpawnObjects(GameObject prefab, List<Transform> spawnPoints, bool useAllPoints, int quantity)
    {
        if (prefab == null)
        {
            //Debug.LogWarning("SpawnObjects called with a null prefab.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            //Debug.LogWarning("No spawn points available for spawning.");
            return;
        }

        if (useAllPoints)
        {
            // Spawn at all specified spawn points
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                    //Debug.Log($"Spawned {prefab.name} at {spawnPoint.position}.");
                }
            }
        }
        else
        {
            // Spawn at random points
            for (int i = 0; i < quantity; i++)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                if (randomSpawnPoint != null)
                {
                    Instantiate(prefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
                    //Debug.Log($"Spawned {prefab.name} at {randomSpawnPoint.position}.");
                }
            }
        }
    }

    /// <summary>
    /// Starts a spawning routine for the default spawn points.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="spawnInterval">The interval between spawns.</param>
    public void StartSpawningAtDefaultPoints(GameObject prefab, float spawnInterval)
    {
        StartSpawning(prefab, spawnInterval, defaultSpawnPoints);
    }

    /// <summary>
    /// Starts a spawning routine for the special spawn points.
    /// </summary>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="spawnInterval">The interval between spawns.</param>
    public void StartSpawningAtSpecialPoints(GameObject prefab, float spawnInterval)
    {
        StartSpawning(prefab, spawnInterval, specialSpawnPoints);
    }

    /// <summary>
    /// Starts a spawning routine for the given spawn points.
    /// </summary>
    public void StartSpawning(GameObject prefab, float spawnInterval, List<Transform> spawnPoints)
    {
        if (isSpawning)
        {
            //Debug.LogWarning("Spawning is already active.");
            return;
        }

        if (prefab == null)
        {
            //Debug.LogWarning("StartSpawning called with a null prefab.");
            return;
        }

        isSpawning = true;
        StartCoroutine(SpawnRoutine(prefab, spawnInterval, spawnPoints));
    }

    /// <summary>
    /// Stops the current spawning routine.
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
    }

    /// <summary>
    /// Coroutine for spawning objects at regular intervals.
    /// </summary>
    private IEnumerator SpawnRoutine(GameObject prefab, float spawnInterval, List<Transform> spawnPoints)
    {
        while (isSpawning)
        {
            SpawnObjects(prefab, spawnPoints, false, 1);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
