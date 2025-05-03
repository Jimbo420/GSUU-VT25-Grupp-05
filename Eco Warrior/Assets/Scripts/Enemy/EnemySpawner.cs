using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject enemyPrefab;
    private Dictionary<int, int> enemySpawnerList = new Dictionary<int, int>
    {
        {1, 2},
        {2, 5},
        {3, 15},
    };
    private int level = 2;
    private float minimumSpawnTime = 1;
    private float maximumSpawnTime = 4;
    private float timeToSpawn;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        int enemiesByLevel = enemySpawnerList.Where(a => a.Key.Equals(level)).Select(b => b.Value).FirstOrDefault();
        var spawnPoint = GameObject.FindWithTag("EnemySpawnWaypoint");
        for (int i = 0; i < enemiesByLevel; i++)
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
