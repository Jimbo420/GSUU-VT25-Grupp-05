using System.Collections;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Enemy Spawning Settings")]
    [Tooltip("Prefab for the enemies.")]
    public GameObject enemyPrefab;
    [Tooltip("Spawn points for enemy backup.")]
    public Transform[] enemySpawnPoints;

    [Header("Gasoline Tank Spawning Settings")]
    [Tooltip("Prefab for the gasoline tank.")]
    public GameObject gasolineTankPrefab;
    [Tooltip("Spawn points for gasoline tanks.")]
    public Transform[] gasolineTankSpawnPoints;
    [Tooltip("Interval between gasoline tank spawns (in seconds).")]
    public float gasolineSpawnInterval = 20f;

    private bool isSpawningGasolineTanks = false;

    public void StartSpawning()
    {
        // Start spawning gasoline tanks
        if (!isSpawningGasolineTanks)
        {
            isSpawningGasolineTanks = true;
            StartCoroutine(SpawnGasolineTanksRoutine());
        }
    }

    public void SpawnEnemiesAtAllPoints()
    {
        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning("No enemy spawn points assigned.");
            return;
        }

        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                Debug.Log($"Spawned enemy at {spawnPoint.position}.");
            }
        }
    }

    private IEnumerator SpawnGasolineTanksRoutine()
    {
        while (isSpawningGasolineTanks)
        {
            SpawnGasolineTank();
            yield return new WaitForSeconds(gasolineSpawnInterval);
        }
    }

    public void SpawnGasolineTank()
    {
        if (gasolineTankPrefab == null || gasolineTankSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Gasoline tank prefab or spawn points are not set.");
            return;
        }

        // Choose a random spawn point
        Transform spawnPoint = gasolineTankSpawnPoints[Random.Range(0, gasolineTankSpawnPoints.Length)];

        // Check if a gasoline tank already exists at the spawn point
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("GasolineTank"))
            {
                Debug.Log("A gasoline tank already exists at this spawn point. Skipping spawn.");
                return;
            }
        }

        // Spawn the gasoline tank
        Instantiate(gasolineTankPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Gasoline tank spawned at {spawnPoint.position}");
    }

    public void StopSpawningGasolineTanks()
    {
        isSpawningGasolineTanks = false;
    }
}
